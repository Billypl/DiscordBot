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


        private readonly DiscordConfiguration clientConfig = new DiscordConfiguration
        {
            Token = "secret_bot_token",
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug
        };

        private readonly CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
        {
            StringPrefixes = new string[] { "!zm " },
            EnableMentionPrefix = true,
            EnableDms = false
        };

        private readonly InteractivityConfiguration interactivityConfig = new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(5)
        };

        public async Task RunAsync()
        {
            Client = new DiscordClient(clientConfig);
            Commands = Client.UseCommandsNext(commandsConfig);
            Client.UseInteractivity(interactivityConfig);
            Client.Ready += OnClientReady;

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
