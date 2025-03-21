using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Manages the player's inventory, including adding, removing, and querying items.
/// It supports asynchronous loading of items via Addressables, and handles saving and loading inventory data.
/// The inventory is stored as a dictionary where each item is associated with a quantity.
/// 
/// It is initialized asynchronously using Addressables, and can be saved and loaded from a <see cref="RecordContainer"/>.
/// The class raises an event <see cref="OnInventoryUpdated"/> whenever the inventory changes.
/// </summary>
public partial class Inventory : ISaveableEntity{
    public int Id => 1002;
    public bool IsInitialized{ get; private set; }
    public event Action OnInventoryUpdated;

    readonly Dictionary<ItemSO, int> m_InventoryDictionary = new();

    [Inject]
    public void InjectInventory(SaveableContainer m_SaveableContainer){
        m_SaveableContainer.Bind(this);
        List<UniTask> AsyncLoadingOperations = new();

        Addressables.LoadResourceLocationsAsync("Item").Completed += handle => {
            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception("Failed to load items");

            foreach (IResourceLocation location in handle.Result){
                UniTask<ItemSO> asyncOperationHandleTask = Addressables.LoadAssetAsync<ItemSO>(location).ToUniTask();

                AsyncLoadingOperations.Add(asyncOperationHandleTask.ContinueWith((ItemSO) => {
                    m_InventoryDictionary.Add(ItemSO, 0);
                }));
            }

            UniTask.WhenAll(AsyncLoadingOperations).ContinueWith(() => IsInitialized = true).Forget();
        };
    }

    public void AddItem(ItemSO item, int quantity){
        if (!m_InventoryDictionary.ContainsKey(item)){
            ThrowItemIsNotRegisteredException(item);
            return;
        }

        m_InventoryDictionary[item] += quantity;
        OnInventoryUpdated?.Invoke();
    }

    public void RemoveItem(ItemSO item, int quantity){
        if (!m_InventoryDictionary.ContainsKey(item)){
            ThrowItemIsNotRegisteredException(item);
        }

        m_InventoryDictionary[item] -= quantity;
        OnInventoryUpdated?.Invoke();
    }

    public int GetQuantity(ItemSO item){
        return m_InventoryDictionary.TryGetValue(item, out var quantity)
            ? quantity
            : ThrowItemIsNotRegisteredException(item);
    }

    int ThrowItemIsNotRegisteredException(ItemSO item) => throw new ArgumentNullException(
        $"Item {item.name} is not found in the inventory. be sure that the item addressable label is set to 'Item'");

}