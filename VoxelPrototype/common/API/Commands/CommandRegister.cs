using LiteNetLib;
namespace VoxelPrototype.common.API.Commands
{
    public static class CommandRegister
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();
        internal static char commandPrefix = '#'; // Le préfixe spécial pour les commandes
        internal static List<string> autocommands = new();
        public static void RegisterCommand(ICommand command)
        {
            Commands.Add(command.Name, command);
            autocommands.Add(commandPrefix.ToString() + command.Name);
            Logger.Info($"New command registered : {command.Name}");
        }
        public static void ExecuteCommand(string input, NetPeer peer)
        {
            if (input[0] != commandPrefix)
            {
                return;
            }
            string[] parts = input.Substring(1).Split(' ');
            string commandName = parts[0];
            if (Commands.ContainsKey(commandName))
            {
                string[] parameters = parts.Skip(1).ToArray();
                Commands[commandName].Execute(parameters, peer);
            }
        }
    }
}
