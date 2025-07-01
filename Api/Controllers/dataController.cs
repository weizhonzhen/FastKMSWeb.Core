using FastAop.Core;
using FastElasticsearch.Core.Model;
using FastKMSApi.Core.Model;
using FastKMSApi.Core.Request;
using FastKMSApi.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FastKMSApi.Core.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class dataController : ControllerBase
    {
        [Autowired]
        private readonly IDataService dataService;

        [Autowired]
        private readonly IVectorService vectorService;

        [HttpGet]
        public PageResult tableList([FromQuery] RequestPage page)
        {
            return dataService.TableList(page);
        }

        [HttpGet]
        public PageResult viewList([FromQuery] RequestPage page)
        {
            return dataService.ViewList(page);
        }

        [HttpGet]
        public List<string> dataList()
        {
            return AppSetting.DataConfig.Select(a=>a.key).ToList();
        }

        [HttpGet]
        public List<ColumnInfo> columnList([FromQuery] ColumnModel model)
        {
           return dataService.ColumnList(model);
        }

        [HttpPost]
        public EsResponse updateTabComments([FromBody] TableModel table)
        {
            var result = new EsResponse();
            result.IsSuccess = dataService.UpdateTabComments(table);
            return result;
        }

        [HttpPost]
        public EsResponse UpdateColComments([FromForm] ColumnModel table, [FromForm] ColumnInfo column)
        {
            var result = new EsResponse();
            result.IsSuccess = dataService.UpdateColComments(table.key, table.tableName, column, table.isView);
            return result;
        }
    }
}
