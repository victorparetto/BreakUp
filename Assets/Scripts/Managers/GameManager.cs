using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PowerUpManager pum;
    ShootPowerUp spu;
    UIControl uic;
    BackgroundScrolling bgScrolling;
    EndOfRunScoreAnimation ersa;

    public float wallLightIntensity = 2.5f;
    [SerializeField] Material wallMaterial;
    public Material backgroundMaterial;

    [HideInInspector] public Vector2 placeToHidePooled = new Vector2(-9f, 1f);

    [Header("Important Bools")]
    [SerializeField] bool resetSingletonOnStart = false;
    public bool isClassicMode = false;
    public bool gameIsPaused = false;
    public bool isChoosingPowerUp = false;
    public bool gameStarted = false;
    public bool ballIsAvailable = false;
    public bool ballIsResettingAfterDeath = false;
    public bool checkForDestroyedBlocks = false;
    public bool isBuildingLevel = false;
    public bool isDead = false;
    public bool gameEnded = false;

    //Level Related Variables
    [Header("Level Variables")]
    public int currentLevel = 0;
    public GameObject currentLevelGO;
    [SerializeField] int chooseCardEveryXLevel = 3;
    [SerializeField] int increaseSpeedEveryXLevel = 30;
    [HideInInspector] public int levelChangePosOffset = 9;
    [HideInInspector] public Vector3 levelChangePos;
    public List<GameObject> levels = new List<GameObject>();
    public List<GameObject> classicLevels = new List<GameObject>();
    public List<LevelBehavID> level0 = new List<LevelBehavID>();
    public List<LevelBehavID> level01To10 = new List<LevelBehavID>();
    public List<LevelBehavID> level11To20 = new List<LevelBehavID>();
    public List<LevelBehavID> level21To30 = new List<LevelBehavID>();
    public List<LevelBehavID> level31To40 = new List<LevelBehavID>();
    public List<LevelBehavID> level41To50 = new List<LevelBehavID>();
    public List<LevelBehavID> bonusLevel = new List<LevelBehavID>();
    public List<GameObject> currentBlocks = new List<GameObject>();
    public List<BreakableInteraction> currentBlocksBreakableInteraction = new List<BreakableInteraction>();
    public List<GameObject> currentRowParents = new List<GameObject>();
    public List<UnbreakableBehav> currentUnbreakableBlocksScript = new List<UnbreakableBehav>();
    [SerializeField] GameObject topWall = null;
    [SerializeField] GameObject topTrigger = null;

    public float levelTimerForClassicMode = 0;
    public float speedIncreaseTimer = 0;
    private float increaseSpeedAfterTimer = 20f;

    [Header("Level Building Variables")]
    public float buildingInterval = 0.2f;
    public float minInterval = 0.05f;
    [HideInInspector] public float maxInterval;
    [HideInInspector] public float t = 0;

    //Ball Related Variables
    [Header("Ball Variables")]
    public bool canLaunchBall = true;
    public Ball mainBall;
    public float currentBallVelocity = 8f;
    public float startBallVelocity = 8f;
    [HideInInspector] public float MinBallVelocity = 6f;
    [HideInInspector] public float MaxBallVelocity = 13f;
    public List<GameObject> ballGos = new List<GameObject>();
    public List<Rigidbody2D> ballRb2ds = new List<Rigidbody2D>();
    public List<Ball> ballScripts = new List<Ball>();
    bool startLaunchBallTimer = false;
    float timerToLaunchBall = 0.5f;
    float currentLaunchBallTimer = 0;

    //Particles Prefabs
    [Header("Particle Effects")]
    public GameObject outwardsParticle = null;
    public GameObject goldCardsChooseParticle = null;
    public GameObject bubbleExplosionParticle = null;
    [SerializeField] GameObject inwardsParticle = null;

    //Paddle related Variables
    [Header("Paddle Variables")]
    [SerializeField] GameObject paddlePrefab = null;
    [SerializeField] Vector2 paddleSpawnPlace = new Vector2(0, -6);
    [HideInInspector] public MovePaddle mp;
    GameObject paddleGO = null;

    //Money Variables
    [Header("Money Variables")]
    public int currentMoney;
    public int moneyToSave;
    bool useSavedMoneyInstead = false;
    public float adBonusMultiplier = 1;
    [SerializeField] RectTransform uiMoneyRT;
    public Vector3 uiMoneyRtPos;

    //Achievements Variables
    [Header("Achievements Variables")]
    [SerializeField] int blocksDestroyedThisLevel = 0;
    public int blocksDestroyedWithBullets = 0;
    public bool hasDiedThisRun = false;

    //Score Variables
    [Header("Score Variables")]
    public int score;
    public int scoreToAdd = 0;
    public int scoreBeforeEndBonuses = 0;
    public bool scoreBonusesWereAdded = false;
    [HideInInspector] public int ballScoreMultiplier = 250;
    [HideInInspector] public int bubbleScoreMultiplier = 100;
    [HideInInspector] public int goldCardMultiplier = 0;
    [HideInInspector] public float laserScoreMultiplier = 0;

    //Score animation variables
    [HideInInspector] public bool scoreAnimationIsPlaying = false;
    [HideInInspector] public int scoreToAddAnimation = 0;
    [HideInInspector] public int middleShowScoreAnimation = 0;

    //End of Game Variables
    [Header("End of Game Variables")]
    public int totalGoldenCardsObtained = 0;
    public int totalCardsActivated = 0;
    public int totalBubblesPicked = 0;
    public float globalRunTimer = 0;
    public bool runWasCompleted = false;

    //temp variables
    public List<LaserBulletBehav> loadedBullets = new List<LaserBulletBehav>();
    GameObject tempgo;

    void Awake()
    {
        Application.targetFrameRate = 60;

        ersa = GetComponent<EndOfRunScoreAnimation>();

        if (resetSingletonOnStart) ResetSavedVariables();

        isClassicMode = Singleton.Instance.isClassicMode_Temp;
        Singleton.Instance.Load((isClassicMode) ? 1 : 0);

        if (!Singleton.Instance.hasSavedData) Singleton.Instance.isRandomRun = Singleton.Instance.isRandomized_Temp;
        else Singleton.Instance.saveWasLoaded_Temp = true;

        bgScrolling = GetComponent<BackgroundScrolling>();
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        uic = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIControl>();

        wallMaterial.SetColor("_MainColor", Color.white * wallLightIntensity);
        backgroundMaterial.color = new Color(29f / 255, 29f / 255, 29f / 255);
        levelChangePos = Vector3.up * levelChangePosOffset;


        if (CustomizableManager.GetCurrentPaddlePrefab() != null) paddlePrefab = CustomizableManager.GetCurrentPaddlePrefab();
        GameObject tempGO = Instantiate(paddlePrefab, paddleSpawnPlace, transform.rotation);
        mp = tempGO.GetComponent<MovePaddle>();
        spu = mp.spu;
        paddleGO = mp.gameObject;

        if (CustomizableManager.GetCurrentBallPrefab() != null)
        {
            tempgo = CustomizableManager.GetCurrentBallPrefab();

            Ball tempball = tempgo.GetComponent<Ball>();

            if (tempball.outwardsParticlesPrefab != null)
                outwardsParticle = tempball.outwardsParticlesPrefab;

            if (tempball.inwardsParticlesPrefab != null)
            {
                inwardsParticle = Instantiate(tempball.inwardsParticlesPrefab);
                inwardsParticle.SetActive(false);
            }
        }

        if (isClassicMode)
            InstantiateLevelsForClassicMode();
        else
            AddRandomLevelsToLevels();

        if (Singleton.Instance.isRandomRun) chooseCardEveryXLevel = 1;
    }

    private void Start()
    {
        LoadSingletonState();

        if (isClassicMode)
        {
            topWall.SetActive(true);
            topTrigger.SetActive(false);
        }
        else
        {
            topWall.SetActive(false);
            topTrigger.SetActive(true);
        }

        PoolManager.current.HidePool(PoolManager.current.ballPool);
        mainBall = PoolManager.current.GetPooledGameObject(PoolManager.current.ballPool).GetComponent<Ball>();
        AddBallToLists(mainBall.gameObject, mainBall.GetComponent<Rigidbody2D>(), mainBall);
        //PoolManager.current.CreatePool(PoolManager.current.sparkCollisionPool, 3, mainBall.sparkCollisionParticles, "SparkParticle", false);

        BuildNextLevel(); //build next level is before Balls because of Singleton.saveWasLoaded_temp
        //LoadSingletonBalls();

        uiMoneyRtPos = Camera.main.ScreenToWorldPoint(uiMoneyRT.transform.position);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A)) //Shortcut to activate level transition
        {
            DestroyCurrentBlocks();
        }
