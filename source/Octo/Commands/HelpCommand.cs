using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("help", "?", "h", Description = "Prints this help text")]
    public class HelpCommand : ICommand
    {
        readonly ICommandLocator commands;
        private readonly ICommandOutputProvider commandOutputProvider;

        public HelpCommand(ICommandLocator commands, ICommandOutputProvider commandOutputProvider)
        {
            this.commands = commands;
            this.commandOutputProvider = commandOutputProvider;
        }

        public void GetHelp(TextWriter writer, string[] args)
        {
        }

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() =>
            {
                var executable = Path.GetFileNameWithoutExtension(typeof(HelpCommand).GetTypeInfo().Assembly.FullLocalPath());

                var commandName = commandLineArguments.FirstOrDefault();

                if (string.IsNullOrEmpty(commandName))
                {
                    PrintGeneralHelp(executable);
                }
                else
                {
                    var command = commands.Find(commandName);

                    if (command == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Command '{0}' is not supported", commandName);
                        Console.ResetColor();
                        PrintGeneralHelp(executable);
                    }
                    else
                    {
                        PrintCommandHelp(command, commandLineArguments);
                    }
                }
            });
        }

        void PrintCommandHelp(ICommand command, string[] args)
        {
            command.GetHelp(Console.Out, args);
        }

        void PrintGeneralHelp(string executable)
        {
            Console.ResetColor();
            Console.Write("Usage: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(executable + " <command> [<options>]");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Where <command> is one of: ");
            Console.WriteLine();

            foreach (var possible in commands.List().OrderBy(x => x.Name))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  " + possible.Name.PadRight(15, ' '));
                Console.ResetColor();
                Console.WriteLine("   " + possible.Description);
            }

            Console.WriteLine();
            Console.Write("Or use ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(executable + " help <command>");
            Console.ResetColor();
            Console.WriteLine(" for more details.");
        }
    }
}