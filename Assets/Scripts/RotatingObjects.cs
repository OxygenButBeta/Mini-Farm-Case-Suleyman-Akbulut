using DG.Tweening;
using UnityEngine;

public class RotatingObjects : MonoBehaviour{
    [SerializeField] Transform m_light;
    [SerializeField] Transform m_blade;
    [SerializeField] bool m_rotateSun = false;

    private void Awake(){
        m_blade.DOLocalRotate(new Vector3(0, 0, 360f), 6f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        if (!m_rotateSun)
            return;

        m_light.transform.DOLocalRotate(new Vector3(0, 0, 360f), 180f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}