using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace myfirstbot
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
            /*
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //*/

            services.AddBot<MyBot>(options => {
                //ログ出力ミドルウェア
                options.Middleware.Add(new MyLoggingMiddleware());
                //添付ファイル拒否ミドルウェア
                options.Middleware.Add(new MyMiddleware());

                ///*
                //ストレージとしてインメモリを利用
                IStorage dataStore = new MemoryStorage();
                //それぞれのステートを作成
                var userState = new UserState(dataStore);
                var conversationState = new ConversationState(dataStore);
                options.State.Add(userState);
                options.State.Add(conversationState);
                //*/
            });

            //MyStateAccessorをIoC（Inversion of Control）に登録
            //　IoCというコンテナがあるらしい（謎
            services.AddSingleton( sp =>
            {
                // AddBotで登録したoptionsを取得
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptionsを事前に構成してください。");
                }
                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserStateを事前に定義してください。");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationStateを事前に定義してください。");
                }

                var accessors = new MyStateAccessors(userState, conversationState)
                {
                    //DialogStateをConversationStateのプロパティとして設定（意味不
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    UserProfile = userState.CreateProperty<UserProfile>("UserProfile")
                };

                return accessors;
            });

            //services.AddBot<MyBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            /*
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            //*/

            app.UseBotFramework();
        }
    }
}
