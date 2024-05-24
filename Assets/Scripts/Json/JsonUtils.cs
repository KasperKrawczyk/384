using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public static class JsonUtils
{
    public static T LoadJsonAsSO<T>(string path) where T : ScriptableObject
    {
        if (!File.Exists(path))
        {
            Debug.LogError("No file found at " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        T instance = ScriptableObject.CreateInstance<T>();
        JsonConvert.PopulateObject(json, instance, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        });

        return instance;
    }

    public static void SaveSOToJson<T>(T so, string path) where T : ScriptableObject
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(so, Formatting.Indented, settings);

        File.WriteAllText(path, json);
    }
}