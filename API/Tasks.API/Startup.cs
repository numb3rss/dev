using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.AzureBatchAccess;
using Tasks.ContainerInstanceAccess;
using Tasks.BO;
using Tasks.BP.Commands;
using Tasks.BP.Validators;
using Tasks.StorageQueueAccess;
using Swashbuckle.AspNetCore.Swagger;

namespace Tasks.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IGetTask, GetTask>();
            services.AddSingleton<IAddTask, AddTask>();
            services.AddSingleton<IHandleAzureBatch, HandleAzureBatch>();
            services.AddSingleton<ICreateContainerInstance, CreateContainerInstance>();

            services.AddSingleton<IGetTaskValidator, GetTaskValidator>();
            services.AddSingleton<IStorageTaskValidator, StorageTaskValidator>();

            services.AddSingleton<IStorageQueueService, StorageQueueService>();
            services.AddSingleton<IAzureBatchService, AzureBatchService>();
            services.AddSingleton<IContainerInstanceProvider, ContainerInstanceProvider>();

            services.Configure<AppSettingsModel>(Configuration.GetSection("ApplicationSettings"));

            services.AddOptions();

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Tasks HTTP API",
                    Version = "v1",
                    Description = "The Tasks Microservice HTTP API which let you manage task in azure storage queue",
                    TermsOfService = "Terms Of Service"
                });
            });

            var container = new ContainerBuilder();
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger()
                .UseSwaggerUI(swa =>
                {
                    swa.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasks.API V1");
                });
        }
    }
}
