using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Leguar.TotalJSON;
using SaveSystem;

public static class SaveLoadManager
{
    public static bool Exists(string filePath) => File.Exists(filePath);

    public static string GetFilePath(string fileName) => Application.persistentDataPath + "/" + fileName;
    
    public static void SaveBinary<T>(string fileName, T data)
    {
        string filePath = GetFilePath(fileName);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (filePath); 
        bf.Serialize(file, data);
        file.Close();
    }

    public static T LoadBinary<T>(string fileName)
    {
        T data = default;
        string filePath = GetFilePath(fileName);

        if (!Exists(filePath)) //Archivo no existe
            return data;
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);
        data = (T) bf.Deserialize(file);
        file.Close();
        return data;
    }
    
    public static void SaveText(string fileName, string data)
    {
        string filePath = GetFilePath(fileName);
        File.WriteAllText(filePath, data);
        
        Debug.Log($"<color=green>File saved </color> at:{filePath}");
    }
    
    public static string LoadText(string fileName)
    {
        string filePath = GetFilePath(fileName);
        return !Exists(filePath) ? null : File.ReadAllText(filePath);
    }
    
    public static void Delete(string fileName)
    {
        File.Delete(GetFilePath(fileName));
    }
    
     public static void SaveEncryptedJson(JSON json, string fileName, int encryptionKey = 0)
    {
        string rawData = Application.isEditor?json.CreatePrettyString():json.CreateString();
        if (encryptionKey != 0) rawData = SecureHelper.EncryptDecrypt(rawData, encryptionKey);
        SaveLoadManager.SaveText(fileName, rawData);
    }

    public static JSON LoadEncryptedJson(string fileName, int encriptionKey = 0)
    {
        Debug.Log($"Loading: {fileName}");
        string rawData = SaveLoadManager.LoadText(fileName);
        if (string.IsNullOrEmpty(rawData)) return new JSON();
        if (encriptionKey != 0) rawData = SecureHelper.EncryptDecrypt(rawData, encriptionKey);
        return JSON.ParseString(rawData);
    }
}


public static class SecureHelper
{
    public static string Hash(string data)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(data);
        SHA256Managed sha256 = new SHA256Managed();
        byte[] hashValue = sha256.ComputeHash(textToBytes);
        return GetHexStringFromHash(hashValue);
    }

    private static string GetHexStringFromHash(byte[] hashValue)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hashValue)
            stringBuilder.Append(b.ToString("x2"));
        return stringBuilder.ToString();
    }

    public static string EncryptDecrypt(string data, int key)
    {
        StringBuilder input = new StringBuilder(data);
        StringBuilder output = new StringBuilder(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            char character = input[i];
            character = (char) (character ^ key);
            output.Append(character);
        }
        return output.ToString();
    }
}