using System.Net;
using System.Net.Http;
using Kin.Stellar.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace KinExplorerCreateAccount
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (bool.TryParse(Configuration["Swagger_Enabled"], out var ret) && ret)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info {Title = "KinExplorer Friendbot API", Version = "v1"});
                    c.DescribeAllEnumsAsStrings();
                    c.DescribeStringEnumsInCamelCase();
                });
            }

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://kinexplorer.com",
                            "http://kinexplorer.io","https://kinexplorer.com",
                        "https://kinexplorer.io");
                    });
            });

            Server server = new Server(Configuration["Horizon_Url"]);
            Network.UsePublicNetwork();
            Network.Use(new Network(Configuration["HorizonNetwork_Id"]));


            services.AddSingleton(server);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (bool.TryParse(Configuration["Swagger_Enabled"], out var ret) && ret)
            {
                app.UseStaticFiles();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KinExplorer Friendbot API");

                });

            }
            app.UseCors(MyAllowSpecificOrigins);

           // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
