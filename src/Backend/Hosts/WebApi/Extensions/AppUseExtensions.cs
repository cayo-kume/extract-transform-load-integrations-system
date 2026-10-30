using Application.Shared.Middlewares;

namespace WebApi.Extensions
{
    public static class AppUseExtensions
    {
        public static IApplicationBuilder UseApplication(this IApplicationBuilder app)
        {
            app.UseMiddleware<APIExceptionHandlingMiddleware>();
            app.UseSwagger(c => { c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0; });
            //#if RELEASE
            app.UseSwaggerUI();
            //#else
            //app.UseSwaggerUI(c =>
            //{
            //    c.OAuthClientId("");
            //    c.OAuthClientSecret("");
            //});
            //#endif
            app.UseHttpsRedirection();
            app.UseAuthorization();

            //app.UseHangfireDashboard();
            return app;
        }
    }
}
