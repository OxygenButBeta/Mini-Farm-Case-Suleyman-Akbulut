using R3;
using UnityEngine;
using Zenject;

public partial class FarmFactory : SaveableBehaviour{
    [field: SerializeField] public FactorySO FactorySO{ get; private set; }
    // Exposed Fields (Can be replaced with ReactiveProperty<>)
    // 'SerializableReactiveProperty' is used in the Unity Editor for serializing and making properties editable in the Inspector,
    // In the Unity Editor, 'SerializableReactiveProperty' allows properties to be visible and editable in the Inspector.
#if UNITY_EDITOR
    public SerializableReactiveProperty<int> OrderCount = new(0);
    public SerializableReactiveProperty<bool> IsStockFull = new(false);
    public SerializableReactiveProperty<int> CurrentStock = new(0);
    public SerializableReactiveProperty<int> SecondsUntilNextProduction = new();
#else
    public ReactiveProperty<int> OrderCount = new(0);
    public readonly ReactiveProperty<bool> IsStockFull = new(false);
    public ReactiveProperty<int> CurrentStock = new(0);
    public ReactiveProperty<int> SecondsUntilNextProduction = new();
#endif
    Inventory m_inventory;

    // Inject dependencies
    [Inject]
    void Construct(FactoryRunner factoryRunner, Inventory inventory){
        m_inventory = inventory;
        factoryRunner.RegisterFactory(this);
    }

    public void UpdatePerSecond(){
        if (OrderCount.Value == 0 && !FactorySO.NoResourceNecessary){
            SecondsUntilNextProduction.Value = 0;
            return;
        }

        if (CheckIfStockFull())
            return;

        IsStockFull.Value = false;
        SecondsUntilNextProduction.Value--;
        if (SecondsUntilNextProduction.Value < 0)
            SecondsUntilNextProduction.Value = FactorySO.ProductionDuration;


        if (SecondsUntilNextProduction.Value == 0){
            Produce();
            CheckIfStockFull();
        }
    }

    private bool CheckIfStockFull(){
        if (CurrentStock.Value < FactorySO.StockCapacity)
            return false;

        SecondsUntilNextProduction.Value = 0;
        IsStockFull.Value = true;
        return true;
    }

    private void Produce(){
        if (!FactorySO.NoResourceNecessary)
            OrderCount.Value--;

        CurrentStock.Value++;
        SecondsUntilNextProduction.Value = FactorySO.ProductionDuration;
    }

    public bool CanOrderMore(int count){
        return !IsStockFull.Value && OrderCount.Value + count + CurrentStock.Value <= FactorySO.StockCapacity &&
               m_inventory.GetQuantity(FactorySO.RequiredMaterial) >= count;
    }

    public void AddOrder(int count){
        if (CanOrderMore(count)){
            OrderCount.Value += count;
            m_inventory.RemoveItem(FactorySO.RequiredMaterial, FactorySO.RequiredMaterialQuantity);
            if (OrderCount.Value == 1)
                SecondsUntilNextProduction.Value = FactorySO.ProductionDuration;
        }
        else{
            // Should be handled by UI but just in case
            Debug.Log("Not enough resources");
        }
    }

    public void CancelOrder(int count){
        OrderCount.Value -= count;

        // Return the resources
        m_inventory.AddItem(FactorySO.RequiredMaterial, FactorySO.RequiredMaterialQuantity);

        // Reset the timer if there is no order left
        if (OrderCount.Value != 0)
            return;

        SecondsUntilNextProduction.Value = 0;
    }

    public void Collect(){
        if (CurrentStock.Value == 0) // There is nothing to collect
            return;

        m_inventory.AddItem(FactorySO.ItemToProduce, CurrentStock.Value);
        CurrentStock.Value = 0;
    }
}