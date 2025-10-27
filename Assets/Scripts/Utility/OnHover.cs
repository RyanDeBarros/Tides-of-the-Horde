using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public readonly UnityEvent onHoverEnter = new();
    public readonly UnityEvent onHoverExit = new();

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit.Invoke();
    }
}
