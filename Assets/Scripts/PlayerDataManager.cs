using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PlayerDataManager
{
    private static readonly string path = Path.Combine(Application.persistentDataPath, "player_data.json");
    private static Dictionary<string, string> data = new();

    static PlayerDataManager()
    {
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load player data: {ex}");
                data = new();
            }
        }
    }

    private static void Set<T>(string key, T value)
    {
        data[key] = JsonConvert.SerializeObject(value, Formatting.Indented);
        Save();
    }

    private static bool Get<T>(string key, ref T value)
    {
        if (!data.ContainsKey(key))
            return false;

        try
        {
            value = JsonConvert.DeserializeObject<T>(data[key]);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse player data for key '{key}': {ex}");
            return false;
        }
    }

    private static void Save()
    {
        try
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save player data: {ex}");
        }
    }
}
