using FastKMSWeb.Core.Model;

namespace FastKMSWeb.Core.Service
{
    public static class AppSetting
    {
        public static readonly string FieldKey = "text";

        public static readonly string KmsIndex = "kmsIndex";

        public static readonly string PromptTemplate = Extension.GetConfig("PromptTemplate");

        public static readonly string ChatInfoIndex = "chatIndex";

        public static readonly string LLmModel = Extension.GetConfig("LLmModel");

        public static readonly string ChatTemplate = Extension.GetConfig("ChatTemplate");

        public static readonly List<DataConfig> DataConfig = Extension.GetConfig<DataConfig>("DataConfig");

        public static readonly string NL2SqlModel = Extension.GetConfig("NL2SqlModel");

        public static readonly string NL2SqlTemplate = Extension.GetConfig("NL2SqlTemplate");

        public static readonly string ChatResult = Extension.GetConfig("ChatResult");
    }
}