using System;
using Ntreev.Library.Commands;
using System.ComponentModel;

namespace Ntreev.Library.Commands.Test.SingleCommand
{
    class Settings
    {
        [CommandSwitch(Name = "param1", Required = true)]
        [Description("parameter1 description")]
        public string Address
        {
            get; set;
        }

        [CommandSwitch(Name = "param2", Required = true)]
        [Description("parameter2 description")]
        public int Port
        {
            get; set;
        }

        [CommandSwitch(ShortName = 'o', NameType = SwitchNameTypes.ShortName)]
        [Description("option1 description")]
        public bool Option1
        {
            get; set;
        }

        [CommandSwitch(Name = "text-option")]
        [Description("option2 description")]
        public string Option2
        {
            get; set;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();
            var parser = new CommandLineParser(settings);

            try
            {
                if (parser.Parse(Environment.CommandLine) == false)
                {
                    Environment.Exit(1);
                }

                // todo

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(2);
            }
        }
    }
}