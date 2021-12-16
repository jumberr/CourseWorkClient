using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class SmoothScrollRect : ScrollRect
    {
        public bool SmoothScrolling { get; set; } = true;
        public float SmoothScrollTime { get; set; } = 0.08f;
 
        public override void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;
 
 
            if (SmoothScrolling)
            {
                Vector2 positionBefore = normalizedPosition;
                this.DOKill(true);
                base.OnScroll(data);
                Vector2 positionAfter = normalizedPosition;
 
                normalizedPosition = positionBefore;
                this.DONormalizedPos(positionAfter, SmoothScrollTime);
            }
            else
            {
                base.OnScroll(data);
            }
        }
    }
}