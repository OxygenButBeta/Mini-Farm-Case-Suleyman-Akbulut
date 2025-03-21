using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using UnityEngine;

/// <summary>
/// Simple save system to save and load data from PlayerPrefs. 
/// </summary>
public static class SaveSystemUtilities{
    public static IReadOnlyDictionary<int, SaveRecord> CollectSaveData(SaveableContainer container){
        Dictionary<int, SaveRecord> records = new();
        foreach (ISaveableEntity saveableEntity in container.GetSaveableEntities()){
            SaveRecord record = new()
            {
                Id = saveableEntity.Id,
                Data = MemoryPackSerializer.Serialize(saveableEntity.ExtractSaveData().GetContainer())
            };
            records[record.Id] = record;
        }

        return records;
    }

    public static void LoadAll(IReadOnlyDictionary<int, SaveRecord> records, SaveableContainer container){
        foreach (ISaveableEntity saveableEntity in container.GetSaveableEntities()){
            if (!records.TryGetValue(saveableEntity.Id, out var record))
                continue;

            Dictionary<string, string> data = MemoryPackSerializer.Deserialize<Dictionary<string, string>>(record.Data);
            saveableEntity.LoadData(new RecordContainer(data));
        }
    }


    /// <summary>
    /// A simple, case-specific solution for saving and loading data using PlayerPrefs.
    /// It serializes the save data into a byte array using MemoryPack, then stores it as a Base64-encoded string.
    /// While this approach is fast and suitable for small-scale testing,
    /// it can be extended for more secure or large-scale solutions such as cloud storage or encryption.
    /// </summary>

    #region For testing purposes

    public static void SaveRecordsToPlayerPrefs(IReadOnlyDictionary<int, SaveRecord> records){
        var data = MemoryPackSerializer.Serialize(records);
        PlayerPrefs.SetString("SaveData", Convert.ToBase64String(data));
    }

    // For testing purposes
    public static IReadOnlyDictionary<int, SaveRecord> LoadSaveRecordsFromPlayerPrefs(){
        var data = PlayerPrefs.GetString("SaveData");
        var bytes = Convert.FromBase64String(data);
        return MemoryPackSerializer.Deserialize<Dictionary<int, SaveRecord>>(bytes);
    }

    // For testing purposes
    public static bool HasSaveData() => PlayerPrefs.HasKey("SaveData");

    #endregion
}