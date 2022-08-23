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
public abstract class BaseGameDataManager<T> : RuntimeScriptableSingleton<T> where T : BaseGameDataManager<T>
{
    public static event Action OnLoadDone;
    public static event Action<JSON> OnSaveDataRequest;
    public static event Action<JSON> OnLoadDataRequest;
    public static event Action<JSON> OnEraseDataRequest;

    public static event Action OnFirstLoad;
    
    public virtual string FileName { get; } = "Save";

    [SerializeField] private bool isAutoSaveEnabled = false;
    public JSON LocalData { get; set; }

    public static bool IsAutoSaveEnabled
    {
        get => Instance.isAutoSaveEnabled;
        set
        {
            Debug.Log($"<color=yellow>    |||     </color> AutoSave:{value}");
            Instance.isAutoSaveEnabled = value;
        }
    }

    protected int EncryptionKey
    {
#if UNITY_EDITOR
        get => 0;
#else
        get => 1235468495 / 2;
#endif
    }

    private bool _isFirstLoad = true;

    public static JSON GetSaveData()
    {
        JSON root = new JSON();
        OnSaveDataRequest?.Invoke(root);
        return root;
    }

    public static void SetSaveData(JSON json)
    {
        Instance.LocalData = json;
        OnLoadDataRequest?.Invoke(json);
    }

    public static void Register(ISaveLoadAsJson target)
    {
        Debug.Log($"<color=cyan>GameDataSystem</color> Sub system registered: <color=white>{target.GetType()}</color>");
        OnSaveDataRequest += target.Save;
        OnLoadDataRequest += target.Load;
    }

    public static void Unregister(ISaveLoadAsJson target)
    {
        OnSaveDataRequest -= target.Save;
        OnLoadDataRequest -= target.Load;
    }

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

    public static void Load()
    {
        if (SaveLoadManager.Exists(SaveLoadManager.GetFilePath(Instance.FileName)))
            SetSaveData(SaveLoadManager.LoadEncryptedJson(Instance.FileName, Instance.EncryptionKey));
        else
            Debug.Log($"FileName:{SaveLoadManager.GetFilePath(Instance.FileName)} doesnt exists");
        
        OnLoadDone?.Invoke();
        if (Instance._isFirstLoad)
        {
            Debug.LogWarning($"{Instance} FIRST LOAD");
            OnFirstLoad?.Invoke();
            Instance._isFirstLoad = false;
        }
    }
   
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
