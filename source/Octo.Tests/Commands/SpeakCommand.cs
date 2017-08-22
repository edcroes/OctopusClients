﻿using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Tests.Commands
{
    public class SpeakCommand : CommandBase, ICommand
    {
        //public OptionSet Options
        //{
        //    get
        //    {
        //        var options = new OptionSet();
                
        //        return options;
        //    }
        //}

        //public void GetHelp(TextWriter writer, string[] args)
        //{
        //    ICommandOutputProvider commandOutputProvider = new CommandOutputProvider();
        //    commandOutputProvider.PrintCommandHelpHeader("executable", "speak", writer);
        //    writer.WriteLine("message=");
        //    Options.WriteOptionDescriptions(writer);
        //    commandOutputProvider.PrintCommandHelpFooter("executable", "speak", writer);
        //}

        public Task Execute(string[] commandLineArguments)
        {
            return Task.Run(() => Assert.Fail("This should not be called"));
        }

        public SpeakCommand(ILogger log, ICommandOutputProvider commandOutputProvider) : base(log, commandOutputProvider)
        {
            var options = Options.For("default");
            options.Add("message=", m => { });
        }
    }
}