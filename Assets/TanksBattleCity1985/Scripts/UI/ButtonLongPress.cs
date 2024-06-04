using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnLongPressDown = new UnityEvent();
    public UnityEvent OnLongPressUp = new UnityEvent();

    public void OnPointerDown(PointerEventData eventData)
    {
        OnLongPressDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnLongPressUp?.Invoke();
    }
}
