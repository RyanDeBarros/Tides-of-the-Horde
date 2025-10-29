using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public readonly UnityEvent onHoverEnter = new();
    public readonly UnityEvent onHoverExit = new();

    private bool isHovering = false;

    private void Awake()
    {
        Assert.IsNotNull(EventSystem.current);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            isHovering = true;
            onHoverEnter.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovering)
        {
            isHovering = false;
            onHoverExit.Invoke();
        }
    }

    private void OnEnable()
    {
        PointerEventData eventData = new(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Any(result => result.gameObject == gameObject))
            OnPointerEnter(eventData);
    }

    private void OnDisable()
    {
        OnPointerExit(new(EventSystem.current));
    }
}
