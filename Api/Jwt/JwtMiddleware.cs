namespace FastKMSApi.Core.Jwt
{
    public static class JwtMiddleware
    {
        public static IApplicationBuilder UseJwt(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtHandler>();
        }
    }
}
