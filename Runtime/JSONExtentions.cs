using Leguar.TotalJSON;
using SaveSystem;
using UnityEngine;

public static class JSONExtentions
{
    public static void Load(this JSON mainData, ISaveLoadAsJson source)
    {
        if (!mainData.ContainsKey(source.RootKey))
            return;

        JSON loadData = mainData.GetJSON(source.RootKey);
        source.ManualLoad(loadData);
    }
    
    public static void Save(this JSON @this, ISaveLoadAsJson data)
    {
        data.ManualSave();
        if (data.SaveData == null) return;
        @this.Add(data.RootKey, data.SaveData);
    }
    
    public static bool TryGet<T>(this JSON @this, string key, out T value) where T : JValue
    {
        if (@this.ContainsKey(key))
        {
            value = @this.Get(key) as T;
            return true;
        }
        value = null;
        return false;
    }
		
    public static bool TryGet(this JSON @this,string key, out bool value)
    {
        if (@this.TryGet(key, out JBoolean jValue))
        {
            value = jValue.AsBool();
            return true;
        }

        value = default;
        return false;
    }
		
    public static bool TryGet(this JSON @this, string key, out int value)
    {
        if (@this.TryGet(key, out JNumber jValue))
        {
            value = jValue.AsInt();
            return true;
        }
        value = default;
        return false;
    }
    
    public static bool TryGet(this JSON @this, string key, out string value)
    {
        if (@this.TryGet(key, out JString jValue))
        {
            value = jValue.AsString();
            return true;
        }
        value = default;
        return false;
    }
}