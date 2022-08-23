using System.Collections.Generic;
using Leguar.TotalJSON;
using UnityEngine;

namespace SaveSystem
{
    public interface ISaveLoadAsJson 
    {
        JSON SaveData { get; set;}
        int CurrentVersion { get; }
        string RootKey { get; }
        /// <summary>
        /// Ejecutado por ISaveAsJsonUtilities.CheckAndUpdate
        /// </summary>
        /// <param name="data"></param>
        void UpdateSaveData(JSON data);
        
        void OnBeforeSave();
        void OnAfterLoad();
    }

    /// <summary>
    /// Verifica si los datos requiere actualizacion y si los tienen ejecuta la funcion UpdateSaveData
    /// </summary>
    public static class ISaveAsJsonUtilities
    {
        public const string VersionKey = "VERSION";

        public static bool IsOldVersion(this ISaveLoadAsJson source, JSON data)
        {
            if (!data.ContainsKey(VersionKey))
                throw new KeyNotFoundException($"El archivo NO contiene la VersionKey '{VersionKey}' para el tipo de archivo: {source.GetType()}");

            int fileVersion = data.GetInt(VersionKey);
            return fileVersion < source.CurrentVersion;
        }

        public static JSON Save(this ISaveLoadAsJson @this)
        {
            @this.OnBeforeSave();
            return @this.SaveData;
        }
        
        public static void Save(this ISaveLoadAsJson @this, JSON mainData)
        {
            @this.OnBeforeSave();
            var saveData = @this.SaveData;
            if (saveData == null) return;
            saveData.AddOrReplace(VersionKey, @this.CurrentVersion);
            mainData.Add(@this.RootKey, saveData);
        }

        public static void Load(this ISaveLoadAsJson source, JSON mainData)
        {
            if(!mainData.ContainsKey(source.RootKey)) return;
            JSON loadData = mainData.GetJSON(source.RootKey);
            
            if (IsOldVersion(source, loadData))
                source.UpdateSaveData(loadData);

            source.SaveData = loadData;
            source.OnAfterLoad();
        }
    }
    
    public interface ISaveLoadAsJValue
    {
        public abstract int CurrentVersion { get; }
        public abstract string RootKey { get; }
        public abstract JValue GetSave();
        public abstract void Load(JValue data);
    }
}