#endif

        if (isBuildingLevel)
        {
            buildingInterval = Mathf.Lerp(maxInterval, minInterval, t);
            t += Time.deltaTime * 0.7f;
            if (t >= 1)
            {
                t = 1;
                buildingInterval = minInterval;
                //isBuildingLevel = false;
            }
        }

        if (checkForDestroyedBlocks)
        {
            if (AllBlocksDestroyed())
            {
                if (currentLevel + 1 >= levels.Count)
                {
                    print("Finished all Levels!");
                    checkForDestroyedBlocks = false;
                    gameEnded = true;
                    return;
                }

                currentLevel += 1;
                BuildNextLevel();
                //ChangeLevel();
            }
        }

        if (startLaunchBallTimer)
        {
            currentLaunchBallTimer += Time.deltaTime;

            if (currentLaunchBallTimer >= timerToLaunchBall)
            {
                canLaunchBall = true;
                startLaunchBallTimer = false;
            }
        }

        if (gameStarted)
        {
            globalRunTimer += Time.deltaTime;
            speedIncreaseTimer += Time.deltaTime;
            levelTimerForClassicMode += Time.deltaTime;
        }

        if (speedIncreaseTimer >= increaseSpeedAfterTimer)
        {
            if (currentBallVelocity < MaxBallVelocity)
            {
                currentBallVelocity++;
            }
            speedIncreaseTimer = 0;
        }

        if (isClassicMode)
        {
            if (AllBlocksDestroyed())
            {
                if (currentLevel + 1 >= levels.Count)
                {
                    print("Finished all Levels!");
                    return;
                }
                ChangeMainBall(ballScripts[0]);
                EndLevel(false);
                currentLevel += 1;
                BuildNextLevel();
            }
        }
    }

    //Level Related Variables
    public void EndLevel(bool isClimbMode) //Used by Ball
    {
        #region Floor achievements progress
        if (isClimbMode)
        {
            AchievementManager.instance.VerifyAchievementProgress(11, "UP_THE_LADDER", currentLevel);
            AchievementManager.instance.VerifyAchievementProgress(12, "LONG_LADDER", currentLevel);
            AchievementManager.instance.VerifyAchievementProgress(13, "END_OF_LADDER", currentLevel);
        }
        else
        {
            AchievementManager.instance.VerifyAchievementProgress(14, "CLASSIC_BEGINNING", currentLevel);
            AchievementManager.instance.VerifyAchievementProgress(15, "CLASSIC_PROGRESS", currentLevel);
            AchievementManager.instance.VerifyAchievementProgress(16, "CLASSIC_FINALE", currentLevel);
        }
        #endregion        

        Singleton.Instance.levelCardWasChosen = false;
        ballIsAvailable = false;
        for (int i = 0; i < ballGos.Count; i++)
        {
            GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.outwardsParticlePool);
            temp.transform.position = ballGos[i].transform.position;
            temp.SetActive(true);

            AddScore(ballScoreMultiplier);
            ballScoreMultiplier += 250;
            ballScripts[i].currentPhasingsLeft = 0;
            ballGos[i].SetActive(false);
            ballGos[i].transform.position = placeToHidePooled;
        }

        ballScoreMultiplier = 250;

        for (int i = 0; i < pum.currentActiveBubbles.Count; i++)
        {
            GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.bubbleExplosionParticlesPool, bubbleExplosionParticle);
            temp.transform.position = pum.currentActiveBubbles[i].transform.position; //FIXED BUG: Breakable interaction was adding a Null value to the list (Make sure returned bubble index exists on bubble's pool
            temp.SetActive(true);
            AddScore(bubbleScoreMultiplier);
            pum.currentActiveBubbles[i].SetActive(false);
        }

        bubbleScoreMultiplier = 100;

        pum.currentActiveBubbles.Clear();

        RemoveAllBallsButMainBall();
        ResetPowerUpsState(false);

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            if (currentBlocksBreakableInteraction[i].isDestroyed)
                blocksDestroyedThisLevel++;
        }
        if (!isClimbMode)
        {
            for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
            {
                currentUnbreakableBlocksScript[i].UnbuildUnbreakable();
            }
            for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
            {
                currentBlocksBreakableInteraction[i].PlayDeathAnim(false);
            }
        }

        //Unlockables
        if (currentLevel == 14)
        {
            UnlockableManager.instance.CompletedUnlockable(0, "Floor 15 beated");
        }
        else if (currentLevel == 29)
        {
            UnlockableManager.instance.CompletedUnlockable(1, "Floor 30 beated");
        }

        if (isClassicMode)
        {
            AddMoney(currentBlocksBreakableInteraction.Count / 2);
        }
        else if (isClimbMode)
        {
            DestroyCurrentBlocks();
            for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
            {
                if (currentBlocksBreakableInteraction[i] != null)
                {
                    AddScore(200);
                }
            }
        }

        useSavedMoneyInstead = true;

        middleShowScoreAnimation = score;
        score += scoreToAdd;

        scoreToAddAnimation = scoreToAdd;
        scoreToAdd = 0;
        uic.SetShowScoreVariablesForAnimation();
        uic.UpdatePauseScore();

        uic.ActivateScoreShowPanel(true);
        StartCoroutine(ShowAddingScore());

        AchievementDataManager.Instance.blocksDestroyed += blocksDestroyedThisLevel;
        blocksDestroyedThisLevel = 0;

        AchievementManager.instance.VerifyAchievementProgress(0, "DESTROYER", AchievementDataManager.Instance.blocksDestroyed);
        AchievementManager.instance.VerifyAchievementProgress(1, "ANNIHILATOR", AchievementDataManager.Instance.blocksDestroyed);
        AchievementManager.instance.VerifyAchievementProgress(2, "CONQUEROR", AchievementDataManager.Instance.blocksDestroyed);
    }

    private void ClearBlockLists()
    {
        currentBlocks.Clear();
        currentBlocksBreakableInteraction.Clear();
        currentRowParents.Clear();
        currentUnbreakableBlocksScript.Clear();
    }

    public void DestroyCurrentBlocks()
    {
        GetCurrentLevelBlocks();
        moneyToSave = currentMoney;
        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            moneyToSave += 1;
            currentBlocksBreakableInteraction[i].PlayDeathAnim(true);
            //currentBlocksBreakableInteraction[i].SetRandomDestroyTimer(0, 1f);
            //currentBlocksBreakableInteraction[i].fallDestroy = true;
        }

        for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
        {
            currentUnbreakableBlocksScript[i].UnbuildUnbreakable();
        }

        checkForDestroyedBlocks = true;
    }

    public void GetCurrentLevelBlocks()
    {
        List<BreakableInteraction> temp = new List<BreakableInteraction>();

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            if (!currentBlocksBreakableInteraction[i].isDestroyed)
                temp.Add(currentBlocksBreakableInteraction[i]);
        }
        //print(temp.Count);
        currentBlocksBreakableInteraction.Clear();
        currentBlocksBreakableInteraction.AddRange(temp);
        temp.Clear();

        //currentBlocks.AddRange(GameObject.FindGameObjectsWithTag("Breakable")); //Change it to only "currentLevel" Breakables when pooling

        //for (int i = 0; i < currentBlocks.Count; i++)
        //{
        //    if (currentBlocks[i].GetComponent<BreakableInteraction>() == null)
        //    {
        //        print("Error getting the BreakableInteraction Component from a block");
        //        continue;
        //    }
        //
        //    currentBlocksBreakableInteraction.Add(currentBlocks[i].GetComponent<BreakableInteraction>());
        //}
    }

    public bool AllBlocksDestroyed()
    {
        if (currentBlocksBreakableInteraction.Count == 0) return true;

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            if (!currentBlocksBreakableInteraction[i].isDestroyed)
            {
                if (currentBlocksBreakableInteraction[i].isSpecial || currentBlocksBreakableInteraction[i].isSpecialCard)
                {
                    continue;
                }
                return false;
            }
            else if (i == currentBlocksBreakableInteraction.Count - 1)
            {
                return true;
            }
        }

        return false;
    }

    public void BuildNextLevel()
    {
        if (!Singleton.Instance.saveWasLoaded_Temp) mainBall.BallBackToPaddle();
        else LoadSingletonBalls();

        ClearBlockLists();
        GetBreakableInteractionsPerRow();
        GetBlocksParentPerRow();
        if (Singleton.Instance.saveWasLoaded_Temp) HideDestroyedBlocksOnLoad();

        Singleton.Instance.runMoney = moneyToSave;

        if (currentLevel > 0)
        {
            if (currentLevel % chooseCardEveryXLevel != 0)
                SaveSingletonVariables();
            else
            {
                if (!Singleton.Instance.levelCardWasChosen && !Singleton.Instance.leftBeforeChoosing)
                {
                    if (pum.FreeCardSlotAvailable() || pum.powerUpsAreRandom)
                    {
                        if (Singleton.Instance.saveWasLoaded_Temp) pum.loadTheSameChoice = true;
                        pum.isChoiceAtLevelStart = true;
                        StartCoroutine(pum.ShowCardChooseSelection());
                    }
                    else
                    {
                        Singleton.Instance.levelCardWasChosen = true;
                    }
                }

                SaveSingletonVariables();
            }

            if (currentLevel % increaseSpeedEveryXLevel == 0)
            {
                if (!Singleton.Instance.saveWasLoaded_Temp) startBallVelocity += 1;
            }
        }

        print("All Blocks are destroyed, Changing levels...");
        gameStarted = false;
        speedIncreaseTimer = 0;
        t = 0;
        bgScrolling.scrollSpeed = 8f;

        levels[currentLevel].transform.position = levelChangePos;
        levels[currentLevel].SetActive(true);

        uic.UpdateFloor(currentLevel);

        checkForDestroyedBlocks = false;

        if (Singleton.Instance.saveWasLoaded_Temp)
            currentBallVelocity = Singleton.Instance.currentBallVelocity;
        else
            currentBallVelocity = startBallVelocity;

        //if (Singleton.Instance.saveWasLoaded_Temp)
        //{
        //    for (int i = 0; i < ballScripts.Count; i++)
        //    {
        //        if (ballScripts[i].hasLaunched)
        //        {
        //            gameStarted = true;
        //            Singleton.Instance.saveWasLoaded_Temp = false;
        //            break;
        //        }
        //    }
        //
        //    if (Singleton.Instance.chooseDirectionPowerUpActive) pum.cd.StartChooseDirectionPowerUp();
        //}

        if (GetNumberOfBlocksInCurrentLevel() <= 100)
        {
            StartCoroutine(StartStageBuildingPerBlock());
        }
        else
        {
            StartCoroutine(StartStageBuildingPerRow());
        }
    }

    public int GetNumberOfBlocksInCurrentLevel()
    {
        return levels[currentLevel].GetComponentsInChildren<BreakableInteraction>().Length;
    }

    public void GetBreakableInteractionsPerRow()
    {
        for (int i = 0; i < levels[currentLevel].transform.childCount; i++)
        {
            Transform tempT = levels[currentLevel].transform.GetChild(i);

            //Unbreakable Blocks are already getting grabbed on GetBlocksParentsPerRow
            //if (tempT.CompareTag("Unbreakable"))
            //{
            //    currentUnbreakableBlocksScript.Add(tempT.GetComponent<UnbreakableBehav>());
            //    continue;
            //}

            for (int j = 0; j < levels[currentLevel].transform.GetChild(i).childCount; j++)
            {
                if (tempT.GetChild(j).GetComponent<BreakableInteraction>() == null) { print("WTF NO BreakableInteraction FOUND"); break; }
                currentBlocks.Add(tempT.GetChild(j).gameObject);
                currentBlocksBreakableInteraction.Add(tempT.GetChild(j).GetComponent<BreakableInteraction>());
            }

        }
    }

    public IEnumerator StartStageBuildingPerBlock()
    {
        //ClearBlockLists();
        //GetBreakableInteractionsPerRow();
        maxInterval = buildingInterval;
        isBuildingLevel = true;
        BreakableInteraction tempBI = null;

        for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
        {
            //currentUnbreakableBlocksScript[i].gameObject.SetActive(false);
            currentUnbreakableBlocksScript[i].transform.Translate(Vector3.up * -9);
            currentUnbreakableBlocksScript[i].gameObject.SetActive(true);
            //print("BuildUnbreakable");
            //currentUnbreakableBlocksScript[i].BuildUnbreakable();
        }

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            if (!currentBlocksBreakableInteraction[i].isDestroyed) tempBI = currentBlocksBreakableInteraction[i];
        }

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            //if (i == currentBlocksBreakableInteraction.Count - 1)
            //{
            //    if (Singleton.Instance.saveWasLoaded_Temp) 
            //    {
            //        if (currentBlocksBreakableInteraction[i].isDestroyed)
            //        {
            //            EndOfLevelBuilding();
            //        }
            //        else
            //            currentBlocksBreakableInteraction[i].isTheLastBlock = true; 
            //    }
            //
            //}
            if (currentBlocksBreakableInteraction[i] == tempBI) currentBlocksBreakableInteraction[i].isTheLastBlock = true;
            currentBlocksBreakableInteraction[i].isFallingToPlace = true;
            if (!currentBlocksBreakableInteraction[i].isDestroyed) yield return new WaitForSeconds(buildingInterval);
        }

        for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
        {
            currentUnbreakableBlocksScript[i].BuildUnbreakable();
        }

        if (Singleton.Instance.saveWasLoaded_Temp)
        {
            for (int i = 0; i < ballScripts.Count; i++)
            {
                if (ballScripts[i].hasLaunched)
                {
                    gameStarted = true;
                    Singleton.Instance.saveWasLoaded_Temp = false;
                    break;
                }
            }

            if (Singleton.Instance.chooseDirectionPowerUpActive) { pum.cd.StartChooseDirectionPowerUp(); gameStarted = true; }
            if (Singleton.Instance.hasShootingPowerUp) spu.enabled = true;
        }
        else
        {
            EndOfLevelBuilding();
        }
    }

    public void EndOfLevelBuilding()
    {
        if (pum.chooseDirectionIsActive)
            ballIsAvailable = false;
        else
            ballIsAvailable = true;
        if (Singleton.Instance.chooseDirectionPowerUpStillShooting)
        {
            Vector2 tempV2 = new Vector2(Singleton.Instance.lastDirectionX, Singleton.Instance.lastDirectionY);
            StartCoroutine(pum.cd.ShootBallsWithInterval(tempV2, currentBallVelocity, pum.cd.launchInterval));
        }
        if (loadedBullets.Count > 0)
        {
            ActivateLoadedBullets();
        }
        mp.canMove = true;
        //mainBall.hasLaunched = false;
        buildingInterval = maxInterval;
        isBuildingLevel = false;
        bgScrolling.scrollSpeed = 0.5f;

        for (int i = 0; i < pum.currentActiveBubbles.Count; i++)
        {
            pum.currentActiveBubbles[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void GetBlocksParentPerRow()
    {
        for (int i = 0; i < levels[currentLevel].transform.childCount; i++)
        {
            Transform temp = levels[currentLevel].transform.GetChild(i);

            if (temp.CompareTag("Unbreakable"))
            {
                currentUnbreakableBlocksScript.Add(temp.GetComponent<UnbreakableBehav>());
                temp.gameObject.SetActive(false);
            }
            else currentRowParents.Add(temp.gameObject);

        }
    }

    public IEnumerator StartStageBuildingPerRow()
    {
        //ClearBlockLists();
        //GetBlocksParentPerRow();
        //GetBreakableInteractionsPerRow();
        maxInterval = buildingInterval;
        isBuildingLevel = true;
        BreakableInteraction tempBI;

        for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
        {
            //currentUnbreakableBlocksScript[i].gameObject.SetActive(false);
            currentUnbreakableBlocksScript[i].transform.Translate(Vector3.up * -9);
            currentUnbreakableBlocksScript[i].gameObject.SetActive(true);
            //print("BuildUnbreakable");
            //currentUnbreakableBlocksScript[i].BuildUnbreakable();
        }

        for (int i = 0; i < currentRowParents.Count; i++)
        {
            for (int j = 0; j < currentRowParents[i].transform.childCount; j++)
            {
                tempBI = currentRowParents[i].transform.GetChild(j).GetComponent<BreakableInteraction>();
                if (i == currentRowParents.Count - 1 && j == currentRowParents[i].transform.childCount - 1)
                {
                    if (tempBI.isDestroyed)
                    {
                        EndOfLevelBuilding();
                    }
                    else
                    {
                        if (Singleton.Instance.saveWasLoaded_Temp) tempBI.isTheLastBlock = true;
                    }
                }
                tempBI.isFallingToPlace = true;
            }
            yield return new WaitForSeconds(buildingInterval);
        }

        for (int i = 0; i < currentUnbreakableBlocksScript.Count; i++)
        {
            currentUnbreakableBlocksScript[i].BuildUnbreakable();
        }

        if (Singleton.Instance.saveWasLoaded_Temp)
        {
            for (int i = 0; i < ballScripts.Count; i++)
            {
                if (ballScripts[i].hasLaunched)
                {
                    gameStarted = true;
                    Singleton.Instance.saveWasLoaded_Temp = false;
                    break;
                }
            }

            if (Singleton.Instance.chooseDirectionPowerUpActive) { pum.cd.StartChooseDirectionPowerUp(); gameStarted = true; }
        }
        else
        {
            EndOfLevelBuilding();
        }
    }

    //Ball methods start here
    private void ClearBallLists()
    {
        ballGos.Clear();
        ballScripts.Clear();
        ballRb2ds.Clear();
    }

    public void AddBallToLists(GameObject ball, Rigidbody2D ballRb2d, Ball ballScript)
    {
        ballGos.Add(ball);
        ballRb2ds.Add(ballRb2d);
        ballScripts.Add(ballScript);
    }

    public void RemoveBallFromLists(GameObject ball, Rigidbody2D ballRb2d, Ball ballScript)
    {
        ballGos.Remove(ball);
        ballRb2ds.Remove(ballRb2d);
        ballScripts.Remove(ballScript);
    }

    public bool ItWasTheLastBall()
    {
        if (GetCurrentAmountofBalls() == 1)
            return true;
        else
            return false;
    }

    public int GetCurrentAmountofBalls()
    {
        return ballGos.Count;
    }

    public void ChangeMainBall(Ball ball)
    {
        if (ball == mainBall) return;

        ball.paddle = mainBall.paddle; //can use GameManager's "pallet" too
        ball.startLocalPos = mainBall.startLocalPos;

        mainBall = ball;
    }

    public void RemoveAllBallsButMainBall()
    {
        if (ballGos.Count > 1)
        {
            for (int i = 0; i < ballGos.Count; i++)
            {
                if (ballGos[i].transform.parent != null) ballGos[i].transform.parent = null;
            }

            ClearBallLists();
            AddBallToLists(mainBall.gameObject, mainBall.GetComponent<Rigidbody2D>(), mainBall);
        }
    }

    public void StartBallCreationParticles()
    {
        inwardsParticle.transform.parent = paddleGO.transform;
        inwardsParticle.transform.localPosition = mainBall.startLocalPos;
        inwardsParticle.SetActive(true);
    }

    public void ShowInwardsParticles(Vector2 pos)
    {
        inwardsParticle.transform.parent = null;
        inwardsParticle.transform.localPosition = pos;
        inwardsParticle.SetActive(true);
    }

    public void ResetPowerUpsState(bool ballDied)
    {
        if (ballDied) ballIsAvailable = true;
        pum.EndChooseDirection();
        mainBall.currentPhasingsLeft = 0; //other variables reset on balls Update()
        mainBall.scoreMultiplier = 0;
        mainBall.timesBouncedOffPlayer = 0;
        pum.magnetIsActive = false;
        mp.ChangePaddleSize(1);
        mp.magnetChildGO.SetActive(false);
        pum.ActivateChooseDirectionIndicators(false);
        spu.EndShootPowerUp();
        currentBallVelocity = startBallVelocity;
        speedIncreaseTimer = 0;
        pum.ActivateShieldPowerUp(false);
    }

    public Color GetRandomColor()
    {
        return new Color(r: Random.Range(0f, 1f),
                         g: Random.Range(0f, 1f),
                         b: Random.Range(0f, 1f));
    }

    public void ResetSavedVariables()
    {
        Singleton.Instance.ResetVariables();
        AchievementDataManager.Instance.ResetVariables();

        Singleton.Instance.Save((isClassicMode) ? 1 : 0);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        uic.UpdateMoney(currentMoney);
    }

    public void HideDestroyedBlocksOnLoad()
    {
        if (Singleton.Instance.currentLevelEnabledBlocks.Count <= 0) return;
        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            if (!Singleton.Instance.currentLevelEnabledBlocks[i])
            {
                currentBlocksBreakableInteraction[i].isDestroyed = true;
                currentBlocksBreakableInteraction[i].gameObject.SetActive(false);
            }
        }
    }

    public void SaveSingletonVariables()
    {
        print("Saving Singleton Variables...");
        Singleton.Instance.hasSavedData = true;
        Singleton.Instance.runMoney = currentMoney;
        if (useSavedMoneyInstead) { Singleton.Instance.runMoney = moneyToSave; useSavedMoneyInstead = false; }
        Singleton.Instance.currentLevel = currentLevel;
        Singleton.Instance.currentHealth = uic.GetCurrentHealth();
        Singleton.Instance.currentScore = score;
        Singleton.Instance.currentScoreToAdd = scoreToAdd;
        Singleton.Instance.currentBallVelocity = currentBallVelocity;
        Singleton.Instance.currentStartBallVelocity = startBallVelocity;
        Singleton.Instance.runTimer = uic.GetCurrentTimer();

        for (int i = 0; i < 3; i++)
        {
            Singleton.Instance.currentCards[i] = pum.cardSlotIndex[i];
            Singleton.Instance.currentCardsGolden[i] = pum.slotIndexIsGold[i];
            if (pum.randomCardChooseIntIndexes.Count > 0) Singleton.Instance.currentCardChoice[i] = pum.randomCardChooseIntIndexes[i];
        }

        Singleton.Instance.currentLevelEnabledBlocks.Clear();

        for (int i = 0; i < currentBlocksBreakableInteraction.Count; i++)
        {
            Singleton.Instance.currentLevelEnabledBlocks.Add(true);
            if (currentBlocksBreakableInteraction[i].isDestroyed) Singleton.Instance.currentLevelEnabledBlocks[i] = false;
        }

        //PowerUp Saving 
        Singleton.Instance.ballPositionX.Clear();
        Singleton.Instance.ballPositionY.Clear();
        Singleton.Instance.ballVelocityX.Clear();
        Singleton.Instance.ballVelocityY.Clear();
        Singleton.Instance.ballHasLaunched.Clear();
        Singleton.Instance.ballPhasingLeft.Clear();

        Singleton.Instance.amountOfBalls = ballGos.Count;
        print("Amount of balls is: " + ballGos.Count);
        for (int i = 0; i < ballGos.Count; i++)
        {
            Singleton.Instance.ballPositionX.Add(ballGos[i].transform.localPosition.x);
            Singleton.Instance.ballPositionY.Add(ballGos[i].transform.localPosition.y);
            if (!Singleton.Instance.saveWasLoaded_Temp)
            {
                Singleton.Instance.ballVelocityX.Add(ballRb2ds[i].velocity.x);
                Singleton.Instance.ballVelocityY.Add(ballRb2ds[i].velocity.y);
            }
            else
            {
                Singleton.Instance.ballVelocityX.Add(ballScripts[i].velocityToLoad.x);
                Singleton.Instance.ballVelocityY.Add(ballScripts[i].velocityToLoad.y);
            }

            Singleton.Instance.ballHasLaunched.Add(ballScripts[i].hasLaunched);

            Singleton.Instance.ballPhasingLeft.Add(ballScripts[i].currentPhasingsLeft);
        }

        Singleton.Instance.chooseDirectionPowerUpActive = pum.chooseDirectionIsActive;
        Singleton.Instance.chooseDirectionPowerUpStillShooting = pum.cd.isShooting;
        Singleton.Instance.hasMagnetPowerUp = pum.magnetIsActive;
        Singleton.Instance.paddleSize = pum.currentPaddleSize;
        Singleton.Instance.hasShieldPowerUp = pum.bottomWall.activeInHierarchy;

        Singleton.Instance.hasShootingPowerUp = spu.isShooting;
        Singleton.Instance.fireRate = spu.fireRate;
        Singleton.Instance.fireCounter = spu.fireCounter;
        Singleton.Instance.shootCounter = spu.shootCounter;

        Singleton.Instance.bulletPosX.Clear();
        Singleton.Instance.bulletPosY.Clear();

        for (int i = 0; i < pum.shotBulletsTransform.Count; i++)
        {
            if (pum.shotBulletsTransform[i].gameObject.activeInHierarchy)
            {
                Singleton.Instance.bulletPosX.Add(pum.shotBulletsTransform[i].position.x);
                Singleton.Instance.bulletPosY.Add(pum.shotBulletsTransform[i].position.y);
            }
        }

        Singleton.Instance.bubbleIndex.Clear();
        Singleton.Instance.bubblePosX.Clear();
        Singleton.Instance.bubblePosY.Clear();

        for (int i = 0; i < pum.currentActiveBubbles.Count; i++)
        {
            Singleton.Instance.bubblePosX.Add(pum.currentActiveBubbles[i].transform.position.x);
            Singleton.Instance.bubblePosY.Add(pum.currentActiveBubbles[i].transform.position.y);
            Singleton.Instance.bubbleIndex.Add(pum.currentActiveBubbles[i].GetComponent<PowerUpIndex>().index);
        }

        Singleton.Instance.totalGoldenCardsObtained = totalGoldenCardsObtained;
        Singleton.Instance.totalCardsActivated = totalCardsActivated;
        Singleton.Instance.totalBubblesPicked = totalBubblesPicked;
        Singleton.Instance.globalRunTimer = globalRunTimer;

        Singleton.Instance.hasDiedThisRun = hasDiedThisRun;

        Singleton.Instance.Save((isClassicMode) ? 1 : 0);
        AchievementDataManager.Instance.Save();
    }

    public void LoadSingletonState()
    {
        if (Singleton.Instance.saveWasLoaded_Temp)
        {
            currentMoney = Singleton.Instance.runMoney;
            moneyToSave = currentMoney;
            currentLevel = Singleton.Instance.currentLevel;
            currentBallVelocity = Singleton.Instance.currentBallVelocity;
            startBallVelocity = Singleton.Instance.currentStartBallVelocity;
            uic.SetCurrentHealth(Singleton.Instance.currentHealth);
            uic.SetCurrentTimer(Singleton.Instance.runTimer);
            score = Singleton.Instance.currentScore;
            scoreToAdd = Singleton.Instance.currentScoreToAdd;
            uic.UpdatePauseScore();

            for (int i = 0; i < 3; i++)
            {
                pum.cardSlotIndex[i] = Singleton.Instance.currentCards[i];
                pum.slotIndexIsGold[i] = Singleton.Instance.currentCardsGolden[i];
                pum.randomCardChooseIntIndexes.Add(Singleton.Instance.currentCardChoice[i]);
            }

            pum.LoadOwnedCards();
        }

        //PowerUps To load //some of them are in BuildNextLevel() to make sure order is correct
        if (Singleton.Instance.hasMagnetPowerUp) pum.StartMagnetPowerUp();
        if (Singleton.Instance.paddleSize != 1) { mp.ChangePaddleSize(Singleton.Instance.paddleSize); }
        if (Singleton.Instance.hasShieldPowerUp) pum.ActivateShieldPowerUp(true);
        if (Singleton.Instance.hasShootingPowerUp) { LoadShootingPowerUpProperties(); }
        if (Singleton.Instance.bulletPosX.Count > 0) LoadBulletsPositions();

        for (int i = 0; i < Singleton.Instance.bubbleIndex.Count; i++)
        {
            GameObject tempBubble = PoolManager.current.GetPooledBubbleBasedOnIndex(Singleton.Instance.bubbleIndex[i]);
            tempBubble.transform.position = new Vector2(Singleton.Instance.bubblePosX[i], Singleton.Instance.bubblePosY[i]);
            pum.currentActiveBubbles.Add(tempBubble);
            tempBubble.SetActive(true);
            tempBubble.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        totalGoldenCardsObtained = Singleton.Instance.totalGoldenCardsObtained;
        totalCardsActivated = Singleton.Instance.totalCardsActivated;
        totalBubblesPicked = Singleton.Instance.totalBubblesPicked;
        globalRunTimer = Singleton.Instance.globalRunTimer;

        hasDiedThisRun = Singleton.Instance.hasDiedThisRun;
    }

    public void LoadSingletonBalls()
    {
        if (Singleton.Instance.hasSavedData)
        {
            mainBall.hasLaunched = Singleton.Instance.ballHasLaunched[0];
            if (mainBall.hasLaunched)
            {
                mainBall.transform.parent = null;
                mainBall.transform.position = new Vector3(Singleton.Instance.ballPositionX[0], Singleton.Instance.ballPositionY[0]);
                mainBall.ballIsLoadingVelocity = true;
                mainBall.velocityToLoad = new Vector2(Singleton.Instance.ballVelocityX[0], Singleton.Instance.ballVelocityY[0]);
                mainBall.cc2d.enabled = false;
            }
            else
            {
                if (Singleton.Instance.chooseDirectionPowerUpActive)
                {
                    mainBall.transform.parent = null;
                    mainBall.transform.position = new Vector3(Singleton.Instance.ballPositionX[0], Singleton.Instance.ballPositionY[0]);
                }
                else
                {
                    mainBall.transform.parent = paddleGO.transform;
                    mainBall.transform.localPosition = new Vector3(Singleton.Instance.ballPositionX[0], Singleton.Instance.ballPositionY[0]);
                }
            }

            mainBall.gameObject.SetActive(true);
            pum.LoadBallPhasingPowerUp(mainBall, Singleton.Instance.ballPhasingLeft[0]);


            GameObject tempBallGO;
            Rigidbody2D tempBallRb2d;
            Ball tempBallScript;

            for (int i = 1; i < Singleton.Instance.amountOfBalls; i++) // i starts at 1 cause mainball is already instantiated
            {
                tempBallGO = PoolManager.current.GetPooledGameObject(PoolManager.current.ballPool);
                tempBallScript = tempBallGO.GetComponent<Ball>();
                tempBallRb2d = tempBallGO.GetComponent<Rigidbody2D>();
                AddBallToLists(tempBallGO, tempBallRb2d, tempBallScript);

                tempBallScript.hasLaunched = Singleton.Instance.ballHasLaunched[i];
                if (tempBallScript.hasLaunched)
                {
                    tempBallGO.transform.parent = null;
                    tempBallGO.transform.position = new Vector3(Singleton.Instance.ballPositionX[i], Singleton.Instance.ballPositionY[i]);
                    tempBallScript.ballIsLoadingVelocity = true;
                    tempBallScript.velocityToLoad = new Vector2(Singleton.Instance.ballVelocityX[i], Singleton.Instance.ballVelocityY[i]);
                    tempBallScript.cc2d.enabled = false;
                }
                else
                {
                    if (Singleton.Instance.chooseDirectionPowerUpActive)
                    {
                        tempBallGO.transform.parent = null;
                        tempBallGO.transform.position = new Vector3(Singleton.Instance.ballPositionX[i], Singleton.Instance.ballPositionY[i]);
                    }
                    else
                    {
                        if (Singleton.Instance.chooseDirectionPowerUpStillShooting)
                        {
                            if (Singleton.Instance.ballVelocityX[i] == 0 && Singleton.Instance.ballVelocityY[i] == 0)
                            {
                                pum.cd.AddLoadedBallsToList(tempBallRb2d, tempBallScript);

                                tempBallScript.ballIsGonnaBeShot = true;
                                tempBallGO.transform.parent = null;
                                tempBallGO.transform.position = new Vector3(Singleton.Instance.ballPositionX[i], Singleton.Instance.ballPositionY[i]);
                                tempBallGO.SetActive(true);
                                pum.LoadBallPhasingPowerUp(tempBallScript, Singleton.Instance.ballPhasingLeft[i]);
                                continue;
                            }
                        }

                        tempBallGO.transform.parent = paddleGO.transform;
                        tempBallGO.transform.localPosition = new Vector3(Singleton.Instance.ballPositionX[i], Singleton.Instance.ballPositionY[i]);
                    }
                }

                tempBallGO.SetActive(true);
                pum.LoadBallPhasingPowerUp(tempBallScript, Singleton.Instance.ballPhasingLeft[i]);
            }
        }
    }

    public void LoadShootingPowerUpProperties()
    {
        spu.enabled = false;
        spu.fireRate = Singleton.Instance.fireRate;
        spu.fireCounter = Singleton.Instance.fireCounter;
        spu.shootCounter = Singleton.Instance.shootCounter;

        spu.isShooting = Singleton.Instance.hasShootingPowerUp;
        spu.ActivatePowerUpGOs(true);
    }

    public void LoadBulletsPositions()
    {
        GameObject tempLaser;
        LaserBulletBehav tempBB;

        for (int i = 0; i < Singleton.Instance.bulletPosX.Count; i++)
        {
            tempLaser = PoolManager.current.GetPooledGameObject(PoolManager.current.laserPool, PoolManager.current.laserPrefab);
            tempBB = tempLaser.GetComponent<LaserBulletBehav>();
            tempBB.enabled = false;

            tempLaser.SetActive(true);
            tempLaser.transform.position = new Vector2(Singleton.Instance.bulletPosX[i], Singleton.Instance.bulletPosY[i]);

            pum.shotBulletsTransform.Add(tempLaser.transform);
            loadedBullets.Add(tempBB);
        }
    }

    public void ActivateLoadedBullets()
    {
        for (int i = 0; i < loadedBullets.Count; i++)
        {
            loadedBullets[i].enabled = true;
        }

        loadedBullets.Clear();
    }

    public void StartShootingPowerUpAfterLoad()
    {
        spu.enabled = true;
    }

    public void BackToMenuAtGameOver()
    {
        EndOfGameDataManager();
        SceneManager.LoadScene(0);
    }

    public void EndOfGameDataManager()
    {
        Singleton.Instance.ResetVariables();
        Singleton.Instance.Save((isClassicMode) ? 1 : 0);

        MenuDataManager.Instance.currentMoney += currentMoney;
        MenuDataManager.Instance.Save();
    }

    public void StartEndOfGameAnimation()
    {
        if (!hasDiedThisRun)
        {
            AchievementManager.instance.VerifyAchievementProgress(23, "THE_PERFECT_RUN", 0);
        }

        uic.ShowPauseButton(false);
        gameStarted = false;
        backgroundMaterial.color = new Color(29f / 255, 29f / 255, 29f / 255);
        SetEndOfGameCoinMultiplier();

        scoreBeforeEndBonuses = score;
        score += HealthBonusScore() + GoldCardBonusScore() + ActivatedCardBonusScore() + BubblesPickedBonusScore() + TimerBonusScore() + CompletedRunBonusScore();

        if (isClassicMode)
        {
            if (score > MenuDataManager.Instance.highestClassicScore) MenuDataManager.Instance.highestClassicScore = score;
            if (CloudOnce.Cloud.IsSignedIn) CloudOnceServices.instance.SubmitScoreToClassicLeaderboard(score);
        }
        else
        {
            if (score > MenuDataManager.Instance.highestClimbScore) MenuDataManager.Instance.highestClimbScore = score;
            if (CloudOnce.Cloud.IsSignedIn) CloudOnceServices.instance.SubmitScoreToClimbLeaderboard(score);
        }

        scoreBonusesWereAdded = true;

        MenuDataManager.Instance.numberOfGamesPlayed++;

        AchievementManager.instance.VerifyAchievementProgress(17, "GETTING_STARTED", MenuDataManager.Instance.numberOfGamesPlayed);

        SetFinalScoreText();

        mp.gameObject.SetActive(false);
        mainBall.gameObject.SetActive(false);
        levels[currentLevel].SetActive(false);

        uic.endOfGamePanel.SetActive(true);

        int tempRan = Random.Range(0, 2);

        if (tempRan == 0)
            ersa.transitionIsAlpha = true;
        else
            ersa.transitionIsAlpha = false;

        ersa.readyToRunAnimation = true;

        Singleton.Instance.ResetVariables();
        Singleton.Instance.Save((isClassicMode) ? 1 : 0);

        if (runWasCompleted)
        {
            UnlockableManager.instance.CompletedUnlockable(2, "Completed run for the 1st time");
            UnlockedPaddleColorBasedOnIndex();
        }
    }

    public void SetFinalScoreText()
    {
        if (scoreBeforeEndBonuses < 1000000)
        {
            ersa.finalScoreText.text = "0" + scoreBeforeEndBonuses.ToString();
        }
        else
        {
            ersa.finalScoreText.text = scoreBeforeEndBonuses.ToString();
        }

        ersa.finalScoreText.gameObject.SetActive(true);
    }

    public void AddScore(int addAmount)
    {
        scoreToAdd += addAmount;
    }

    public void ReduceScore(int reduceAmount)
    {
        if (score - reduceAmount < 0) { score = 0; return; }

        score -= reduceAmount;

        uic.UpdatePauseScore();
    }

    IEnumerator ShowAddingScore()
    {
        print("STARTING");
        scoreAnimationIsPlaying = true;
        float t = 0;
        float a = 0;

        int tempScoreToAdd = scoreToAddAnimation;
        int tempMiddleScore = middleShowScoreAnimation;
        uic.ChangeShowScoreCGAlpha(0);
        uic.ActivateScoreAnimation();
        print("Started");
        while (t <= 1)
        {
            uic.ChangeShowScoreCGAlpha(a);
            scoreToAddAnimation = (int)Mathf.Lerp(tempScoreToAdd, 0, t);
            middleShowScoreAnimation = (int)Mathf.Lerp(tempMiddleScore, score, t);

            t += Time.deltaTime;
            a += Time.deltaTime * 1.8f;

            uic.SetShowScoreVariablesForAnimation();

            yield return null;
        }
        print("Finished");
        uic.IdleScoreAnimation();
        scoreToAddAnimation = 0;
        middleShowScoreAnimation = score;
        uic.SetShowScoreVariablesForAnimation();

        if (gameEnded)
        {
            if (currentLevel >= 49)
            {
                runWasCompleted = true;
            }
            StartEndOfGameAnimation();
            uic.ActivateScoreShowPanel(false);
            scoreAnimationIsPlaying = false;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        while (t >= 0)
        {
            t -= Time.deltaTime;
            uic.ChangeShowScoreCGAlpha(t);

            yield return null;
        }

        uic.ChangeShowScoreCGAlpha(1f);
        uic.ActivateScoreShowPanel(false);
        scoreAnimationIsPlaying = false;
    }

    public void SetLaunchBallTimer()
    {
        currentLaunchBallTimer = 0;
        startLaunchBallTimer = true;
    }

    public bool IsGamePausedOutsidePauseMenu()
    {
        if (isChoosingPowerUp) return true;
        else return false;
    }

    public int GetCurrentHealth()
    {
        return uic.GetCurrentHealth();
    }

    //Transforming Bonuses into Score
    public int HealthBonusScore()
    {
        int tempBonus = 0;

        for (int i = 1; i <= uic.GetCurrentHealth(); i++)
        {
            tempBonus += 7500;
        }
        print("Health Bonus was: " + tempBonus + " for " + uic.GetCurrentHealth() + " health");

        return tempBonus;
    }

    public int GoldCardBonusScore()
    {
        int tempBonus = 0;

        if (totalGoldenCardsObtained > 0)
        {
            for (int i = 0; i < totalGoldenCardsObtained; i++)
            {
                tempBonus += 11111;
            }
        }
        //else
        //{
        //    tempBonus += 90000;
        //}

        print("Gold Cards Bonus was: " + tempBonus + " for " + totalGoldenCardsObtained + " gold cards obtained");

        return tempBonus;
    }

    public int ActivatedCardBonusScore()
    {
        int tempBonus = 0;

        if (totalCardsActivated > 0)
        {
            for (int i = 0; i < totalCardsActivated; i++)
            {
                tempBonus += 1854;
            }
        }
        //else
        //{
        //    tempBonus = 150000;
        //}

        print("Activated Cards Bonus was: " + tempBonus + " for " + totalCardsActivated + " cards activated");

        return tempBonus;
    }

    public int BubblesPickedBonusScore()
    {
        int tempBonus = 0;

        if (totalBubblesPicked > 0)
        {
            for (int i = 0; i < totalBubblesPicked; i++)
            {
                tempBonus += 756;
            }
        }
        //else
        //{
        //    tempBonus += 250000;
        //}

        print("Bubbles Picked Bonus was: " + tempBonus + " for " + totalBubblesPicked + " bubbles picked");

        return tempBonus;
    }

    public int TimerBonusScore()
    {

        return 1000;
    }

    public int CompletedRunBonusScore()
    {
        if (runWasCompleted)
        { print("CURRENT LEVEL WAS 50"); return 150000; }
        else
            return 0;
    }

    public string NewBestScore()
    {
        string tempText = string.Empty;

        if (isClassicMode)
        {
            if (score > MenuDataManager.Instance.highestClassicScore)
                tempText = "new best score!";
            else
                tempText = "final score";
        }
        else
        {
            if (score > MenuDataManager.Instance.highestClimbScore)
                tempText = "new best score!";
            else
                tempText = "final score";
        }

        return tempText;
    }

    public void SetEndOfGameCoinMultiplier()
    {
        if (currentLevel < 24)
            adBonusMultiplier = 1.2f;
        else if (currentLevel < 49)
            adBonusMultiplier = 1.5f;
        else if (currentLevel >= 49)
            adBonusMultiplier = 2f;
    }

    public void AddRandomLevelsToLevels()
    {
        if (Singleton.Instance.hasSavedData)
        {
            levels.Clear();
            BuildLevelsListBasedOnSingleton();
        }
        else
        {
            levels.Clear();

            int tempCount = 0;
            GameObject tempGO;

            ShuffleLevelList(level0);
            tempGO = Instantiate(level0[0].levelPrefab);
            tempGO.SetActive(false);
            AddLevelToSingleton(level0[0].levelID);
            levels.Add(tempGO);

            tempCount = 9;
            ShuffleLevelList(level01To10);
            for (int i = 0; i < tempCount; i++)
            {
                tempGO = Instantiate(level01To10[i].levelPrefab);
                tempGO.SetActive(false);
                AddLevelToSingleton(level01To10[i].levelID);
                levels.Add(tempGO);
            }

            tempCount = 9;
            ShuffleLevelList(level11To20);
            for (int i = 0; i < tempCount; i++)
            {
                tempGO = Instantiate(level11To20[i].levelPrefab);
                tempGO.SetActive(false);
                AddLevelToSingleton(level11To20[i].levelID);
                levels.Add(tempGO);
            }

            tempCount = 10;
            ShuffleLevelList(level21To30);
            for (int i = 0; i < tempCount; i++)
            {
                tempGO = Instantiate(level21To30[i].levelPrefab);
                tempGO.SetActive(false);
                AddLevelToSingleton(level21To30[i].levelID);
                levels.Add(tempGO);
            }

            ShuffleLevelList(bonusLevel);
            tempGO = Instantiate(bonusLevel[0].levelPrefab);
            tempGO.SetActive(false);
            AddLevelToSingleton(bonusLevel[0].levelID);
            levels.Add(tempGO);

            tempCount = 10;
            ShuffleLevelList(level31To40);
            for (int i = 0; i < tempCount; i++)
            {
                tempGO = Instantiate(level31To40[i].levelPrefab);
                tempGO.SetActive(false);
                AddLevelToSingleton(level31To40[i].levelID);
                levels.Add(tempGO);
            }

            tempCount = 10;
            ShuffleLevelList(level41To50);
            for (int i = 0; i < tempCount; i++)
            {
                tempGO = Instantiate(level41To50[i].levelPrefab);
                tempGO.SetActive(false);
                AddLevelToSingleton(level41To50[i].levelID);
                levels.Add(tempGO);
            }
        }
    }

    public void AddLevelToSingleton(int levelID)
    {
        Singleton.Instance.levelsOrder.Add(levelID);
    }

    public void BuildLevelsListBasedOnSingleton()
    {
        GameObject tempGO;

        for (int i = 0; i < Singleton.Instance.levelsOrder.Count; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < level0.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level0[j].levelID)
                    {
                        tempGO = Instantiate(level0[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i < 10)
            {
                for (int j = 0; j < level01To10.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level01To10[j].levelID)
                    {
                        tempGO = Instantiate(level01To10[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i < 19)
            {
                for (int j = 0; j < level11To20.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level11To20[j].levelID)
                    {
                        tempGO = Instantiate(level11To20[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i < 29)
            {

                for (int j = 0; j < level21To30.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level21To30[j].levelID)
                    {
                        tempGO = Instantiate(level21To30[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i == 29)
            {
                for (int j = 0; j < bonusLevel.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == bonusLevel[j].levelID)
                    {
                        tempGO = Instantiate(bonusLevel[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i < 40)
            {
                for (int j = 0; j < level31To40.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level31To40[j].levelID)
                    {
                        tempGO = Instantiate(level31To40[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
            else if (i < 50)
            {
                for (int j = 0; j < level41To50.Count; j++)
                {
                    if (Singleton.Instance.levelsOrder[i] == level41To50[j].levelID)
                    {
                        tempGO = Instantiate(level41To50[j].levelPrefab);
                        tempGO.SetActive(false);
                        levels.Add(tempGO);
                        break;
                    }
                }
            }
        }
    }

    public void InstantiateLevelsForClassicMode()
    {
        GameObject tempGo;

        for (int i = 0; i < classicLevels.Count; i++)
        {
            tempGo = Instantiate(classicLevels[i]);
            tempGo.SetActive(false);
            levels.Add(tempGo);
        }
    }

    public void ShuffleLevelList(List<LevelBehavID> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            LevelBehavID temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void UnlockedPaddleColorBasedOnIndex()
    {
        if (MenuDataManager.Instance.currentPaddleIndex == 11)
            UnlockableManager.instance.CompletedUnlockable(3, "Gold Paddle-Inator Unlocked");
        else if (MenuDataManager.Instance.currentPaddleIndex == 12)
            UnlockableManager.instance.CompletedUnlockable(4, "Gold TwinDragon Unlocked");
        else if (MenuDataManager.Instance.currentPaddleIndex == 13)
            UnlockableManager.instance.CompletedUnlockable(5, "Gold BL-CK Unlocked");
    }
}
