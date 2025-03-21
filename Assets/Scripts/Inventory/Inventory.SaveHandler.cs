using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;


// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Manages the player's inventory, including adding, removing.
/// It supports asynchronous loading of items via Addressables, and handles saving and loading inventory data.
/// The inventory is stored as a dictionary where each item is associated with a quantity.
/// 
/// It is initialized asynchronously using Addressables, and can be saved and loaded from a <see cref="RecordContainer"/>.
/// The class raises an event <see cref="OnInventoryUpdated"/> whenever the inventory changes.
/// </summary>
public partial class Inventory{
    public RecordContainer ExtractSaveData(){
        return new RecordContainer(m_InventoryDictionary.ToDictionary(itemQuantityPair => itemQuantityPair.Key.name,
            itemQuantityPair => itemQuantityPair.Value.ToString()));
    }


    async UniTaskVoid WaitUntilInitAndLoadSave(RecordContainer container){
        // ReSharper disable once HeapView.CanAvoidClosure
        await UniTask.WaitUntil(() => IsInitialized);
        foreach (KeyValuePair<string, string> itemQuantityPair in container.GetContainer()){
            ItemSO item = m_InventoryDictionary.Keys.FirstOrDefault(i => i.name == itemQuantityPair.Key);
            if (item is null){
                Debug.LogWarning($"Item {itemQuantityPair.Key} is not found in the inventory.");
                continue;
            }

            m_InventoryDictionary[item] = int.Parse(itemQuantityPair.Value);
        }

        OnInventoryUpdated?.Invoke();
    }

    public void LoadData(RecordContainer container) => WaitUntilInitAndLoadSave(container).Forget();
}