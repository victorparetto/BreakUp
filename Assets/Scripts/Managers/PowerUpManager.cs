using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PowerUpManager : MonoBehaviour
{
    GameManager gm;
    MovePaddle mp;
    [HideInInspector] public ChooseDirection cd;
    ShootPowerUp spu;
    UIControl uic;

    [SerializeField] GameObject blackBG = null;
    [SerializeField] ParticleSystem cardsParticles = null;
    [SerializeField] float chosenCardAnimSpeed = 1f;

    [Header("Arrays")]
    [SerializeField] Transform[] powerUpCards;
    [SerializeField] PowerUpIndex[] cardsChooseIndexScript;
    [SerializeField] Color[] cardsChooseColor;
    [SerializeField] Transform[] cardSlotTransforms;
    [SerializeField] Transform[] cardChooseSlotTransforms;
    [SerializeField] Transform[] randomCardsForOnlyRandom;
    [SerializeField] Transform[] randomGoldCardsForOnlyRandom;

    //Card Slots variables
    [Header("Card Slots")]
    public int[] cardSlotIndex = new int[3];
    public bool[] slotIndexIsGold = new bool[3];
    [SerializeField] LayerMask cardSlotsLayer;

    //PowerUps Variables
    [Header("PowerUps")]
    public List<GameObject> currentActiveBubbles = new List<GameObject>();
    public bool powerUpsAreRandom = false;
    public int phasingPowerUpMaxBlocks = 4;
    public int phasingPowerUpGoldMaxBlocks = 8;
    public int currentPaddleSize = 1;
    public bool chooseDirectionIsActive = false;
    public bool magnetIsActive = false;
    public bool blockCleanerIsActive = false;
    bool comingFromRandomPowerUp = false;
    public List<Transform> shotBulletsTransform = new List<Transform>();

    public GameObject bottomWall = null;
    [SerializeField] Sprite[] bubbleBlockPowerUpSprites;
    [SerializeField] SpriteRenderer bubbleBlockSR = null;
    [SerializeField] List<string> levelBlockColors = new List<string>();

    [SerializeField] GameObject dragPanel = null;
    [SerializeField] GameObject dragHand = null;

    //Touch Variables
    Touch touch;
    int lastTouchIndex;
    Vector3 touchPosWorld;

    //Initialize Temp Variables
    GameObject ballTempGo;
    List<GameObject> ballTempList = new List<GameObject>();
    List<PowerUpIndex> tempCardsChooseIndexScript = new List<PowerUpIndex>();
    List<Transform> allCardsTransform = new List<Transform>();
    public List<int> randomCardChooseIntIndexes = new List<int>();
    BoxCollider2D[] cardSlotColliders;
    BoxCollider2D[] chooseCardSlotColliders;

    int repeatedChosenIndex = 99;
    public bool loadTheSameChoice = false;
    public bool isChoiceAtLevelStart = false;

    //Achievements Variables
    List<int> everyBubblePowerUp = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    List<int> everyCardPowerUp = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int bubbleCounter = 0;
    int cardCounter = 0;
    int onlyRandomBubbleCounter = 0;
    int ballMaxSpeedCounter = 0;
    int paddleMinSizeCounter = 0;

    //debug variables

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        mp = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePaddle>();
        uic = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIControl>();
        cd = GetComponent<ChooseDirection>();

        SetCardSlotsColliders();
        SetCardChooseSlotsColliders();

        //for (int i = 0; i < 3; i++)
        //{
        //    cardSlotIndex[i] = Singleton.Instance.currentCards[i];
        //    slotIndexIsGold[i] = Singleton.Instance.currentCardsGolden[i];
        //    randomCardChooseIntIndexes.Add(Singleton.Instance.currentCardChoice[i]);
        //}

        chooseDirectionIsActive = Singleton.Instance.chooseDirectionPowerUpActive;
        cd.isShooting = Singleton.Instance.chooseDirectionPowerUpStillShooting;
        currentPaddleSize = Singleton.Instance.paddleSize;
    }

    void Start()
    {
        spu = gm.mp.spu;

        powerUpsAreRandom = Singleton.Instance.isRandomRun;

        print("singleton bool:" + Singleton.Instance.leftBeforeChoosing);
        if (Singleton.Instance.leftBeforeChoosing)
        {
            loadTheSameChoice = true;
            StartCoroutine(ShowCardChooseSelection());
        }
    }

    void Update()
    {
        if (!gm.gameIsPaused)
        {
            if (Input.touchCount > 0)
            {
                lastTouchIndex = Input.touchCount - 1;

                if (lastTouchIndex == 0)        //If added to prevent first touch to move and activate skill after being over it...
                {
                    if (Input.GetTouch(lastTouchIndex).phase == TouchPhase.Moved)
                    {
                        return;
                    }
                }

                if (Input.GetTouch(lastTouchIndex).phase == TouchPhase.Ended)
                {
                    touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(lastTouchIndex).position);
                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                    //touch = Input.GetTouch(lastTouchIndex);

                    //We now raycast with this information. If we have hit something we can process it.
                    RaycastHit2D hit = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward, Mathf.Infinity, cardSlotsLayer);

                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("CardSlot1"))
                        {
                            if (gm.isChoosingPowerUp)
                            {
                                GoToFreeSlotAndHide(hit);
                            }
                            else
                            {
                                ActivateCardPowerUp(cardSlotIndex[0], 0, hit.collider.transform.GetChild(0));
                            }
                        }
                        else if (hit.collider.CompareTag("CardSlot2"))
                        {
                            if (gm.isChoosingPowerUp)
                            {
                                GoToFreeSlotAndHide(hit);
                            }
                            else
                            {
                                ActivateCardPowerUp(cardSlotIndex[1], 1, hit.collider.transform.GetChild(0));
                            }
                        }
                        else if (hit.collider.CompareTag("CardSlot3"))
                        {
                            if (gm.isChoosingPowerUp)
                            {
                                GoToFreeSlotAndHide(hit);
                            }
                            else
                            {
                                ActivateCardPowerUp(cardSlotIndex[2], 2, hit.collider.transform.GetChild(0));
                            }
                        }
                        else
                        {
                            print("NO HIT");
                            return;
                        }
                    }
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (FreeCardSlotAvailable() || powerUpsAreRandom) StartCoroutine(ShowCardChooseSelection());
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                ActivateBubblePowerUp(0);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                ActivateBubblePowerUp(1);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                ActivateBubblePowerUp(2);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                ActivateBubblePowerUp(3);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                ActivateBubblePowerUp(4);
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                ActivateBubblePowerUp(9);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                ActivateBubblePowerUp(10);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                ActivateBubblePowerUp(11);
            }


            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(mousePos, Camera.main.transform.forward, Mathf.Infinity, cardSlotsLayer);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("CardSlot1"))
                    {
                        //print("Cardslot 1");
                        if (gm.isChoosingPowerUp)
                        {
                            GoToFreeSlotAndHide(hit);
                        }
                        else
                        {
                            ActivateCardPowerUp(cardSlotIndex[0], 0, hit.collider.transform.GetChild(0));
                        }
                    }
                    else if (hit.collider.CompareTag("CardSlot2"))
                    {
                        //print("Cardslot 2");
                        if (gm.isChoosingPowerUp)
                        {
                            GoToFreeSlotAndHide(hit);
                        }
                        else
                        {
                            ActivateCardPowerUp(cardSlotIndex[1], 1, hit.collider.transform.GetChild(0));
                        }
                    }
                    else if (hit.collider.CompareTag("CardSlot3"))
                    {
                        //print("Cardslot 3");
                        if (gm.isChoosingPowerUp)
                        {
                            GoToFreeSlotAndHide(hit);
                        }
                        else
                        {
                            ActivateCardPowerUp(cardSlotIndex[2], 2, hit.collider.transform.GetChild(0));
                        }
                    }
                    else
                    {
                        print("NO HIT");
                        return;
                    }
                }
            }
