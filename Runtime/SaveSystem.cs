using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Lajawi
{
    public static class SaveSystem
    {
        /// <summary>
        /// Save provided data for later retrieval. Previous data will be overwritten
        /// Saves at persistent data location of your application
        /// </summary>
        /// <typeparam name="T">Your custom class, needs to be serializable</typeparam>
        /// <param name="location">Relative path or key of where to save the data, without the file extension</param>
        /// <param name="data">Data that needs saving</param>
        public static void Save<T>(string location, T data)
        {
            string json = JsonUtility.ToJson(data, true);

#if UNITY_WEBGL
            PlayerPrefs.SetString(location, json);
#else
            location = $@"{Application.persistentDataPath}\{location}.json";

            Directory.CreateDirectory(Directory.GetParent(location).ToString());

            using (StreamWriter sw = new StreamWriter(location))
            {
                sw.WriteLine(json);
            }
#endif
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
            string json = "";
            data = new T();

#if UNITY_WEBGL
            json = PlayerPrefs.GetString(location);
#else
            try
            {
                using (StreamReader sr = new StreamReader($@"{Application.persistentDataPath}\{location}.json"))
                {
                    json = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#endif
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
    }
}
