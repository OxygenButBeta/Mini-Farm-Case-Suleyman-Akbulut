/// <summary>
/// Represents an entity that can be saved and loaded.
/// </summary>
public interface ISaveableEntity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Extracts the current state of the entity as a RecordContainer.
    /// This data can be used for saving the entity's state.
    /// </summary>
    RecordContainer ExtractSaveData();

    /// <summary>
    /// Loads the saved state from the given RecordContainer,
    /// restoring the entity to a previous state.
    /// </summary>
    /// <param name="data">The saved data to be applied to the entity.</param>
    void LoadData(RecordContainer data);
}