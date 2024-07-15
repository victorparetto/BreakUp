using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPowerUp : MonoBehaviour
{
    PowerUpManager pum;

    public bool isShooting = false;

    [SerializeField] GameObject shootPowerUpGOs = null;

    [SerializeField] Transform leftLaser = null;
    [SerializeField] Transform rightLaser = null;
    [SerializeField] Transform spawner1 = null;
    [SerializeField] Transform spawner2 = null;

    float[] xPositions = { 0.09f, 0.345f, 0.665f, 0.985f };
    public float fireCounter = 0;
    public float fireRate = 0.5f;
    public float shootCounter = 0;
    float shootDuration = 3;

    private void Awake()
    {
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (isShooting)
        {
            shootCounter += Time.deltaTime;
            fireCounter += Time.deltaTime;

            if (fireCounter >= fireRate)
            {
                Fire();
            }

            if(shootCounter >= shootDuration)
            {
                EndShootPowerUp();
            }
        }
    }

    public void StartShootPowerUp(bool isGold)
    {
        pum.shotBulletsTransform.Clear();

        shootCounter = 0;
        fireCounter = 0;

        if(isGold) fireRate = 0.25f;
        else fireRate = 0.5f;

        UpdateToPaddleSize();
        ActivatePowerUpGOs(true);
        isShooting = true;
    }

    void Fire()
    {
        GameObject laser1 = PoolManager.current.GetPooledGameObject(PoolManager.current.laserPool, PoolManager.current.laserPrefab);
        laser1.SetActive(true);
        GameObject laser2 = PoolManager.current.GetPooledGameObject(PoolManager.current.laserPool, PoolManager.current.laserPrefab);
        laser2.SetActive(true);

        laser1.transform.position = spawner1.position;
        laser2.transform.position = spawner2.position;

        fireCounter = 0;

        pum.shotBulletsTransform.Add(laser1.transform);
        pum.shotBulletsTransform.Add(laser2.transform);
    }

    public void EndShootPowerUp()
    {
        isShooting = false;
        shootCounter = 0;
        fireCounter = 0;
        ActivatePowerUpGOs(false);
    }

    public void UpdateToPaddleSize()
    {
        float temp = xPositions[pum.currentPaddleSize];

        Vector2 leftLaserPos = new Vector2(-temp, leftLaser.localPosition.y);
        Vector2 RightLaserPos = new Vector2(temp, rightLaser.localPosition.y);

        leftLaser.localPosition = leftLaserPos;
        rightLaser.localPosition = RightLaserPos;
    }

    public void ActivatePowerUpGOs(bool activate)
    {
        shootPowerUpGOs.SetActive(activate);
    }

    public void SetCountersToLoad(float newFireCounter, float newShootCounter)
    {
        fireCounter = newFireCounter;
        shootCounter = newShootCounter;
    }
}
