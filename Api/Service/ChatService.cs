using FastAop.Core;
using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Model;
using FastKMSApi.Core.Request;
using FastOllama.Core;
using FastOllama.Core.Model;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Service
{
    public class ChatService : IChatService
    {
        [Autowired]
        public readonly IElasticsearchVector elasticsearchVector;

        [Autowired]
        private readonly IElasticsearch elasticsearch;

        [Autowired]
        private readonly IFastOllamaRepository ollamaRepository;

        [Autowired]
        private readonly IDataService dataService;
 
        public string Chat(string message, List<KmsModel> kmsModel, string chatIndex, List<string> history = null)
        {
            var msg = string.Empty;
            var chatRecordModel = new ChatRecordModel { beginTime = DateTime.Now, request = message };
            var field = new List<float>();
            var lLMResponse = ollamaRepository.Embed(new EmbedModel { content = { message } });
            if (lLMResponse.IsSuccess && lLMResponse.EmbedData.Count > 0)
            {
                lLMResponse.EmbedData.ForEach(item =>
                {
                    foreach (var key in item)
                    {
                        field = key.Value;
                    }
                });

                var data = elasticsearchVector.QueryVector(kmsModel.Select(a => a.vectorIndex).ToList(), new VectorQuery
                {
                    Match = new Dictionary<string, object> { { AppSetting.FieldKey, message } },
                    Data = field.ToArray(),
                    Fields = new List<string> { AppSetting.FieldKey }
                }).List;

                if (data.Count > 0)
                {
                    var content = data.Select(a => a.GetValue("text").ToStr()).ToList();

                    chatRecordModel.isPrompt = true;
                    chatRecordModel.vectorContent = string.Join("\n\r", content);
                    chatRecordModel.model = AppSetting.LLmModel;

                    var chat = ollamaRepository.Chat(new FastOllama.Core.Model.ChatModel { content = message, model = AppSetting.LLmModel, history = content });

                    chatRecordModel.endTime = DateTime.Now;
                    chatRecordModel.response = chat.IsSuccess ? chat.ChatData : chat.Exception;
                    msg = chatRecordModel.response;
                }
                else
                {
                    chatRecordModel.model = AppSetting.LLmModel;
                    msg = string.Format(AppSetting.ChatResult, message);
                    chatRecordModel.response = msg;
                }
            }
            else
            {
                msg = string.Format(AppSetting.ChatResult, message);
                chatRecordModel.response = msg;
            }

            AddChatRecord(chatRecordModel, chatIndex, message, kmsModel);

            return msg;
        }

        private void AddChatRecord(ChatRecordModel chatRecordModel, string chatIndex, string message, List<KmsModel> kmsModel, DbInfo dbInfo = null)
        {
            var json = JsonConvert.SerializeObject(chatRecordModel).ToStr();

            elasticsearch.Create<ChatRecordModel>(chatIndex);

            elasticsearch.Add(chatIndex, Guid.NewGuid().ToStr(), json.JsonToDic(true));

            if (elasticsearch.GetList(AppSetting.ChatInfoIndex, new QueryModel()).List.Count == 0)
                elasticsearch.Create<ChatInfo>(AppSetting.ChatInfoIndex);

            var chatInfo = elasticsearch.GetList(AppSetting.ChatInfoIndex,
                                                new QueryModel
                                                {
                                                    IsPhrase = true,
                                                    Match = new Dictionary<string, object> { { nameof(ChatInfo.chatIndex), chatIndex } }
                                                }, 1);

            if (chatInfo.List.Count == 0)
            {
                var chatModel = new ChatInfo();
                chatModel.chatIndex = chatIndex;
                chatModel.name = message;
                chatModel.kms = kmsModel;
                chatModel.isNL2Sql = kmsModel.Exists(a => a.isNL2Sql == true);
                chatModel.total = 1;
                chatModel.beginTime = DateTime.Now;
                chatModel.dbInfo = dbInfo;
                var dic = JsonConvert.SerializeObject(chatModel).ToStr().JsonToDic(true);
                dic.Remove("beginTime");
                dic.Add("beginTime", chatModel.beginTime.ToString("yyyy-MM-dd HH:mm:ss"));

                elasticsearch.Add(AppSetting.ChatInfoIndex, Guid.NewGuid().ToStr(), dic);
            }

            if (chatInfo.IsSuccess && chatInfo.List.Count > 0)
            {
                var total = chatInfo.List[0].GetValue("total").ToStr().ToInt(0);
                total++;

                var id = chatInfo.List[0].GetValue("_id").ToStr();

                elasticsearch.Update(AppSetting.ChatInfoIndex, id, new { EndTime = DateTime.Now, Total = total });
            }
        }

        private string Chat(string message, ChatRecordModel kmsChatModel, List<string> history)
        {
            var chatData = Chat(message, history);
            if (chatData.IsSuccess)
            {
                kmsChatModel.endTime = DateTime.Now;
                kmsChatModel.response = chatData.ChatData;
                return kmsChatModel.response;
            }
            else
            {
                kmsChatModel.response = chatData.Exception;
                return kmsChatModel.response;
            }
        }

        public string NL2Sql(string message, DbInfo dbInfo, string chatIndex)
        {
            var msg = string.Empty;
            var chatRecordModel = new ChatRecordModel { beginTime = DateTime.Now };
            var config = AppSetting.DataConfig.Find(a => a.key == dbInfo.key);
            var template = string.Format(AppSetting.NL2SqlTemplate, message, dataService.TableSql(dbInfo.key, dbInfo.tableName), config.dbType);

            chatRecordModel.isNL2Sql = true;
            chatRecordModel.request = message;
            chatRecordModel.model = AppSetting.NL2SqlModel;

            var chat = ollamaRepository.Chat(new FastOllama.Core.Model.ChatModel { content = template, model = AppSetting.NL2SqlModel });

            if (chat.ChatData.IndexOf("```sql") >= 0)
                chat.ChatData = chat.ChatData.Substring(chat.ChatData.IndexOf("```sql"), chat.ChatData.Length - chat.ChatData.IndexOf("```sql"));
            if (chat.ChatData.LastIndexOf("```") >= 0)
                chat.ChatData = chat.ChatData.Substring(0, chat.ChatData.LastIndexOf("```"));

            chat.ChatData = chat.ChatData.Replace("```sql", string.Empty).Replace("```", string.Empty).Replace("\n", " ").Replace(";", string.Empty);
            chatRecordModel.vectorContent = chat.ChatData;
            if (chat.IsSuccess)
            {
                var nl2Data = dataService.NL2Data(dbInfo.key, chat.ChatData, dbInfo.tableName, 10, dbInfo.isView);

                if (nl2Data.Count > 0)
                {
                    var data = new List<string>();

                    nl2Data.ForEach(a =>
                    {
                        data.Add(JsonConvert.SerializeObject(a));
                    });

                    chat = ollamaRepository.Chat(new FastOllama.Core.Model.ChatModel { content = message, model = AppSetting.LLmModel, history = data });

                    chatRecordModel.endTime = DateTime.Now;
                    chatRecordModel.response = chat.IsSuccess ? chat.ChatData : chat.Exception;
                    msg = chatRecordModel.response;
                }
                else
                    msg = string.Format(AppSetting.ChatResult, message);

                chatRecordModel.nL2Sql = chat.ChatData;
            }
            else
                msg = string.Format(AppSetting.ChatResult, message);

            chatRecordModel.endTime = DateTime.Now;
            chatRecordModel.response = msg;

            var kmsModel = new List<KmsModel>();
            kmsModel.Add(new KmsModel { isNL2Sql = true, dateTime = DateTime.Now, name = $"{dbInfo.key}.{string.Join(" ", dbInfo.tableName)}" });
            AddChatRecord(chatRecordModel, chatIndex, message, kmsModel, dbInfo);

            return msg;
        }

        public string ChatIndex(string index)
        {
            index = string.IsNullOrEmpty(index) ? Guid.NewGuid().ToString() : index;

            if (index.IndexOf("chat") < 0)
                index = $"chat-{index}";

            return index;
        }

        private LLMResponse Chat(string message, List<string> history)
        {
            return ollamaRepository.Chat(new FastOllama.Core.Model.ChatModel { content = message, history = history, model = AppSetting.LLmModel });
        }

        public PageResult Page(RequestPage page)
        {
            var data = elasticsearch.Page(page.pageSize, page.pageId, AppSetting.ChatInfoIndex,
                                            new QueryModel
                                            {
                                                Sort = new Dictionary<string, object> { { nameof(ChatInfo.beginTime), "desc" } }
                                            });
            return data.PageResult;
        }

        public EsResponse GetChatRecord(ChatInfo model)
        {
            return elasticsearch.GetList(model.chatIndex, new QueryModel(), 100);
        }

        public EsResponse DeleteChatRecord(Dictionary<string, object> dic, string _id)
        {
            var chatInfo = elasticsearch.GetItem(dic.GetValue("ChatIndex").ToStr(), _id);
            if (chatInfo.IsSuccess && chatInfo.List.Count == 0)
                return new EsResponse { IsSuccess = false, Exception = new Exception($"未找到{dic.GetValue("name").ToStr()}：{dic.GetValue("ChatIndex").ToStr()}：{_id}数据") };
            else
            {
                var total = chatInfo.List[0].GetValue("total").ToStr().ToInt(0);
                total--;
                elasticsearch.Update(AppSetting.ChatInfoIndex, dic.GetValue("_id").ToStr(), new { Total = total });

                return new EsResponse();
            }
        }

        public EsResponse DeleteChat(Dictionary<string, object> dic)
        {
            var result = elasticsearch.Delete(AppSetting.ChatInfoIndex, new List<string> { dic.GetValue("_id").ToStr() });
            if (result.IsSuccess)
                return elasticsearch.Delete(dic.GetValue("ChatIndex").ToStr());
            else
                return result;
        }
    }

    public interface IChatService
    {
        string Chat(string message, List<KmsModel> kmsModel, string chatIndex, List<string> history = null);
        string NL2Sql(string message, DbInfo dbInfo, string chatIndex);
        PageResult Page(RequestPage page);
        EsResponse GetChatRecord(ChatInfo model);
        EsResponse DeleteChatRecord(Dictionary<string, object> dic, string _id);
        EsResponse DeleteChat(Dictionary<string, object> dic);
        string ChatIndex(string index);
    }
}