#endif
        }
    }

    public void GiveRandomCard()
    {
        if (isChoiceAtLevelStart) { Singleton.Instance.levelCardWasChosen = true; isChoiceAtLevelStart = false; }
        if (Singleton.Instance.leftBeforeChoosing) Singleton.Instance.leftBeforeChoosing = false;

        if (cardSlotIndex[0] != 9)
        {
            cardSlotIndex[0] = 9;
            randomCardsForOnlyRandom[0].position = cardChooseSlotTransforms[2].position;
            randomCardsForOnlyRandom[0].parent = cardSlotTransforms[0];
            SetCardChooseParticlesColor(cardSlotIndex[0], randomCardsForOnlyRandom[0].position);

            StartCoroutine(GiveRandomCard(randomCardsForOnlyRandom[0]));
        }
        else if (cardSlotIndex[1] != 9)
        {
            cardSlotIndex[1] = 9;
            randomCardsForOnlyRandom[1].position = cardChooseSlotTransforms[2].position;
            randomCardsForOnlyRandom[1].parent = cardSlotTransforms[1];
            SetCardChooseParticlesColor(cardSlotIndex[1], randomCardsForOnlyRandom[1].position);
            StartCoroutine(GiveRandomCard(randomCardsForOnlyRandom[1]));
        }
        else if (cardSlotIndex[2] != 9)
        {
            cardSlotIndex[2] = 9;
            randomCardsForOnlyRandom[2].position = cardChooseSlotTransforms[2].position;
            randomCardsForOnlyRandom[2].parent = cardSlotTransforms[2];
            SetCardChooseParticlesColor(cardSlotIndex[2], randomCardsForOnlyRandom[2].position);
            StartCoroutine(GiveRandomCard(randomCardsForOnlyRandom[2]));
        }
        else if (!slotIndexIsGold[0])
        {
            slotIndexIsGold[0] = true;
            randomGoldCardsForOnlyRandom[0].position = cardChooseSlotTransforms[2].position;
            randomGoldCardsForOnlyRandom[0].parent = cardSlotTransforms[0];
            gm.goldCardsChooseParticle.transform.position = randomGoldCardsForOnlyRandom[0].position;
            gm.goldCardsChooseParticle.SetActive(true);
            StartCoroutine(GiveRandomCard(randomGoldCardsForOnlyRandom[0]));
            HideCard(randomCardsForOnlyRandom[0]);
        }
        else if (!slotIndexIsGold[1])
        {
            slotIndexIsGold[1] = true;
            randomGoldCardsForOnlyRandom[1].position = cardChooseSlotTransforms[2].position;
            randomGoldCardsForOnlyRandom[1].parent = cardSlotTransforms[1];
            gm.goldCardsChooseParticle.transform.position = randomGoldCardsForOnlyRandom[1].position;
            gm.goldCardsChooseParticle.SetActive(true);
            StartCoroutine(GiveRandomCard(randomGoldCardsForOnlyRandom[1]));
            HideCard(randomCardsForOnlyRandom[1]);
        }
        else if (!slotIndexIsGold[2])
        {
            slotIndexIsGold[2] = true;
            randomGoldCardsForOnlyRandom[2].position = cardChooseSlotTransforms[2].position;
            randomGoldCardsForOnlyRandom[2].parent = cardSlotTransforms[2];
            gm.goldCardsChooseParticle.transform.position = randomGoldCardsForOnlyRandom[2].position;
            gm.goldCardsChooseParticle.SetActive(true);
            StartCoroutine(GiveRandomCard(randomGoldCardsForOnlyRandom[2]));
            HideCard(randomCardsForOnlyRandom[2]);
        }
        else
        {
            print("No Active Random Card"); //should never happen
            CheckIfCardSlotIsActive();
            return;
        }
    }

    public IEnumerator GiveRandomCard(Transform randomCardT)
    {
        //if (isChoiceAtLevelStart) { Singleton.Instance.levelCardWasChosen = true; isChoiceAtLevelStart = false; }
        //if (Singleton.Instance.leftBeforeChoosing) Singleton.Instance.leftBeforeChoosing = false;
        float t = 0;
        Vector3 target = randomCardT.transform.parent.position;
        Vector3 tempPos = randomCardT.position;

        randomCardT.gameObject.SetActive(true);

        while (t < 1)
        {
            randomCardT.position = Vector2.Lerp(tempPos, target, t);

            t += Time.deltaTime * chosenCardAnimSpeed;
            yield return null;
        }

        randomCardT.localPosition = Vector3.zero;
        CheckIfCardSlotIsActive();
    }

    public IEnumerator ShowCardChooseSelection()
    {
        if (gm.isChoosingPowerUp) yield break;

        if (powerUpsAreRandom)
        {
            GiveRandomCard();
            yield break;
        }

        gm.isChoosingPowerUp = true;
        gm.canLaunchBall = false;
        uic.ActivateScoreShowPanel(false);
        Time.timeScale = 0;
        //gm.gameIsPaused = true;
        blackBG.SetActive(true);
        if (!loadTheSameChoice) Get3RandomGeneratedCards();
        else
        {
            loadTheSameChoice = false;
        }
        CardSlotsAllCollidersEnabled(cardSlotColliders, false);
        CardSlotsAllCollidersEnabled(chooseCardSlotColliders, false);

        for (int i = 0; i < randomCardChooseIntIndexes.Count; i++)
        {
            cardsChooseIndexScript[randomCardChooseIntIndexes[i]].gameObject.SetActive(true);
            cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.parent = cardChooseSlotTransforms[i];
            cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.localPosition = Vector3.zero;
            cardsChooseIndexScript[randomCardChooseIntIndexes[i]].ActivateColorParticles(true);

            if (cardsChooseIndexScript[randomCardChooseIntIndexes[i]].goldPowerUpForSlot != null)
            {
                cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.GetChild(0).gameObject.SetActive(false);
                cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.GetChild(1).gameObject.SetActive(false);
            }

            for (int j = 0; j < cardSlotIndex.Length; j++)
            {
                if (cardSlotIndex[j] == randomCardChooseIntIndexes[i])
                {
                    if (cardsChooseIndexScript[randomCardChooseIntIndexes[i]].goldPowerUpForSlot != null)//if (cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.childCount >= 2)
                    {
                        cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.GetChild(0).gameObject.SetActive(true);
                        cardsChooseIndexScript[randomCardChooseIntIndexes[i]].transform.GetChild(1).gameObject.SetActive(true);
                        cardsChooseIndexScript[randomCardChooseIntIndexes[i]].ActivateColorParticles(false);
                    }
                }
                //yield return null;
            }

            //yield return null;
        }

        Vector3 tempScale = cardChooseSlotTransforms[0].localScale;

        for (int k = 0; k < cardChooseSlotTransforms.Length; k++)
        {
            cardChooseSlotTransforms[k].localScale = Vector3.zero;
            //yield return null;
        }

        float t = 0;
        CardChooseSetActiveAll(true);

        while (t < 1)
        {
            for (int l = 0; l < cardChooseSlotTransforms.Length; l++)
            {
                cardChooseSlotTransforms[l].localScale = Vector2.Lerp(Vector2.zero, tempScale, t);
            }

            t += Time.unscaledDeltaTime * 5f;
            yield return null;
        }

        for (int p = 0; p < cardChooseSlotTransforms.Length; p++)
        {
            cardChooseSlotTransforms[p].localScale = tempScale;
            //yield return null;
        }

        CardSlotsAllCollidersEnabled(chooseCardSlotColliders, true);
        //yield return null;
    }

    public void GoToFreeSlotAndHide(RaycastHit2D hit)
    {
        gm.isChoosingPowerUp = false;
        CheckIfCardSlotIsActive();
        if (isChoiceAtLevelStart) { Singleton.Instance.levelCardWasChosen = true; isChoiceAtLevelStart = false; }
        if (Singleton.Instance.leftBeforeChoosing) Singleton.Instance.leftBeforeChoosing = false;
        Time.timeScale = 1;
        if (gm.scoreAnimationIsPlaying)
        {
            uic.ActivateScoreShowPanel(true);
            uic.ActivateScoreAnimation();
        }

        //gm.gameIsPaused = false;
        PowerUpIndex tempPUI = hit.collider.GetComponentInChildren<PowerUpIndex>();
        tempPUI.ActivateColorParticles(false);

        if (!gm.IsGamePausedOutsidePauseMenu()) gm.SetLaunchBallTimer();

        if (GetSlotWithSameIndexIfExists(GetChosenCardIndex(tempPUI)) < 99)
        {
            powerUpCards[cardSlotIndex[repeatedChosenIndex]].parent = null;
            //powerUpCards[cardSlotIndex[repeatedChosenIndex]].localPosition = Vector3.zero;
            SetCardChooseParticlesColor(cardSlotIndex[repeatedChosenIndex], hit.collider.transform.GetChild(0).position);
            slotIndexIsGold[repeatedChosenIndex] = true;

            StartCoroutine(PlayGoldCardAnimation(tempPUI, powerUpCards[cardSlotIndex[repeatedChosenIndex]],
                                                tempPUI.goldPowerUpForSlot.transform, cardSlotTransforms[repeatedChosenIndex]));

            gm.totalGoldenCardsObtained++;
            gm.AddScore(5000 + (10000 * gm.goldCardMultiplier));
            gm.goldCardMultiplier += 2;
            HideChooseCards();
            CardChooseSetActiveAll(false);
            blackBG.SetActive(false);
            return;
        }

        if (cardSlotIndex[0] > cardsChooseIndexScript.Length - 1)
        {
            cardSlotIndex[0] = GetChosenCardIndex(tempPUI);
            powerUpCards[cardSlotIndex[0]].parent = cardSlotTransforms[0];
            powerUpCards[cardSlotIndex[0]].localPosition = Vector3.zero;
            SetCardChooseParticlesColor(cardSlotIndex[0], hit.collider.transform.GetChild(0).position);

            StartCoroutine(PlayChosenCardAnimation(tempPUI, powerUpCards[cardSlotIndex[0]].gameObject));
        }
        else if (cardSlotIndex[1] > cardsChooseIndexScript.Length - 1)
        {
            cardSlotIndex[1] = GetChosenCardIndex(tempPUI);
            powerUpCards[cardSlotIndex[1]].parent = cardSlotTransforms[1];
            powerUpCards[cardSlotIndex[1]].localPosition = Vector3.zero;
            SetCardChooseParticlesColor(cardSlotIndex[1], hit.collider.transform.GetChild(0).position);

            StartCoroutine(PlayChosenCardAnimation(tempPUI, powerUpCards[cardSlotIndex[1]].gameObject));
        }
        else if (cardSlotIndex[2] > cardsChooseIndexScript.Length - 1)
        {
            cardSlotIndex[2] = GetChosenCardIndex(tempPUI);
            powerUpCards[cardSlotIndex[2]].parent = cardSlotTransforms[2];
            powerUpCards[cardSlotIndex[2]].localPosition = Vector3.zero;
            SetCardChooseParticlesColor(cardSlotIndex[2], hit.collider.transform.GetChild(0).position);

            StartCoroutine(PlayChosenCardAnimation(tempPUI, powerUpCards[cardSlotIndex[2]].gameObject));
        }
        else
        {
            print("No Active PowerUp Slots"); //should never happen
            HideChooseCards();
            CheckIfCardSlotIsActive();
            CardChooseSetActiveAll(false);
            blackBG.SetActive(false);
            if (!gm.IsGamePausedOutsidePauseMenu()) gm.SetLaunchBallTimer();
        }

        HideChooseCards();
        //CheckIfCardSlotIsActive();
        CardChooseSetActiveAll(false);
        blackBG.SetActive(false);
    }

    IEnumerator PlayChosenCardAnimation(PowerUpIndex chosenCard, GameObject powerUpCard)
    {
        chosenCard.transform.parent = null;
        powerUpCard.SetActive(false);

        float t = 0;
        Vector3 target = powerUpCard.transform.position;
        Vector3 tempPos = chosenCard.transform.position;
        Vector3 tempScale = chosenCard.transform.localScale;

        while (t < 1)
        {
            chosenCard.transform.position = Vector2.Lerp(tempPos, target, t);
            chosenCard.transform.localScale = Vector2.Lerp(tempScale, Vector2.one, t);

            //print(t);
            t += Time.deltaTime * chosenCardAnimSpeed;
            yield return null;
        }


        HideCard(chosenCard);
        chosenCard.transform.localScale = Vector3.one * 2f;
        powerUpCard.SetActive(true);
        //gm.isChoosingPowerUp = false;
        CheckIfCardSlotIsActive();
        yield return null;
    }

    IEnumerator PlayGoldCardAnimation(PowerUpIndex chosenCard, Transform powerUpCard, Transform goldCard, Transform cardSlotToGo)
    {
        chosenCard.transform.parent = null;

        float t = 0;
        Vector3 target2 = powerUpCard.position;
        Vector3 target = (powerUpCard.position + chosenCard.transform.position) / 2;
        Vector3 tempChosenPos = chosenCard.transform.position;
        Vector3 tempChosenScale = chosenCard.transform.localScale;
        Vector3 tempPowerUpPos = powerUpCard.transform.position;
        Vector3 tempgoldCardPos = target;

        while (t < 1)
        {
            chosenCard.transform.position = Vector2.Lerp(tempChosenPos, target, t);
            chosenCard.transform.localScale = Vector2.Lerp(tempChosenScale, Vector2.one, t);
            powerUpCard.position = Vector2.Lerp(tempPowerUpPos, target, t);

            //print(t);
            t += Time.unscaledDeltaTime * chosenCardAnimSpeed;
            yield return null;
        }

        t = 0;
        HideCard(chosenCard);

        Quaternion tempPowerUpRot = transform.rotation;
        while (t < 1)
        {
            powerUpCard.Rotate(0f, 36f, 0f);

            t += Time.unscaledDeltaTime * chosenCardAnimSpeed * 2;

            if (t >= 0.6f)
            {
                gm.goldCardsChooseParticle.transform.position = target;
                gm.goldCardsChooseParticle.SetActive(true);
            }
            yield return null;
        }

        t = 0;
        HideCard(powerUpCard);
        powerUpCard.rotation = tempPowerUpRot;

        goldCard.parent = cardSlotToGo;
        goldCard.position = target;
        goldCard.gameObject.SetActive(true);

        goldCard.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            goldCard.DOScale(1f, 0.15f).SetEase(Ease.InQuad);
        });

        yield return new WaitForSeconds(0.5f);

        while (t < 1)
        {
            goldCard.position = Vector2.Lerp(tempgoldCardPos, target2, t);

            t += Time.deltaTime * chosenCardAnimSpeed;
            yield return null;
        }

        CheckIfCardSlotIsActive();
        goldCard.localPosition = Vector2.zero;
        chosenCard.transform.localScale = Vector3.one * 2f;
        //gm.isChoosingPowerUp = false;
        yield return null;
    }

    public void ActivateCardPowerUp(int powerUpIndex, int slotNumber, Transform t)
    {
        if (powerUpIndex > powerUpCards.Length - 1)
        {
            print("The index doesn't match the amount of cards");
            return;
        }

        switch (powerUpIndex)
        {
            case 0:
                {
                    if (!gm.gameStarted || gm.ballIsResettingAfterDeath) return;
                    if (CheckGlobalConflictedPowerUpsActive()) return;

                    if (slotIndexIsGold[slotNumber]) { DivideBalls(true); slotIndexIsGold[slotNumber] = false; }
                    else DivideBalls(false);

                    break;
                }
            case 1:
                {
                    if (!gm.gameStarted || !gm.ballIsAvailable) return;
                    if (CheckGlobalConflictedPowerUpsActive()) return;

                    cd.StartChooseDirectionPowerUp();
                    break;
                }
            case 2:
                {
                    if (!gm.gameStarted) return;

                    if (slotIndexIsGold[slotNumber]) { StartBallPhasingPowerUp(true); slotIndexIsGold[slotNumber] = false; }
                    else StartBallPhasingPowerUp(false);

                    break;
                }
            case 3:
                {
                    if (!gm.gameStarted || magnetIsActive) return;
                    StartMagnetPowerUp();
                    break;
                }
            case 4:
                {
                    if (!gm.gameStarted || currentPaddleSize >= 3) return;

                    if (slotIndexIsGold[slotNumber]) { mp.ChangePaddleSize(currentPaddleSize + 2); slotIndexIsGold[slotNumber] = false; }
                    else mp.ChangePaddleSize(currentPaddleSize + 1);

                    //spu.UpdateToPaddleSize();

                    break;
                }
            case 5:
                {
                    if (!gm.gameStarted) return;

                    int ran = Random.Range(0, 2);

                    if (gm.currentBallVelocity == gm.MinBallVelocity) ran = 0;
                    else if (gm.currentBallVelocity == gm.MaxBallVelocity) ran = 1;

                    if (ran == 0) IncreaseOrDecreaseBallSpeed(true);
                    else IncreaseOrDecreaseBallSpeed(false);

                    break;
                }
            case 6:
                {
                    if (!gm.gameStarted) return;
                    if (bottomWall.activeInHierarchy) return;

                    ActivateShieldPowerUp(true);

                    break;
                }
            case 7:
                {
                    if (!gm.gameStarted) return;

                    if (slotIndexIsGold[slotNumber]) { spu.StartShootPowerUp(true); slotIndexIsGold[slotNumber] = false; }
                    else spu.StartShootPowerUp(false);

                    break;
                }
            case 8:
                {

                    if (!gm.gameStarted) return;
                    HideCard(t);
                    cardSlotIndex[slotNumber] = 99;
                    cardSlotColliders[slotNumber].enabled = false;
                    StartCoroutine(ShowCardChooseSelection());
                    Singleton.Instance.leftBeforeChoosing = true;
                    gm.SaveSingletonVariables();

                    if (comingFromRandomPowerUp)
                        comingFromRandomPowerUp = false;
                    else
                    {
                        gm.totalCardsActivated++;

                        if (everyCardPowerUp.Contains(powerUpIndex))
                        {
                            everyCardPowerUp.Remove(powerUpIndex);
                            cardCounter++;
                            AchievementManager.instance.VerifyAchievementProgress(4, "TIME_TO_DUEL", cardCounter);
                        }
                    }
                    return;
                }
            case 9:
                {
                    comingFromRandomPowerUp = true;
                    int temp = Random.Range(0, cardsChooseIndexScript.Length - 1);
                    print(temp);
                    ActivateCardPowerUp(temp, slotNumber, t);
                    if (slotIndexIsGold[slotNumber]) { ActivateBubblePowerUp(13); slotIndexIsGold[slotNumber] = false; }

                    if (everyCardPowerUp.Contains(powerUpIndex))
                    {
                        everyCardPowerUp.Remove(powerUpIndex);
                        cardCounter++;
                        AchievementManager.instance.VerifyAchievementProgress(4, "TIME_TO_DUEL", cardCounter);
                    }

                    gm.totalCardsActivated++;
                    return;
                }
        }

        if (comingFromRandomPowerUp)
            comingFromRandomPowerUp = false;
        else
        {
            gm.totalCardsActivated++;

            if (everyCardPowerUp.Contains(powerUpIndex))
            {
                everyCardPowerUp.Remove(powerUpIndex);
                cardCounter++;
                AchievementManager.instance.VerifyAchievementProgress(4, "TIME_TO_DUEL", cardCounter);
            }
        }

        HideCard(t);
        cardSlotIndex[slotNumber] = 99;
        cardSlotColliders[slotNumber].enabled = false;
    }

    public void ActivateBubblePowerUp(int bubbleIndex)
    {
        switch (bubbleIndex)
        {
            case 0:
                {
                    if (!gm.gameStarted || gm.ballIsResettingAfterDeath) return;
                    if (CheckGlobalConflictedPowerUpsActive()) return;

                    DivideBalls(false);
                    break;
                }
            case 1:
                {
                    if (!gm.gameStarted || !gm.ballIsAvailable) return;
                    if (CheckGlobalConflictedPowerUpsActive()) return;
                    cd.StartChooseDirectionPowerUp();
                    break;
                }
            case 2:
                {
                    if (!gm.gameStarted) return;
                    StartBallPhasingPowerUp(false);
                    break;
                }
            case 3:
                {
                    if (!gm.gameStarted) return;
                    StartMagnetPowerUp();
                    break;
                }
            case 4:
                {
                    if (!gm.gameStarted) return;
                    mp.ChangePaddleSize(currentPaddleSize + 1);

                    spu.UpdateToPaddleSize();
                    break;
                }
            case 5:
                {
                    if (!gm.gameStarted) return;
                    if (currentPaddleSize >= 1) paddleMinSizeCounter = 0;
                    mp.ChangePaddleSize(currentPaddleSize - 1);

                    spu.UpdateToPaddleSize();

                    if (currentPaddleSize <= 0)
                    {
                        paddleMinSizeCounter++;
                        AchievementManager.instance.VerifyAchievementProgress(10, "TEENY_TINY", paddleMinSizeCounter);
                    }

                    break;
                }
            case 6:
                {
                    if (!gm.gameStarted) return;
                    if (gm.currentBallVelocity < gm.MaxBallVelocity) ballMaxSpeedCounter = 0;
                    IncreaseOrDecreaseBallSpeed(true);

                    if (gm.currentBallVelocity >= gm.MaxBallVelocity)
                    {
                        ballMaxSpeedCounter++;
                        AchievementManager.instance.VerifyAchievementProgress(9, "GOTTA_GO_FAST", ballMaxSpeedCounter);
                    }

                    break;
                }
            case 7:
                {
                    if (!gm.gameStarted) return;
                    IncreaseOrDecreaseBallSpeed(false);

                    break;
                }
            case 8:
                {
                    if (!gm.gameStarted) return;
                    uic.UpdateHealth(1, true);
                    break;
                }
            case 9:
                {
                    if (!gm.gameStarted) return;
                    ActivateShieldPowerUp(true);
                    break;
                }
            case 10:
                {
                    if (!gm.gameStarted) return;
                    StartCoroutine(DestroyAllBlocksOfColor());
                    break;
                }
            case 11:
                {
                    if (!gm.gameStarted) return;
                    spu.StartShootPowerUp(false);
                    break;
                }
            case 12:
                {
                    if (!gm.gameStarted) return;
                    if (!FreeCardSlotAvailable() && !powerUpsAreRandom) return;
                    Singleton.Instance.leftBeforeChoosing = true;
                    StartCoroutine(ShowCardChooseSelection());
                    gm.SaveSingletonVariables();
                    break;
                }
            case 13:
                {
                    comingFromRandomPowerUp = true;
                    int temp = Random.Range(0, 13);
                    print(temp);
                    ActivateBubblePowerUp(temp);

                    if (everyBubblePowerUp.Contains(bubbleIndex))
                    {
                        everyBubblePowerUp.Remove(bubbleIndex);
                        bubbleCounter++;
                        AchievementManager.instance.VerifyAchievementProgress(3, "BUBBLY_RUN", bubbleCounter);
                    }
                    return;
                }
            case 14:
                {
                    for (int i = 0; i < gm.ballGos.Count; i++)
                    {
                        GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.outwardsParticlePool);
                        temp.transform.position = gm.ballGos[i].transform.position;
                        temp.SetActive(true);

                        gm.ballScripts[i].currentPhasingsLeft = 0;
                        gm.ballGos[i].SetActive(false);
                        gm.ballGos[i].transform.position = gm.placeToHidePooled;
                    }

                    for (int i = 0; i < currentActiveBubbles.Count; i++)
                    {
                        GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.bubbleExplosionParticlesPool, gm.bubbleExplosionParticle);
                        temp.transform.position = currentActiveBubbles[i].transform.position; //FIXED BUG: Breakable interaction was adding a Null value to the list (Make sure returned bubble index exists on bubble's pool
                        temp.SetActive(true);
                        currentActiveBubbles[i].SetActive(false);
                    }

                    gm.RemoveAllBallsButMainBall();
                    currentActiveBubbles.Clear();
                    uic.UpdateHealth(1, false);

                    if (uic.GetCurrentHealth() >= 0)
                    {
                        gm.ballIsResettingAfterDeath = true;
                        gm.mainBall.BallBackToPaddle();
                        gm.ResetPowerUpsState(true);

                        gm.SaveSingletonVariables();
                    }
                    else
                    {
                        gm.isDead = true;
                        gm.gameEnded = true;
                        gm.StartEndOfGameAnimation();
                    }

                    return;
                }
            case 15:
                {
                    comingFromRandomPowerUp = true;
                    int temp = Random.Range(0, 15);

                    if (temp == 14) temp = Random.Range(0, 15);

                    onlyRandomBubbleCounter++;
                    AchievementManager.instance.VerifyAchievementProgress(5, "PRAISE_RNG", onlyRandomBubbleCounter);

                    ActivateBubblePowerUp(temp);
                    return;
                }
        }

        if (comingFromRandomPowerUp)
            comingFromRandomPowerUp = false;
        else
        {
            if (everyBubblePowerUp.Contains(bubbleIndex))
            {
                everyBubblePowerUp.Remove(bubbleIndex);
                bubbleCounter++;
                AchievementManager.instance.VerifyAchievementProgress(3, "BUBBLY_RUN", bubbleCounter);
            }
        }

        ShootAllUnlaunchedBalls();
    }

    public void ShootAllUnlaunchedBalls()
    {
        if (chooseDirectionIsActive || !gm.gameStarted) return;

        int tempBallScriptsCount = gm.ballScripts.Count;
        for (int i = 0; i < tempBallScriptsCount; i++)
        {
            if (gm.ballScripts[i] != null)
            {
                if (!gm.ballScripts[i].hasLaunched)
                {
                    gm.ballScripts[i].LaunchBall(true);
                }
            }
        }
    }

    public bool CheckGlobalConflictedPowerUpsActive()
    {
        if (cd.isDragging) return true;
        else return false;
        //Keep Adding powerUps that globally conflict with others
    }

    public void HideChooseCards()
    {
        for (int i = 0; i < cardChooseSlotTransforms.Length; i++)
        {
            if (cardChooseSlotTransforms[i].childCount > 0)
                HideCard(cardChooseSlotTransforms[i].GetChild(0));
        }
    }

    public void HideCard(Transform cardTransform)
    {
        cardTransform.parent = null;
        cardTransform.position = gm.placeToHidePooled;
        cardTransform.gameObject.SetActive(false);

        //if (cardTransform.childCount >= 2)
        //{
        //    if (cardTransform.GetChild(0).name.CompareTo("Glow") != 0) cardTransform.GetChild(0).gameObject.SetActive(false);
        //    if (cardTransform.GetChild(1).name.CompareTo("Glow") != 0) cardTransform.GetChild(1).gameObject.SetActive(false);
        //}
    }

    public void HideCard(PowerUpIndex pui)
    {
        pui.transform.parent = null;
        pui.transform.position = gm.placeToHidePooled;
        pui.gameObject.SetActive(false);

        if (pui.goldPowerUpForSlot != null)
        {
            pui.transform.GetChild(0).gameObject.SetActive(false);
            pui.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void Get3RandomGeneratedCards() //DEBUG: if this is not working properly, check GameObjects with PowerUpIndex Script, cardsChooseIndexScript
    {
        randomCardChooseIntIndexes.Clear();
        RemoveActiveCardPowerUps();

        int[] ran = new int[3];
        PowerUpIndex temp;

        for (int i = 0; i < ran.Length; i++)
        {
            temp = tempCardsChooseIndexScript[Random.Range(0, tempCardsChooseIndexScript.Count)];
            ran[i] = temp.index;
            tempCardsChooseIndexScript.Remove(temp);
            randomCardChooseIntIndexes.Add(ran[i]);
        }

        //print("ran 0: " + ran[0] + " , " + "ran 1: " + ran[1] + " , " + "ran 2: " + ran[2]);
        if (ran[0] == ran[1] || ran[0] == ran[2] || ran[1] == ran[2])
        {
            print("Randomly Generated Cards index REPEATING");
            Debug.Break();
        }
    }

    public void RemoveActiveCardPowerUps()
    {
        tempCardsChooseIndexScript.Clear();
        tempCardsChooseIndexScript.AddRange(cardsChooseIndexScript);

        for (int i = 0; i < cardSlotIndex.Length; i++)
        {
            if (cardSlotIndex[i] < 99)
            {
                if (cardsChooseIndexScript[cardSlotIndex[i]].goldPowerUpForSlot == null)
                {
                    tempCardsChooseIndexScript.Remove(cardsChooseIndexScript[cardSlotIndex[i]]);
                    print("Card " + cardsChooseIndexScript[cardSlotIndex[i]].name + " was Removed");
                }
                else
                {
                    if (slotIndexIsGold[i])
                    {
                        tempCardsChooseIndexScript.Remove(cardsChooseIndexScript[cardSlotIndex[i]]);
                        print("Card " + cardsChooseIndexScript[cardSlotIndex[i]].name + " was Removed");
                    }
                }
            }
        }
    }

    public int GetSlotWithSameIndexIfExists(int index)
    {
        for (int i = 0; i < cardSlotIndex.Length; i++)
        {
            if (cardSlotIndex[i] == index)
            {
                return repeatedChosenIndex = i;
            }
        }

        return repeatedChosenIndex = 99;
    }

    public bool FreeCardSlotAvailable()
    {
        for (int i = 0; i < cardSlotIndex.Length; i++)
        {
            if (cardSlotIndex[i] >= 99)
                return true;
        }
        return false;
    }


    //Start of PowerUps Methods
    public void DivideBalls(bool isGold)
    {
        int currentBalls = gm.GetCurrentAmountofBalls();

        if (!isGold)
        {
            for (int i = 0; i < currentBalls; i++)
            {
                ballTempGo = PoolManager.current.GetPooledGameObject(PoolManager.current.ballPool);
                if (ballTempGo == null) return;

                Rigidbody2D tempRb2d = ballTempGo.GetComponent<Rigidbody2D>();
                Ball tempBallScript = ballTempGo.GetComponent<Ball>();
                tempBallScript.timesBouncedOffPlayer = 0;

                if (!chooseDirectionIsActive)
                {
                    if (gm.ballScripts[i].hasLaunched)
                        ballTempGo.transform.position = gm.ballGos[i].transform.position;
                    else
                        ballTempGo.transform.position = gm.ballGos[i].transform.position + (Vector3.up * 0.4f);

                    tempBallScript.hasLaunched = true;
                    ballTempGo.SetActive(true);

                    tempRb2d.velocity = new Vector2(-gm.ballRb2ds[i].velocity.x, Mathf.Abs(gm.ballRb2ds[i].velocity.y));
                    if (tempRb2d.velocity == Vector2.zero || tempRb2d.velocity.x == 0) tempBallScript.BallVerticalImpulse();
                }
                else
                {
                    ballTempGo.transform.position = gm.ballGos[i].transform.position;
                    tempBallScript.hasLaunched = false;
                    ballTempGo.SetActive(true);

                    cd.currentActiveBallScripts.Add(tempBallScript);
                    cd.currentActiveBallRb2ds.Add(tempRb2d);

                    if (!cd.isShooting)
                    {
                        tempBallScript.BallRandomImpulse(-8f, 8f);
                        cd.allBallsAtZeroSpeed = false;
                    }
                    else
                    {
                        tempBallScript.hasLaunched = true;
                        tempRb2d.velocity = new Vector2(-gm.ballRb2ds[i].velocity.x, Mathf.Abs(gm.ballRb2ds[i].velocity.y));
                        if (tempRb2d.velocity == Vector2.zero || tempRb2d.velocity.x == 0) tempBallScript.BallRandomImpulse(-8f, 8f);
                    }
                }

                gm.AddBallToLists(ballTempGo, tempRb2d, tempBallScript);
            }
        }
        else
        {
            for (int i = 0; i < currentBalls; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    ballTempGo = PoolManager.current.GetPooledGameObject(PoolManager.current.ballPool);
                    if (ballTempGo == null) return;

                    Rigidbody2D tempRb2d = ballTempGo.GetComponent<Rigidbody2D>();
                    Ball tempBallScript = ballTempGo.GetComponent<Ball>();
                    tempBallScript.timesBouncedOffPlayer = 0;

                    if (!chooseDirectionIsActive)
                    {
                        if (gm.ballScripts[i].hasLaunched)
                            ballTempGo.transform.position = gm.ballGos[i].transform.position;
                        else
                            ballTempGo.transform.position = gm.ballGos[i].transform.position + (Vector3.up * 0.4f);

                        tempBallScript.hasLaunched = true;
                        ballTempGo.SetActive(true);

                        if (j == 0)
                            tempBallScript.BallRandomImpulse(-4, -4);
                        else if (j == 1)
                            tempBallScript.BallRandomImpulse(0, 0);
                        else if (j == 2)
                            tempBallScript.BallRandomImpulse(4, 4);
                    }
                    else
                    {
                        ballTempGo.transform.position = gm.ballGos[i].transform.position;
                        tempBallScript.hasLaunched = false;
                        ballTempGo.SetActive(true);

                        cd.currentActiveBallScripts.Add(tempBallScript);
                        cd.currentActiveBallRb2ds.Add(tempRb2d);

                        if (!cd.isShooting)
                        {
                            if (j == 0)
                                tempBallScript.BallRandomImpulse(-4, -4);
                            else if (j == 1)
                                tempBallScript.BallRandomImpulse(0, 0);
                            else if (j == 2)
                                tempBallScript.BallRandomImpulse(4, 4);

                            cd.allBallsAtZeroSpeed = false;
                        }
                        else
                        {
                            tempBallScript.hasLaunched = true;

                            if (j == 0)
                                tempBallScript.BallRandomImpulse(-4, -4);
                            else if (j == 1)
                                tempBallScript.BallRandomImpulse(0, 0);
                            else if (j == 2)
                                tempBallScript.BallRandomImpulse(4, 4);
                        }
                    }

                    gm.AddBallToLists(ballTempGo, tempRb2d, tempBallScript);
                }
            }
        }
    }

    public void StartBallPhasingPowerUp(bool isGold) //BallPhasing layer = 10
    {
        //gm.GetBallLists();
        for (int i = 0; i < gm.ballScripts.Count; i++)
        {
            gm.ballScripts[i].gameObject.layer = 10;
            gm.ballScripts[i].instantKillGo.SetActive(true);
            if (!isGold)
                gm.ballScripts[i].currentPhasingsLeft = phasingPowerUpMaxBlocks;
            else
                gm.ballScripts[i].currentPhasingsLeft = phasingPowerUpGoldMaxBlocks;

            gm.ballScripts[i].phasingActive = true;
        }
    }

    public void LoadBallPhasingPowerUp(Ball ballScript, int phasingsLeft) //BallPhasing layer = 10
    {
        if (phasingsLeft <= 0)
        {
            return;
        }
        else
        {
            ballScript.gameObject.layer = 10;
            ballScript.instantKillGo.SetActive(true);

            ballScript.currentPhasingsLeft = phasingsLeft;

            ballScript.phasingActive = true;
        }
    }

    public void StartMagnetPowerUp()
    {
        magnetIsActive = true;
        gm.mp.magnetChildGO.SetActive(true);
        gm.mp.magnetAnim.SetInteger("currentSize", currentPaddleSize);

        //New line added - Shinno
        if (gm.mp.ExtraGlow) gm.mp.extra_glowRenderer.SetActive(true);
        //
    }

    public void IncreaseOrDecreaseBallSpeed(bool increase)
    {
        gm.speedIncreaseTimer = 0;

        int tempRan = Random.Range(1, 3);
        float tempVel = gm.currentBallVelocity;

        tempVel = (increase) ? (tempVel + tempRan) : (tempVel - tempRan);

        if (tempVel > gm.MaxBallVelocity) tempVel = gm.MaxBallVelocity;
        else if (tempVel < gm.MinBallVelocity) tempVel = gm.MinBallVelocity;

        gm.currentBallVelocity = tempVel;

        int tempCurrentAmount = gm.GetCurrentAmountofBalls();
        for (int i = 0; i < tempCurrentAmount; i++)
        {
            gm.ballRb2ds[i].velocity = gm.ballRb2ds[i].velocity.normalized * gm.currentBallVelocity;
        }
    }

    public void ActivateShieldPowerUp(bool activate)
    {
        bottomWall.SetActive(activate);
    }

    public void EndChooseDirection()
    {
        chooseDirectionIsActive = false;
        cd.DisableChooseDirection();
    }

    #region Block Color Index
    // 0 = Red
    // 1 = Blue
    // 2 = LightBlue
    // 3 = Green
    // 4 = Yellow
    // 5 = Orange
    // 6 = Purple
    #endregion
    public IEnumerator DestroyAllBlocksOfColor()
    {
        levelBlockColors.Clear();
        int colorIndex = 0;

        for (int i = 0; i < gm.currentBlocksBreakableInteraction.Count; i++)
        {
            if (gm.currentBlocksBreakableInteraction[i].block_Color == "") continue;

            if (gm.currentBlocksBreakableInteraction[i].gameObject.activeInHierarchy)
            {
                if (!levelBlockColors.Contains(gm.currentBlocksBreakableInteraction[i].block_Color))    //If it cant choose a color that no longer exists, Check for activeInHierarchy too
                {
                    levelBlockColors.Add(gm.currentBlocksBreakableInteraction[i].block_Color);
                }
            }
        }

        if (levelBlockColors.Count == 0)
        {
            yield break;
        }

        bubbleBlockSR.gameObject.SetActive(true);
        int counter = 0;

        while (counter < 2)
        {
            while (colorIndex < bubbleBlockPowerUpSprites.Length)
            {
                bubbleBlockSR.sprite = bubbleBlockPowerUpSprites[colorIndex];
                colorIndex++;

                yield return new WaitForSeconds(0.1f);
            }

            colorIndex = 0;
            counter++;
        }

        counter = 0;
        bool tempBool = false;
        int tempRan = Random.Range(0, levelBlockColors.Count);

        colorIndex = GetBlockColorIndex(levelBlockColors[tempRan]);
        bubbleBlockSR.sprite = bubbleBlockPowerUpSprites[colorIndex];

        while (counter < 6)
        {
            if (!tempBool) bubbleBlockSR.color = new Color(bubbleBlockSR.color.r, bubbleBlockSR.color.g, bubbleBlockSR.color.b, 0.5f);
            else bubbleBlockSR.color = new Color(bubbleBlockSR.color.r, bubbleBlockSR.color.g, bubbleBlockSR.color.b, 1f);

            tempBool = !tempBool;
            counter++;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < gm.currentBlocksBreakableInteraction.Count; i++)
        {
            if (gm.currentBlocksBreakableInteraction[i] != null)
            {
                if (gm.currentBlocksBreakableInteraction[i].block_Color.CompareTo(levelBlockColors[tempRan]) == 0)
                {
                    if (gm.currentBlocksBreakableInteraction[i].gameObject.activeInHierarchy)
                    {
                        gm.currentBlocksBreakableInteraction[i].DeathSequence();
                        gm.AddScore(101);
                    }
                }
            }
        }

        gm.SaveSingletonVariables(); //Added so you cant exploit the bubble to break different random blocks after loading
        bubbleBlockSR.gameObject.SetActive(false);
    }

    private int GetBlockColorIndex(string t)
    {
        if (t.CompareTo("Red") == 0) return 0;
        else if (t.CompareTo("Blue") == 0) return 1;
        else if (t.CompareTo("LightBlue") == 0) return 2;
        else if (t.CompareTo("Green") == 0) return 3;
        else if (t.CompareTo("Yellow") == 0) return 4;
        else if (t.CompareTo("Orange") == 0) return 5;
        else if (t.CompareTo("Purple") == 0) return 6;

        print("no block index found");
        return 0;
    }

    public int GetChosenCardIndex(PowerUpIndex puIndex)
    {
        return puIndex.index;
    }

    public void SetCardSlotsColliders()
    {
        cardSlotColliders = new BoxCollider2D[cardSlotTransforms.Length];

        for (int i = 0; i < cardSlotTransforms.Length; i++)
        {
            cardSlotColliders[i] = cardSlotTransforms[i].GetComponent<BoxCollider2D>();
        }
    }

    public void SetCardChooseSlotsColliders()
    {
        chooseCardSlotColliders = new BoxCollider2D[cardChooseSlotTransforms.Length];

        for (int i = 0; i < cardChooseSlotTransforms.Length; i++)
        {
            chooseCardSlotColliders[i] = cardChooseSlotTransforms[i].GetComponent<BoxCollider2D>();
        }
    }

    public void CardSlotsAllCollidersEnabled(BoxCollider2D[] colliders, bool enable)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = enable;
        }
    }

    public void CheckIfCardSlotIsActive()
    {
        for (int i = 0; i < cardSlotTransforms.Length; i++)
        {
            if (cardSlotTransforms[i].childCount > 0)
            {
                cardSlotColliders[i].enabled = true;
            }
            else
            {
                cardSlotColliders[i].enabled = false;
            }
        }
    }

    public void CardChooseSetActiveAll(bool enable)
    {
        for (int i = 0; i < cardChooseSlotTransforms.Length; i++)
        {
            cardChooseSlotTransforms[i].gameObject.SetActive(enable);
        }
    }

    public void SpawnBubbleOnBlock(GameObject bubble, Transform blockPos) //Use this function on Collision
    {
        if (bubble == null) return;
        bubble.transform.position = blockPos.position;
        bubble.SetActive(true);
    }

    #region Which PowerUp is which Index
    //Index 0 = Multiply PowerUp
    //Index 1 = ReDirection PowerUp
    //Index 2 = Ball Phasing PowerUp
    //Index 3 = Magnet PowerUp
    //Index 4 = Paddle Size Up PowerUp
    //Index 5 = Paddle Size Down PowerDown
    //Index 6 = Ball Speed Up PowerUp
    //Index 7 = Ball Speed Down PowerDown
    //Index 8 = +1 Life
    //Index 9 = Shield PowerUp
    //Index 10 = Block Cleaner PowerUp
    //Index 11 = Shoot Laser PowerUp
    //Index 12 = Show Card Selection
    //Index 13 = Random
    //Index 14 = Death!
    #endregion
    public int GetRandomBubbleIndex()
    {
        if (powerUpsAreRandom) return 15;

        int ran = Random.Range(1, 101);

        if (ran <= 10)     //10%
            return 0;

        else if (ran <= 20) //10%
            return 1;

        else if (ran <= 30) //10%
            return 2;

        else if (ran <= 41) //11%
            return 3;

        else if (ran <= 52) //11%
        {
            ran = Random.Range(1, 101);

            if (ran <= 60) //60%
                return 4;
            else           //40%
                return 5;
        }

        else if (ran <= 67) //15%
        {
            ran = Random.Range(1, 101);

            if (ran <= 60) //60%
                return 6;
            else           //40%
                return 7;
        }

        else if (ran <= 72) //5%
        {
            ran = Random.Range(1, 101);

            if (ran <= 80) //80%
                return 8;
            else           //20%
            { gm.ReduceScore(5000); return 14; }
        }

        else if (ran <= 74) //2%
            return 9;

        else if (ran <= 77) //3%
            return 10;

        else if (ran <= 85) //8%
            return 11;

        else if (ran <= 90) //5%
            return 12;

        else if (ran <= 100) //10%
            return 13;

        return 99; //Should never happen
    }

    public void SetCardChooseParticlesColor(int index, Vector2 pos)
    {
        GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.cardsParticlesPool);
        ParticleSystem ps = null;
        if (temp.GetComponent<ParticleSystem>() != null) ps = temp.GetComponent<ParticleSystem>();
        else { print("No particle system to obtain"); return; }

        cardsParticles = ps;
        cardsParticles.startColor = cardsChooseColor[index];

        temp.transform.position = pos;
        temp.SetActive(true);

        //gm.cardsChooseParticle.transform.position = pos;
        //gm.cardsChooseParticle.SetActive(true);
    }

    public void ActivateChooseDirectionIndicators(bool active)
    {
        dragPanel.SetActive(active);
        dragHand.SetActive(active);
    }

    public void LoadOwnedCards()
    {
        if (Singleton.Instance.isRandomRun)
        {
            for (int i = 0; i < cardSlotIndex.Length; i++)
            {
                if (cardSlotIndex[i] == 99) continue;

                if (!slotIndexIsGold[i])
                {
                    randomCardsForOnlyRandom[i].parent = cardSlotTransforms[i];
                    randomCardsForOnlyRandom[i].localPosition = Vector3.zero;
                }
                else
                {
                    randomGoldCardsForOnlyRandom[i].parent = cardSlotTransforms[i];
                    randomGoldCardsForOnlyRandom[i].localPosition = Vector3.zero;
                }
            }
        }
        else
        {
            for (int i = 0; i < cardSlotIndex.Length; i++)
            {
                if (cardSlotIndex[i] == 99) continue;
                GameObject temp;
                if (slotIndexIsGold[i]) temp = cardsChooseIndexScript[cardSlotIndex[i]].goldPowerUpForSlot;
                else temp = powerUpCards[cardSlotIndex[i]].gameObject;

                temp.transform.parent = cardSlotTransforms[i];
                temp.transform.localPosition = Vector3.zero;
            }
        }
        CheckIfCardSlotIsActive();
    }
}
