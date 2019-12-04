using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Daimler.Bot.PasswordReset.Bot.Bots;
using Daimler.Bot.PasswordReset.Bot.States;

namespace Daimler.Bot.PasswordReset.Bot
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
            services.AddControllers();

            // Create the credential provider to be used with the Bot Framework Adapter.  
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            // Create the Bot Framework Adapter.  
           // services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            services.AddSingleton<IAdapterIntegration, BotFrameworkHttpAdapter>();

            var storage = new MemoryStorage();

            // Create the User state passing in the storage layer.
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);


            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.  
            services.AddTransient<IBot, PasswordResetBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

           // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseBotFramework();
        }
    }
}
