using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace Lajawi
{
    /// <summary>
    /// Utility to save and load data to and from JSON inside the persistent datapath of the application,
    /// using the Newtonsoft JSON Package.
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// Save provided data for later retrieval. Previous data will be overwritten.
        /// Saves at persistent data location of your application.
        /// </summary>
        /// <typeparam name="T">Your custom class, needs to be serializable.</typeparam>
        /// <param name="location">Relative path or key of where to save the data, without the file extension.</param>
        /// <param name="data">Data that needs saving.</param>
        public static void Save<T>(string location, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

#if UNITY_WEBGL
            PlayerPrefs.SetString(location, json);
#else
            AbsolutePath(ref location);

            Directory.CreateDirectory(Directory.GetParent(location).ToString());

            File.WriteAllText(location, json);
#endif
        }

        /// <summary>
        /// Load previously saved data.
        /// </summary>
        /// <typeparam name="T">Your custom class, needs to be serializable.</typeparam>
        /// <param name="location">Relative path or key of where the data is saved, without file extension.</param>
        /// <param name="data">Output variable.</param>
        /// <returns>Whether reading the data was a success. Use this instead of a manual null check for ease.</returns>
        public static bool Load<T>(string location, out T data)
        {
            data = default;

            if (!LoadJson(location, out string json))
                return false;

            return ParseJson(json, out data);
        }

        /// <summary>
        /// Reads and outputs the JSON data.
        /// </summary>
        /// <param name="location">Relative path to the file, without extension.</param>
        /// <param name="json">The variable store the JSON into.</param>
        /// <returns>Whether reading the file was a success. Use this instead of a manual null check for ease.</returns>
        private static bool LoadJson(string location, out string json)
        {
            json = "";

#if UNITY_WEBGL
            json = PlayerPrefs.GetString(location);
#else
            try
            {
                json = File.ReadAllText(AbsolutePath(ref location));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#endif
            if (json.Equals(string.Empty))
                return false;

            return true;
        }

        /// <summary>
        /// Parse JSON into the specified format.
        /// </summary>
        /// <typeparam name="T">Type of data to parse to.</typeparam>
        /// <param name="json">String of JSON to parse.</param>
        /// <param name="data">Variable to save the parsed data into.</param>
        /// <returns>Whether parsing the JSON was a success. Use this instead of a manual null check for ease.</returns>
        private static bool ParseJson<T>(string json, out T data)
        {
            data = default;

            try
            {
                data = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// Get the absolute path to any save data.
        /// </summary>
        /// <param name="path">The relative path to the data.</param>
        /// <returns>Absolute path to the save data.</returns>
        private static string AbsolutePath(ref string path)
        {
            return path = Path.Combine(Application.persistentDataPath, $"{path}.json");
        }
    }
}
