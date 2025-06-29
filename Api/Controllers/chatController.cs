using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Model;
using FastKMSApi.Core.Request;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Util;

namespace FastKMSApi.Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class chatController : ControllerBase
    {
        private readonly DataService dataService;
        private readonly VectorService vectorService;
        private readonly ChatService chatService;
        private readonly IElasticsearch elasticsearch;

        public chatController(DataService dataService, VectorService vectorService,
                            ChatService chatService, IElasticsearch elasticsearch)
        {
            this.dataService = dataService;
            this.vectorService = vectorService;
            this.chatService = chatService;
            this.elasticsearch = elasticsearch;
        }

        [HttpGet]
        public PageResult page([FromQuery] RequestPage page)
        {
            return chatService.Page(page);
        }

        [HttpPost]
        public EsResponse deleteChat([FromForm] ChatModel chat)
        {
            var result = elasticsearch.Delete(AppSetting.ChatInfoIndex, new List<string> { chat._id });
            if (result.IsSuccess)
                return elasticsearch.Delete(chat.chatIndex);
            else
                return result;
        }

        [HttpGet]
        public EsResponse chatRecord([FromQuery] ChatModel chat)
        {
            var data = chatService.GetChatRecord(new ChatInfo { chatIndex = chat.chatIndex });

            data.List.ForEach(a =>
            {
                var request = a.GetValue("request").ToStr();
                a.Remove("request");
                request = request.Replace("\n", "<br/>");
                a.Add("request", request);

                var response = a.GetValue("response").ToStr();
                a.Remove("response");
                response = response.Replace("\n", "<br/>");
                a.Add("response", response);
            });
            return data;
        }

        [HttpPost]
        public Dictionary<string,object> nl2Sql([FromBody] Nl2SqlModel model)
        {
            var result = new Dictionary<string, object>();

            model.chatIndex = chatService.ChatIndex(model.chatIndex);
            chatService.NL2Sql(model.message, model.dbInfo, model.chatIndex);

            result.Add("IsSuccess", true);
            result.Add("chatIndex", model.chatIndex);
            return result;
        }

        [HttpPost]
        public Dictionary<string, object> chat([FromBody] ChatModel model)
        {
            var result = new Dictionary<string, object>();

            model.chatIndex = chatService.ChatIndex(model.chatIndex);
            chatService.Chat(model.message, model.kmsModel, model.chatIndex);

            result.Add("IsSuccess", true);
            result.Add("chatIndex", model.chatIndex);
            return result;
        }
    }
}
