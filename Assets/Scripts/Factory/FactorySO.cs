using UnityEngine;


/// <summary>
/// A ScriptableObject that represents a factory.
/// A new factory can be easily created and configured in the editor.  
/// </summary>
[CreateAssetMenu(fileName = "FactorySO", menuName = "Factory/New Factory SO")]
public class FactorySO : ScriptableObject{
    [field: SerializeField] public bool NoResourceNecessary{ get; private set; }
    [field: SerializeField] public int StockCapacity{ get; private set; } = 10;
    [field: SerializeField] public ItemSO RequiredMaterial{ get; private set; }
    [field: SerializeField] public int RequiredMaterialQuantity{ get; private set; }
    [field: SerializeField] public ItemSO ItemToProduce{ get; private set; }
    [field: SerializeField] public int ProductionDuration{ get; private set; }
}