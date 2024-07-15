using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnbreakableBehav : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    
    public void BuildUnbreakable()
    {
        //gameObject.SetActive(true);
        anim.SetTrigger("Build");
    }

    public void UnbuildUnbreakable()
    {
        anim.SetTrigger("Unbuild");
    }

    void HideGameObject()
    {
        gameObject.SetActive(false);
    }
}
