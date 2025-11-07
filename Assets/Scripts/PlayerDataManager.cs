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

    public static void Set<T>(string key, T value)
    {
        data[key] = JsonConvert.SerializeObject(value, Formatting.Indented);
        Save();
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

    public static void SetBool(string key, bool value)
    {
        Set(key, value);
    }

    public static void SetInt(string key, int value)
    {
        Set(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        Set(key, value);
    }

    public static void SetString(string key, string value)
    {
        Set(key, value);
    }

    public static void Get<T>(string key, ref T value)
    {
        if (!data.ContainsKey(key))
            return;

        try
        {
            value = JsonConvert.DeserializeObject<T>(data[key]);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to parse player data for key '{key}': {ex}");
        }
    }

    public static bool GetBool(string key, in bool default_value)
    {
        bool value = default_value;
        Get(key, ref value);
        return value;
    }

    public static int GetInt(string key, in int default_value)
    {
        int value = default_value;
        Get(key, ref value);
        return value;
    }

    public static float GetFloat(string key, in float default_value)
    {
        float value = default_value;
        Get(key, ref value);
        return value;
    }

    public static string GetString(string key, in string default_value)
    {
        string value = default_value;
        Get(key, ref value);
        return value;
    }

    public static T GetObject<T>(string key, in T default_value)
    {
        T value = default_value;
        Get(key, ref value);
        return value;
    }
}
