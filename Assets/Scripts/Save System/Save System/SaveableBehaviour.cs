using UnityEngine;
using Zenject;

/// <summary>
/// A base class for saveable MonoBehaviour entities that automatically generate an ID upon reset and register to the save system.
/// </summary>
public abstract class SaveableBehaviour : MonoBehaviour, ISaveableEntity{
    [field: SerializeField] public int Id{ get; private set; }
    public abstract RecordContainer ExtractSaveData();
    private void Reset() => Id = GetInstanceID();

    [Inject]
    void RegisterToSaveSystem(SaveableContainer saveableContainer) => saveableContainer.Bind(this);
    public abstract void LoadData(RecordContainer data);

}