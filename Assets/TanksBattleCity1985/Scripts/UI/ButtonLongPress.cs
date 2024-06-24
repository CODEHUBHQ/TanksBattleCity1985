using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonLongPress : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent OnLongPressDown = new UnityEvent();
    public UnityEvent OnLongPressUp = new UnityEvent();

    private float isPressedTimer;
    private float isPressedTimerMax = 0.1f;

    private bool isPressed;

    private void FixedUpdate()
    {
        if (isPressed)
        {
            isPressedTimer -= Time.deltaTime;

            if (isPressedTimer < 0f)
            {
                OnLongPressDown?.Invoke();

                isPressedTimer = isPressedTimerMax;
            }
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
