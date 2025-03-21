using System;
using System.Collections.Generic;


/// <summary>
/// A simple container for storing key-value pairs of records.
/// </summary>
public class RecordContainer{
    readonly Dictionary<string, string> records;

    public RecordContainer AddRecord(object obj, string key){
        if (!records.TryAdd(key, obj.ToString())){
            throw new ArgumentException($"Key {key} already exists in records");
        }

        return this;
    }

    public T GetRecord<T>(string key){
        if (records.TryGetValue(key, out var result)){
            return (T)Convert.ChangeType(result, typeof(T));
        }

        throw new KeyNotFoundException($"Key {key} not found in records");
    }

    public T TryGetRecord<T>(string key, T defaultValue){
        if (records.TryGetValue(key, out var result)){
            return (T)Convert.ChangeType(result, typeof(T));
        }

        return defaultValue;
    }

    public RecordContainer(Dictionary<string, string> records) => this.records = records;
    public RecordContainer() => records = new Dictionary<string, string>();
    public IEnumerable<KeyValuePair<string, string>> GetContainer() => records;
}