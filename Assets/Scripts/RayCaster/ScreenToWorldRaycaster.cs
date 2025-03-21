using System;
using UnityEngine;

/// <summary>
/// A simple utility class for casting a sphere ray from the screen's mouse position into the world space. 
/// It attempts to detect and return a target object of type TTarget if a hit occurs within a specified range.
/// </summary>
[Serializable]
public class ScreenToWorldRaycaster{
    Camera m_povCamera;
    [SerializeField] float rayLength = 5f;
    [SerializeField] private float castSphereRadius = .2f;
    [SerializeField] LayerMask TargetLayer;
    public void Initialize() => m_povCamera = Camera.main;

    public bool TryGetSpherecastHit<TTarget>(out TTarget target){
        Ray ray = m_povCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.SphereCast(ray, castSphereRadius, out RaycastHit hit, rayLength, TargetLayer.value)){
            if (hit.collider.TryGetComponent(out target))
                return true;
        }

        target = default;
        return false;
    }
}