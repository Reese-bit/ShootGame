using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    #region PlayerPrefs
    
    public static void SaveByPlayerPrefs(string key, object data)
    {
        // 将data序列化到json里面
        var json = JsonUtility.ToJson(data);
        
        PlayerPrefs.SetString(key,json);
        PlayerPrefs.Save();
    }

    public static string LoadFromPlayerPrefs(string key)
    {
        return PlayerPrefs.GetString(key, null);
    }
    
    #endregion

    #region JSON

    public static void SaveByJson(string saveFileName, object data)
    {
        var json = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            File.WriteAllText(path,json);
            
            #if UNITY_EDITOR
            Debug.Log($"Successfully saved data to {path}");
            #endif
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            Debug.Log($"Failed to save data to{path}, \n{e}");
            #endif
        }
    }

    public static T LoadFromJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(json);

            return data;
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            Debug.LogError($"Failed to load data from {path}, \n{e}");
            #endif

            return default;
        }
    }

    public static void DeleteSaveFile(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            #if UNITY_EDITOR
            Debug.LogError($"Failed to delete {path}, \n{e}");
            #endif
        }
    }

    public static bool SaveFileExists(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        return File.Exists(path);
    }

    #endregion
}
