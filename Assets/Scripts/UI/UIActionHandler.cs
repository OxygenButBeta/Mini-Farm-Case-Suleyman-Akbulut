using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Zenject;

public class UIActionHandler : MonoBehaviour{
    [SerializeField] private ScreenToWorldRaycaster m_rayCaster;
    InputSystemUIInputModule m_inputSystemUIInputModule;
    public readonly ReactiveProperty<FarmFactory> SelectedFactory = new();

    [Inject]
    void Construct(InputSystemUIInputModule inputSystemUIInputModule){
        m_inputSystemUIInputModule = inputSystemUIInputModule;
        m_inputSystemUIInputModule.leftClick.action.started += OnLeftClick;
    }

    private void OnDestroy(){
        m_inputSystemUIInputModule.leftClick.action.started -= OnLeftClick;
    }

    private async void OnLeftClick(InputAction.CallbackContext obj){
        try{
            // Wait for the next frame to ensure UI state is properly updated before checking pointer position
            await UniTask.Yield();

            if (IsPointerOverUI())
                return;

            if (!m_rayCaster.TryGetSpherecastHit(out FarmFactory factory))
                return;

            if (SelectedFactory.Value != null && SelectedFactory.Value.Equals(factory)){
                SelectedFactory.Value = null;
                return;
            }

            SelectedFactory.Value = factory;
        }
        catch (Exception ){
           return; // TODO handle exception
        }
    }

    static bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    void Awake() => m_rayCaster.Initialize();
}