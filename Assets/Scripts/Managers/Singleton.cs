using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{

    private static Singleton instance;
    string climbPath;
    string classicPath;

    //Data to save
    public bool hasSavedData = false;
    public bool isRandomRun = false;
    public int runMoney = 0;
    public int currentLevel = 0;
    public int currentHealth = 2;
    public int currentScore = 0;
    public int currentScoreToAdd = 0;
    public float currentBallVelocity = 8;
    public float currentStartBallVelocity = 8;
    public float[] runTimer = new float[3];

    public int[] currentCards = new int[3];
    public bool[] currentCardsGolden = new bool[3];
    public int[] currentCardChoice = new int[3];
    public bool levelCardWasChosen = false; //This is for choosing at the beginning of the level
    public bool leftBeforeChoosing = false; //This is for choosing thanks to a power up/block    

    public bool isBeginningOfLevel = false;
    public List<int> levelsOrder = new List<int>();
    public List<bool> currentLevelEnabledBlocks = new List<bool>();

    //PowerUp data to save
    public int amountOfBalls = 0;
    public List<float> ballPositionX = new List<float>();
    public List<float> ballPositionY = new List<float>();
    public List<float> ballVelocityX = new List<float>();
    public List<float> ballVelocityY = new List<float>();
    public List<bool> ballHasLaunched = new List<bool>();
    public List<int> ballPhasingLeft = new List<int>();

    public bool chooseDirectionPowerUpActive = false;
    public bool chooseDirectionPowerUpStillShooting = false;
    public float lastDirectionX = 0;
    public float lastDirectionY = 0;
    public bool hasMagnetPowerUp = false;
    public int paddleSize = 1;
    public bool hasShieldPowerUp = false;
    public bool hasShootingPowerUp = false;
    public float fireRate = 0.5f;
    public float fireCounter = 0;
    public float shootCounter = 0;
    public List<float> bulletPosX = new List<float>();
    public List<float> bulletPosY = new List<float>();

    public List<float> bubblePosX = new List<float>();
    public List<float> bubblePosY = new List<float>();
    public List<int> bubbleIndex = new List<int>();

    public int totalGoldenCardsObtained = 0;
    public int totalCardsActivated = 0;
    public int totalBubblesPicked = 0;
    public float globalRunTimer = 0;

    public bool hasDiedThisRun = false;

    //public bool postprocessing_On = false;

    //Temp Singleton variables
    [Header("Temp Variables")]
    public bool saveWasLoaded_Temp = false;
    public bool isClassicMode_Temp = false;
    public bool isRandomized_Temp = false;

    public static Singleton Instance
    {
        get
        {
            if (instance == null)
            {
                //Debug.LogWarning("Creating Singleton");
                GameObject owner = new GameObject("Singleton");
                instance = owner.AddComponent<Singleton>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        climbPath = Application.persistentDataPath + "/ClimbInfo.dat";
        classicPath = Application.persistentDataPath + "/ClassicInfo.dat";

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            //Debug.LogWarning("Destroying Singleton");
            Destroy(this);
        }

        for (int i = 0; i < 3; i++)
        {
            currentCards[i] = 99;
            currentCardsGolden[i] = false;
            currentCardChoice[i] = 99;
        }
    }

    public void ResetTempVariables()
    {
        saveWasLoaded_Temp = false;
        isClassicMode_Temp = false;
        isRandomized_Temp = false;
    }

    public void CreateFilePath(int gameMode)
    {
        if (File.Exists((gameMode == 0) ? climbPath : classicPath))
            return;
        else
            Save(gameMode);
    }

    public void Save(int gameMode)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create((gameMode == 0) ? climbPath : classicPath);
        PlayerData data = new PlayerData();
        DataToSave(data);
        bf.Serialize(file, data);
        file.Close();

    }

    public void Load(int gameMode)
    {
        if (File.Exists((gameMode == 0) ? climbPath : classicPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open((gameMode == 0) ? climbPath : classicPath, FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            DataToLoad(data);
        }
    }

    void DataToSave(PlayerData data)
    {
        data.saved_hasSaveData = hasSavedData;
        data.saved_isRandomRun = isRandomRun;
        data.saved_totalMoney = runMoney;
        data.saved_currentLevel = currentLevel;
        data.saved_currentHealth = currentHealth;
        data.saved_currentScore = currentScore;
        data.saved_currentScoreToAdd = currentScoreToAdd;
        data.saved_currentBallVelocity = currentBallVelocity;
        data.saved_currentStartBallVelocity = currentStartBallVelocity;
        data.saved_runTimer = runTimer;
        data.saved_currentCards = currentCards;
        data.saved_currentCardsGolden = currentCardsGolden;
        data.saved_currentCardChoice = currentCardChoice;
        data.saved_levelCardWasChosen = levelCardWasChosen;
        data.saved_leftBeforeChoosing = leftBeforeChoosing;
        data.saved_isBeginningOfLevel = isBeginningOfLevel;
        data.saved_currentLevelEnabledBlocks = currentLevelEnabledBlocks.ToArray();
        data.saved_levelsOrder = levelsOrder.ToArray();
        data.saved_amountOfBalls = amountOfBalls;
        data.saved_ballPositionX = ballPositionX.ToArray();
        data.saved_ballPositionY = ballPositionY.ToArray();
        data.saved_ballVelocityX = ballVelocityX.ToArray();
        data.saved_ballVelocityY = ballVelocityY.ToArray();
        data.saved_ballHasLaunched = ballHasLaunched.ToArray();
        data.saved_ballPhasingLeft = ballPhasingLeft.ToArray();
        data.saved_chooseDirectionPowerUpActive = chooseDirectionPowerUpActive;
        data.saved_chooseDirectionPowerUpStillShooting = chooseDirectionPowerUpStillShooting;
        data.saved_lastDirectionX = lastDirectionX;
        data.saved_lastDirectionY = lastDirectionY;
        data.saved_hasMagnetPowerUp = hasMagnetPowerUp;
        data.saved_paddleSize = paddleSize;
        data.saved_hasShieldPowerUp = hasShieldPowerUp;
        data.saved_hasShootingPowerUp = hasShootingPowerUp;
        data.saved_fireRate = fireRate;
        data.saved_fireCounter = fireCounter;
        data.saved_shootCounter = shootCounter;
        data.saved_bulletPosX = bulletPosX.ToArray();
        data.saved_bulletPosY = bulletPosY.ToArray();
        data.saved_bubbleIndex = bubbleIndex.ToArray();
        data.saved_bubblePosX = bubblePosX.ToArray();
        data.saved_bubblePosY = bubblePosY.ToArray();

        data.saved_totalGoldenCardsObtained = totalGoldenCardsObtained;
        data.saved_totalCardsActivated = totalCardsActivated;
        data.saved_totalBubblesPicked = totalBubblesPicked;
        data.saved_globalRunTimer = globalRunTimer;

        data.saved_hasDiedThisRun = hasDiedThisRun;
        //data.saved_postprocessing_On = postprocessing_On;
    }

    void DataToLoad(PlayerData data)
    {
        //print("IS LOADING");
        hasSavedData = data.saved_hasSaveData;
        isRandomRun = data.saved_isRandomRun;
        runMoney = data.saved_totalMoney;
        currentLevel = data.saved_currentLevel;
        currentHealth = data.saved_currentHealth;
        currentScore = data.saved_currentScore;
        currentScoreToAdd = data.saved_currentScoreToAdd;
        currentBallVelocity = data.saved_currentBallVelocity;
        currentStartBallVelocity = data.saved_currentStartBallVelocity;
        runTimer = data.saved_runTimer;
        currentCards = data.saved_currentCards;
        currentCardsGolden = data.saved_currentCardsGolden;
        currentCardChoice = data.saved_currentCardChoice;
        levelCardWasChosen = data.saved_levelCardWasChosen;
        leftBeforeChoosing = data.saved_leftBeforeChoosing;
        isBeginningOfLevel = data.saved_isBeginningOfLevel;

        currentLevelEnabledBlocks.Clear();
        if (data.saved_currentLevelEnabledBlocks.Length > 0)
        {
            for (int i = 0; i < data.saved_currentLevelEnabledBlocks.Length; i++)
            {
                currentLevelEnabledBlocks.Add(data.saved_currentLevelEnabledBlocks[i]);
            }
        }

        currentLevelEnabledBlocks.Clear();
        if (data.saved_currentLevelEnabledBlocks.Length > 0)
        {
            for (int i = 0; i < data.saved_currentLevelEnabledBlocks.Length; i++)
            {
                currentLevelEnabledBlocks.Add(data.saved_currentLevelEnabledBlocks[i]);
            }
        }

        levelsOrder.Clear();
        if (data.saved_levelsOrder.Length > 0)
        {
            for (int i = 0; i < data.saved_levelsOrder.Length; i++)
            {
                levelsOrder.Add(data.saved_levelsOrder[i]);
            }
        }

        amountOfBalls = data.saved_amountOfBalls;
        ballPositionX.Clear();
        ballPositionY.Clear();
        ballVelocityX.Clear();
        ballVelocityY.Clear();
        ballHasLaunched.Clear();
        ballPhasingLeft.Clear();
        for (int i = 0; i < amountOfBalls; i++)
        {
            ballPositionX.Add(data.saved_ballPositionX[i]);
            ballPositionY.Add(data.saved_ballPositionY[i]);
            ballVelocityX.Add(data.saved_ballVelocityX[i]);
            ballVelocityY.Add(data.saved_ballVelocityY[i]);
            ballHasLaunched.Add(data.saved_ballHasLaunched[i]);
            ballPhasingLeft.Add(data.saved_ballPhasingLeft[i]);
        }

        chooseDirectionPowerUpActive = data.saved_chooseDirectionPowerUpActive;
        chooseDirectionPowerUpStillShooting = data.saved_chooseDirectionPowerUpStillShooting;
        lastDirectionX = data.saved_lastDirectionX;
        lastDirectionY = data.saved_lastDirectionY;
        hasMagnetPowerUp = data.saved_hasMagnetPowerUp;
        paddleSize = data.saved_paddleSize;
        hasShieldPowerUp = data.saved_hasShieldPowerUp;
        hasShootingPowerUp = data.saved_hasShootingPowerUp;
        fireRate = data.saved_fireRate;
        fireCounter = data.saved_fireCounter;
        shootCounter = data.saved_shootCounter;

        if (data.saved_bulletPosX != null)
        {
            bulletPosX.Clear();
            bulletPosY.Clear();

            for (int i = 0; i < data.saved_bulletPosX.Length; i++)
            {
                bulletPosX.Add(data.saved_bulletPosX[i]);
                bulletPosY.Add(data.saved_bulletPosY[i]);
            }
        }

        if (data.saved_bubbleIndex != null)
        {
            bubblePosX.Clear();
            bubblePosY.Clear();
            bubbleIndex.Clear();

            for (int i = 0; i < data.saved_bubbleIndex.Length; i++)
            {
                bubblePosX.Add(data.saved_bubblePosX[i]);
                bubblePosY.Add(data.saved_bubblePosY[i]);
                bubbleIndex.Add(data.saved_bubbleIndex[i]);
            }
        }

        totalGoldenCardsObtained = data.saved_totalGoldenCardsObtained;
        totalCardsActivated = data.saved_totalCardsActivated;
        totalBubblesPicked = data.saved_totalBubblesPicked;
        globalRunTimer = data.saved_globalRunTimer;

        hasDiedThisRun = data.saved_hasDiedThisRun;
        //postprocessing_On = data.saved_postprocessing_On;
    }

    public void ResetVariables()
    {
        hasSavedData = false;
        isRandomRun = false;
        runMoney = 0;
        currentLevel = 0;
        currentHealth = 2;
        currentScore = 0;
        currentScoreToAdd = 0;
        currentBallVelocity = 8;
        currentStartBallVelocity = 8;

        for (int i = 0; i < 3; i++)
        {
            runTimer[i] = 0;
            currentCards[i] = 99;
            currentCardsGolden[i] = false;
            currentCardChoice[i] = 99;
        }

        levelCardWasChosen = false;
        leftBeforeChoosing = false;
        isBeginningOfLevel = false;
        currentLevelEnabledBlocks.Clear();
        levelsOrder.Clear();
        amountOfBalls = 0;
        ballPositionX.Clear();
        ballPositionY.Clear();
        ballVelocityX.Clear();
        ballVelocityY.Clear();
        ballHasLaunched.Clear();
        ballPhasingLeft.Clear();

        chooseDirectionPowerUpActive = false;
        chooseDirectionPowerUpStillShooting = false;
        lastDirectionX = 0;
        lastDirectionY = 0;
        hasMagnetPowerUp = false;
        paddleSize = 1;
        hasShieldPowerUp = false;
        hasShootingPowerUp = false;
        fireRate = 0.5f;
        fireCounter = 0;
        shootCounter = 0;
        bulletPosX.Clear();
        bulletPosY.Clear();
        bubblePosX.Clear();
        bubblePosY.Clear();
        bubbleIndex.Clear();

        totalGoldenCardsObtained = 0;
        totalCardsActivated = 0;
        totalBubblesPicked = 0;
        globalRunTimer = 0;

        hasDiedThisRun = false;
    }
}

[Serializable]
class PlayerData
{
    public bool saved_hasSaveData;
    public bool saved_isRandomRun;
    public int saved_totalMoney;
    public int saved_currentLevel;
    public int saved_currentHealth;
    public int saved_currentScore;
    public int saved_currentScoreToAdd;
    public float saved_currentBallVelocity;
    public float saved_currentStartBallVelocity;
    public float[] saved_runTimer = new float[3];
    public bool saved_cardsAreSaved;
    public int[] saved_currentCards = new int[3];
    public bool[] saved_currentCardsGolden = new bool[3];
    public int[] saved_currentCardChoice = new int[3];
    public bool saved_levelCardWasChosen = false;
    public bool saved_leftBeforeChoosing = false;
    public bool saved_isBeginningOfLevel = false;
    public bool[] saved_currentLevelEnabledBlocks;
    public int[] saved_levelsOrder;
    public int saved_amountOfBalls;
    public float[] saved_ballPositionX;
    public float[] saved_ballPositionY;
    public float[] saved_ballVelocityX;
    public float[] saved_ballVelocityY;
    public bool[] saved_ballHasLaunched;
    public int[] saved_ballPhasingLeft;
    public bool saved_chooseDirectionPowerUpActive;
    public bool saved_chooseDirectionPowerUpStillShooting;
    public float saved_lastDirectionX;
    public float saved_lastDirectionY;
    public bool saved_hasMagnetPowerUp = false;
    public int saved_paddleSize;
    public bool saved_hasShieldPowerUp;
    public bool saved_hasShootingPowerUp = false;
    public float saved_fireRate;
    public float saved_fireCounter;
    public float saved_shootCounter;
    public float[] saved_bulletPosX;
    public float[] saved_bulletPosY;

    public float[] saved_bubblePosX;
    public float[] saved_bubblePosY;
    public int[] saved_bubbleIndex;

    public int saved_totalGoldenCardsObtained;
    public int saved_totalCardsActivated;
    public int saved_totalBubblesPicked;
    public float saved_globalRunTimer;

    public bool saved_hasDiedThisRun;

    public bool saved_postprocessing_On;
}
