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
    }
}