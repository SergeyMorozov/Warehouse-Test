using GAME;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventsTriggerButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerDown()
    {
//        JoystickSystem.Events.OnEventTrigger?.Invoke(_button, EventTriggerType.PointerDown);
    }

    public void OnPointerUp()
    {
//        JoystickSystem.Events.OnEventTrigger?.Invoke(_button, EventTriggerType.PointerUp);
    }

}
