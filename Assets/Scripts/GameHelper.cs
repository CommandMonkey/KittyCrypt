using System.Collections.Generic;
using UnityEngine;

public static class GameHelper 
{
    public static Vector2 GetRandomPosInCollider(BoxCollider2D collider, LayerMask filter = new())
    {
        Vector2 min = collider.bounds.min;
        Vector2 max = collider.bounds.max;

        float randomX = Random.Range(min.x, max.x);
        float randomY = Random.Range(min.y, max.y);

        Vector2 pos = new Vector2(randomX, randomY);

        if (Physics2D.OverlapCircle(pos, 1.3f, filter))
            return GetRandomPosInCollider(collider, filter);
        else
            return pos;
    }

    public static bool IsBoxColliderTouching(Vector3 _pos, BoxCollider2D _collider, ContactFilter2D _filter)
    {
        Collider2D[] results = new Collider2D[10];

        int numColliders = Physics2D.OverlapBox((Vector2)_pos+_collider.offset, _collider.size, 0f, _filter, results);

        return numColliders > 0;
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledList = new List<T>(list); // Copy the original list
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // Get a random index
            T value = shuffledList[k]; // Swap elements
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList;
    }


    public static List<GameObject> InstanciateInCollider(BoxCollider2D roomCollider, List<GameObject> toSpawn, LayerMask noEnemyLayer)
    {
        List<GameObject> instances = new List<GameObject>();

        foreach (var t in toSpawn)
        {
            Vector2 pos = GetRandomPosInCollider(roomCollider, noEnemyLayer);
            instances.Add(GameObject.Instantiate(t, pos, Quaternion.identity));
        }
        return instances;
    }

    public static GameObject GetChildWithTag(this GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return null; // No child with the specified tag found
    }
    
    public static T GetComponentInAllChildren<T>(this Transform parent) where T : Component
    {
        // Check if the parent itself has the component
        T comp = parent.GetComponent<T>();
        if (comp != null)
        {
            return comp;
        }

        // Iterate through all children and their descendants
        foreach (Transform child in parent)
        {
            // Recursively search for the component in the child
            comp = child.transform.GetComponentInAllChildren<T>();
            if (comp != null)
            {
                return comp;
            }
        }

        // If no component found in children, return null
        return null;
    }
    
    public static List<T> GetComponentsInAllChildren<T>(this Transform parent) where T : Component
    {
        // Initialize an empty list to store the components
        List<T> components = new List<T>();

        // Check if the parent itself has the component
        T parentComponent = parent.GetComponent<T>();
        if (parentComponent != null)
        {
            components.Add(parentComponent);
        }

        // Iterate through all children and their descendants
        foreach (Transform child in parent)
        {
            // Recursively search for the component in the child
            List<T> childComponents = child.transform.GetComponentsInAllChildren<T>();
            components.AddRange(childComponents);
        }

        return components;
    }

    public static float MapValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        // Calculate the width of each range
        float inputSpan = inputMax - inputMin;
        float outputSpan = outputMax - outputMin;

        // Normalize the input value to a 0-1 range
        float normalizedValue = (value - inputMin) / inputSpan;

        // Map the normalized value to the output range
        float mappedValue = outputMin + (normalizedValue * outputSpan);

        return mappedValue;
    }
}