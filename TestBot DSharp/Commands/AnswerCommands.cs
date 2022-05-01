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

    }
}
