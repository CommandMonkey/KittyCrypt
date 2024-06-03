using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New SpawnItemCommand Command", menuName = "DeveloperConsole/Commands/SpawnItemCommand")]
public class SpawnItemCommand : ConsoleCommand
{
    [SerializeField] itemXNames[] items;
    [SerializeField] GameObject pickupPrefab;

    public override bool Process(string[] args)
    {
        if(args.Length != 1) return false;

        foreach (itemXNames item in items)
        {
            foreach (string name in item.names)
            {
                if (string.Equals(args[0], name))
                {
                    SpawnItem(item.prefab);
                    return true;
                }
            }
        }
        return true;
    }

    void SpawnItem(GameObject item)
    {
        Vector3 spawnPos = GameSession.Instance.Player.transform.position;
        ItemPickupInteractable pickupInstance = Instantiate(pickupPrefab, spawnPos, Quaternion.identity).GetComponent<ItemPickupInteractable>();

        pickupInstance.item = item;
    }


    [System.Serializable]
    public struct itemXNames
    {
        public List<string> names;
        public GameObject prefab;
    }
}


