using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonLongPress : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent OnLongPressDown = new UnityEvent();
    public UnityEvent OnLongPressUp = new UnityEvent();

    private bool isPressed;

    private void FixedUpdate()
    {
        if (isPressed)
        {
            OnLongPressDown?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //OnLongPressDown?.Invoke();
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //OnLongPressUp?.Invoke();
        isPressed = false;
    }
}
