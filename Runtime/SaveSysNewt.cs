using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Lajawi
{
    public static class SaveSysNewt
    {
        public static void Save<T>(string location, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

#if UNITY_WEBGL
            PlayerPrefs.SetString(location, json);
#else
            Lajawi.SaveSystem.AbsolutePath(ref location);

            Directory.CreateDirectory(Directory.GetParent(location).ToString());

            File.WriteAllText(location, json);
#endif
        }

        public static bool Load<T>(string location, out T data) where T : new()
        {
            data = new();

            if (!SaveSystem.LoadJson(location, out string json))
                return false;

            return !ParseJson(json, out data);
        }

        private static bool ParseJson<T>(string json, out T data) where T : new()
        {
            data = new();

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
    }
}
