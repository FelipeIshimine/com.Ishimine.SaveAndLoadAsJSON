using Leguar.TotalJSON;
using SaveSystem;

public static class JSONExtentions
{
    public static void Save(this JSON @this, ISaveLoadAsJson data)
    {
        data.OnBeforeSave();
        var saveData = data.SaveData;
        if (saveData == null) return;
        saveData.AddOrReplace(ISaveAsJsonUtilities.VersionKey, data.CurrentVersion);
        @this.Add(data.RootKey, saveData);
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
}