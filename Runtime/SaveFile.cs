using System;
using System.Collections.Generic;
using Leguar.TotalJSON;
using UnityEngine;

namespace SaveSystem
{
    public class SaveFile : ISaveLoadAsJson
    {
        public readonly string Name;
        private readonly int _encryptionKey;

        public SaveFile(string name, int encryptionKey = 0)
        {
            Name = name;
            _encryptionKey = encryptionKey;
        }

        private readonly Dictionary<string,ISaveLoadAsJson> _sources = new Dictionary<string,ISaveLoadAsJson>(128);

        public Action<JSON> OnUpdateAction { get; set; }

        public void LoadFromDisk()
        {
            SaveData = SaveLoadManager.LoadEncryptedJson(Name, _encryptionKey);
            OnAfterLoad();
        }

        public void SaveToDisk()
        {
            OnBeforeSave();
            SaveLoadManager.SaveEncryptedJson(SaveData, Name, _encryptionKey);
        }

        public void Erase() => SaveLoadManager.Delete(Name);

        public void Register(ISaveLoadAsJson source) => _sources.Add(source.RootKey,source);
    
        public void Unregister(ISaveLoadAsJson source) => _sources.Remove(source.RootKey);

        public JSON SaveData { get; set; } = new JSON();
        public int CurrentVersion { get; set; } = 0;
        public string RootKey => Name;
        public void UpdateSaveData(JSON data) => OnUpdateAction?.Invoke(data);
        
        public void OnBeforeSave()
        {
            //Debug.Log($"*** OnBeforeSave {Name}");
            SaveData.Clear();
            foreach (ISaveLoadAsJson saveLoadAsJson in _sources.Values)
                SaveData.Save(saveLoadAsJson);
            
            SaveData.DebugInEditor(Name);
        }

        public void OnAfterLoad()
        {
            //Debug.Log($"*** OnAfterLoad {Name}");
            foreach (ISaveLoadAsJson saveLoadAsJson in _sources.Values)
                SaveData.Load(saveLoadAsJson);
            SaveData.DebugInEditor(Name);
        }
    }
}