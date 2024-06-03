using UnityEngine;


[CreateAssetMenu(fileName = "New LoadLevel Command", menuName = "DeveloperConsole/Commands/LoadLevel Command")]
public class LoadLevelCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (args.Length != 1) { return false; }

        if (!int.TryParse(args[0], out int value))
        {
            return false;
        }

        Debug.Log(value);
        GameSession.Instance.levelIndex = value-1;
        FindObjectOfType<SceneLoader>().LoadLevel1();

        return true;
    }
}