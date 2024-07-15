using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager current;

    GameManager gm;

    [System.Serializable]
    public class BubblePowerUpPool
    {
        public GameObject prefab;
        public int size = 3;
        public List<GameObject> pool = new List<GameObject>();
    }

    [Header("Bubble Pooling")]
    public List<BubblePowerUpPool> bubblePools = new List<BubblePowerUpPool>();
    public List<GameObject> bubbleExplosionParticlesPool = new List<GameObject>();
    public int bubbleParticleSize = 6;

    [Header("Ball Pooling")]
    public GameObject ballPrefab;
    public List<GameObject> ballPool = new List<GameObject>();
    public List<GameObject> outwardsParticlePool = new List<GameObject>();
    public List<GameObject> leftSparkCollisionPool = new List<GameObject>();
    public List<GameObject> rightSparkCollisionPool = new List<GameObject>();
    public int ballPoolSize = 8;

    [Header("Laser Pooling")]
    public GameObject laserPrefab = null;
    public List<GameObject> laserPool = new List<GameObject>();
    public int laserPoolSize = 24;

    [Header("Particles Pooling")]
    public GameObject cardsParticlesPrefab = null;
    public List<GameObject> cardsParticlesPool = new List<GameObject>();
    public int cardsParticlesPoolSize = 3;

    private void Awake()
    {
        current = this;
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (CustomizableManager.GetCurrentBallPrefab() != null) ballPrefab = CustomizableManager.GetCurrentBallPrefab();
        CreateAllPools();

        if (CustomizableManager.GetCurrentTrailPrefab() != null)
        {
            for (int i = 0; i < ballPool.Count; i++)
            {
                Instantiate(CustomizableManager.GetCurrentTrailPrefab(), ballPool[i].transform);
            }
        }
    }

    private void CreateAllPools()
    {
        CreateAllBubblePools();
        CreatePool(ballPool, ballPoolSize, ballPrefab, "Ball", true);
        CreatePool(outwardsParticlePool, ballPoolSize, gm.outwardsParticle, "OutwardsParticleSFX", false);
        CreatePool(bubbleExplosionParticlesPool, bubbleParticleSize, gm.bubbleExplosionParticle, "BubbleExplosionParticle", false);
        CreatePool(laserPool, laserPoolSize, laserPrefab, "LaserBullet", false);
        CreatePool(cardsParticlesPool, cardsParticlesPoolSize, cardsParticlesPrefab, "CardsParticle", false);
    }

    private void HideAllPools()
    {
        HidePool(ballPool);
        HidePool(outwardsParticlePool);
        HidePool(bubbleExplosionParticlesPool);
        HidePool(laserPool);
    }

    private void CreateAllBubblePools()
    {
        foreach (BubblePowerUpPool bPool in bubblePools)
        {
            for (int i = 0; i < bPool.size; i++)
            {
                bPool.pool.Add(Instantiate(bPool.prefab, gm.placeToHidePooled, transform.rotation));
                bPool.pool[i].SetActive(false);
            }
        }
    }    

    public GameObject GetPooledBubbleBasedOnIndex(int index)
    {
        if (index > bubblePools.Count - 1)
        {
            print("Index is larger than the amount of PowerUps");
            return null;
        }

        for (int i = 0; i < bubblePools[index].pool.Count; i++)
        {
            if (!bubblePools[index].pool[i].activeInHierarchy)
            {
                return bubblePools[index].pool[i];
            }
        }

        GameObject obj = Instantiate(bubblePools[index].prefab, gm.placeToHidePooled, transform.rotation);
        bubblePools[index].pool.Add(obj);
        return obj;
    }

    public void CreatePool(List<GameObject> pool, int size, GameObject prefab, string name, bool setActive)
    {
        for (int i = 0; i < size; i++)
        {
            pool.Add(Instantiate(prefab, gm.placeToHidePooled, transform.rotation));
            pool[i].SetActive(setActive);
            pool[i].name = name + " " + i;
        }
    }

    public void HidePool(List<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            pool[i].SetActive(false);
        }
    }

    public GameObject GetPooledGameObject(List<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        return null;
    }

    public GameObject GetPooledGameObject(List<GameObject> pool, GameObject prefab) //Same function as above but for objects that CAN make the pool grow (Dynamic)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }
}
