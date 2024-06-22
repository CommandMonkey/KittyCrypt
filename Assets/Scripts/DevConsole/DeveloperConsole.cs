
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

    internal List<string> GetSuggestionList(string startText)
    {
        return commands
            .Where(command => command.CommandWord.StartsWith(startText, StringComparison.OrdinalIgnoreCase))
            .OrderBy(command => GameHelper.LevenshteinDistance(command.CommandWord, startText))
            .ThenBy(command => command.CommandWord.StartsWith(startText, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .Select(command => command.CommandWord)
            .ToList();
    }


    internal List<string> GetSpecificCommandSuggestionList(string command, string followingText)
    {
        IConsoleCommand commandObj = commands.FirstOrDefault(c => c.CommandWord.Equals(command, StringComparison.OrdinalIgnoreCase));
        if (commandObj != null)
        {
            string[] inputSplit = followingText.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            return commandObj.GetSuggestions(args)
                .Where(suggestion => suggestion.StartsWith(followingText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(suggestion => GameHelper.LevenshteinDistance(suggestion, followingText))
                .ToList();
        }
        return new List<string>();
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

