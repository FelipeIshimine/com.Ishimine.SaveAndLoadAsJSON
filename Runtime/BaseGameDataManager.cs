using System;
using Leguar.TotalJSON;
using SaveSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Clase principal para centralizar un archivo de guardado. Los objetos/sistemas que requieran guardar y cargar datos deben registrarse a traves del metodo "Register" e implementar la interfaz ISaveAsJSON
/// </summary>
public abstract class BaseGameDataManager : RuntimeScriptableSingleton<BaseGameDataManager>
{
    public static event Action OnLoadDone;
    public static event Action<JSON> OnSaveDataRequest;
    public static event Action<JSON> OnLoadDataRequest;

    public virtual int CurrentVersion {get;} =0;
    public virtual string RootKey { get; } = "Root";
    public virtual string FileName { get; } = "Save";

    private static bool _isAutoSaveEnabled = false;

    public static bool IsAutoSaveEnabled
    {
        get => _isAutoSaveEnabled;
        set
        {
            Debug.Log($"<color=yellow>    |||     </color> AutoSave:{value}");
            _isAutoSaveEnabled = value;
        }
    }

    private int EncryptionKey
    {
#if UNITY_EDITOR
        get => 0;
#else
        get => 1235468495 / 2;
#endif
    }

    private JSON _localData;

    public static JSON GetSaveData()
    {
        JSON root = new JSON();
        OnSaveDataRequest?.Invoke(root);
        return root;
    }

    public static void SetSaveData(JSON json)
    {
        Instance._localData = json;
        OnLoadDataRequest?.Invoke(json);
    }

    public JSON GetSave()
    {
        JSON data = new JSON();
        data.Add("VERSION", CurrentVersion);
        return data;
    }

    public void Load(JSON data)
    {
    }

    public void UpdateSaveData(JSON data)
    {
        throw new System.NotImplementedException();
    }
    
    
    public static void Register(ISaveLoadAsJson target)
    {
        Debug.Log($"<color=cyan>GameDataSystem</color> Sub system registered: <color=white>{target.GetType()}</color>");
        OnSaveDataRequest += target.SaveData;
        OnLoadDataRequest += target.LoadData;
    }

    public static void Unregister(ISaveLoadAsJson target)
    {
        OnSaveDataRequest -= target.SaveData;
        OnLoadDataRequest -= target.LoadData;
    }

#if UNITY_EDITOR
    [MenuItem("SaveAndLoad/Save")]
#endif
    public static void Save() => Save(false);

    public static void Save(bool forceSave)
    {
        if (!forceSave && !IsAutoSaveEnabled)
        {
            Debug.Log($" Save request <color=yellow> Denied </color> | ForceSave:{forceSave}  AutoSave:{IsAutoSaveEnabled}");
            return;
        }
        SaveLoadManager.SaveEncryptedJson(GetSaveData(), Instance.FileName, Instance.EncryptionKey);
        Debug.Log($"Save request: <color=green>  Success </color> | ForceSave:{forceSave} AutoSave:{IsAutoSaveEnabled}");
    }
    
#if UNITY_EDITOR
    [MenuItem("SaveAndLoad/Load")]
#endif
    public static void Load()
    {
        if (SaveLoadManager.Exists(SaveLoadManager.GetFilePath(Instance.FileName)))
            SetSaveData(SaveLoadManager.LoadEncryptedJson(Instance.FileName, Instance.EncryptionKey));
        else
            Debug.Log($"FileName:{SaveLoadManager.GetFilePath(Instance.FileName)} doesnt exists");
        
        OnLoadDone?.Invoke();
    }
    #if UNITY_EDITOR
    [MenuItem("SaveAndLoad/Erase")]
    #endif
    public static void Erase()
    {
        #if UNITY_EDITOR
        if(Application.isPlaying)
        #endif
            SaveLoadManager.Delete(Instance.FileName);
        #if UNITY_EDITOR
        else if (EditorUtility.DisplayDialog("Cuidado", "Esta por borrar los datos locales ¿esta seguro?", "Aceptar", "Cancelar"))
            SaveLoadManager.Delete(Instance.FileName);
        #endif
    }
}
