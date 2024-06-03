using UnityEngine;


[CreateAssetMenu(fileName = "New UnlockBossCommand Command", menuName = "DeveloperConsole/Commands/UnlockBossCommand")]
public class UnlockBossCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        FindObjectOfType<BossDoorInteractable>().OpenDoor();
        return true;
    }
}