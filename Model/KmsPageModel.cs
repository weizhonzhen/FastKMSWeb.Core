﻿using FastElasticsearch.Core.Model;

namespace FastKMSWeb.Core.Model
{
    public class KmsPageModel
    {
        private PageResult _PageResult;
        public int PageSize { get; set; } = 10;
        public int PageId { get; set; } = 1;

        public PageResult PageResult
        {
            get { return _PageResult; }
            set
            {
                _PageResult = value;
                InitPageId();
            }
        }

        public List<int> ListPage { get; set; } = new List<int>();

        private void InitPageId()
        {
            var preId = (PageResult.Page.PageId - 1) <= 0 ? 1 : (PageResult.Page.PageId - 1);
            var startId = (PageResult.Page.PageId - 6) <= 0 ? 1 : (PageResult.Page.PageId - 6);

            var endId = startId + 6;
            if (endId > PageResult.Page.TotalPage)
            {
                endId = PageResult.Page.TotalPage;
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
