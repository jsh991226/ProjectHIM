using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{

    [Header("Button Imgs")]
    public Texture buttonIdle;
    public Texture buttonHover;

    private RawImage texObj;

    void Start()
    {
        texObj = gameObject.GetComponent<RawImage>();
        texObj.texture = buttonIdle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texObj.texture = buttonHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texObj.texture = buttonIdle;
    }

}
