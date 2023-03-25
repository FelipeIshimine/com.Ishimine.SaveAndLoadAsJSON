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

        value = default;
        return false;
    }
		
    public static bool TryGet(this JSON @this,string key, ref bool value)
    {
        if (@this.TryGet(key, out JBoolean jValue))
        {
            value = jValue.AsBool();
            return true;
        }
        return false;
    }
		
    public static bool TryGet(this JSON @this, string key, ref int value)
    {
        if (@this.TryGet(key, out JNumber jValue))
        {
            value = jValue.AsInt();
            return true;
        }
        return false;
    }
    public static bool TryGet(this JSON @this, string key, ref float value)
    {
        if (@this.TryGet(key, out JNumber jValue))
        {
            value = jValue.AsFloat();
            return true;
        }
        return false;
    }
    
    public static bool TryGet(this JSON @this, string key, ref string value)
    {
        if (@this.TryGet(key, out JString jValue))
        {
            value = jValue.AsString();
            return true;
        }
        return false;
    }
}