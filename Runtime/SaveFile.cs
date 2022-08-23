using System;
using System.Collections.Generic;
using Leguar.TotalJSON;

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

        public void LoadFromDisk() => this.Load(SaveLoadManager.LoadEncryptedJson(Name, _encryptionKey));

        public void SaveToDisk() => SaveLoadManager.SaveEncryptedJson(this.Save(), Name, _encryptionKey);

        public void Erase() => SaveLoadManager.Delete(Name);

        public void Register(ISaveLoadAsJson source)
        {
            _sources.Add(source.RootKey,source);
        }
    
        public void Unregister(ISaveLoadAsJson source)
        {
            _sources.Remove(source.RootKey);
        }

        public JSON SaveData { get; set; } = new JSON();
        public int CurrentVersion { get; set; } = 0;
        public string RootKey => Name;
        public void UpdateSaveData(JSON data) => OnUpdateAction?.Invoke(data);
        
        public void OnBeforeSave()
        {
            SaveData.Clear();
            foreach (ISaveLoadAsJson saveLoadAsJson in _sources.Values)
                SaveData.Save(saveLoadAsJson);
        }

        public void OnAfterLoad()
        {
            foreach (ISaveLoadAsJson saveLoadAsJson in _sources.Values)
                saveLoadAsJson.Load(SaveData);
        }
    }
}