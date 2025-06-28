using FastElasticsearch.Core.Model;

namespace FastKMSWeb.Core.Model
{
    public class KmsPageModel
    {
        private PageResult _pageResult;
        public int pageSize { get; set; } = 10;
        public int pageId { get; set; } = 1;

        public PageResult pageResult
        {
            get { return _pageResult; }
            set
            {
                _pageResult = value;
                InitPageId();
            }
        }

        public List<int> ListPage { get; set; } = new List<int>();

        private void InitPageId()
        {
            var startId = (pageResult.Page.PageId - 6) <= 0 ? 1 : (pageResult.Page.PageId - 6);

            var endId = startId + 6;
            if (endId > pageResult.Page.TotalPage)
            {
                endId = pageResult.Page.TotalPage;
                if ((endId - 6) > 0)
                { startId = endId - 6; }
            }

            ListPage = new List<int>();
            for (var i = startId; i <= endId; i++)
            {
                ListPage.Add(i);
            }
        }
    }
}
