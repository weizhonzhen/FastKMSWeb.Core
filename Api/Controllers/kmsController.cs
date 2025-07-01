using FastAop.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Model;
using FastKMSApi.Core.Request;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastKMSApi.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class kmsController : ControllerBase
    {
        [Autowired]
        private readonly IKmsService kmsService;

        [Autowired]
        private readonly IVectorService vectorService;

        [Autowired]
        private readonly IImpService impService;

        [HttpGet]
        public PageResult page([FromQuery] RequestPage page)
        {
            return kmsService.Page(page);
        }

        [HttpGet]
        public List<Model.KmsModel> list()
        {
            return kmsService.List();
        }

        [HttpPost]
        public EsResponse deleteVector([FromBody] vectorModel model)
        {
            return vectorService.DeleteVector(model);
        }

        [HttpPost]
        public EsResponse update([FromBody] Request.OptionModel model)
        {
            return vectorService.UpdateVectorItem(model);
        }

        [HttpPost]
        public async Task<Dictionary<string,object>> uploadFile(IFormFile formFile,[FromForm] Model.KmsModel model)
        {
            var result = new Dictionary<string, object>();
            var data = new Dictionary<string, object>();

            if (formFile == null || formFile.Length == 0)
            {
                result.Add("msg", "请选择文件");
                result.Add("success", false);

                return result;
            }

            if (formFile.FileName.EndsWith(".xls") || formFile.FileName.EndsWith(".xlsx"))
                data = await impService.ExcelAsync(formFile);
            else if (formFile.FileName.EndsWith(".doc") || formFile.FileName.EndsWith(".docx"))
                data = await impService.WordAsync(formFile);
            else if (formFile.FileName.EndsWith(".txt") || formFile.FileName.EndsWith(".json"))
                data = await impService.TextAync(formFile);
            else if (formFile.FileName.EndsWith(".pdf"))
                data = await impService.PdfAsync(formFile);
            else if (formFile.FileName.EndsWith(".ppt") || formFile.FileName.EndsWith(".pptx"))
                data = await impService.PptAsync(formFile);
            else if (formFile.FileName.EndsWith(".jpeg") || formFile.FileName.EndsWith(".jpg") || formFile.FileName.EndsWith(".png")
                         || formFile.FileName.EndsWith(".bmp") || formFile.FileName.EndsWith(".tiff"))
                data = await impService.ImageAsync(formFile);
            else
            {
                result.Add("msg", "导入失败：目前只支持.xls,.txt,.json,.xlsx,.pdf,.doc,.docx,.jpeg,.png,.jpg,.bmp,.tiff,.ppt,.pptx文件");
                result.Add("success", false);
                return result;
            }

            if (data.Count > 0)
            {
                var kmsModel = new Model.KmsModel
                {
                    dateTime = DateTime.Now,
                    index = Guid.NewGuid().ToStr(),
                    name = model.name ?? formFile.Name,
                    remark = model.remark ?? formFile.Name
                };

                var vectorResult = vectorService.CreateVector(kmsModel,
                    new VectorModel
                    {
                        Name = model.name,
                        Id = Guid.NewGuid().ToStr()
                    });

                if (!vectorResult.IsSuccess)
                {
                    result.Add("msg", $"导入失败：{vectorResult.Exception.Message}");
                    result.Add("success", false);
                }

                var llm = vectorService.AddVectorData(data, kmsModel);

                if (llm.IsSuccess)
                {
                    result.Add("msg", "导入成功");
                    result.Add("success", true);
                }
                else
                {
                    result.Add("msg", $"导入失败：{llm.Exception.Message}");
                    result.Add("success", false);
                }
            }
            else
            {
                result.Add("msg", "导入失败：未读取到数据");
                result.Add("success", false);
            }

            return result;
        }
    }
}
