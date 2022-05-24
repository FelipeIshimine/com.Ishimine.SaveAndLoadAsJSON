using System;
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

        private event Action<JSON> OnLoad; 
        private event Action<JSON> OnSave;

        public Action<JSON> OnUpdateAction { get; set; }

        public void LoadFromFile() => Load(SaveLoadManager.LoadEncryptedJson(Name, _encryptionKey));

        public void SaveToFile() => SaveLoadManager.SaveEncryptedJson(GetSave(), Name, _encryptionKey);

        public void EraseFile() => SaveLoadManager.Delete(Name);

        public void Register(ISaveLoadAsJson source)
        {
            OnLoad += source.LoadData;
            OnSave += source.SaveData;
        }
    
        public void Unregister(ISaveLoadAsJson source)
        {
            OnLoad -= source.LoadData;
            OnSave -= source.SaveData;
        }


        public JSON Data { get; set; } = new JSON();
        public int CurrentVersion { get; set; } = 0;
        public string RootKey => Name;
        public void UpdateSaveData(JSON data) => OnUpdateAction?.Invoke(data);
        
        public JSON GetSave()
        {
            Data.Clear();
            OnSave?.Invoke(Data);
            return Data;
        }

        public void Load(JSON data)
        {
            Data = data;
            OnLoad?.Invoke(Data);
        }

    }
}