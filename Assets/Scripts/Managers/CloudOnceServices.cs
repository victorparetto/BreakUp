using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudOnce;

public class CloudOnceServices : MonoBehaviour
{
    public static CloudOnceServices instance;

    private void Awake()
    {
        CloudOnceSingleton();
    }

    private void CloudOnceSingleton()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SubmitScoreToClimbLeaderboard(int score)
    {
        Leaderboards.ClimbModeLeaderboard.SubmitScore(score);
    }

    public void SubmitScoreToClassicLeaderboard(int score)
    {
        Leaderboards.ClassicModeLeaderboard.SubmitScore(score);
    }
}
