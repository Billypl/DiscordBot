using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace TestBot_DSharp
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }


        private readonly DiscordConfiguration clientConfig = new DiscordConfiguration
        {
            Token = "OTY0Nzk5MjIyOTY2ODEyNzUz.Ylp5Sw.fDwplQtd5ORwVVEHdb7TMJ4gXQ0",
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug,
            Intents = DiscordIntents.All // needed for RequestMembersAsync()
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

            Client.GuildMemberAdded += onUserJoin;
            //Client.MessageCreated += onSentMessage;

            await Client.ConnectAsync();
            await Task.Delay(-1); // prevents auto-disconnecting
        }

        private async Task onSentMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Author == sender.CurrentUser)
                return;

            var logMessage = new DiscordEmbedBuilder
            {
                Title = "Message has been sent",
                Color = DiscordColor.Red,
                Thumbnail = new EmbedThumbnail { Url = e.Message.Author.AvatarUrl },
                Timestamp = DateTime.Now,
            };
            logMessage.AddField("Message content", $"\"{e.Message.Content}\"", true);
            logMessage.AddField("Author", e.Message.Author.Username, true);

            const ulong LOG_CHANNEL_ID = 963372506260045855;
            await sender.SendMessageAsync(e.Guild.GetChannel(LOG_CHANNEL_ID), logMessage);
        }

        private async Task onUserJoin(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            // TODO: Make sure that user haven't been ealier in this guild
            var logMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = "New member joined!",
                Description = $"\"{e.Member.Username}\" has joined our server!",
                Thumbnail = new EmbedThumbnail { Url = e.Member.AvatarUrl},
                Timestamp = DateTime.Now
            };
            const ulong GREETINGS_CHANNEL_ID = 966853736582496266;
            await sender.SendMessageAsync(e.Guild.GetChannel(GREETINGS_CHANNEL_ID), logMessage);
            await TestBot_DSharp.Commands.RoleCommands.newMemberSetup(e.Guild, e.Member.Id);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
}
