using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace TestBot_DSharp.Commands
{
    public class RoleCommands : BaseCommandModule
    {
        private readonly DiscordEmoji thumbsUp = DiscordEmoji.FromName(Bot.Client, ":+1:");
        private readonly DiscordEmoji thumbsDown = DiscordEmoji.FromName(Bot.Client, ":-1:");

        // useful for giving users roles according to their preferences
        // i.e. give c# or .NET role to view only those categories
        [Command("roles")]
        [Description("Manages user roles (admin-only for now)")]
        public async Task Roles(CommandContext ctx, params string[] roleFormalName)
        {
            string roleName = string.Join(" ", roleFormalName);
            long roleID = getRoleID(ctx.Guild, roleName);

            if(!hasNeededPermissions(ctx.Member, ctx.Guild))
            {
                await ctx.Message.RespondAsync("You don't have permission to run this command!");
                return;
            }
            if (!isValidRole(roleID))
            {
                await ctx.Message.RespondAsync("Couldn't find the role");
                return;
            }

            DiscordMessage createdMessage = await ctx.Channel.SendMessageAsync(createMemberEmbed(ctx, roleName));
            await createReactions(createdMessage);
            var reactionResult = collectReaction(ctx).Result;
            await grantOrRevokeRole(ctx, roleID, reactionResult);

            await createdMessage.DeleteAsync();
        }

        private bool isValidRole(long roleID)
        {
            return roleID > 0;
        }
        private long getRoleID(DiscordGuild guild, string roleName)
        {
            var roles = guild.Roles.Values;
            foreach (var role in roles)
                if (role.Name.ToString() == roleName)
                    return (long)role.Id;

            return -1;
        }
        private bool hasNeededPermissions(DiscordMember member, DiscordGuild guild)
        {
            var Bartek = guild.GetRole((ulong)getRoleID(guild, "#01-Bartek"));
            var Milosz = guild.GetRole((ulong)getRoleID(guild, "#02-Miłosz"));
            var Ibrahim = guild.GetRole((ulong)getRoleID(guild, "#05-Ibrahim"));

            if (member.Roles.Contains(Ibrahim) || member.Roles.Contains(Bartek) || member.Roles.Contains(Milosz))
                return true;

            return false;
        }
        private DiscordEmbedBuilder createMemberEmbed(CommandContext ctx, string roleName)
        {
            var avatar = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl };
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Would you like to be a {roleName}?",
                Color = DiscordColor.Green,
                Thumbnail = avatar
            };
            return embed;
        }
        private async Task<InteractivityResult<MessageReactionAddEventArgs>> collectReaction(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var reactionResult = await interactivity.WaitForReactionAsync(x =>
                x.Channel == ctx.Channel &&
               (x.Emoji == thumbsUp || x.Emoji == thumbsDown) &&
                x.User == ctx.User);

            return reactionResult;
        }
        private async Task grantOrRevokeRole(CommandContext ctx, long roleID, InteractivityResult<MessageReactionAddEventArgs> reactionResult)
        {
            DiscordRole role = ctx.Guild.GetRole((ulong)roleID);

            if (reactionResult.Result.Emoji == thumbsUp)
            {
                await ctx.Member.GrantRoleAsync(role);
                await ctx.Message.RespondAsync("Role granted!");
            }
            else if (reactionResult.Result.Emoji == thumbsDown)
            {
                await ctx.Member.RevokeRoleAsync(role);
                await ctx.Message.RespondAsync("Role revoked!");
            }
        }
        private async Task createReactions(DiscordMessage createdMessage)
        {
            await createdMessage.CreateReactionAsync(thumbsUp);
            await createdMessage.CreateReactionAsync(thumbsDown);
        }

    }
}

