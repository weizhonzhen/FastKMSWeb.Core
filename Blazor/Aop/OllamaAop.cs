using FastOllama.Core.Aop;

namespace FastKMSWeb.Core.Aop
{
    public class OllamaAop : IFastOllamaAop
    {
        public void Chat(ChatContext context)
        {
            if (!context.IsSuccess)
            {
                
            }
        }

        public void Embed(EmbedContext context)
        {
            if (!context.IsSuccess)
            {

            }
        }

        public void Exception(ExceptionContext context)
        {

        }

        public void FunctionCall(FunctionCallContext context)
        {
            if (!context.IsSuccess)
            {

            }
        }

        public void Prompt(PromptContext context)
        {
            if (!context.IsSuccess)
            {

            }
        }
    }
}