using FastAop.Core;
using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Request;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Service
{
    public class McpService
    {
        [Autowired]
        private readonly IElasticsearchVector elasticsearchVector;

        [Autowired]
        private readonly IElasticsearch elasticsearch;

        public PageResult Page(RequestPage page)
        {
            var data = elasticsearch.Page(page.pageSize, page.pageId, AppSetting.McpIndex, new QueryModel());

            if (data.IsSuccess)
                return data.PageResult;
            else
                return new PageResult();
        }

        public EsResponse Update(McpModel model)
        {
            return elasticsearch.Update(model._index, model._id, new { name = model.name, description = model.description, parameters = JsonConvert.SerializeObject(model.parameters) });
        }

        public EsResponse Add(McpModel model)
        {
            var json = JsonConvert.SerializeObject(model).ToStr();
            return elasticsearch.Add(AppSetting.McpIndex, Guid.NewGuid().ToStr(), json.JsonToDic(true));
        }

        public EsResponse Delete(McpModel model)
        {
            return elasticsearch.Delete(AppSetting.McpIndex, new List<string> { model._id });
        }
    }
}