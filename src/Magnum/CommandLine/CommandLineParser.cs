namespace Magnum.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CommandLineParser
    {
        private readonly Dictionary<string, IArgumentParsingInstructions> _commands = new Dictionary<string, IArgumentParsingInstructions>();
        private readonly IArgumentCommandNameConvention _namePolicy = new Use_types_name_lowercased_removing_Args_or_Arguments();
        private readonly IArgumentOrderPolicy _policy = new Arguments_must_be_positional_then_named();
        

        public ParsedCommandLineOutput Parse(string[] commandLine)
        {
            var commandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            _policy.Verify(remainder);
            object args = _commands[commandName].Build(remainder);

            return new ParsedCommandLineOutput(){CommandName = commandName, ParsedArguments = args};
        }


        public void RegisterArguments<ArgsType>() where ArgsType : new()
        {
            RegisterArgumentsForCommand<ArgsType>(_namePolicy.GetName<ArgsType>());
        }

        public void RegisterArgumentsForCommand<ArgsType>(string commandName) where ArgsType : new()
        {
            var argType = typeof (ArgsType);
            IArgumentParsingInstructions api = new ArgumentParsingInstructions(argType);
            _commands.Add(commandName, api);
        }

        public string WhatsRegistered()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Commands:");
            foreach (var pair in _commands)
            {
                sb.AppendFormat("{0}{1}", pair.Key, Environment.NewLine);
                sb.AppendFormat(pair.Value.WhatAreTheOptions());
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}