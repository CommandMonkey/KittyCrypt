
using System.Collections.Generic;

public interface IConsoleCommand
{
    string CommandWord { get; }
    bool Process(string[] args);
    List<string> GetSuggestions(string[] args);
}

