using FastKMSApi.Core.Service;

namespace FastKMSApi.Core.Jwt
{
    public class JwtHandler
    {
        private readonly RequestDelegate next;
        public JwtHandler(RequestDelegate request)
        {
            next = request;
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (context.GetEndpoint()?.Metadata.Count > 0)
            {
                var filterAction = new List<string> { "login", "loginOut" };
                var action = context.Request.RouteValues["action"].ToStr();
                var token = context.Request.Headers.Authorization.Count > 0 ? context.Request.Headers.Authorization[0] : string.Empty;
                if (!AppSetting.User.ContainsKey(token) && !filterAction.Contains(action))
                {
                    context.Response.StatusCode = 401;
                    return context.Response.WriteAsync(string.Empty);
                }
            }
            return next(context);
        }
    }
}