using System;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MongoDB.Integrations.JsonDotNet.Converters;
using nc.Nexts;

namespace nc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;

                options.ClientErrorMapping[404].Link ="https://httpstatuses.com/404";
            }).AddJsonOptions(options =>
            {
                // Adds automatic json parsing to BsonDocuments.
                options.SerializerSettings.Converters.Add(new BsonArrayConverter());
                options.SerializerSettings.Converters.Add(new BsonMinKeyConverter());
                options.SerializerSettings.Converters.Add(new BsonBinaryDataConverter());
                options.SerializerSettings.Converters.Add(new BsonNullConverter());
                options.SerializerSettings.Converters.Add(new BsonBooleanConverter());
                options.SerializerSettings.Converters.Add(new BsonObjectIdConverter());
                options.SerializerSettings.Converters.Add(new BsonDateTimeConverter());
                options.SerializerSettings.Converters.Add(new BsonRegularExpressionConverter());
                options.SerializerSettings.Converters.Add(new BsonDocumentConverter());
                options.SerializerSettings.Converters.Add(new BsonStringConverter());
                options.SerializerSettings.Converters.Add(new BsonDoubleConverter());
                options.SerializerSettings.Converters.Add(new BsonSymbolConverter());
                options.SerializerSettings.Converters.Add(new BsonInt32Converter());
                options.SerializerSettings.Converters.Add(new BsonTimestampConverter());
                options.SerializerSettings.Converters.Add(new BsonInt64Converter());
                options.SerializerSettings.Converters.Add(new BsonUndefinedConverter());
                options.SerializerSettings.Converters.Add(new BsonJavaScriptConverter());
                options.SerializerSettings.Converters.Add(new BsonValueConverter());
                options.SerializerSettings.Converters.Add(new BsonJavaScriptWithScopeConverter());
                options.SerializerSettings.Converters.Add(new BsonMaxKeyConverter());
                options.SerializerSettings.Converters.Add(new ObjectIdConverter());
            });
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
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
                app.UseHsts();
            }
            app.UseCors("CorsPolicy");

            app.UseStaticFiles(); // For the wwwroot folder
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
            //    RequestPath = "/StaticFiles"
            //});

            app.UseMiddleware<Auth>();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
