using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableBehaviour : MonoBehaviour
{
    public int unlockableIDToCheck = -1;
    public GameObject[] objectsToActivate;

    private void Start()
    {
        if (unlockableIDToCheck < 0) return;

        if (MenuDataManager.Instance.unlockableUnlockState.Count > 0)
        {
            if (MenuDataManager.Instance.unlockableUnlockState[unlockableIDToCheck])
            {
                for (int i = 0; i < objectsToActivate.Length; i++)
                {
                    objectsToActivate[i].SetActive(true);
                }

                gameObject.SetActive(false);
            }
        }
    }
}
