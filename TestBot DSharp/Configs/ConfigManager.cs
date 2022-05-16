using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace TestBot_DSharp.Configs
{
    public static class ConfigManager
    {
        public static DiscordConfiguration clientConfig;
        public static CommandsNextConfiguration commandsConfig;
        public static InteractivityConfiguration interactivityConfig;

        public static void assignConfigs()
        {
            getConfigs();
            Bot.Client = new DiscordClient(clientConfig);
            Bot.Commands = Bot.Client.UseCommandsNext(commandsConfig);
            Bot.Client.UseInteractivity(interactivityConfig);
        }
        private static void getConfigs()
        {
            ConfigReader.readConfigs();
        }

    }
}
