using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [System.Serializable]
    public class Achievement
    {
        public string header;
        public int number;
        public string description;
        public string ID;
        public int goal;
        public int prizeAmount;
        public bool unlocked;
        public bool claimed;
    }

    public Achievement[] achievements;

    [SerializeField] NotificationBehaviour nb;

    private void Awake()
    {
        AchievementDataManager.Instance.Load();

        instance = this;

        VerifyAchievementCountData();

        for (int i = 0; i < achievements.Length; i++)
        {
            achievements[i].unlocked = AchievementDataManager.Instance.achievementsUnlockState[i];
            achievements[i].claimed = AchievementDataManager.Instance.achievementsClaimedState[i];
        }
    }

    public void VerifyAchievementProgress(int idNumber, string ID, int current)
    {
        if (achievements[idNumber].unlocked) { print("Achievement was unlocked already, returned"); return; }
        Achievement ach = achievements.FirstOrDefault(x => x.ID == ID);

        if (!ach.unlocked)
        {
            if(current >= ach.goal)
            {
                ach.unlocked = true;
                AchievementDataManager.Instance.achievementsUnlockState[ach.number] = true;
                AchievementDataManager.Instance.Save();
                print("Unlocked Achievement: " + ach.header);

                nb.AddNewNotification(ach.header, ach.description);
            }
        }
    }

    public void VerifyAchievementCountData()
    {
        if (AchievementDataManager.Instance.achievementsUnlockState.Count == 0)
        {
            for (int i = 0; i < achievements.Length; i++)
            {
                AchievementDataManager.Instance.achievementsUnlockState.Add(false);
            }
        }
        else if (AchievementDataManager.Instance.achievementsUnlockState.Count < achievements.Length)
        {
            int temp = achievements.Length - AchievementDataManager.Instance.achievementsUnlockState.Count;
            for (int i = 0; i < temp; i++)
            {
                AchievementDataManager.Instance.achievementsUnlockState.Add(false);
            }
        }

        if (AchievementDataManager.Instance.achievementsClaimedState.Count == 0)
        {
            for (int i = 0; i < achievements.Length; i++)
            {
                AchievementDataManager.Instance.achievementsClaimedState.Add(false);
            }
        }
        else if (AchievementDataManager.Instance.achievementsClaimedState.Count < achievements.Length)
        {
            int temp = achievements.Length - AchievementDataManager.Instance.achievementsClaimedState.Count;
            for (int i = 0; i < temp; i++)
            {
                AchievementDataManager.Instance.achievementsClaimedState.Add(false);
            }
        }
    }
}
