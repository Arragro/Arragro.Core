using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Arragro.Core.Web.Auth.IntegrationTests
{
    public class TestStartup
    {
        public TestStartup()
        {
        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc().AddApplicationPart(typeof(TestController).Assembly);

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options
                    .MapControllers();
            });
        }
    }
}
