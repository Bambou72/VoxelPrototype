using LiteNetLib;

namespace VoxelPrototype.common.Commands
{
    public class CommandRegister
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();
        internal char commandPrefix = '#'; // Le préfixe spécial pour les commandes
        internal List<string> autocommands = new();
        public void RegisterCommand(ICommand command)
        {
            Commands.Add(command.Name, command);
            autocommands.Add(commandPrefix.ToString() + command.Name);
            Logger.Info($"New command registered : {command.Name}");
        }
        public void ExecuteCommand(string input, NetPeer peer)
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
