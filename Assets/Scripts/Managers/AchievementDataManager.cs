using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementDataManager : MonoBehaviour
{
    private static AchievementDataManager instance;
    string path;

    //Data to save
    public int blocksDestroyed = 0;
    public int amountOfPickedUpPowerUps = 0;
    public List<bool> achievementsUnlockState = new List<bool>();
    public List<bool> achievementsClaimedState = new List<bool>();

    public static AchievementDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning("Creating AchievementDataManager");
                GameObject owner = new GameObject("AchievementDataManager");
                instance = owner.AddComponent<AchievementDataManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        path = Application.persistentDataPath + "/achievementInfo.dat";

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning("Destroying AchievementDataManager");
            Destroy(this);
        }

    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        AchievementData data = new AchievementData();
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
            AchievementData data = (AchievementData)bf.Deserialize(file);
            file.Close();
            DataToLoad(data);
        }
    }

    void DataToSave(AchievementData data)
    {
        data.saved_blocksDestroyed = blocksDestroyed;
        data.saved_amountOfPickedUpPowerUps = amountOfPickedUpPowerUps;
        data.saved_achievementsUnlockState = achievementsUnlockState.ToArray();
        data.saved_achievementsClaimedState = achievementsClaimedState.ToArray();
    }

    void DataToLoad(AchievementData data)
    {
        blocksDestroyed = data.saved_blocksDestroyed;
        amountOfPickedUpPowerUps = data.saved_amountOfPickedUpPowerUps;

        if (data.saved_achievementsUnlockState != null)
        {
            achievementsUnlockState.Clear();
            for (int i = 0; i < data.saved_achievementsUnlockState.Length; i++)
            {
                achievementsUnlockState.Add(data.saved_achievementsUnlockState[i]);
            }
        }

        if(data.saved_achievementsClaimedState != null)
        {
            achievementsClaimedState.Clear();
            for (int i = 0; i < data.saved_achievementsClaimedState.Length; i++)
            {
                achievementsClaimedState.Add(data.saved_achievementsClaimedState[i]);
            }
        }
    }

    public void ResetVariables()
    {
        blocksDestroyed = 0;
        amountOfPickedUpPowerUps = 0;
        achievementsUnlockState.Clear();
        achievementsClaimedState.Clear();

        Instance.Save();
    }
}

[Serializable]
class AchievementData
{
    public int saved_blocksDestroyed;
    public int saved_amountOfPickedUpPowerUps;
    public bool[] saved_achievementsUnlockState;
    public bool[] saved_achievementsClaimedState;
}