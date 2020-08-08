using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuthenticationHandler.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                // register here typed HttpClient (SecuredServiceClient) to access secured api
                .AddHttpClient<SecuredServiceClient>(client =>
                {
                    client.BaseAddress = new Uri("http://secured-service-provider.com");
                })
                // Add identity provider credentials
                .AddAuthentication(
                    new ClientCredentials
                    {
                        ClientId = "your-client-id",
                        ClientSecret = "your-client-secret",
                        Scopes = "required,scopes,separated,by,comma"
                    }, 
                    "https://your-identity-provider.com");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
