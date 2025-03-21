using UnityEngine;

/// <summary>
/// A basic structure for items in the game, designed for future extensibility. 
/// This ScriptableObject can be expanded to support various item types.
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "Items/ItemSO", order = 1)]
public class ItemSO : ScriptableObject{
    public Sprite Icon => m_icon;
    [SerializeField] Sprite m_icon;
}