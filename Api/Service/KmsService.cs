using FastAop.Core;
using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Model;
using FastKMSApi.Core.Request;
using FastOllama.Core;
using Newtonsoft.Json;

namespace FastKMSApi.Core.Service
{
    public class KmsService : IKmsService
    {
        [Autowired]
        private readonly IElasticsearchVector elasticsearchVector;

        [Autowired]
        private readonly IElasticsearch elasticsearch;

        [Autowired]
        private readonly IFastOllamaRepository ollamaRepository;

        public EsResponse Update(KmsModel model)
        {
            return elasticsearch.Update(model._index, model._id, new { name = model.name, remark = model.remark });
        }

        public PageResult Page(RequestPage page)
        {
            var data = elasticsearch.Page(page.pageSize, page.pageId, AppSetting.KmsIndex,
                 new QueryModel
                 {
                     Sort = new Dictionary<string, object> { { nameof(KmsModel.dateTime), "desc" } }
                 });

            if (data.IsSuccess)
                return data.PageResult;
            else
                return new PageResult();
        }

        public List<KmsModel> List()
        {
            var list = new List<KmsModel>();
            var data = new FastElasticsearch.Core.Elasticsearch().GetList(AppSetting.KmsIndex, new QueryModel(), 999);
            if (data.IsSuccess)
            {
                data.List.ForEach(a =>
                {
                    var json = JsonConvert.SerializeObject(a);
                    var model = JsonConvert.DeserializeObject<KmsModel>(json) ?? new KmsModel();
                    model._id = a.GetValue("id").ToStr();
                    model._index = a.GetValue("index").ToStr();
                    list.Add(model);
                });
            }

            return list;
        }
    }

    public interface IKmsService
    {
        List<KmsModel> List();
        PageResult Page(RequestPage page);
        EsResponse Update(KmsModel model);
    }
}
