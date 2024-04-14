using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "ScriptableObjects/GameSessionData")]
public class GameSessionData : ScriptableObject
{
    [Header("Continious Scaling")]
    [SerializeField] private float roomValueCapacityMultiplier;

    [Header("Set Level Scaling")]
    [Tooltip("if LevelIndex > LevelData.Count, it will defaut to the highest level in LevelData")]
    public List<LevelSettings> LevelData;


    [Serializable]
    public struct LevelSettings
    {
        public RoomGenObject roomGenSettings;
        public EnemyValue[] enemyPool;
        public ItemProbability[] itemPool;

        // non serialized
        [NonSerialized] public float roomValueCapacityMultiplier;
        [NonSerialized] public float enemyHP_Multiplier;

        public List<GameObject> GetEnemyList(float targetValue)
        {
            List<GameObject> selectedEnemies = new List<GameObject>();
            float totalValue = 0f;

            // Shuffle the enemy pool (optional but recommended for randomness)
            List<EnemyValue> shuffledEnemyPool = GameHelper.ShuffleList<EnemyValue>(new List<EnemyValue>(enemyPool));
            
            while (totalValue < targetValue)
            {
                int enemiesThatDontFit = 0;
                foreach (var enemy in shuffledEnemyPool)
                {
                    // Calculate the adjusted value based on the multiplier
                    float value = enemy.value;

                    // Check if adding this enemy exceeds the target value
                    if (totalValue + value <= targetValue)
                    {
                        selectedEnemies.Add(enemy.prefab);
                        totalValue += value;
                    }
                    else
                    {
                        enemiesThatDontFit++;
                    }
                }
                if (enemiesThatDontFit >= shuffledEnemyPool.Count)
                {
                    break;
                }

            }


            return selectedEnemies;
        }


        public GameObject GetRandomItem()
        {
            // Calculate the total weight (sum of probabilities)
            float totalWeight = 0f;
            foreach (var item in itemPool)
            {
                totalWeight += item.probability;
            }

            // Generate a random value between 0 and the total weight
            float randomValue = UnityEngine.Random.Range(0f, totalWeight);

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
            return itemPool[0].prefab;
        }

    }

    [Serializable]
    public struct ItemProbability
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
    }

    [Serializable]
    public struct EnemyValue
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
        public int value;
    }

}