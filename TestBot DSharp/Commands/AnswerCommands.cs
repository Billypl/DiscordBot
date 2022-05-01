using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;

namespace TestBot_DSharp.Commands
{
    public class AnswerCommands : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong");
        }

        [Command("add")]
        [Description("Adds two numbers")]
        public async Task Add(
            CommandContext ctx, 
            [Description("First number")] int n1, 
            [Description("Second number")] int n2
            )
        {
            var result = n1 + n2;
            await ctx.Channel.SendMessageAsync(result.ToString());
        }

        [Command("respond")]
        [Description("Responds with content of message following this command")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel);
            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }

        [Command("poll")]
        [Description("Creates poll, with specified duration and title")]
        public async Task Poll(CommandContext ctx, 
            [Description("Duration of poll, after number specify time unit (s/m/h/d)")] TimeSpan duration, 
            [Description("Title for your poll")] params string[] title)
        {
            await ctx.Message.DeleteAsync();
            
            string pollTitle = string.Join(" ", title);

            DiscordMessage pollMessage = 
                await ctx.Channel.SendMessageAsync(
                    createPollEmbed(pollTitle, ctx.Member, duration));

            List<DiscordEmoji> reactionEmojis = pollEmojis(ctx);
            var result = await pollMessage.DoPollAsync(reactionEmojis, null, duration);
            var resultEmbed = createResultEmbed(pollTitle, ctx.Member, result);
            await ctx.Channel.SendMessageAsync(resultEmbed);
            
            await pollMessage.DeleteAsync();
        }
        private DiscordEmbedBuilder createPollEmbed(string pollTitle, DiscordMember author, TimeSpan duration)
        {
            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = pollTitle,
                Description = $"Poll ends at: \n " +
                    $"Date: {endDate(duration, "d")} \n " +
                    $"Hour: {endDate(duration, "HH:mm")}",
                Color = DiscordColor.Cyan,
                Author = new DiscordEmbedBuilder.EmbedAuthor { Name = author.DisplayName },
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = author.AvatarUrl }
            };
            return pollEmbed;
        }
        private DiscordEmbedBuilder createResultEmbed(string pollTitle, DiscordMember author, ReadOnlyCollection<PollEmoji> result)
        {
            var resultEmbed = new DiscordEmbedBuilder
            {
                Title = $"Results of \"{pollTitle}\" poll",
                Author = new DiscordEmbedBuilder.EmbedAuthor { Name = author.DisplayName },
                Description = string.Join("\n", result.Select(x => $"{x.Emoji}: {x.Total}")),
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = author.AvatarUrl }
            };
            return resultEmbed;
        }
        private List<DiscordEmoji> pollEmojis(CommandContext ctx)
        {
            var emojis = new List<DiscordEmoji>
            {
                DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"),
                DiscordEmoji.FromName(ctx.Client, ":x:")
            };
            return emojis;
        }
        private string endDate(TimeSpan duration, string dateFormat)
        {
            return (DateTime.Now + duration).ToString(dateFormat);
        }

    }
}
