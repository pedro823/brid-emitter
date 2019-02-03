using brid_emitter.Connectors;
using brid_emitter.Util;
using FoundationDB.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace brid_emitter
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter(true));
            });

            services.AddSingleton<IBridEmitterConnector, BridEmitterConnector>();
            services.AddSingleton<IIdGenerator, IdGenerator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Fdb.Start(600);
            var fdbConnection = Fdb.OpenAsync().GetAwaiter().GetResult();

            var emitterUrl = env.IsDevelopment() ? "localhost:5000" : "Insert production url here";
            
            FoundationDbConnector.Initialize(fdbConnection);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}