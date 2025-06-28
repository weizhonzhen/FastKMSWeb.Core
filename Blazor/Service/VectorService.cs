using FastElasticsearch.Core;
using FastElasticsearch.Core.Model;
using FastKMSWeb.Core.Model;
using FastOllama.Core;
using FastOllama.Core.Model;
using Newtonsoft.Json;

namespace FastKMSWeb.Core.Service
{
    public class VectorService
    {
        private readonly IElasticsearchVector elasticsearchVector;
        private readonly IElasticsearch elasticsearch;
        private readonly IFastOllamaRepository ollamaRepository;
        public VectorService(IElasticsearchVector _elasticsearchVector, IFastOllamaRepository _ollamaRepository
                            , IElasticsearch _elasticsearch)
        {
            elasticsearchVector = _elasticsearchVector;
            ollamaRepository = _ollamaRepository;
            elasticsearch = _elasticsearch;
        }

        public EsResponse AddVectorData(Dictionary<string, object> data, KmsModel kmsModel)
        {
            var llms = new List<LLMResponse>();
            var pageId = 1;
            var pageSize = 100;

            var query = data.Select(a => a.Value.ToStr());
            var list = query.Skip(pageSize * (pageId - 1)).Take(pageSize).ToList();

            while (list.Count > 0)
            {
                var lLMResponse = ollamaRepository.Embed(new EmbedModel { content = list});
                lLMResponse.EmbedData.ForEach(item =>
                {
                    foreach (var keyValue in item)
                    {
                        var vectorData = new VectorData();
                        vectorData.Field = new Dictionary<string, object> { { AppSetting.FieldKey, keyValue.Key } };
                        vectorData.Data = keyValue.Value.ToArray();
                        elasticsearchVector.AddVectorData(kmsModel.vectorIndex, vectorData);
                    }
                });

                llms.Add(lLMResponse);
                pageId++;
                list = query.Skip(pageSize * (pageId - 1)).Take(pageSize).ToList();
            }

            if (!llms.Exists(a => !a.IsSuccess))
                return new EsResponse() { IsSuccess = true };
            else
            {
                DeleteVector(kmsModel.vectorIndex, kmsModel.index);
                return new EsResponse { Exception = new Exception(string.Join("\n\r", llms.FindAll(a => !a.IsSuccess).Select(a => a.Exception))) };
            }
        }

        public EsResponse CreateVector(KmsModel kmsModel, VectorModel vectorModel)
        {
            vectorModel.Field = new Dictionary<string, object> { { AppSetting.FieldKey, "text" } };
            var result = elasticsearchVector.CreateVector(kmsModel.vectorIndex, vectorModel);

            if (result.IsSuccess)
            {
                var json = JsonConvert.SerializeObject(kmsModel).ToStr();

                if (elasticsearch.GetList(AppSetting.KmsIndex, new QueryModel()).List.Count == 0)
                    elasticsearch.Create<KmsModel>(AppSetting.KmsIndex);

                return elasticsearch.Add(AppSetting.KmsIndex, kmsModel.index, json.JsonToDic(true));
            }
            else
                return result;
        }

        public EsResponse DeleteVector(string vectorIndex,string index)
        {
            var data = elasticsearchVector.DeleteVector(vectorIndex);
            if (data.IsSuccess)
                return elasticsearch.Delete(AppSetting.KmsIndex, new List<string> { index });
            else
                return data;
        }

        public PageResult GetPage(string vectorIndex, int pageId, int pageSize)
        {
            var data = elasticsearch.Page(pageSize, pageId, vectorIndex, new QueryModel());
            return data.PageResult;
        }

        public EsResponse DeleteVectorItem(Dictionary<string,object> item)
        {
            return elasticsearch.Delete(item.GetValue("_index").ToStr(), new List<string> { item.GetValue("_id").ToStr() });
        }

        public EsResponse UpdateVectorItem(Dictionary<string, object> item)
        {
            var value = item.GetValue(AppSetting.FieldKey).ToStr();
            var lLMResponse = ollamaRepository.Embed(new EmbedModel { content =  new List<string> { value } });

            var name = item.Keys.First(a => a != AppSetting.FieldKey);

            var kmsModel = new KmsModel
            {
                dateTime = DateTime.Now,
                index = Guid.NewGuid().ToStr(),
                name = name,
                remark = name,
                vectorIndex = item.GetValue("_index").ToStr()
            };

            lLMResponse.EmbedData.ForEach(item =>
            {
                foreach (var keyValue in item)
                {
                    var vectorData = new VectorData();
                    vectorData.Field = new Dictionary<string, object> { { AppSetting.FieldKey, keyValue.Key } };
                    vectorData.Data = keyValue.Value.ToArray();
                    elasticsearchVector.AddVectorData(kmsModel.vectorIndex, vectorData);
                }
            });


            if (lLMResponse.IsSuccess)
            {
                DeleteVectorItem(item);
                return new EsResponse() { IsSuccess = true };
            }
            else
                return new EsResponse { Exception = new Exception(lLMResponse.Exception) };;
        }
    }
}