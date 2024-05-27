
using System;
using System.Collections.Generic;
using System.Linq;


public class DeveloperConsole
{
    private readonly string prefix;
    private readonly IEnumerable<IConsoleCommand> commands;

    public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
    {
        this.prefix = prefix;
        this.commands = commands;
    }

    public bool ProcessCommand(string inputValue)
    {
        if (!inputValue.StartsWith(prefix)) { return false; }

        inputValue = inputValue.Remove(0, prefix.Length);

        string[] inputSplit = inputValue.Split(' ');

        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        return ProcessCommand(commandInput, args);
    }

    public bool ProcessCommand(string commandInput, string[] args)
    {
        foreach (var command in commands)
        {
            if (!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (command.Process(args))
            {

                return true;
            }
        }
        return false;
    }
}

