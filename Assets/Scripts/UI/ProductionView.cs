using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Manages the user interface for the farm factory production system. 
/// Handles production progress, order management, and resource requirements.
/// Displays the current stock, active orders, and required materials for production.
/// Allows the player to add, cancel, and collect orders based on available resources.
/// 
/// Utilizes reactive programming with observables to automatically update UI elements 
/// whenever relevant data changes. Reactively subscribes to updates from the farm factory, 
/// inventory, and the production system, ensuring the UI stays in sync with the game state.
/// </summary>
public class ProductionView : MonoBehaviour{
    [SerializeField] Slider m_remainingTimeSlider;
    [SerializeField] TMP_Text m_remainingTimeText;
    [SerializeField] TMP_Text m_currentStockText;
    [SerializeField] TMP_Text m_activeOrderAndCapacityText;
    [SerializeField] Button m_addOrderButton;
    [SerializeField] Button m_cancelOrderButton;
    [SerializeField] Button m_collectButton;
    [SerializeField] Image m_itemToProduceImage;
    [SerializeField] Image m_requiredItemImage;
    [SerializeField] TMP_Text m_requiredItemAmount;

    // Dependencies
    [Inject] Inventory m_inventory;

    // private fields
    readonly List<IDisposable> m_reactiveSubscriptions = new();
    FarmFactory m_factory;
    TweenerCore<float, float, FloatOptions> tween;
    RectTransform m_rectTransform;


    // Unity Lifecycle Methods
    private void Awake(){
        m_rectTransform = transform as RectTransform;
        m_collectButton.onClick.AddListener(() => m_factory.Collect());
        m_addOrderButton.onClick.AddListener(() => m_factory.AddOrder(1));
        m_cancelOrderButton.onClick.AddListener(CancelOrder);
    }

    private void OnDisable() => DisposeSubscriptions();

    // Public Methods
    public void FocusTo(FarmFactory factory){
        if (!m_inventory.IsInitialized){
            gameObject.SetActive(false);
            return;
        }

        DisposeSubscriptions();
        tween?.Kill();

        m_factory = factory;

        m_addOrderButton.gameObject.SetActive(!factory.FactorySO.NoResourceNecessary);
        m_cancelOrderButton.gameObject.SetActive(!factory.FactorySO.NoResourceNecessary);

        m_rectTransform.position = Mouse.current.position.ReadValue();
        m_activeOrderAndCapacityText.gameObject.SetActive(!factory.FactorySO.NoResourceNecessary);

        // Set static data
        m_remainingTimeSlider.maxValue = factory.FactorySO.ProductionDuration;
        m_remainingTimeSlider.value = factory.SecondsUntilNextProduction.Value;

        m_itemToProduceImage.sprite = factory.FactorySO.ItemToProduce.Icon;
        // Will not be visible anyway
        m_requiredItemImage.sprite = factory.FactorySO.RequiredMaterial?.Icon;
        m_requiredItemAmount.text = $"{factory.FactorySO.RequiredMaterialQuantity}x";

        SubscribeToFarmFactory(factory);
    }

    void CancelOrder(){
        m_factory.CancelOrder(1);
        m_cancelOrderButton.interactable = m_factory.OrderCount.Value > 0;
    }

    // Subscription Handling
    private void SubscribeToFarmFactory(FarmFactory factory){
        m_reactiveSubscriptions.Add(factory.CurrentStock.Subscribe(OnStockChanged));

        //Merge streams
        Observable<int> orderCountStream = factory.OrderCount.AsObservable();
        Observable<int> inventoryUpdateStream = Observable.FromEvent(
            addHandler => m_inventory.OnInventoryUpdated += addHandler,
            removeHandler => m_inventory.OnInventoryUpdated -= removeHandler
        ).Select(Selector);

        Observable<int> mergedStream = Observable.Merge(orderCountStream, inventoryUpdateStream);
        Observable<int> buttonMergeStream = Observable.Merge(mergedStream, m_factory.CurrentStock.AsObservable());
        Observable<Unit> sliderStream = Observable.Merge(
            m_factory.SecondsUntilNextProduction.AsUnitObservable(),
            m_factory.IsStockFull.AsUnitObservable()
        );


        m_reactiveSubscriptions.Add(mergedStream.Subscribe(SetOrderAndCapacity));
        m_reactiveSubscriptions.Add(sliderStream.Subscribe(SetSliderData));

        if (!m_factory.FactorySO.NoResourceNecessary)
            m_reactiveSubscriptions.Add(buttonMergeStream.Subscribe(SetButtonState));

        return;

        // Selectors for stream merging
        void SetSliderData(Unit _) => SetTimeSlider();
        int Selector(Unit _) => factory.OrderCount.Value;
    }

    void SetButtonState(int _){
        if (m_factory.FactorySO.NoResourceNecessary)
            return;
        m_addOrderButton.interactable = m_factory.CanOrderMore(1);
        m_cancelOrderButton.interactable = m_factory.OrderCount.Value > 0;
    }

    private void DisposeSubscriptions(){
        m_reactiveSubscriptions.ForEach(subscription => subscription.Dispose());
        m_reactiveSubscriptions.Clear();
    }

    // UI Update Methods
    private void OnStockChanged(int stock) =>
        m_currentStockText.text = stock.ToString();

    private void SetOrderAndCapacity(int orderCount) =>
        m_activeOrderAndCapacityText.text = $"{orderCount}/{m_factory.FactorySO.StockCapacity}";


    private void SetTimeSlider(){
        tween?.Kill();

        if (m_factory.IsStockFull.Value){
            m_remainingTimeSlider.value = m_remainingTimeSlider.maxValue;
            m_remainingTimeText.text = "Full";
            return;
        }

        var targetValue = m_factory.SecondsUntilNextProduction.Value;

        if (Math.Abs(m_remainingTimeSlider.value - targetValue) > 1)
            m_remainingTimeSlider.value = targetValue;
        else
            // Just to make slider move smoothly
            tween = m_remainingTimeSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);

        m_remainingTimeText.text = $"{targetValue} sn"; // Not using string interpolation for compatibility
    }
}