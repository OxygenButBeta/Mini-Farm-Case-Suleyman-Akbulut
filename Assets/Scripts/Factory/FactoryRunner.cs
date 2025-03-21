using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

/// <summary>
/// A runner class that updates all factories in the scene.
/// </summary>
public class FactoryRunner : MonoBehaviour{
    readonly HashSet<FarmFactory> m_factories = new();
    [SerializeField] SerializableReactiveProperty<bool> isPaused = new(false);

    private void Start() => StartProductionCycleAsync(destroyCancellationToken).Forget();

    private async UniTaskVoid StartProductionCycleAsync(CancellationToken token){
        while (!token.IsCancellationRequested){
            await UniTask.Delay(1000, cancellationToken: token);
            if (isPaused.Value)
                continue;

            UpdateFactories();
        }
    }

    void UpdateFactories(){
        foreach (FarmFactory factory in m_factories)
            factory.UpdatePerSecond();
    }

    public void RegisterFactory(FarmFactory farmFactory){
        var isRegistered = m_factories.Add(farmFactory);
        Debug.Assert(farmFactory != null, "Factory is null");
        Debug.Assert(isRegistered, "Factory is already registered");
    }

    // Test 
    public void UnregisterFactory(FarmFactory farmFactory) => m_factories.Remove(farmFactory);

    public void SetPause(bool pause) => isPaused.Value = pause;
}