using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDataManager : MonoBehaviour
{
    private static MenuDataManager instance;
    string path;

    //Data to save
    public bool firstTimeEnteringGameIsDone = false;
    public int currentMoney = 0;

    public int currentPaddleIndex = 0;
    public int currentPaddleColorID = 0;
    public int currentBallIndex = 0;
    public int currentBallColorID = 0;
    public int currentTrailIndex = 0;
    public int currentTrailColorID = 0;

    public int highestClimbScore = 0;
    public int highestClassicScore = 0;

    public int numberOfGamesPlayed = 0;
    public int numberOfAdsWatched = 0;

    //Unlockables
    public List<bool> unlockableUnlockState = new List<bool>();

    //Shop
    public List<bool> boughtPaddles = new List<bool>();
    public List<bool> boughtBalls = new List<bool>();
    public List<bool> boughtTrails = new List<bool>();

    public static MenuDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                //Debug.LogWarning("Creating MenuDataManager");
                GameObject owner = new GameObject("MenuDataManager");
                instance = owner.AddComponent<MenuDataManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        path = Application.persistentDataPath + "/menuInfo.dat";

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            //Debug.LogWarning("Destroying MenuDataManager");
            Destroy(this);
        }

    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        MenuData data = new MenuData();
        DataToSave(data);
        bf.Serialize(file, data);
        file.Close();

    }

    public void Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            MenuData data = (MenuData)bf.Deserialize(file);
            file.Close();
            DataToLoad(data);
        }
    }

    void DataToSave(MenuData data)
    {
        data.saved_firstTimeEnteringGameIsDone = firstTimeEnteringGameIsDone;
        data.saved_currentMoney = currentMoney;
        data.saved_currentPaddleIndex = currentPaddleIndex;
        data.saved_currentPaddleColorID = currentPaddleColorID;
        data.saved_currentBallIndex = currentBallIndex;
        data.saved_currentBallColorID = currentBallColorID;
        data.saved_currentTrailIndex = currentTrailIndex;
        data.saved_currentTrailColorID = currentTrailColorID;

        data.saved_highestClimbScore = highestClimbScore;
        data.saved_highestClassicScore = highestClassicScore;

        data.saved_unlockableUnlockState = unlockableUnlockState.ToArray();

        data.saved_numberOfGamesPlayed = numberOfGamesPlayed;
        data.saved_numberOfAdsWatched = numberOfAdsWatched;

        data.saved_boughtPaddles = boughtPaddles.ToArray();
        data.saved_boughtBalls = boughtBalls.ToArray();
        data.saved_boughtTrails = boughtTrails.ToArray();
    }

    void DataToLoad(MenuData data)
    {
        firstTimeEnteringGameIsDone = data.saved_firstTimeEnteringGameIsDone;
        currentMoney = data.saved_currentMoney;
        currentPaddleIndex = data.saved_currentPaddleIndex;
        currentPaddleColorID = data.saved_currentPaddleColorID;
        currentBallIndex = data.saved_currentBallIndex;
        currentBallColorID = data.saved_currentBallColorID;
        currentTrailIndex = data.saved_currentTrailIndex;
        currentTrailColorID = data.saved_currentTrailColorID;

        highestClimbScore = data.saved_highestClimbScore;
        highestClassicScore = data.saved_highestClassicScore;

        if (data.saved_unlockableUnlockState != null)
        {
            unlockableUnlockState.Clear();
            for (int i = 0; i < data.saved_unlockableUnlockState.Length; i++)
            {
                unlockableUnlockState.Add(data.saved_unlockableUnlockState[i]);
            }
        }

        numberOfGamesPlayed = data.saved_numberOfGamesPlayed;
        numberOfAdsWatched = data.saved_numberOfAdsWatched;

        if (data.saved_boughtPaddles != null)
        {
            boughtPaddles.Clear();
            boughtBalls.Clear();
            boughtTrails.Clear();

            for (int i = 0; i < data.saved_boughtPaddles.Length; i++)
            {
                boughtPaddles.Add(data.saved_boughtPaddles[i]);
                boughtBalls.Add(data.saved_boughtBalls[i]);
                boughtTrails.Add(data.saved_boughtTrails[i]);
            }
        }
    }

    public void ResetVariables()
    {
        firstTimeEnteringGameIsDone = false;
        currentMoney = 0;
        currentPaddleIndex = 0;
        currentPaddleColorID = 0;
        currentBallIndex = 0;
        currentBallColorID = 0;
        currentTrailIndex = 0;
        currentTrailColorID = 0;

        highestClimbScore = 0;
        highestClassicScore = 0;

        numberOfGamesPlayed = 0;
        numberOfAdsWatched = 0;

        unlockableUnlockState.Clear();
        boughtPaddles.Clear();
        boughtBalls.Clear();
        boughtTrails.Clear();
    }
}

[Serializable]
class MenuData
{
    public bool saved_firstTimeEnteringGameIsDone;
    public int saved_currentMoney;
    public int saved_currentPaddleIndex;
    public int saved_currentPaddleColorID;
    public int saved_currentBallIndex;
    public int saved_currentBallColorID;
    public int saved_currentTrailIndex;
    public int saved_currentTrailColorID;
    public int saved_highestClimbScore;
    public int saved_highestClassicScore;
    public bool[] saved_unlockableUnlockState;

    public int saved_numberOfGamesPlayed;
    public int saved_numberOfAdsWatched;

    public bool[] saved_boughtPaddles;
    public bool[] saved_boughtBalls;
    public bool[] saved_boughtTrails;
}

