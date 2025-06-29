﻿using FastKMSApi.Core.Model;
using System.Collections.Concurrent;

namespace FastKMSApi.Core.Service
{
    public static class AppSetting
    {
        public static readonly string FieldKey = "text";

        public static readonly string KmsIndex = "kmsIndex";

        public static readonly string AgentIndex = "agentIndex";

        public static readonly string McpIndex = "mcpIndex";

        public static readonly string ViewTableIndex = "view-Table-Index";
        public static readonly string ViewColumnIndex = "view-Column-Index";

        public static readonly string ChatInfoIndex = "chatIndex";

        public static readonly string LLmModel = Extension.GetConfig("LLmModel");

        public static readonly List<DataConfig> DataConfig = Extension.ConfigList<DataConfig>("DataConfig");

        public static readonly string NL2SqlModel = Extension.GetConfig("NL2SqlModel");

        public static readonly string NL2SqlTemplate = Extension.GetConfig("NL2SqlTemplate");

        public static readonly string ChatResult = Extension.GetConfig("ChatResult");

        public static readonly JwtModel Jwt = Extension.Config<JwtModel>("Jwt");

        public static ConcurrentDictionary<string, object> User { get; set; } = new ConcurrentDictionary<string, object>();
    }
}