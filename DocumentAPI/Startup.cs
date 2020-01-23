using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace DocumentAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private ILogger _logger;
        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);

            services.AddSingleton(httpClient);

            services.AddLogging(l =>
            {
                l.AddAWSProvider(_configuration.GetAWSLoggingConfigSection());
                l.SetMinimumLevel(LogLevel.Debug);
            });


            services.TryAddTransient<IQueryAppsServices, QueryAppsServices>();
            services.AddHttpClient<IQueryAppsServices, QueryAppsServices>();

            services.AddCors(options =>
                 {
                     options.AddPolicy("AllowAll",
                         builder =>
                         {
                             builder
                                 .AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials()
                                 .SetIsOriginAllowed((host) => true);
                         });
                 });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Document API", Version = "v1.0" });
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                c.IncludeXmlComments(xmlFile);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (env.IsProduction())
            {
                app.UseExceptionHandler(config =>
                {
                    config.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var ex = error?.Error;
                            var message = ex.Message;
                            var description = ex;

                            _logger.LogError("An Error Has Occurred", ex);

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                Message = message,
                                Description = description
                            }));

                        }
                    });
                });
            }

            app.UseMvc();
        }
    }
}
