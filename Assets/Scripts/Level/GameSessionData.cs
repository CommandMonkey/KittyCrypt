using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSessionData", menuName = "ScriptableObjects/GameSessionData")]
public class GameSessionData : ScriptableObject
{
    [Header("Continious Scaling")]
    [SerializeField] private float roomValueCapacityMultiplier;
    [SerializeField] private float enemyHP_Multiplier;

    [Header("Set Level Scaling")]
    [Tooltip("if LevelIndex > LevelData.Count, it will defaut to the highest level in LevelData")]
    public List<LevelSettings> levelData;

    public LevelSettings GetLevelData(int levelIndex)
    {
        LevelSettings resultSettings = levelData[Mathf.Min(levelIndex, levelData.Count-1)];

        float power = levelIndex + 1;

        resultSettings.roomValueCapacityMultiplier = Mathf.Ceil(Mathf.Pow(roomValueCapacityMultiplier, power));
        resultSettings.enemyHP_Multiplier = Mathf.Pow(enemyHP_Multiplier, power);   

        return resultSettings;
    }


    [Serializable]
    public struct LevelSettings
    {
        // Inspector variables
        public RoomGenObject roomGenSettings;
        public EnemyValue[] enemyPool;
        public ItemProbability[] itemPool;

        // non serialized
        // (depend on general scaling, Set in GameSessionSettings.GetLevelData())
        [NonSerialized] public float roomValueCapacityMultiplier;
        [NonSerialized] public float enemyHP_Multiplier;


        // /// Get Functions /// //

        public List<GameObject> GetEnemyList(float targetValue)
        {
            List<GameObject> selectedEnemies = new List<GameObject>();

            targetValue = targetValue * roomValueCapacityMultiplier;
            float totalValue = 0f;
            float totalWeight = 0f;

            // Shuffle the enemy pool (optional but recommended for randomness)
            List<EnemyValue> shuffledEnemyPool = GameHelper.ShuffleList<EnemyValue>(new List<EnemyValue>(enemyPool));

            foreach (var enemy in shuffledEnemyPool)
            {
                totalWeight += enemy.probability;
            }
            while (totalValue < targetValue)
            {
                int enemiesThatDontFit = 0;
                foreach (var enemy in shuffledEnemyPool)
                {

                    float randomValue = UnityEngine.Random.Range(0f, totalWeight) * (1 / totalWeight);

                    if (randomValue < enemy.probability)
                    { 

                        if (totalValue + enemy.value <= targetValue)
                        {
                            selectedEnemies.Add(enemy.prefab);
                            totalValue += enemy.value;
                        }
                        else
                        {
                            enemiesThatDontFit++;
                        }
                    }
                    randomValue -= enemy.probability;

                }

                if (enemiesThatDontFit >= shuffledEnemyPool.Count)
                {
                    break;
                }
            }
            return selectedEnemies;
        }

        public GameObject GetRandomSpawnRoomItem()
        {
            // Calculate the total weight (sum of probabilities)
            //float totalWeight = 0f;
            //foreach (var item in itemPool)
            //{
            //    totalWeight += item.probability;
            //}

            // Generate a random value between 0 and the total weight
            float randomValue = UnityEngine.Random.Range(0f, 1);

            // Find the item corresponding to the random value
            foreach (ItemProbability item in itemPool)
            {
                if (item.inSpawnRoom && randomValue < item.probability)
                {
                    return item.prefab; // Return the selected item
                }
                randomValue -= item.probability;
            }

            // Fallback: Return the first item (if the probabilities don't add up to 1)

            foreach (ItemProbability item in itemPool)
            {
                if (item.inSpawnRoom)
                {
                    return item.prefab; // Return the selected item
                }
            }
            Debug.LogError("No items selected as Spawn room items. Configure Item pool! - Anton");
            return null;
        }
    
        public GameObject GetRandomItem()
        {
            // Calculate the total weight (sum of probabilities)
            //float totalWeight = 0f;
            //foreach (var item in itemPool)
            //{
            //    totalWeight += item.probability;
            //}

            // Generate a random value between 0 and the total weight
            float randomValue = UnityEngine.Random.Range(0f, 1f);

            // Find the item corresponding to the random value
            foreach (var item in itemPool)
            {
                if (randomValue < item.probability)
                {
                    return item.prefab; // Return the selected item
                }
                randomValue -= item.probability;
            }

            // Fallback: Return the first item (if the probabilities don't add up to 1)
            if (itemPool[0].prefab != null)
                return itemPool[0].prefab;
            else
            {
                Debug.LogError("Cannot find Any items in the item pool... configure the GameSessionSettings ItemPool! - Anton");
                return null;
            }


        }

    }

    [Serializable]
    public struct ItemProbability
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
        public bool inSpawnRoom;
    }

    [Serializable]
    public struct EnemyValue
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
        public int value;
    }

}