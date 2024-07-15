using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;

    public Color tabIdleColor;
    public Color tabSelectedColor;

    public TabButton selectedTab;

    private void Awake()
    {
        selectedTab = transform.GetChild(0).GetComponent<TabButton>();
    }

    private void Start()
    {
        StartMenu(selectedTab);
    }

    private void StartMenu(TabButton btn)
    {
        selectedTab.Select();
        //btn.bgImage.color = tabSelectedColor;
    }

    public void AddTab(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();

        tabButtons.Add(button);
    }

    public void OnTabSelected(TabButton btn)
    {
        if (selectedTab == btn) return;

        if(selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = btn;
        selectedTab.Select();

        ResetTabs();
        //btn.bgImage.color = tabSelectedColor;
    }

    public void ResetTabs()
    {
        foreach(TabButton btn in tabButtons)
        {
            if (selectedTab != null && btn == selectedTab) continue;
            //btn.bgImage.color = tabIdleColor;
        }
    }

    public void Select()
    {

    }
    
    public void Deselect()
    {

    }
}
