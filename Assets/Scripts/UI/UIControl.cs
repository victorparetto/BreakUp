using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
	[SerializeField] GameManager gm;

    [SerializeField] int health;
    float miliseconds;
    float seconds;
    float minutes;

    [Header("Text Variables")]
    [SerializeField] TMPro.TextMeshProUGUI textTimer;
    [SerializeField] TMPro.TextMeshProUGUI textHealth;
    [SerializeField] TMPro.TextMeshProUGUI currentFloor_Text;
    [SerializeField] TMPro.TextMeshProUGUI currentMoney_Text;
    [SerializeField] Animator currentMoneyAnimator;

    //Pause Variables
    [Header("UI Buttons Variables")]
    [SerializeField] GameObject pausePanel = null;
	[SerializeField] Image pause_Button;
	[SerializeField] Sprite pause_Sprite;
	[SerializeField] Sprite unpause_Sprite;
    [SerializeField] Animator settingsExpandableAnimator;

    //Confirmation Panel Variables
    [Header("Confirmation Panel Variables")]
    [SerializeField] GameObject confirmationPanel = null;
    [SerializeField] Button AcceptButton = null;
    [SerializeField] TMPro.TMP_Text panelDescription;

    //Score Variables
    [Header("Score Variables")]
    [SerializeField] TMP_Text scoreHeaderText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject showScorePanel;
    [SerializeField] TMP_Text scoreMiddleText;
    [SerializeField] TMP_Text scoreToAddText;
    [SerializeField] CanvasGroup showScoreCG;
    public GameObject endOfGamePanel;

    private void Start() //was Awake
    {
        //health = Singleton.Instance.currentHealth;
        textHealth.text = ":" + health.ToString();

        //miliseconds = Singleton.Instance.runTimer[0];
        //seconds = Singleton.Instance.runTimer[1];
        //minutes = Singleton.Instance.runTimer[2];

        UpdateMoney(gm.currentMoney);
    }

    void Update()
    {
        CalculatingTimer();

        if (Input.GetKeyDown(KeyCode.T))
        {
            PauseButton();
        }
    }

    private void OnApplicationPause(bool pause)
    {
#if !UNITY_EDITOR
        if (!gm.gameEnded)
        {
            print("App is Paused");
            gm.gameIsPaused = false;
            PauseButton();
        }            
#endif
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        if (gm.gameEnded)
        {
            if (gm.scoreBonusesWereAdded)
            {
                MenuDataManager.Instance.currentMoney += gm.currentMoney;
                MenuDataManager.Instance.Save();

                Singleton.Instance.ResetVariables();
                Singleton.Instance.Save((gm.isClassicMode) ? 1 : 0);
            }
            else
            {
                gm.score += gm.HealthBonusScore() + gm.GoldCardBonusScore() + gm.ActivatedCardBonusScore() + gm.BubblesPickedBonusScore() + gm.TimerBonusScore() + gm.CompletedRunBonusScore();

                if (gm.isClassicMode)
                {
                    if (gm.score > MenuDataManager.Instance.highestClassicScore) MenuDataManager.Instance.highestClassicScore = gm.score;
                }
                else
                {
                    if (gm.score > MenuDataManager.Instance.highestClimbScore) MenuDataManager.Instance.highestClimbScore = gm.score;
                    CloudOnceServices.instance.SubmitScoreToClimbLeaderboard(gm.score);
                }

                MenuDataManager.Instance.currentMoney += gm.currentMoney;
                MenuDataManager.Instance.Save();

                Singleton.Instance.ResetVariables();
                Singleton.Instance.Save((gm.isClassicMode) ? 1 : 0);
            }
        }
        else
        {
            gm.SaveSingletonVariables();
        }
#endif
    }

    public void UpdateHealth(int amountToChange, bool isAdding)
    {
        if (isAdding)
            health += amountToChange;
        else
            health -= amountToChange;

        textHealth.text = ":" + health.ToString();

        if(health < 0)
        {
            textHealth.text = "--";
        }

        AchievementManager.instance.VerifyAchievementProgress(22, "THE_FIVE_OF_HEARTS", health);
    }

    public void UpdateFloor(int currentLevel)
    {
        currentFloor_Text.text = "Floor: " + (currentLevel + 1).ToString();
    }

    public void UpdateMoney(int currentMoney)
    {
        currentMoney_Text.text = "<sprite index=0>0" + currentMoney.ToString();
        currentMoneyAnimator.SetTrigger("scaleUp");
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public void SetCurrentHealth(int currentHealth)
    {
        health = currentHealth;
    }

    public float[] GetCurrentTimer()
    {
        float[] timer = new float[3];

        timer[0] = miliseconds;
        timer[1] = seconds;
        timer[2] = minutes;

        return timer;
    }

    public void SetCurrentTimer(float[] timer)
    {
        miliseconds = timer[0];
        seconds = timer[1];
        minutes = timer[2];
    }

	void CalculatingTimer()
	{
		if(!gm.gameIsPaused && !gm.checkForDestroyedBlocks && !gm.isBuildingLevel && gm.ballIsAvailable && gm.gameStarted) miliseconds += Time.deltaTime * 100;

		if (miliseconds >= 100)
		{
			seconds++;
			miliseconds = 0;
		}

		if (seconds >= 60)
		{
			minutes++;
			seconds = 0;
		}
		//Debug.Log(miliseconds);
		textTimer.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + ((int)miliseconds).ToString("00");
	}

    //Buttons Functionality Methods
    public void PauseButton()
    {
        if (gm.gameIsPaused)
        {
            pausePanel.SetActive(false);
            SetTimeScale(true);
            pause_Button.sprite = unpause_Sprite;
            if (!gm.IsGamePausedOutsidePauseMenu()) gm.SetLaunchBallTimer();
        }
        else
        {
            pausePanel.SetActive(true);
            SetTimeScale(false);
            pause_Button.sprite = pause_Sprite;
            gm.canLaunchBall = false;
        }
    }

    public void ShowPauseButton(bool show)
    {
        pause_Button.gameObject.SetActive(show);
    }

    public void GoBackToMenu()
    {
        SetTimeScale(true);
        SceneManager.LoadScene(0);
    }

    public void SetTimeScale(bool active)
    {        
        if (active)
        {            
            gm.gameIsPaused = false;
            if (gm.isChoosingPowerUp) return;
            Time.timeScale = 1;
        }
        else
        {
            gm.gameIsPaused = true;
            if (gm.isChoosingPowerUp) return;
            Time.timeScale = 0;
        }
    }

    public void CancelButton()
    {
        confirmationPanel.SetActive(false);
    }

    public void GiveUpAcceptButton() //Change to the end of level animation when completed
    {
        Singleton.Instance.ResetVariables();
        Singleton.Instance.Save((gm.isClassicMode) ? 1 : 0);

        GoBackToMenu();
    }

    public void SaveAndQuitAcceptButton()
    {
        gm.SaveSingletonVariables();
        //Singleton.Instance.Save((gm.isClassicMode) ? 1 : 0);

        GoBackToMenu();
    }

    public void GiveUpConfirmationPanel()
    {
        panelDescription.text = "Do you want to give up the current run and go back to the main menu?";

        confirmationPanel.SetActive(true);

        AcceptButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.AddListener(delegate { GiveUpAcceptButton(); });
    }

    public void SaveAndQuitConfirmationPanel()
    {
        panelDescription.text = "Save and go back to the main menu?";

        confirmationPanel.SetActive(true);

        AcceptButton.onClick.RemoveAllListeners();
        AcceptButton.onClick.AddListener(delegate { SaveAndQuitAcceptButton(); });
    }

    public void UpdatePauseScore()
    {
        if (gm.score < 1000000)
        {
            scoreText.text = "0" + gm.score.ToString();
        }
        else scoreText.text = gm.score.ToString();
    }

    public void SetShowScoreVariablesForAnimation()
    {
        scoreToAddText.text = "+0" + gm.scoreToAddAnimation.ToString();
        scoreMiddleText.text = "0"+ gm.middleShowScoreAnimation.ToString();        
    }

    public void ActivateScoreShowPanel(bool activate)
    {
        showScorePanel.SetActive(activate);
    }

    public void ChangeShowScoreCGAlpha(float t)
    {
        showScoreCG.alpha = t;
    }

    public void SettingsExpandable()
    {
        if (settingsExpandableAnimator.GetBool("expanded"))
        {
            settingsExpandableAnimator.SetBool("expanded", false);
            settingsExpandableAnimator.SetBool("collapsed", true);
        }
        else if (settingsExpandableAnimator.GetBool("collapsed"))
        {
            settingsExpandableAnimator.SetBool("collapsed", false);
            settingsExpandableAnimator.SetBool("expanded", true);
        }
    }

    public void MuteMusic(GameObject redLine)
    {
        if (redLine.activeInHierarchy)
        {
            redLine.SetActive(false);
        }
        else
        {
            redLine.SetActive(true);
        }
    }

    public void MuteSoundFX(GameObject redLine)
    {
        if (redLine.activeInHierarchy)
        {
            redLine.SetActive(false);
        }
        else
        {
            redLine.SetActive(true);
        }
    }

    public void ChangeScoreHeaderTextColor(Color color)
    {
        scoreHeaderText.color = color;
    }
}