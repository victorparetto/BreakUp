using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationBehaviour : MonoBehaviour
{
    Animator anim;

    public GameObject panelToShow = null;
    public TextMeshProUGUI headerT;
    public TextMeshProUGUI descriptionT;

    public bool currentlyShowing = false;

    public class NotificationShown
    {
        public string header;
        public string description;
    }

    public List<NotificationShown> notificationsShown = new List<NotificationShown>();

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public NotificationShown CreateNotificationShown(string header, string description)
    {
        NotificationShown achSh = new NotificationShown();

        achSh.header = header;
        achSh.description = description;

        return achSh;
    }

    public void ShowNextNotification()
    {
        if (notificationsShown.Count == 0)
        {
            currentlyShowing = false;
            return;
        }
        else
        {
            currentlyShowing = true;
            headerT.text = notificationsShown[0].header;
            descriptionT.text = notificationsShown[0].description;

            notificationsShown.RemoveAt(0);
            anim.SetTrigger("unlocked");
        }
    }

    public void AddNewNotification(string header, string description)
    {
        notificationsShown.Add(CreateNotificationShown(header, description));

        if (!currentlyShowing)
        {
            ShowNextNotification();
        }
    }

    public void ShowNotificationPanel()
    {
        panelToShow.SetActive(true);
    }
}
