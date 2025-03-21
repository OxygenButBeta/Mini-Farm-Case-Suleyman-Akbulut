using R3;
using UnityEngine;
using Zenject;

public class UIController : MonoBehaviour{
    [SerializeField] ProductionView m_productionView;

    [Inject]
    void Construct(UIActionHandler uiActionHandler){
        uiActionHandler.SelectedFactory.Subscribe(OnFactorySelected);
    }

    private void OnFactorySelected(FarmFactory obj){
        if (obj == null){
            m_productionView.gameObject.SetActive(false);
            return;
        }

        if (!m_productionView.gameObject.activeSelf){
            m_productionView.gameObject.SetActive(true);
        }

        m_productionView.FocusTo(obj);
    }
}