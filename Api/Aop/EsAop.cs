﻿using FastElasticsearch.Core.Aop;

namespace FastKMSApi.Core.Aop
{
    public class EsAop : IAop
    {
        public void After(AfterContext context)
        {
            if (!context.IsSuccess)
            {

            }
        }

        public void Before(BeforeContext context)
        {

        }
    }
}