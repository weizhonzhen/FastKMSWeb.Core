using FastAop.Core;
using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Request;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace FastKMSApi.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class mcpController : ControllerBase
    {
        [Autowired]
        private readonly IElasticsearch elasticsearch;

        [Autowired]
        private readonly McpService mcService;

        [HttpGet]
        public PageResult page([FromQuery] RequestPage page)
        {
            return mcService.Page(page);
        }

        [HttpPost]
        public EsResponse Update([FromBody]McpModel model)
        {
            return mcService.Update(model);
        }

        [HttpPost]
        public EsResponse Add([FromBody]McpModel model)
        {
            return mcService.Add(model);
        }

        [HttpPost]
        public EsResponse Delete([FromBody]McpModel model)
        {
            return mcService.Delete(model);
        }
    }
}
