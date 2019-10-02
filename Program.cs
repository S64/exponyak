using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S64.Bot.Builder.Adapters.Slack;

namespace ExPonyak
{

    public class Program
    {

        public static string VerificationToken
        {
            get { return Environment.GetEnvironmentVariable("EXPONYAK_SLACK_VERIFICATION_TOKEN"); }
        }

        public static string BotToken
        {
            get { return Environment.GetEnvironmentVariable("EXPONYAK_SLACK_BOT_USER_TOKEN"); }
        }

        public static string Mode
        {
            get { return Environment.GetEnvironmentVariable("EXPONYAK_MODE"); }
        }

        public static string ExciteApiKey
        {
            get { return Environment.GetEnvironmentVariable("EXPONYAK_EXCITE_API_KEY"); }
        }

        public static SlackOptions Options
        {
            get
            {
                return new SlackOptions
                {
                    BotUserToken = BotToken,
                };
            }
        }

        public static void Main(string[] args)
        {
            if ("events".Equals(Mode) || "events".Equals(args[0]))
            {
                WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>().Build().Run();
            }
            else if ("rtm".Equals(Mode) || "rtm".Equals(args[0]))
            {
                var adapter = new SlackAdapter(Program.Options)
                    .Use(new SlackMessageTypeMiddleware());

                var bot = new ExPonyakBot();

                adapter.ProcessActivityBySocketAsync(async (turnContext, cancellationToken) =>
                {
                    await bot.OnTurnAsync(turnContext, cancellationToken);
                }).Wait();
            }
            else
            {
                throw new ArgumentException();
            }
        }

    }

}
