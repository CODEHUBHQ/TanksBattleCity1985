using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonLongPress : MonoBehaviour, IPointerUpHandler, IPointerMoveHandler
{
    public UnityEvent OnLongPressDown = new UnityEvent();
    public UnityEvent OnLongPressUp = new UnityEvent();

    public void OnPointerMove(PointerEventData eventData)
    {
        //OnLongPressDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //OnLongPressUp?.Invoke();
    }
}
