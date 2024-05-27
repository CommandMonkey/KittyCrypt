using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TeleportCommand Command", menuName = "DeveloperConsole/Commands/TeleportCommand")]
public class TeleportCommand : ConsoleCommand
{

    [SerializeField] List<string> bossRoomStrings;
    [SerializeField] List<string> keyRoomStrings;
    [SerializeField] List<string> tressureRoomStrings;
    [SerializeField] List<string> startRoomStrings;
    public override bool Process(string[] args)
    {
        if (args.Length != 1) return false;

        if (bossRoomStrings.Contains(args[0]))
        {
            Teleport(GetBossRoomPos());
        }
        else if (keyRoomStrings.Contains(args[0]))
        {
            Teleport(GetKeyRoomPos());
        }
        else if (tressureRoomStrings.Contains(args[0]))
        {
            Teleport(GetTressureRoomPos());
        }
        else if (startRoomStrings.Contains(args[0]))
        {
            Teleport(GetStartRoomPos());
        }

        return true;
    }





    private Vector2 GetBossRoomPos()
    {
        BossDoorInteractable bossDoor = FindObjectOfType<BossDoorInteractable>();
        Vector3 outDirection = bossDoor.direction == Direction.Top ? Vector2.up :
                                bossDoor.direction == Direction.Bottom ? Vector2.down :
                                bossDoor.direction == Direction.Right ? Vector2.right :
                                Vector2.left;

        return bossDoor.transform.position - outDirection*3;
    }

    private Vector2 GetKeyRoomPos()
    {
        Vector3 pos = FindObjectOfType<KeyPickupInteractable>().transform.position;

        return pos + Vector3.down;
    }

    private Vector2 GetTressureRoomPos()
    {
        Vector3 pos = FindObjectOfType<ChestInteractable>().transform.position;

        return pos + Vector3.down;
    }

    private Vector2 GetStartRoomPos()
    {
        return FindObjectOfType<SpawnRoom>().transform.position;
    }




    private void Teleport(Vector2 pos)
    {
        GameSession.Instance.Player.transform.position = pos;
    }
}
