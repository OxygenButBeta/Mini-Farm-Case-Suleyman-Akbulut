using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI{
    public class ResourcesUIBindings : MonoBehaviour{
        [SerializeField] ItemTypePair[] m_itemTypePairs;

        Inventory m_inventory;

        [Inject]
        async UniTask Construct(Inventory inventory){
            m_inventory = inventory;
            m_inventory.OnInventoryUpdated += UpdateGUI;

            // Closure irrelevant
            // ReSharper disable once HeapView.CanAvoidClosure
            await UniTask.WaitUntil(() => m_inventory.IsInitialized);

            UpdateGUI();
        }

        private void UpdateGUI(){
            // Update UI
            foreach (ItemTypePair pair in m_itemTypePairs){
                pair.Tmp.text = m_inventory.GetQuantity(pair.Item).ToString();
            }
        }


        // A local struct to bind each items easily in the inspector
        [System.Serializable]
        struct ItemTypePair{
            public ItemSO Item;
            public TMP_Text Tmp;
        }
    }
}