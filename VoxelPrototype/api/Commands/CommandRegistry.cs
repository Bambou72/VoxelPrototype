using LiteNetLib;
using System.Runtime.InteropServices;

namespace VoxelPrototype.api.Commands
{
    public class CommandRegistry
    {
        private static CommandRegistry Instance;
        public static CommandRegistry GetInstance()
        {
            return Instance;
        }
        public static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("CommandRegistry");
        public Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();
        public char commandPrefix = '#'; // Le préfixe spécial pour les commandes
        public List<string> autocommands = new();
        public int RegisteredCommandCount;

        public CommandRegistry()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new InvalidOperationException("You can't instanciate more than 1 instance of singleton");
            }

        }

        public void RegisterCommand(ICommand command)
        {
            Commands.Add(command.Name, command);
            autocommands.Add(commandPrefix.ToString() + command.Name);
            //Logger.Info($"New command registered : {command.Name}");
            RegisteredCommandCount++;
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
        public void Finalize()
        {
            Logger.Info(RegisteredCommandCount + " commands have been loaded.");
        }

    }
}
