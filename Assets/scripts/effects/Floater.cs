using DG.Tweening;
using UnityEngine;

namespace effects
{
    public class Floater : MonoBehaviour
    {
        [SerializeField] private float maxY;
        [SerializeField] private float time;
        [SerializeField] private float delay = 0;


        private void Start()
        {
            transform.DOBlendableMoveBy(new Vector2(0, maxY), time)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetDelay(delay);
        }
    }
}
