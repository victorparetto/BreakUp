using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableManager : MonoBehaviour
{
    public static GameObject currentSelectedPaddlePrefab = null;
    public static GameObject currentSelectedBallPrefab = null;
    public static GameObject currentSelectedTrailPrefab = null;

    public static GameObject GetCurrentPaddlePrefab()
    {
        return currentSelectedPaddlePrefab;
    }

    public static void SetCurrentPaddleGameObject(GameObject paddlePrefab)
    {
        currentSelectedPaddlePrefab = paddlePrefab;
    }

    public static GameObject GetCurrentBallPrefab()
    {
        return currentSelectedBallPrefab;
    }

    public static void SetCurrentBallGameObject(GameObject ballPrefab)
    {
        currentSelectedBallPrefab = ballPrefab;
    }

    public static GameObject GetCurrentTrailPrefab()
    {
        return currentSelectedTrailPrefab;
    }

    public static void SetCurrentTrailGameObject(GameObject trailPrefab)
    {
        currentSelectedTrailPrefab = trailPrefab;
    }

    public static void SetTrailGameObjectToNull()
    {
        currentSelectedTrailPrefab = null;
    }
}
