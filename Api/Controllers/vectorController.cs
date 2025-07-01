using FastAop.Core;
using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Request;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastKMSApi.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class vectorController : ControllerBase
    {
        [Autowired]
        private readonly IVectorService vectorService;

        [Autowired]
        private readonly IElasticsearchVector elasticsearchVector;

        [Autowired]
        private readonly IElasticsearch elasticsearch;
        
        [HttpGet]
        public PageResult page([FromQuery] RequestPage page)
        {
            return vectorService.GetPage(page);
        }

        [HttpPost]
        public EsResponse deleteVector([FromBody] vectorModel model)
        {
            return elasticsearch.Delete(model.index, new List<string> { model.id });
        }
    }
}