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
            var value = data.Select(b => b.Value.ToStr()).ToList();
            var lLMResponse = ollamaRepository.Embed(new EmbedModel { content = value });

            lLMResponse.EmbedData.ForEach(item =>
            {
                foreach (var keyValue in item)
                {
                    var vectorData = new VectorData();
                    vectorData.Field = new Dictionary<string, object> { { AppSetting.FieldKey, keyValue.Key } };
                    vectorData.Data = keyValue.Value.ToArray();
                    elasticsearchVector.AddVectorData(kmsModel.VectorIndex, vectorData);
                }
            });

            if (lLMResponse.IsSuccess)
                return new EsResponse() { IsSuccess = true };
            else
            {
                DeleteVector(kmsModel.VectorIndex, kmsModel.Index);
                return new EsResponse { Exception = new Exception(lLMResponse.Exception) };
            }
        }

        public EsResponse CreateVector(KmsModel kmsModel, VectorModel vectorModel)
        {
            vectorModel.Field = new Dictionary<string, object> { { AppSetting.FieldKey, "text" } };
            var result = elasticsearchVector.CreateVector(kmsModel.VectorIndex, vectorModel);

            if (result.IsSuccess)
            {
                var json = JsonConvert.SerializeObject(kmsModel).ToStr();
                return elasticsearch.Add(AppSetting.KmsIndex, kmsModel.Index, json.JsonToDic(true));
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
                DateTime = DateTime.Now,
                Index = Guid.NewGuid().ToStr(),
                Name = name,
                Remark = name,
                VectorIndex = item.GetValue("_index").ToStr()
            };

            lLMResponse.EmbedData.ForEach(item =>
            {
                foreach (var keyValue in item)
                {
                    var vectorData = new VectorData();
                    vectorData.Field = new Dictionary<string, object> { { AppSetting.FieldKey, keyValue.Key } };
                    vectorData.Data = keyValue.Value.ToArray();
                    elasticsearchVector.AddVectorData(kmsModel.VectorIndex, vectorData);
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