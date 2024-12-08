using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Lajawi
{
    /// <summary>
    /// Utility to save and load data to and from JSON inside the persistent datapath of the application
    /// </summary>
    public static class SaveSystem
    {
        /// <summary>
        /// Save provided data for later retrieval. Previous data will be overwritten.
        /// Saves at persistent data location of your application
        /// </summary>
        /// <typeparam name="T">Your custom class, needs to be serializable</typeparam>
        /// <param name="location">Relative path or key of where to save the data, without the file extension</param>
        /// <param name="data">Data that needs saving</param>
        public static void Save<T>(string location, T data)
        {
            if (IsTypeOfType(data))
            {
                SaveType(location, data);
                return;
            }

            SaveClass(location, data);
        }

        /// <inheritdoc cref="Save{T}(string, T)"/>
        public static void Save<T>(string location, List<T> data)
        {
            Save(location, data.ToArray());
        }

        /// <inheritdoc cref="Save{T}(string, T)"/>
        public static void Save<T>(string location, T[] data)
        {
            ArrayWrapper<T> wrapper = new() { Items = data };
            SaveClass(location, wrapper);
        }

        /// <summary>
        /// Save a custom class
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="location"><inheritdoc cref="Save{T}(string, T)" path="/param[@name='location']"/></param>
        /// <param name="data"><inheritdoc cref="Save{T}(string, T)" path="/param[@name='data']"/></param>
        private static void SaveClass<T>(string location, T data)
        {
            string json = JsonUtility.ToJson(data, true);

#if UNITY_WEBGL
            PlayerPrefs.SetString(location, json);
#else
            AbsolutePath(ref location);

            Directory.CreateDirectory(Directory.GetParent(location).ToString());

            File.WriteAllText(location, json);
#endif
        }

        /// <summary>
        /// Save a basic type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="location"><inheritdoc cref="Save{T}(string, T)" path="/param[@name='location']"/></param>
        /// <param name="data"><inheritdoc cref="Save{T}(string, T)" path="/param[@name='data']"/></param>
        private static void SaveType<T>(string location, T data)
        {
            ItemWrapper<T> wrapper = new() { Item = data };
            SaveClass(location, wrapper);
        }

        /// <summary>
        /// Load previously saved data
        /// </summary>
        /// <typeparam name="T">Your custom class, needs to be serializable</typeparam>
        /// <param name="location">Relative path or key of where the data is saved, without file extension</param>
        /// <param name="data">Out variable</param>
        /// <returns>Whether reading the data was a success. Use this instead of a manual null check for ease</returns>
        public static bool Load<T>(string location, out T data) where T : new()
        {
            data = new();

            if (!LoadJson(location, out string json))
                return false;

            if (!ParseJson(json, out data))
                return false;

            if (data == null)
                return false;

            return true;
        }

        /// <inheritdoc cref="Load{T}(string, out T)"/>
        public static bool Load<T>(string location, out List<T> data)
        {
            data = new();

            if (!LoadJson(location, out string json))
                return false;

            if (!ParseJson(json, out ArrayWrapper<T> wrapper))
                return false;

            if (wrapper.Items == null)
                return false;

            data = wrapper.Items.ToList();
            return true;
        }

        /// <inheritdoc cref="Load{T}(string, out T)"/>
        public static bool Load<T>(string location, out T[] data)
        {
            data = new T[0];

            if (!LoadJson(location, out string json))
                return false;

            if (!ParseJson(json, out ArrayWrapper<T> wrapper))
                return false;

            if (wrapper.Items == null)
                return false;

            data = wrapper.Items;
            return true;
        }

        /// <summary>
        /// Reads and outputs the JSON data
        /// </summary>
        /// <param name="location">Relative path to the file, without extension</param>
        /// <param name="json">The variable store the JSON into</param>
        /// <returns>Whether reading the file was a success. Use this instead of a manual null check for ease</returns>
        private static bool LoadJson(string location, out string json)
        {
            json = "";

#if UNITY_WEBGL
            json = PlayerPrefs.GetString(location);
#else
            try
            {
                json = File.ReadAllText(AbsolutePath(ref location));

                if (json.Equals(string.Empty))
                    return false;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#endif
        }

        /// <summary>
        /// Parse JSON into the specified format
        /// </summary>
        /// <typeparam name="T">Type of data to parse to</typeparam>
        /// <param name="json">String of JSON to parse</param>
        /// <param name="data">Variable to save the parsed data into</param>
        /// <returns>Whether parsing the JSON was a success. Use this instead of a manual null check for ease</returns>
        private static bool ParseJson<T>(string json, out T data) where T : new()
        {
            data = new();

            if (IsTypeOfType(data))
                return ParseType(json, out data);

            return ParseClass(json, out data);
        }

        private static bool ParseClass<T>(string json, out T data) where T : new()
        {
            data = new();

            try
            {
                JsonUtility.FromJsonOverwrite(json, data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private static bool ParseType<T>(string json, out T data) where T : new()
        {
            data = new();

            if (!ParseClass(json, out ItemWrapper<T> wrapper))
                return false;

            if (wrapper == null)
                return false;

            data = wrapper.Item;
            return true;
        }

        /// <summary>
        /// Get the absolute path to any save data
        /// </summary>
        /// <param name="path">The relative path to the data</param>
        /// <returns>Absolute path to the save data</returns>
        private static string AbsolutePath(ref string path)
        {
            return path = Path.Combine(Application.persistentDataPath, $"{path}.json");
        }

        /// <summary>
        /// Get whether or not data is a basic type (e.g. int, float, string, uint)
        /// </summary>
        /// <typeparam name="T">Type of data</typeparam>
        /// <param name="data">Data to check whether it is of basic type</param>
        /// <returns>True if it is a basic type</returns>
        private static bool IsTypeOfType<T>(T data)
        {
            return data.GetType() == typeof(bool)
                || data.GetType() == typeof(byte)
                || data.GetType() == typeof(sbyte)
                || data.GetType() == typeof(char)
                || data.GetType() == typeof(decimal)
                || data.GetType() == typeof(double)
                || data.GetType() == typeof(float)
                || data.GetType() == typeof(int)
                || data.GetType() == typeof(uint)
                || data.GetType() == typeof(nint)
                || data.GetType() == typeof(nuint)
                || data.GetType() == typeof(long)
                || data.GetType() == typeof(ulong)
                || data.GetType() == typeof(short)
                || data.GetType() == typeof(ushort)
                || data.GetType() == typeof(object)
                || data.GetType() == typeof(string)
                || data.GetType().IsEnum;
        }

        [Serializable]
        private class ArrayWrapper<T>
        {
            public T[] Items;
        }

        [Serializable]
        private class ItemWrapper<T>
        {
            public T Item;
        }
    }
}
