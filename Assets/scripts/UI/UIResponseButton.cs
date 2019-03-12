using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIResponseButton : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
{
    private Image selector;

    private void Awake()
    {
        selector = GetComponentInChildren<Image>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        selector.enabled = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        selector.enabled = false;
    }
}
