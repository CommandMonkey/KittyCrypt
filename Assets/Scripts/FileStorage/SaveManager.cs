using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using System;

public static class SaveManager
{

    static string dataPath = Path.Combine(Application.persistentDataPath, "KittyData.Spoingus");

    public static void SaveData(GameData gameData)
    {
        try
        {
            // Create the directory stuff will be stored in (If it doesn't already exist)
            Directory.CreateDirectory(Path.GetDirectoryName(dataPath));

            // Serialize data to Json
            string dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream stream = new FileStream(dataPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Problem occured while saving data: " + e);
        }

    }

    public static GameData LoadData()
    {
        GameData loadedData = null;
        if (File.Exists(dataPath))
        {

            try
            {
                // Get the Json Data
                string dataToLoad = "";

                using (FileStream stream = new FileStream(dataPath, FileMode.Create))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // DeSerialize the json data
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Problem occured while loading data: " + e);
            }
        }
        return loadedData;
    }
}
