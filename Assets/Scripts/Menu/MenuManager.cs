using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI Controls")]
    [SerializeField] float transitionDuration = 1f;

    [Header("UI RectTransforms")]
    [SerializeField] RectTransform rtMainPanels = null;
    [SerializeField] RectTransform rtTopBarMoveable = null;
    [SerializeField] Vector2[] mainPanelsPositionsToGo = null;

    [SerializeField] GameObject skinSwapParent = null;
    [SerializeField] GameObject[] skinSwapPanels = null;

    [SerializeField] RectTransform rtGamePanel = null;
    [SerializeField] List<RectTransform> rtTopBarCurrentPanels = new List<RectTransform>();

    [SerializeField] RectTransform rtSkinSwapPanel = null;
    [SerializeField] Vector2[] skinSwapPositionsToGo = null;

    [SerializeField] RectTransform rtTabGroup = null;
    Vector2 rtTabGroupPositionToGo = new Vector2(0, 144);

    [SerializeField] TMP_Text randomizedText;
    [SerializeField] RainbowText rainbowTextComponent;

    [Header("MenuButtons")]
    [SerializeField] TabButton[] tabButtons;
    [SerializeField] GameObject leaderboardButton;

    //Skin Variables
    int currentSkinPanelIndex = 0;
    bool isOnCustomizePanel = false;

    [Header("Text Variables")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text climbHighScore;
    [SerializeField] TMP_Text classicHighScore;

    //Paddle Related Variables
    [Header("Paddle Skins Variables")]

    PaddleSkinCollapsableBehaviour currentPaddleSkinBehav = null;
    PaddleSkinCollapsableBehaviour openPaddleBehav = null;

    //Ball Related Variables
    [Header("Ball Skins Variables")]
    BallSkinCollapsableBehaviour currentBallSkinBehav = null;
    BallSkinCollapsableBehaviour openBallBehav = null;

    //Trail Related Variables
    [Header("Trail Skins Variables")]
    TrailSkinCollapsableBehaviour currentTrailSkinBehav = null;
    TrailSkinCollapsableBehaviour openTrailBehav = null;

    [Header("Canvas groups")]
    [SerializeField] CanvasGroup[] mainPanelsCanvasGroups = null;
    [SerializeField] CanvasGroup[] skinCanvasGroups = null;

    [Header("Background Variables")]
    [SerializeField] Material bgMaterial = null;

    //Achievements Variables
    [System.Serializable]
    public class AchievementShown
    {
        public int achNumber = 0;
        public Transform achBarTransform;
        public Image achBarImage;
        public Image trophyImage;
        public Image chestImage;
    }

    [SerializeField] AchievementShown[] achsShown;

    [Header("Achievements Variables")]
    [SerializeField] Sprite unlockedBarSprite;
    [SerializeField] Sprite unlockedTrophySprite;
    [SerializeField] Sprite glowingRewardChest = null;
    [SerializeField] GameObject achievementTabParticles;
    [SerializeField] TMP_Text achievementsCompleted;
    [SerializeField] int achievementsCurrent;
    [SerializeField] int achievementsTotal;

    [Header("Reward Chest Animation")]
    [SerializeField] GameObject rewardChestParent = null;
    [SerializeField] GameObject rewardAnimationPanel = null;
    public int selectedAchievementMoneyReward = 0;

    //GameState Saves Variables
    [Header("Game State Variables")]
    [SerializeField] GameObject[] SavedDataInfoPanels = null;
    [SerializeField] GameObject[] NewGameTexts = null;

    [SerializeField] TMP_Text[] floor = new TMP_Text[2];
    [SerializeField] TMP_Text[] health = new TMP_Text[2];
    [SerializeField] TMP_Text[] money = new TMP_Text[2];

    //Leaderboard Variables
    [Header("Leaderboard Variables")]
    public bool isLoggedIn = false;
    [SerializeField] TMP_Text signedInText;
    Animator leadButtonAnim;
    //float showTimer = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;

        MenuDataManager.Instance.Load();

        CreateBuyLists();

        if (!MenuDataManager.Instance.firstTimeEnteringGameIsDone)
        {
            Singleton.Instance.CreateFilePath(0);
            Singleton.Instance.CreateFilePath(1);
            
            MenuDataManager.Instance.firstTimeEnteringGameIsDone = true;
            MenuDataManager.Instance.Save();
        }

        Singleton.Instance.Load(1);
        ShowSingletonSaveStateVariables(1);

        Singleton.Instance.Load(0);
        ShowSingletonSaveStateVariables(0);
        
        leadButtonAnim = leaderboardButton.GetComponent<Animator>();
    }

    private void Start()
    {
        SetMenuLightColor(2);
        Singleton.Instance.ResetTempVariables();

        StartCoroutine(ScaleUISmoothly(rtGamePanel, true));        
        CheckCurrentAchievements();
        skinSwapParent.SetActive(true);

        UpdateMoneyTopBar();
        UpdateHighestScoreTexts();
        DeactivateAllSkinSwapPanelsButCurrent(false);
    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject != leaderboardButton)
        {
            leadButtonAnim.SetBool("showing", false);
        }
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void ClimbMode(int buildIndex)
    {
        Singleton.Instance.Load(0);
        if (Singleton.Instance.hasSavedData) Singleton.Instance.saveWasLoaded_Temp = true;
        LoadScene(buildIndex);
    }

    public void ClassicMode(int buildIndex)
    {
        Singleton.Instance.Load(1);
        if (Singleton.Instance.hasSavedData) Singleton.Instance.saveWasLoaded_Temp = true;
        Singleton.Instance.isClassicMode_Temp = true;
        LoadScene(buildIndex);
    }

    public void GameTab()
    {
        SetMenuLightColor(2);
        isOnCustomizePanel = false;
        DeactivateAllSkinSwapPanelsButCurrent(true);
        StartCoroutine(MoveUISmoothly(rtMainPanels, mainPanelsPositionsToGo[0]));
        StartCoroutine(MoveUISmoothly(rtTopBarMoveable, mainPanelsPositionsToGo[0]));

        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[1]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[2]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[3]));
    }

    public void CustomizeTab()
    {
        SetMenuLightColor(5);
        isOnCustomizePanel = true;
        ActivateCurrentSkinPanel();
        StartCoroutine(MoveUISmoothly(rtMainPanels, mainPanelsPositionsToGo[1]));
        StartCoroutine(MoveUISmoothly(rtTopBarMoveable, mainPanelsPositionsToGo[1]));

        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[0]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[2]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[3]));
    }

    public void AchievementsTab()
    {
        SetMenuLightColor(6);
        isOnCustomizePanel = false;
        DeactivateAllSkinSwapPanelsButCurrent(true);
        StartCoroutine(MoveUISmoothly(rtMainPanels, mainPanelsPositionsToGo[2]));
        StartCoroutine(MoveUISmoothly(rtTopBarMoveable, mainPanelsPositionsToGo[2]));

        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[0]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[1]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[3]));
    }

    public void SettingsTab()
    {
        SetMenuLightColor(1);
        isOnCustomizePanel = false;
        DeactivateAllSkinSwapPanelsButCurrent(true);
        StartCoroutine(MoveUISmoothly(rtMainPanels, mainPanelsPositionsToGo[3]));
        StartCoroutine(MoveUISmoothly(rtTopBarMoveable, mainPanelsPositionsToGo[3]));

        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[0]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[1]));
        StartCoroutine(SetAlphaTo0Smoothly(mainPanelsCanvasGroups[2]));
    }

    public void PaddleTab()
    {
        StartCoroutine(MoveUISmoothly(rtSkinSwapPanel, skinSwapPositionsToGo[0]));

        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[1]));
        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[2]));

        currentSkinPanelIndex = 0;
    }

    public void BallTab()
    {
        StartCoroutine(MoveUISmoothly(rtSkinSwapPanel, skinSwapPositionsToGo[1]));

        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[0]));
        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[2]));

        currentSkinPanelIndex = 1;
    }

    public void TrailTab()
    {
        StartCoroutine(MoveUISmoothly(rtSkinSwapPanel, skinSwapPositionsToGo[2]));

        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[0]));
        StartCoroutine(SetAlphaTo0Smoothly(skinCanvasGroups[1]));

        currentSkinPanelIndex = 2;
    }

    public void ChangeCurrentEquippedPaddleBehaviour(PaddleSkinCollapsableBehaviour behav)
    {
        if(currentPaddleSkinBehav != null) currentPaddleSkinBehav.UnequipPaddle();
        currentPaddleSkinBehav = behav;
    }

    public bool IsTheCurrentPaddleBehaviour(PaddleSkinCollapsableBehaviour behav)
    {
        if (currentPaddleSkinBehav == behav) return true;

        return false;
    }

    public void ChangeOpenPaddleBehaviour(PaddleSkinCollapsableBehaviour behav)
    {
        if (openPaddleBehav == behav) return;

        if (openPaddleBehav == null) 
        {
            openPaddleBehav = behav;
        }
        else
        {
            if(openPaddleBehav.isExpanded) openPaddleBehav.ExpandOrCollapse();
            openPaddleBehav = behav;
        }
    }

    public void ChangeCurrentEquippedBallBehaviour(BallSkinCollapsableBehaviour behav)
    {
        if (currentBallSkinBehav != null) currentBallSkinBehav.UnequipBall();
        currentBallSkinBehav = behav;
    }

    public bool IsTheCurrentBallBehaviour(BallSkinCollapsableBehaviour behav)
    {
        if (currentBallSkinBehav == behav) return true;

        return false;
    }

    public void ChangeOpenBallBehaviour(BallSkinCollapsableBehaviour behav)
    {
        if (openBallBehav == behav) return;

        if (openBallBehav == null)
        {
            openBallBehav = behav;
        }
        else
        {
            if (openBallBehav.isExpanded) openBallBehav.ExpandOrCollapse();
            openBallBehav = behav;
        }
    }

    public void ChangeCurrentEquippedTrailBehaviour(TrailSkinCollapsableBehaviour behav)
    {
        if (currentTrailSkinBehav != null) currentTrailSkinBehav.UnequipTrail();
        currentTrailSkinBehav = behav;
    }

    public bool IsTheCurrentTrailBehaviour(TrailSkinCollapsableBehaviour behav)
    {
        if (currentTrailSkinBehav == behav) return true;

        return false;
    }

    public void ChangeOpenTrailBehaviour(TrailSkinCollapsableBehaviour behav)
    {
        if (openTrailBehav == behav) return;

        if (openTrailBehav == null)
        {
            openTrailBehav = behav;
        }
        else
        {
            if (openTrailBehav.isExpanded) openTrailBehav.ExpandOrCollapse();
            openTrailBehav = behav;
        }
    }

    public IEnumerator MoveUISmoothly(RectTransform rt, Vector3 target)
    {
        SetButtonInteractable(tabButtons, false);
        PreventRaycastWhileMoving(mainPanelsCanvasGroups, false);
        PreventRaycastWhileMoving(skinCanvasGroups, false);

        float currentTime = 0;
        //float duration = 0.5f;

        Vector3 origin = rt.anchoredPosition;

        while (currentTime <= transitionDuration)
        {
            currentTime += Time.deltaTime;
            float percent = Mathf.Clamp01(currentTime / transitionDuration);

            float smooth = percent * percent * (3f - 2f * percent);
            rt.anchoredPosition = Vector3.Lerp(origin, target, smooth);
            yield return null;
        }

        SetButtonInteractable(tabButtons, true);
        PreventRaycastWhileMoving(mainPanelsCanvasGroups, true);
        PreventRaycastWhileMoving(skinCanvasGroups, true);

        if (isOnCustomizePanel)
        {
            ActivateAllSkinSwapPanels();
        }
    }

    public IEnumerator ScaleUISmoothly(RectTransform rt, bool scaleUp)
    {
        SetButtonInteractable(tabButtons, false);
        PreventRaycastWhileMoving(mainPanelsCanvasGroups, false);
        PreventRaycastWhileMoving(skinCanvasGroups, false);

        float currentTime = 0;
        Vector3 origin;

        if (scaleUp) origin = Vector3.zero;
        else origin = Vector3.one;
        rt.localScale = origin;

        while (currentTime <= transitionDuration)
        {
            currentTime += Time.deltaTime;
            float percent = Mathf.Clamp01(currentTime / transitionDuration);

            float smooth = percent * percent * (3f - 2f * percent);
            rt.localScale = Vector3.Lerp(origin, scaleUp? Vector3.one : Vector3.zero, smooth);
            yield return null;
        }

        SetButtonInteractable(tabButtons, true);
        PreventRaycastWhileMoving(mainPanelsCanvasGroups, true);
        PreventRaycastWhileMoving(skinCanvasGroups, true);
    }

    IEnumerator SetAlphaTo0Smoothly(CanvasGroup cg)
    {        
        while (cg.alpha > 0)
        {
            cg.alpha -= Time.unscaledDeltaTime * 1.3f;
            yield return null;
        }

        cg.alpha = 1;
    }

    public void ForceLayoutRebuild(RectTransform rt)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }

    private void SetButtonInteractable(TabButton[] btn, bool interactable)
    {
        for (int i = 0; i < btn.Length; i++)
        {
            btn[i].enabled = interactable;
        }
    }

    private void PreventRaycastWhileMoving(CanvasGroup[] cgs, bool active)
    {
        for (int i = 0; i < cgs.Length; i++)
        {
            cgs[i].blocksRaycasts = active;
        }        
    }

    private void ActivateCurrentSkinPanel()
    {
        skinSwapPanels[currentSkinPanelIndex].SetActive(true);
    }

    private void ActivateAllSkinSwapPanels()
    {
        for (int i = 0; i < skinSwapPanels.Length; i++)
        {
            skinSwapPanels[i].SetActive(true);
        }
    }

    private void DeactivateAllSkinSwapPanelsButCurrent(bool leaveSelectedActive)
    {
        for (int i = 0; i < skinSwapPanels.Length; i++)
        {
            if(leaveSelectedActive)
                if (i == currentSkinPanelIndex) continue;

            skinSwapPanels[i].SetActive(false);
        }
    }

    public void SetMenuLightColor(int color)
    {
        Color tempColor = GetColorForGlowMaterial(color);
        bgMaterial.color = tempColor;
    }

    public Color GetColorForGlowMaterial(int color)
    {
        //int ran = Random.Range(0, 7);
        
        if (color == 0)
            return new Color(200f / 255, 11f / 255, 0f / 255); //red
        else if (color == 1)
            return new Color(0f / 255, 30f / 255, 130f / 255); //blue
        else if (color == 2)
            return new Color(100f / 255, 0f / 255, 205f / 255); //purple
        else if (color == 3)
            return new Color(0f / 255, 95f / 255, 50f / 255); //green
        else if (color == 4)
            return new Color(110f / 255, 100f / 255, 0f / 255); //yellow
        else if (color == 5)
            return new Color(0f / 255, 120f / 255, 150f / 255); //cyan
        else if (color == 6)
            return new Color(170f / 255, 70f / 255, 0f / 255); //orange

        return Color.black;
    }

    public void ActivateRandomGame()
    {
        if(Singleton.Instance.isRandomized_Temp)
        {
            Singleton.Instance.isRandomized_Temp = false;
            rainbowTextComponent.enabled = false;
            randomizedText.UpdateVertexData();
            
        }
        else
        {
            Singleton.Instance.isRandomized_Temp = true;
            rainbowTextComponent.enabled = true;
        }
    }

    public void CheckCurrentAchievements()
    {
        for (int i = 0; i < achsShown.Length; i++)
        {
            if (AchievementDataManager.Instance.achievementsUnlockState[achsShown[i].achNumber])
            {
                UnlockAchievementBar(i);
                achievementsCurrent++;

                if (!AchievementDataManager.Instance.achievementsClaimedState[achsShown[i].achNumber])
                {
                    achievementTabParticles.SetActive(true);
                }
            }
            if (AchievementDataManager.Instance.achievementsClaimedState[achsShown[i].achNumber])
            {
                IsClaimedAchievement(i);
            }
        }

        achievementsCompleted.text = ((achievementsCurrent * 100) / achievementsTotal).ToString() + "%";
    }

    public void UnlockAchievementBar(int i)
    {
        achsShown[i].achBarTransform.SetSiblingIndex(0);
        achsShown[i].achBarImage.sprite = unlockedBarSprite;
        achsShown[i].trophyImage.sprite = unlockedTrophySprite;
        achsShown[i].chestImage.sprite = glowingRewardChest;
        achsShown[i].achBarTransform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true); //this gets pulse particles
    }

    public void IsClaimedAchievement(int id)
    {
        achsShown[id].achBarTransform.GetChild(2).gameObject.SetActive(false); //GetChild(2) is suppoused to be "RightSide"
    }

    public void ClaimAchievementPrize(AchievementIDTracker aIDT) //in order of listing
    {
        if (AchievementManager.instance.achievements[aIDT.AchievementID].unlocked)
        {
            selectedAchievementMoneyReward = AchievementManager.instance.achievements[aIDT.AchievementID].prizeAmount;

            AchievementManager.instance.achievements[aIDT.AchievementID].claimed = true;
            AchievementDataManager.Instance.achievementsClaimedState[aIDT.AchievementID] = true;
            AchievementDataManager.Instance.Save();
            IsClaimedAchievement(aIDT.AchievementSiblingIndex);

            MenuDataManager.Instance.currentMoney += selectedAchievementMoneyReward;
            MenuDataManager.Instance.Save();

            UpdateMoneyTopBar();

            EventSystem currentEvent = EventSystem.current;
            GameObject selectedBtn = currentEvent.currentSelectedGameObject;

            print(selectedBtn.transform.position);
            rewardChestParent.transform.position = selectedBtn.transform.position;
            rewardAnimationPanel.SetActive(true);
            rewardChestParent.gameObject.SetActive(true);
        }
        else
        {
            print("Achievement hasnt been unlocked yet");
        }
    }

    public void ShowSingletonSaveStateVariables(int gameMode)
    {
        if (!Singleton.Instance.hasSavedData)
        {
            SavedDataInfoPanels[gameMode].SetActive(false);
            NewGameTexts[gameMode].SetActive(true);
            return;
        }

        floor[gameMode].text = "Floor: " + (Singleton.Instance.currentLevel < 9 ? "0" : "") + (Singleton.Instance.currentLevel + 1).ToString();
        health[gameMode].text = "<sprite index=0> " + (Singleton.Instance.currentHealth < 10 ? "0" : "") + Singleton.Instance.currentHealth.ToString();
        money[gameMode].text = "<sprite index=0>" + "0" + Singleton.Instance.runMoney.ToString();

        NewGameTexts[gameMode].SetActive(false);
        SavedDataInfoPanels[gameMode].SetActive(true);
    }    

    public void ClickOnLeaderboardButton()
    {
        if (isLoggedIn)
        {
            leadButtonAnim.SetBool("showing", true);
        }
        else
        {
            signedInText.gameObject.SetActive(true);
            CloudOnce.Cloud.SignIn();

            if (CloudOnce.Cloud.IsSignedIn)
            {
                isLoggedIn = true;
                signedInText.text = "logged in!";
                signedInText.color = Color.green;
                ClickOnLeaderboardButton();
            }
            else
            {
                isLoggedIn = false;
                signedInText.text = "logging failed";
                signedInText.color = Color.red;
            }
        }        
    }

    public void UpdateMoneyTopBar()
    {
        moneyText.text = (MenuDataManager.Instance.currentMoney < 100000) ? "0" + MenuDataManager.Instance.currentMoney.ToString() : MenuDataManager.Instance.currentMoney.ToString();
    }

    public void UpdateHighestScoreTexts()
    {
        if(MenuDataManager.Instance.highestClimbScore > 0) climbHighScore.text = MenuDataManager.Instance.highestClimbScore.ToString();
        if (MenuDataManager.Instance.highestClassicScore > 0) classicHighScore.text = MenuDataManager.Instance.highestClassicScore.ToString();
    }

    public void CreateBuyLists()
    {
        int temp = 50;

        if (MenuDataManager.Instance.boughtPaddles.Count == temp)
        {
            return;
        }
        else if (MenuDataManager.Instance.boughtPaddles.Count < temp)
        {
            for (int i = MenuDataManager.Instance.boughtPaddles.Count; i < temp; i++)
            {
                MenuDataManager.Instance.boughtPaddles.Add(false);
                MenuDataManager.Instance.boughtBalls.Add(false);
                MenuDataManager.Instance.boughtTrails.Add(false);
            }
        }
    }
}
