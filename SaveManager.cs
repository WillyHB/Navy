using System.Text.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace Navy
{
    public static class SaveManager
    {
        public const string SavePath = "SaveData.txt";
        public static void Save<T>(string key, T value)
        {
            Dictionary<string, dynamic> data = new();

            if (File.Exists(SavePath))
            {
                string loadedJson = File.ReadAllText(SavePath);
                data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(loadedJson);
            }


            data[key] = value;

            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);

            File.WriteAllText(SavePath, json);
        }

        
        public static bool TryLoad<T>(string key, out T data)
        {
            if (!File.Exists(SavePath))
            {
                data = default;
                return false;
            }
            string json = File.ReadAllText(SavePath);

            Dictionary<string, JsonElement> loadedData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (!loadedData.TryGetValue(key, out JsonElement value))
            {
                data = default;
                return false;
            }

            data = value.ToObject<T>();

            return true;
        }

        public static bool ContainsKey(string key)
        {
            if (!File.Exists(SavePath))
            {

                return false;
            }

            string json = File.ReadAllText(SavePath);

            Dictionary<string, JsonElement> data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (data.ContainsKey(key))
            {
                return true;
            }

            return false;
        }


        public static T Load<T>(string key)
        {
            if (!File.Exists(SavePath))
            {
                return default;
            }

            string json = File.ReadAllText(SavePath);

            Dictionary<string, JsonElement> data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);


            if (!data.ContainsKey(key))
            {
                return default;
            }

            return data[key].ToObject<T>();
        }


        
        public static void RemoveSaveData(string key)
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            string json = File.ReadAllText(SavePath);

            Dictionary<string, JsonElement> data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (!data.ContainsKey(key))
            {
                return;
            }

            data.Remove(key);

            JsonSerializerOptions options = new() { WriteIndented = true };
            string j = JsonSerializer.Serialize(data, options);

            File.WriteAllText(SavePath, j);


        }

        public static void RemoveAllSaveData()
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            File.Delete(SavePath);
        }


        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

    }
}
