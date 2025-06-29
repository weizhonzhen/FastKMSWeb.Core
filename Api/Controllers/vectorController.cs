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
        private readonly VectorService vectorService;
        private readonly IElasticsearchVector elasticsearchVector;
        private readonly IElasticsearch elasticsearch;
        
        public vectorController(VectorService vectorService, IElasticsearchVector elasticsearchVector
                                , IElasticsearch elasticsearch)
        {
            this.vectorService = vectorService;
            this.elasticsearchVector = elasticsearchVector;
            this.elasticsearch = elasticsearch;
        }

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