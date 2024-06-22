using System.Collections.Generic;
using UnityEngine;


public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField] private string commandWord = string.Empty;

    public string CommandWord => commandWord;

    public virtual List<string> GetSuggestions(string[] args)
    {
        return new List<string>() { "" };
    }

    public abstract bool Process(string[] args);
}

