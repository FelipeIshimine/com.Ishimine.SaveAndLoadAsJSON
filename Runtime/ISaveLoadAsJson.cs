using System.Collections.Generic;
using Leguar.TotalJSON;
using UnityEngine;

namespace SaveSystem
{
    public interface IJson
    {
        JSON GetSave();
        void Load(JSON data);
    }
    
    public interface ISaveLoadAsJson : IJson
    {
        JSON Data { get; set;}
        int CurrentVersion { get; }
        string RootKey { get; }
        /// <summary>
        /// Ejecutador por ISaveAsJsonUtilities.CheckAndUpdate
        /// </summary>
        /// <param name="data"></param>
        void UpdateSaveData(JSON data);
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

        public static void SaveData(this ISaveLoadAsJson source, JSON mainData)
        {
            var saveData = source.GetSave();
            if (saveData == null) return;
            saveData.AddOrReplace(VersionKey, source.CurrentVersion);
            mainData.Add(source.RootKey, saveData);
        }

        public static void LoadData(this ISaveLoadAsJson source, JSON mainData)
        {
            if(!mainData.ContainsKey(source.RootKey)) return;
            JSON loadData = mainData.GetJSON(source.RootKey);
            
            if (IsOldVersion(source, loadData))
                source.UpdateSaveData(loadData);

            source.Data = loadData;
            source.Load(loadData);
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