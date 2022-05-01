using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBot_DSharp.Commands
{
    public class Debug
    {
        public async Task printRoles(CommandContext ctx)
        {
            foreach (var roleName in ctx.Guild.Roles)
                Console.WriteLine(roleName.Value);

            await ctx.Channel.SendMessageAsync("Roles printed to the console");
        }

        public async Task printMembers(CommandContext ctx)
        {
            await ctx.Guild.RequestMembersAsync();

            foreach (var roleName in ctx.Guild.Roles)
                Console.WriteLine(roleName.Value);

            await ctx.Channel.SendMessageAsync("Members printed to the console");
        }
    }
}
