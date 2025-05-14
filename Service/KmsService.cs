using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSWeb.Core.Model;
using FastOllama.Core;
using Newtonsoft.Json;

namespace FastKMSWeb.Core.Service
{
    public class KmsService
    {
        private readonly IElasticsearchVector elasticsearchVector;
        private readonly IElasticsearch elasticsearch;
        private readonly IFastOllamaRepository ollamaRepository;
        public KmsService(IElasticsearchVector _elasticsearchVector, IFastOllamaRepository _ollamaRepository
                            , IElasticsearch _elasticsearch)
        {
            elasticsearchVector = _elasticsearchVector;
            ollamaRepository = _ollamaRepository;
            elasticsearch = _elasticsearch;
        }

        public EsResponse UpdateKms(KmsModel model)
        {
            return elasticsearch.Update(model._index, model._id, new { Name = model.Name, Remark = model.Remark });
        }

        public PageResult GetKmsPage(int pageId, int pageSize)
        {
            var data = elasticsearch.Page(pageSize, pageId, AppSetting.KmsIndex, new QueryModel());

            if (data.IsSuccess)
                return data.PageResult;
            else
                return new PageResult();
        }

        public List<KmsModel> GetKmsList()
        {
            var list = new List<KmsModel>();
            var data = elasticsearch.GetList(AppSetting.KmsIndex, new QueryModel(), 999);
            if (data.IsSuccess)
            {
                data.List.ForEach(a =>
                {
                    var json = JsonConvert.SerializeObject(a);
                    var model = JsonConvert.DeserializeObject<KmsModel>(json);
                    model._id = a.GetValue("id").ToStr();
                    model._index = a.GetValue("index").ToStr();
                    list.Add(model);
                });
            }

            return list;
        }
    }
}
