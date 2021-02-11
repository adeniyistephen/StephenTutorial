// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.11.1

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlurasightBot.Bots;
using PlurasightBot.Services;
using Microsoft.Bot.Builder.Azure;
using PlurasightBot.Dialogs;

namespace PlurasightBot
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
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Configure Services
            services.AddSingleton<BotServices>();

            ///ConfigureState
            ConfigureState(services);

            ConfigureDialog(services);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        public void ConfigureDialog(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            //create the storage we will be using for user and conversation state. (Memory is great for testing purpose)
            //services.AddSingleton<IStorage, MemoryStorage>();

            var storageAccount = "DefaultEndpointsProtocol=https;AccountName=stephentutorialstorage;AccountKey=mK3EIJ6ZGmy3LRVv6paRDCXwPjN3k8D00FaUDD9+MzgGqaowCu6z6NHTUA5d1Pa2OruGcu6wbDtwZ73XJJdKOw==;EndpointSuffix=core.windows.net";
            var storageContainer = "mystoragedata";

            services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));

            //Create the User state
            services.AddSingleton<UserState>();

            //Create the Conversation state
            services.AddSingleton<ConversationState>();

            //Create an Instance of the state service
            services.AddSingleton<StateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
