using MemoryPack;

/// <summary>
/// A data structure for saving game state, containing an ID and a byte array to hold serialized data.
/// This class is intended to be used for serialization and deserialization purposes.
/// </summary>
[System.Serializable]
[MemoryPackable]
public sealed partial class SaveRecord{
    public int Id;
    public byte[] Data;
}