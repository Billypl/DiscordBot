using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace TestBot_DSharp
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }


        private DiscordConfiguration clientConfig;
        private CommandsNextConfiguration commandsConfig;
        private InteractivityConfiguration interactivityConfig;

        public async Task RunAsync()
        {
            Scripts.ConfigReader.readConfigs(ref clientConfig, ref commandsConfig, ref interactivityConfig);

            Client = new DiscordClient(clientConfig);
            Commands = Client.UseCommandsNext(commandsConfig);
            Client.UseInteractivity(interactivityConfig);
            Client.Ready += OnClientReady;

            Commands.RegisterCommands<Commands.AnswerCommands>();
            Commands.RegisterCommands<Commands.RoleCommands>();

            Client.GuildMemberAdded += onUserJoin;

            await Client.ConnectAsync();
            await Task.Delay(-1); // prevents auto-disconnecting
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