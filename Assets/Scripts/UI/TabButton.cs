using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerDownHandler
{
    public TabGroup tabGroup;

    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private void Start()
    {
        //bgImage = GetComponent<Image>();
        tabGroup.AddTab(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void Select()
    {
        if(onTabDeselected != null)
        {
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if(onTabDeselected != null)
        {
            onTabDeselected.Invoke();
        }
    }
}
