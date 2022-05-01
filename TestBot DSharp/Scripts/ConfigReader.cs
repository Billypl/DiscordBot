using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestBot_DSharp.Scripts
{
    public static class ConfigReader
    {
        public static void readConfigs(
            ref DiscordConfiguration clientConfig, 
            ref CommandsNextConfiguration commandsConfig,
            ref InteractivityConfiguration interactivityConfig)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            clientConfig = readClientConfig(options);
            commandsConfig = readCommandsConfig();
            interactivityConfig = readInteractivityConfig();
        }

        private static DiscordConfiguration readClientConfig(JsonSerializerOptions options)
        {
            string clientFile = File.ReadAllText("../../Configs/ClientConfig.json");
            var clientCfg = JsonSerializer.Deserialize<DiscordConfiguration>(clientFile, options);
            return clientCfg;
        }

        private static CommandsNextConfiguration readCommandsConfig()
        {
            string commandsFile = File.ReadAllText("../../Configs/CommandsConfig.json");
            var commandsCfg = JsonSerializer.Deserialize<CommandsNextConfiguration>(commandsFile);

            JsonNode commandNode = JsonNode.Parse(commandsFile);
            JsonNode prefixNode = commandNode["StringPrefixes"];
            string prefixes = prefixNode.ToJsonString();
            commandsCfg.StringPrefixes = JsonSerializer.Deserialize<IEnumerable<string>>(prefixes);

            return commandsCfg;
        }

        private static InteractivityConfiguration readInteractivityConfig()
        {
            string interactivityFile = File.ReadAllText("../../Configs/InteractivityConfig.json");
            var interactivityCfg = JsonSerializer.Deserialize<InteractivityConfiguration>(interactivityFile);
            return interactivityCfg;
        }
    }
}
