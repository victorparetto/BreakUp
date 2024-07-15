using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableInteraction : MonoBehaviour
{
    GameManager gm;
    PowerUpManager pum;
    Rigidbody2D rb2d;
    Animator anim;
    TrailRenderer trail;
    SpriteRenderer sr;
    [SerializeField] SpriteRenderer glowSR;
    BoxCollider2D bc2d;
    Transform paddle;

    public bool isDestroyed = false;

    //Special Blocks Variables
    [Header("Special Blocks")]
    public bool isSpecial = false;
    public bool isSpecialCard = false;
    [SerializeField] bool droppedPowerUp = false;

    //float timer = 0f;
    //float ranDestroyTimer = 0.5f;
    //public bool fallDestroy = false;


    //Beginning of Level Variables
    [Header("Beginning Of Level")]
    public bool isFallingToPlace = false;
    Vector3 placeToFall;
    float fallSpeed = 10f;
    public float constantSpeed = 10f;

    //Collision variables
    [Header("On Collision")]
    bool falling = false;
    float alpha = 1f;

    //Level ended Variables
    [Header("End Of Level")]
    public bool canGoToPaddle = false;
    public bool levelEnded = false;
    float speedToGoToPaddle = 25f;

	Vector3 initScale;
	public string block_Color;

    public bool isTheLastBlock = false;

	private void Awake()
	{
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        paddle = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
		rb2d = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
        bc2d = GetComponent<BoxCollider2D>();
        initScale = transform.lossyScale; //initScale = new Vector3(0.37f, 0.5011473f, 1f);
	}

	void Start()
    {
        isDestroyed = false;
        bc2d.enabled = false;
        //Must be at y = 9 for it to fall correctly (fix later if needed?) or parent object Inactive
        placeToFall = new Vector3(transform.position.x, transform.position.y - gm.levelChangePosOffset);
    }

    void Update()
    {
        //if (fallDestroy) //Only happens when level ends (not on collision)
        //{
        //    bc2d.isTrigger = true;
        //    timer += Time.deltaTime;
        //    if(timer >= ranDestroyTimer)
        //    {
        //        levelEnded = true;
        //        FallDeathSequence();
        //        timer = 0;
        //        fallDestroy = false;
        //    }
        //}

        if (canGoToPaddle)
        {
            transform.position = Vector3.MoveTowards(transform.position, gm.uiMoneyRtPos, Time.deltaTime * speedToGoToPaddle);

            if(Vector2.Distance(transform.position, gm.uiMoneyRtPos) <= 0.1f)
            {
                gm.AddMoney(1);
                gameObject.SetActive(false);
            }

            return;
        }

        if (isFallingToPlace)
        {
            fallSpeed = constantSpeed + (fallSpeed * gm.t); //REWORK Fallspeed shouldnt be affected by T (Increase it in the GameManager Coroutine?)
            transform.position = Vector3.MoveTowards(transform.position, placeToFall, Time.deltaTime * fallSpeed);

            if(transform.position == placeToFall)
            {
                trail.enabled = false;
                bc2d.enabled = true;
                isFallingToPlace = false;

                if (isTheLastBlock)
                {
                    gm.EndOfLevelBuilding();
                    //if (Singleton.Instance.chooseDirectionPowerUpActive) pum.cd.StartChooseDirectionPowerUp();
                }
            }
        }

        if (falling && !levelEnded)
        {
            alpha -= Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            glowSR.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

            if (alpha <= 0)
            {
                HideBackToPool();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BotBound"))
        {
            HideBackToPool();
        }
        else if (other.CompareTag("InstantKill"))
        {
            //bc2d.enabled = false;
            DeathSequence();
        }
        else if (other.CompareTag("Bullet"))
        {
            DeathSequence();
            gm.AddScore(100 + (int)(100 * gm.laserScoreMultiplier));
            gm.laserScoreMultiplier += 0.01f;

            gm.blocksDestroyedWithBullets++;
            AchievementManager.instance.VerifyAchievementProgress(8, "SHOOTER", gm.blocksDestroyedWithBullets);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            //bc2d.enabled = false;
            DeathSequence();
        }
    }

    public void DeathSequence() //BlockDestroyed layer = 12
    {
        gameObject.layer = 12;
        falling = true;
        isDestroyed = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        bc2d.isTrigger = true;
        anim.SetBool("isIdle", false);
        anim.SetTrigger("isDestroying");    //Changed to Trigger (from Bool) to save resources and avoid calling it 2 times

        if (!droppedPowerUp)
        {
            if (isSpecialCard)
            {
                pum.ActivateBubblePowerUp(12); //index for card power up
                droppedPowerUp = true;
                return;
            }

            if (isSpecial)
            {
                GameObject tempBubble = PoolManager.current.GetPooledBubbleBasedOnIndex(pum.GetRandomBubbleIndex());
                pum.SpawnBubbleOnBlock(tempBubble, transform);
                pum.currentActiveBubbles.Add(tempBubble);
                droppedPowerUp = true;
                return;
            }
            else
            {
                if (RegularBlockSpawnPowerUp())
                {
                    GameObject tempBubble = PoolManager.current.GetPooledBubbleBasedOnIndex(pum.GetRandomBubbleIndex());
                    pum.SpawnBubbleOnBlock(tempBubble, transform);
                    pum.currentActiveBubbles.Add(tempBubble);
                    droppedPowerUp = true;
                }
            }
        }
    }

    //public void FallDeathSequence() //BlockDestroyed layer = 12
    //{
    //    gameObject.layer = 12;
    //    rb2d.bodyType = RigidbodyType2D.Dynamic;
    //    //rb2d.AddForce(Vector2.up * Random.Range(0.2f, 0.8f), ForceMode2D.Impulse);
    //}

    //IEnumerator PlayDeathAnim()
    //{
    //    yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
    //    anim.SetBool("isIdle", false);
    //    anim.SetTrigger("isDestroying");
    //    isDestroyed = true;
    //}

    public void PlayDeathAnim(bool endOfClimbMode) //BlockDestroyed layer = 12 //Only happens when level ended not collision
    {
        bc2d.isTrigger = true;
        gameObject.layer = 12;        
        if(endOfClimbMode) levelEnded = true;
        else rb2d.bodyType = RigidbodyType2D.Dynamic;
        anim.SetBool("isIdle", false);
        anim.SetTrigger("isDestroying");        
    }

    public void ActivateIfLevelEnded()
    {
        if (levelEnded)
        {
            isDestroyed = true;
            canGoToPaddle = true;
        }
    }

    //public void SetRandomDestroyTimer(float min, float max)
    //{
    //    ranDestroyTimer = Random.Range(min, max);
    //}

    public void HideBackToPool() //Block layer = 9
    {
        gameObject.SetActive(false);
        gameObject.layer = 9;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        droppedPowerUp = false;
        falling = false;
        levelEnded = false;
        bc2d.isTrigger = false;
        anim.SetBool("isIdle", true);
    }

    bool RegularBlockSpawnPowerUp()
    {
        int ran = Random.Range(1, 101);

        if(ran <= 1)
            return true;
        else
            return false;
    }
}
