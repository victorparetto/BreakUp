using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class EndOfRunScoreAnimation : MonoBehaviour
{
    GameManager gm;
    [SerializeField] UnityMonetization um;

    enum EndOfGame
    {
        START_ANIMATION,
        EXTRA_ANIM_IF_DEAD,
        MOVE_SCORE_UP,
        HEARTS_BONUS_ALPHA,
        HEARTS_BONUS_SCORE,
        GOLD_CARDS_BONUS_ALPHA,
        GOLD_CARDS_BONUS_SCORE,
        ACTIVATED_CARDS_BONUS_ALPHA,
        ACTIVATED_CARDS_BONUS_SCORE,
        BUBBLES_GRABBED_BONUS_ALPHA,
        BUBBLES_GRABBED_BONUS_SCORE,
        TIMER_BONUS_ALPHA,
        TIMER_BONUS_SCORE,
        RUN_COMPLETED_BONUS_ALPHA,
        RUN_COMPLETED_BONUS_SCORE,
        MONEY_EARNED_ALPHA,
        MONEY_EARNED_SHOW,
        SHOW_AD_PANEL,
        ADD_BONUS_COINS_TO_TOTAL,
        MOVE_SCORE_MIDDLE,
        TRANSITION_TO_END_OF_ANIMATION,
        LAST_ANIMATION_STATE
    }

    [SerializeField] EndOfGame state;

    public bool readyToRunAnimation = false;
    public bool transitionIsAlpha = true;

    [SerializeField] float animationGeneralSpeed = 1.5f;

    [Header("UI Text Variables")]
    public TMP_Text finalScoreText = null;
    [SerializeField] TMP_Text finalScoreHeaderText = null;

    [SerializeField] TMP_Text remainingHearts = null;
    [SerializeField] TMP_Text heartBonusScore = null;

    [SerializeField] TMP_Text goldCardsObtained = null;
    [SerializeField] TMP_Text goldCardsBonusScore = null;

    [SerializeField] TMP_Text cardsActivated = null;
    [SerializeField] TMP_Text cardsActivatedBonusScore = null;

    [SerializeField] TMP_Text bubblesGrabbed = null;
    [SerializeField] TMP_Text bubblesBonusScore = null;

    [SerializeField] TMP_Text timerBonusScore = null;

    [SerializeField] TMP_Text runCompletedBonusScore = null;

    [SerializeField] TMP_Text totalCoinsEarnedText = null;
    [SerializeField] TMP_Text currentCoinsText = null;
    [SerializeField] TMP_Text addedBonusCoinsText = null;
    [SerializeField] TMP_Text adCoinBonusMultiplierText = null;

    [Header("Animation Variables")]
    [SerializeField] RectTransform finalScoreRT;
    [SerializeField] CanvasGroup finalScoreCG;
    [SerializeField] RectTransform finalScoreHeaderRT;
    [SerializeField] RectTransform coinsRT;
    [SerializeField] RectTransform gameOverRT;
    [SerializeField] RectTransform completedRunGratzRT;
    [SerializeField] RectTransform MenuButtonRT;
    [SerializeField] GameObject bonusesPanelGO;
    [SerializeField] CanvasGroup[] bonusesCGs;
    [SerializeField] GameObject ownedCardsUI;
    [SerializeField] GameObject infoPanelUI;

    [Header("Ad Related Variables")]
    [SerializeField] GameObject adPanelGO;
    [SerializeField] GameObject noInternetTextGO;
    [SerializeField] GameObject watchButton;
    [SerializeField] GameObject ignoreButton;

    //Work Variables
    Vector2 scoreStartPos;
    Vector2 coinsStartPos;
    Vector2 scoreUpPos = new Vector2(0, 750);
    Vector2 scoreMiddlePos = new Vector2(0, 150);
    Vector2 coinsMiddlePos;
    RectTransform bonusRT;
    Vector2 tempBonusPosition;
    float startFontSize;
    float newScoreFontSize = 300;
    float t = 0;
    float currentAlpha = 0;
    int currentCG = 0;
    int bonusScore = 0;
    int tempBonusScore = 0;
    int finalScore = 0;
    int finalCoins = 0;
    int tempFinalScore = 0;
    int newFinalScore = 0;

    bool has0ScoreOnNextBonus = false;
    bool watchedAd = false;
    bool ignoredAd = false;

    void Awake()
    {
        gm = GetComponent<GameManager>();

        state = EndOfGame.START_ANIMATION;
    }

    void Update()
    {
        if (readyToRunAnimation)
        {
            switch (state)
            {
                case EndOfGame.START_ANIMATION:
                    {
                        scoreStartPos = finalScoreRT.anchoredPosition;
                        startFontSize = finalScoreText.fontSize;

                        coinsStartPos = coinsRT.anchoredPosition;
                        coinsMiddlePos = new Vector2(coinsRT.anchoredPosition.x, -150);

                        //Set the Final Score without bonuses (Only 1 time)
                        finalScore = gm.scoreBeforeEndBonuses;
                        UpdateScoreText(finalScoreText, finalScore);

                        if (gm.GetCurrentHealth() >= 1)
                            UpdateTotalNumbersText(remainingHearts, gm.GetCurrentHealth());
                        else
                            UpdateTotalNumbersText(remainingHearts, 0);
                        UpdateTotalNumbersText(goldCardsObtained, gm.totalGoldenCardsObtained);
                        UpdateTotalNumbersText(cardsActivated, gm.totalCardsActivated);
                        UpdateTotalNumbersText(bubblesGrabbed, gm.totalBubblesPicked);

                        if (gm.isDead)
                        {
                            state = EndOfGame.EXTRA_ANIM_IF_DEAD;
                            finalScoreCG.alpha = 0;
                        }
                        else
                            state = EndOfGame.MOVE_SCORE_UP;

                        break;
                    }

                case EndOfGame.EXTRA_ANIM_IF_DEAD:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 0.5f);

                        finalScoreCG.alpha = Mathf.Lerp(0, 1, t);

                        if (t >= 1)
                        {
                            t = 0;
                            state = EndOfGame.MOVE_SCORE_UP;
                        }

                        break;
                    }

                case EndOfGame.MOVE_SCORE_UP:
                    {
                        t += Time.deltaTime * animationGeneralSpeed;
                        finalScoreRT.anchoredPosition = Vector2.Lerp(scoreStartPos, scoreUpPos, t);
                        //finalScoreText.fontSize = Mathf.Lerp(startFontSize, newScoreFontSize, t);

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG = 0;
                            
                            //Setting Hearts Score Bonuses
                            bonusScore = gm.HealthBonusScore();
                            UpdateScoreText(heartBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.HEARTS_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.HEARTS_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;                           

                            state = EndOfGame.HEARTS_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.HEARTS_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(heartBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if(t >= 1)
                        {
                            t = 0;

                            //Setting Gold Cards Score Bonuses
                            bonusScore = gm.GoldCardBonusScore();
                            UpdateScoreText(goldCardsBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(-1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.GOLD_CARDS_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.GOLD_CARDS_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.GOLD_CARDS_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.GOLD_CARDS_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(goldCardsBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if (t >= 1)
                        {
                            t = 0;

                            //Setting Activated Cards Score Bonuses
                            bonusScore = gm.ActivatedCardBonusScore();
                            UpdateScoreText(cardsActivatedBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.ACTIVATED_CARDS_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.ACTIVATED_CARDS_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.ACTIVATED_CARDS_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.ACTIVATED_CARDS_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(cardsActivatedBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if (t >= 1)
                        {
                            t = 0;

                            //Setting Bubbles Picked Score Bonuses
                            bonusScore = gm.BubblesPickedBonusScore();
                            UpdateScoreText(bubblesBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(-1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.BUBBLES_GRABBED_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.BUBBLES_GRABBED_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.BUBBLES_GRABBED_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.BUBBLES_GRABBED_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(bubblesBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if (t >= 1)
                        {
                            t = 0;

                            //Setting Timer Score Bonuses
                            bonusScore = gm.TimerBonusScore();
                            UpdateScoreText(timerBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.TIMER_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.TIMER_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.TIMER_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.TIMER_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(timerBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if (t >= 1)
                        {
                            t = 0;

                            //Setting Run Completed Score Bonuses
                            bonusScore = gm.CompletedRunBonusScore();
                            UpdateScoreText(runCompletedBonusScore, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            tempFinalScore = finalScore;
                            newFinalScore = finalScore + bonusScore;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(-1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.RUN_COMPLETED_BONUS_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.RUN_COMPLETED_BONUS_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.RUN_COMPLETED_BONUS_SCORE;
                        }

                        break;
                    }

                case EndOfGame.RUN_COMPLETED_BONUS_SCORE:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(tempFinalScore, newFinalScore, t);

                        UpdateScoreText(runCompletedBonusScore, bonusScore);
                        UpdateScoreText(finalScoreText, finalScore);

                        if (t >= 1)
                        {
                            t = 0;

                            //Setting Gold Cards Score Bonuses
                            bonusScore = gm.currentMoney;
                            UpdateScoreText(currentCoinsText, bonusScore);
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            finalCoins = gm.currentMoney;
                            tempFinalScore = finalCoins;

                            if (!transitionIsAlpha)
                            {
                                bonusRT = bonusesCGs[currentCG].GetComponent<RectTransform>();
                                tempBonusPosition = new Vector2(1080, bonusRT.anchoredPosition.y);
                                bonusRT.anchoredPosition = tempBonusPosition;
                                bonusesCGs[currentCG].alpha = 1;
                            }

                            state = EndOfGame.MONEY_EARNED_ALPHA;
                        }

                        break;
                    }

                case EndOfGame.MONEY_EARNED_ALPHA:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1f);

                        if (transitionIsAlpha)
                        {
                            bonusesCGs[currentCG].alpha = Mathf.Lerp(currentAlpha, 1, t);
                        }
                        else
                        {
                            bonusRT.anchoredPosition = Vector2.Lerp(tempBonusPosition, new Vector2(0, bonusRT.anchoredPosition.y), t);
                        }

                        if (t >= 1)
                        {
                            t = 0;
                            currentAlpha = 0;
                            currentCG++;

                            state = EndOfGame.MONEY_EARNED_SHOW;
                        }

                        break;
                    }

                case EndOfGame.MONEY_EARNED_SHOW:
                    {
                        if (has0ScoreOnNextBonus)
                        {
                            t = 1;
                            //has0ScoreOnNextBonus = false;
                        }

                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(0, tempFinalScore, t);

                        UpdateCoinsText(currentCoinsText, bonusScore, false);
                        UpdateCoinsText(totalCoinsEarnedText, finalScore, true);

                        if (t >= 1)
                        {
                            t = 0;
                            bonusesPanelGO.SetActive(false);
                            if (um.AdvertisementIsReady())
                            {
                                if (has0ScoreOnNextBonus)
                                {
                                    infoPanelUI.SetActive(false);
                                    ownedCardsUI.SetActive(false);

                                    state = EndOfGame.MOVE_SCORE_MIDDLE;
                                    has0ScoreOnNextBonus = false;

                                    break;
                                }

                                adPanelGO.SetActive(true);
                                adCoinBonusMultiplierText.text = "x" + gm.adBonusMultiplier.ToString();
                                infoPanelUI.SetActive(false);
                                ownedCardsUI.SetActive(false);
                                state = EndOfGame.SHOW_AD_PANEL;                                
                            }
                            else
                            {
                                has0ScoreOnNextBonus = false;
                                noInternetTextGO.SetActive(true);
                                infoPanelUI.SetActive(false);
                                ownedCardsUI.SetActive(false);
                                state = EndOfGame.MOVE_SCORE_MIDDLE;
                            }
                        }

                        break;
                    }

                case EndOfGame.SHOW_AD_PANEL:
                    {
                        if (watchedAd)
                        {
                            //Setting Gold Cards Score Bonuses                            
                            SetBonusCoinsText(addedBonusCoinsText, bonusScore); //Bonus score is set on WatchAdButton function
                            tempBonusScore = bonusScore;

                            if (bonusScore == 0) has0ScoreOnNextBonus = true;

                            //Setting the new Final Score after this BONUS
                            finalCoins = finalScore;
                            tempFinalScore = gm.currentMoney;
                            if (finalCoins == tempFinalScore) print("This is weird, the values are the same");

                            addedBonusCoinsText.gameObject.SetActive(true);
                            state = EndOfGame.ADD_BONUS_COINS_TO_TOTAL;
                        }
                        else if (ignoredAd)
                        {
                            adPanelGO.SetActive(false);
                            state = EndOfGame.MOVE_SCORE_MIDDLE;
                        }

                        break;
                    }

                case EndOfGame.ADD_BONUS_COINS_TO_TOTAL:
                    {
                        t += Time.deltaTime * 0.5f;
                        bonusScore = (int)Mathf.Lerp(tempBonusScore, 0, t);
                        finalScore = (int)Mathf.Lerp(finalCoins, tempFinalScore, t);

                        SetBonusCoinsText(addedBonusCoinsText, bonusScore);
                        UpdateCoinsText(totalCoinsEarnedText, finalScore, true);

                        if(t >= 1)
                        {
                            t = 0;

                            adPanelGO.SetActive(false);
                            addedBonusCoinsText.gameObject.SetActive(false);                            
                            state = EndOfGame.MOVE_SCORE_MIDDLE;
                        }

                        break;
                    }

                case EndOfGame.MOVE_SCORE_MIDDLE:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 0.5f);

                        finalScoreRT.anchoredPosition = Vector2.Lerp(scoreUpPos, scoreMiddlePos, t);
                        finalScoreText.fontSize = Mathf.Lerp(startFontSize, newScoreFontSize, t);
                        coinsRT.anchoredPosition = Vector2.Lerp(coinsStartPos, coinsMiddlePos, t);

                        if(t >= 1)
                        {
                            t = 0;

                            MenuButtonRT.gameObject.SetActive(true);

                            if (gm.isDead)
                                gameOverRT.gameObject.SetActive(true);
                            else
                                completedRunGratzRT.gameObject.SetActive(true);

                            finalScoreHeaderText.text = gm.NewBestScore();

                            //Save what needs to be saved
                            //gm.EndOfGameDataManager();

                            state = EndOfGame.TRANSITION_TO_END_OF_ANIMATION;
                        }

                        break;
                    }

                case EndOfGame.TRANSITION_TO_END_OF_ANIMATION:
                    {
                        t += Time.deltaTime * (animationGeneralSpeed + 1.5f);

                        if (gm.isDead)
                            gameOverRT.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                        else
                            completedRunGratzRT.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

                        MenuButtonRT.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                        finalScoreHeaderRT.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

                        if (t >= 1)
                        {
                            t = 0;

                            state = EndOfGame.LAST_ANIMATION_STATE;
                        }

                        break;
                    }

                case EndOfGame.LAST_ANIMATION_STATE:
                    {

                        break;
                    }
            }

            if(Input.touchCount > 0)
            {
                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject()) return;

                    t = 1;
                }
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                t = 1;
            }
#endif 
        }
    }

    public void UpdateScoreText(TMP_Text textToChange, int scoreNum)
    {
        if (scoreNum < 1000000)
            textToChange.text = "0" + scoreNum.ToString();
        else
            textToChange.text = scoreNum.ToString();
    }

    public void UpdateTotalNumbersText(TMP_Text textToChange, int totalNum)
    {
        textToChange.text = "(" + totalNum.ToString() + "):";
    }

    public void UpdateCoinsText(TMP_Text textToChange, int coinsAmount, bool isEndShow)
    {
        if (isEndShow)
        {
            textToChange.text = "<sprite index=0>" + coinsAmount.ToString();
        }
        else
        {
            textToChange.text = "<sprite index=0>0" + coinsAmount.ToString();
        }
    }

    public void SetBonusCoinsText(TMP_Text textToChange, int coinsAmount)
    {
        textToChange.text = "+0" + coinsAmount.ToString();        
    }

    public void WatchAdButton()
    {
        Time.timeScale = 0;
        um.ShowRewardedVideo();
        watchedAd = true;
        watchButton.SetActive(false);
        ignoreButton.SetActive(false);
        bonusScore = (int)(gm.adBonusMultiplier * gm.currentMoney) - gm.currentMoney;
        gm.currentMoney = (int)(gm.currentMoney * gm.adBonusMultiplier );

        MenuDataManager.Instance.numberOfAdsWatched++;
        AchievementManager.instance.VerifyAchievementProgress(20, "HELPFUL_ADS", MenuDataManager.Instance.numberOfAdsWatched);
        //print("current money times the bonus is: " + gm.currentMoney);
    }

    public void IgnoreAdButton()
    {
        ignoredAd = true;
        watchButton.SetActive(false);
        ignoreButton.SetActive(false);
    }
}
