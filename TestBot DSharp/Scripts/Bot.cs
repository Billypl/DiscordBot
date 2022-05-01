using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

namespace TestBot_DSharp
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var clientConfig = new DiscordConfiguration
            {
                Token = "secret_bot_token",
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "!zm " },
                EnableMentionPrefix = true,
                EnableDms = false
            };
            var interactivityConfig = new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            Client = new DiscordClient(clientConfig);
            Client.Ready += OnClientReady;
            Commands = Client.UseCommandsNext(commandsConfig);
            Client.UseInteractivity(interactivityConfig);

            Commands.RegisterCommands<Commands.AnswerCommands>();
            Commands.RegisterCommands<Commands.RoleCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1); // prevents auto-disconnecting
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
}
