using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace TestBot_DSharp.Commands
{
    public class RoleCommands : BaseCommandModule
    {

        [Command("member")]
        [Description("Manages member role")]
        public async Task Member(CommandContext ctx)
        {
            var avatar = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl };
            var embed = new DiscordEmbedBuilder
            {
                Title = "Would you like to be a member?",
                Color = DiscordColor.Green,
                Thumbnail = avatar
            };
            var createdMessage = await ctx.Channel.SendMessageAsync(embed);

            var thumbsUp = DiscordEmoji.FromName(ctx.Client, ":+1:");
            var thumbsDown = DiscordEmoji.FromName(ctx.Client, ":-1:");

            await createdMessage.CreateReactionAsync(thumbsUp);
            await createdMessage.CreateReactionAsync(thumbsDown);

            var interactivity = ctx.Client.GetInteractivity();
            var reactionResult = await interactivity.WaitForReactionAsync(x =>
                x.Channel == ctx.Channel &&
               (x.Emoji == thumbsUp || x.Emoji == thumbsDown) &&
                x.User == ctx.User);

            const long roleID = 964164476507140127;
            DiscordRole role = ctx.Guild.GetRole(roleID);
            
            if (reactionResult.Result.Emoji == thumbsUp)
                await ctx.Member.GrantRoleAsync(role);
            else if (reactionResult.Result.Emoji == thumbsDown)
                await ctx.Member.RevokeRoleAsync(role);

            await createdMessage.DeleteAsync();
            await ctx.Message.DeleteAsync();
        }
    }
}

