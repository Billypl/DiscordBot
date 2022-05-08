using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using static DSharpPlus.Entities.DiscordEmbedBuilder;
using TestBot_DSharp.Configs;

namespace TestBot_DSharp
{
    public class Bot
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        public static InteractivityExtension Interactivity { get; set; }


        public async Task RunAsync()
        {
            ConfigManager.assignConfigs();
            Client.Ready += OnClientReady;
            
            Commands.RegisterCommands<Commands.AnswerCommands>();
            Commands.RegisterCommands<Commands.RoleCommands>();

            Client.GuildMemberAdded += onUserJoin;

            await Client.ConnectAsync();
            await Task.Delay(-1); // prevents auto-disconnecting
        }

        private async Task onUserJoin(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            // TODO: Make sure that user haven't been earlier in this guild
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