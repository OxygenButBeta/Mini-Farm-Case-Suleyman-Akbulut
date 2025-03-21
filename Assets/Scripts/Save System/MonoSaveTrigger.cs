using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// A simple save handler that manages loading and saving data for all saveable entities.
/// This solution is case-specific and intended for quick implementation. 
/// </summary>
[DefaultExecutionOrder(99999)]
public class MonoSaveTrigger : MonoBehaviour{
    [Inject] SaveableContainer m_saveableContainer;

    private void Awake(){
        if (!SaveSystemUtilities.HasSaveData()){
            Debug.Log("No save data found.");
            return;
        }

        // Load saved records
        IReadOnlyDictionary<int, SaveRecord> records = SaveSystemUtilities.LoadSaveRecordsFromPlayerPrefs();
        SaveSystemUtilities.LoadAll(records, m_saveableContainer);
    }

    [Inject]
    void injectPoint(List<ISaveableEntity> m_saveableEntities){
        foreach (ISaveableEntity mSaveableEntity in m_saveableEntities){
            Debug.Log("Loading Save Data for " + mSaveableEntity.GetType().Name);
        }
    }

    void SaveAll(){
        Debug.Log("Saving all data.");
        SaveSystemUtilities.SaveRecordsToPlayerPrefs(SaveSystemUtilities.CollectSaveData(m_saveableContainer));
    }

    // Case-specific implementation
    private void OnApplicationQuit() => SaveAll();
}