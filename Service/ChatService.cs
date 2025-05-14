using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSWeb.Core.Model;
using FastOllama.Core;
using FastOllama.Core.Model;
using Newtonsoft.Json;

namespace FastKMSWeb.Core.Service
{
    public class ChatService
    {
        private readonly IElasticsearchVector elasticsearchVector;
        private readonly IElasticsearch elasticsearch;
        private readonly IFastOllamaRepository ollamaRepository;
        public ChatService(IElasticsearchVector _elasticsearchVector, IFastOllamaRepository _ollamaRepository
                            , IElasticsearch _elasticsearch)
        {
            elasticsearchVector = _elasticsearchVector;
            ollamaRepository = _ollamaRepository;
            elasticsearch = _elasticsearch;
        }

        public string Prompt(string message, List<KmsModel> kmsModel, string chatIndex, List<string> history = null)
        {
            var msg = string.Empty;
            chatIndex = chatIndex ?? Guid.NewGuid().ToString();

            if (chatIndex.IndexOf("chat") < 0)
                chatIndex = $"chat-{chatIndex}";

            var chatRecordModel = new ChatRecordModel {beginTime = DateTime.Now, request = message };
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

                var data = elasticsearchVector.QueryVector(kmsModel.Select(a => a.VectorIndex).ToList(), new VectorQuery
                {
                    Match = new Dictionary<string, object> { { AppSetting.FieldKey, message } },
                    Data = field.ToArray(),
                    Fields = new List<string> { AppSetting.FieldKey }
                }).List;

                if (data.Count > 0)
                {
                    var content = data.Select(a => a.GetValue("text").ToStr()).ToList();
                    var template = string.Format(AppSetting.PromptTemplate, string.Join("\n\r", content));

                    chatRecordModel.isPrompt = true;
                    chatRecordModel.vectorContent = template;
                    chatRecordModel.model = AppSetting.LLmModel;

                    var promptData = ollamaRepository.Prompt(new PromptModel { content = template, model = AppSetting.LLmModel });

                    chatRecordModel.endTime = DateTime.Now;
                    chatRecordModel.response = promptData.IsSuccess ? promptData.PromptData : promptData.Exception;
                    msg = chatRecordModel.response;
                }
                else
                {
                    chatRecordModel.model = AppSetting.LLmModel;
                    msg = Chat(message, chatRecordModel, history);
                }
            }
            else
                msg = Chat(message, chatRecordModel, history);

            var json = JsonConvert.SerializeObject(chatRecordModel).ToStr();
           
            elasticsearch.Create<ChatRecordModel>(chatIndex);
                        
            elasticsearch.Add(chatIndex, Guid.NewGuid().ToStr(), json.JsonToDic(true));

            var chatInfo = elasticsearch.GetList(AppSetting.ChatInfoIndex, new QueryModel { IsPhrase = true, Match = new Dictionary<string, object> { { nameof(ChatInfo.ChatIndex), chatIndex } } }, 1);
            if (chatInfo.List.Count == 0)
            {
                var chatModel = new ChatInfo();
                chatModel.ChatIndex = chatIndex;
                chatModel.Name = message;
                chatModel.Kms = kmsModel;
                chatModel.Total = 1;
                chatModel.BeginTime = DateTime.Now;
                json = JsonConvert.SerializeObject(chatModel).ToStr();
                elasticsearch.Add(AppSetting.ChatInfoIndex, Guid.NewGuid().ToStr(), json.JsonToDic(true));
            }

            if (chatInfo.IsSuccess && chatInfo.List.Count > 0)
            {
                var total = chatInfo.List[0].GetValue("total").ToStr().ToInt(0);
                total++;

                var id = chatInfo.List[0].GetValue("_id").ToStr();

                elasticsearch.Update(AppSetting.ChatInfoIndex, id, new { EndTime = DateTime.Now, Total = total });
            }

            if (!chatInfo.IsSuccess)
            {
                var total = chatInfo.List[0].GetValue("total").ToStr().ToInt(0);
                total++;

                var id = chatInfo.List[0].GetValue("_id").ToStr();

                elasticsearch.Update(AppSetting.ChatInfoIndex, id, new { EndTime = DateTime.Now, Total = total });
            }

            return msg;
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

        private LLMResponse Chat(string message, List<string> history)
        {
            var template = string.Format(AppSetting.ChatTemplate, message);
            return ollamaRepository.Chat(new ChatModel { content = template, history = history, model = AppSetting.LLmModel });
        }

        public PageResult GetPage(int pageId, int pageSize)
        {
            var data = elasticsearch.Page(pageSize, pageId, AppSetting.ChatInfoIndex, new QueryModel());
            return data.PageResult;
        }

        public EsResponse GetChatRecord(ChatInfo model)
        {
            return elasticsearch.GetList(model.ChatIndex, new QueryModel(), 100);
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
}