using System.Collections.Generic;
using Leguar.TotalJSON;
using UnityEngine;

namespace SaveSystem
{
    public interface ISaveLoadAsJson 
    {
        public const string VersionKey = "_V_";
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
        public static bool IsOldVersion(this ISaveLoadAsJson source, JSON data)
        {
            if (!data.ContainsKey(ISaveLoadAsJson.VersionKey))
            {
                data.DebugInEditor($"Key:'{ISaveLoadAsJson.VersionKey}' not found");
                throw new KeyNotFoundException($"El archivo NO contiene la VersionKey '{ISaveLoadAsJson.VersionKey}' para el tipo de archivo: {source.GetType()}");
            }

            int fileVersion = data.GetInt(ISaveLoadAsJson.VersionKey);
            return fileVersion < source.CurrentVersion;
        }

        public static JSON ManualSave(this ISaveLoadAsJson @this)
        {
            @this.SaveData ??= new JSON();
            @this.SaveData.Clear();
            @this.SaveData.Add(ISaveLoadAsJson.VersionKey, @this.CurrentVersion);
            @this.OnBeforeSave();
            @this.SaveData.AddOrReplace(ISaveLoadAsJson.VersionKey, @this.CurrentVersion);
            return @this.SaveData;
        }
        
        public static void ManualLoad(this ISaveLoadAsJson @this, JSON data)
        {
            if(@this.IsOldVersion(data))
                @this.UpdateSaveData(data);

            @this.SaveData = data;
            @this.OnAfterLoad();
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