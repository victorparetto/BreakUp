using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UnlockableManager : MonoBehaviour
{
    public static UnlockableManager instance;
    Animator anim;

    [System.Serializable]
    public class Unlockable
    {
        public string header;
        public int number;
        public string description;
        //public string ID;
        public bool unlocked;
    }

    public Unlockable[] unlockables;

    [SerializeField] NotificationBehaviour nb;

    private void Start()
    {
        //MenuDataManager.Instance.Load();

        instance = this;

        VerifyUnlockableCountData();

        for (int i = 0; i < unlockables.Length; i++)
        {
            unlockables[i].unlocked = MenuDataManager.Instance.unlockableUnlockState[i];
        }
    }

    public void CompletedUnlockable(int idNumber, string ID)
    {
        if (unlockables[idNumber].unlocked) { print("Unlockable was already completed"); return; }
        Unlockable unl = unlockables.FirstOrDefault(x => x.number == idNumber);

        if (!unl.unlocked)
        {
            unl.unlocked = true;
            MenuDataManager.Instance.unlockableUnlockState[unl.number] = true;
            MenuDataManager.Instance.Save();
            print(ID);
            //print("Unlockable Completed: " + unl.header);

            nb.AddNewNotification(unl.header, unl.description);
        }
    }

    public void VerifyUnlockableCountData()
    {
        if (MenuDataManager.Instance.unlockableUnlockState.Count == 0)
        {
            for (int i = 0; i < unlockables.Length; i++)
            {
                MenuDataManager.Instance.unlockableUnlockState.Add(false);
            }
        }
        else if (MenuDataManager.Instance.unlockableUnlockState.Count < unlockables.Length)
        {
            int temp = unlockables.Length - MenuDataManager.Instance.unlockableUnlockState.Count;
            for (int i = 0; i < temp; i++)
            {
                MenuDataManager.Instance.unlockableUnlockState.Add(false);
            }
        }
    }
}
