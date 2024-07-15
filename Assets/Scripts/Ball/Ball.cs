using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    //Call to other Scripts
    GameManager gm;
    PowerUpManager pum;
    UIControl uic;

    Rigidbody2D rb2d;
    [HideInInspector] public Transform paddle;
    [HideInInspector] public CircleCollider2D cc2d;

    [SerializeField] LayerMask layerToIgnoreOnLaunch;

    //Child GOs
    public GameObject instantKillGo = null;
    public GameObject outwardsParticlesPrefab = null;
    public GameObject inwardsParticlesPrefab = null;
    public GameObject leftSparkCollisionParticles = null;
    public GameObject rightSparkCollisionParticles = null;

    //Wall Variables
    GameObject[] walls;
    SpriteRenderer[] wallsSr;
	[SerializeField] Material wallMaterial;

    bool huggingLeftWall = false;
    bool huggingRightWall = false;

    public bool readyToFixLeftHugging = false;
    public bool readyToFixRightHugging = false;
    float timeHugging = 0f;

    //Start Parent Variables
    public Vector2 startLocalPos = new Vector2(0, 0.3f);

    //[SerializeField] float currentBallVelocity = 8f;

    Vector3 lastFrameVelocity;

    [SerializeField] float negXPush = -2f;
    [SerializeField] float posiXPush = 2f;
    [SerializeField] float yPush = 8f;
    [SerializeField] float minYVel = 1.2f;

    //public bool isLockedToPaddle = true;
    public bool hasLaunched = false;
    public bool velocityZeroLastFrame = false;
    bool magnitudeSlowerLastFrame = false;

    public bool ballIsLoadingVelocity = false;
    public Vector2 velocityToLoad;
    public bool ballIsGonnaBeShot = false;

    //PowerUps
    public int currentPhasingsLeft = 0;
    public bool phasingActive = false;

    //public bool magnetActive = false;

    public float scoreMultiplier = 0;
    public int timesBouncedOffPlayer = 0;

    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        uic = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIControl>();

        rb2d = GetComponent<Rigidbody2D>();
        cc2d = GetComponent<CircleCollider2D>();
        paddle = GameObject.FindGameObjectWithTag("Player").transform;
		walls = GameObject.FindGameObjectsWithTag("Wall");

        wallsSr = new SpriteRenderer[GameObject.FindGameObjectsWithTag("Wall").Length];

        for (int i = 0; i < walls.Length; i++)
        {
            wallsSr[i] = walls[i].GetComponent<SpriteRenderer>();
        }

        //startLocalPos = transform.localPosition; //doesn't work in Start(), I have no clue why
    }

    void Start()
    {
    }

    void Update()
    {        
        //Make Code to check if the ball is out of bounds, PREVENTING SOFT LOCK
        if (!hasLaunched)
        {          
            if(gm.canLaunchBall) if(gm.ballIsAvailable) LaunchOnMouseClick();
        }
        else
        {            
            lastFrameVelocity = rb2d.velocity;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1))
            {
                LockBallToPaddle();
            }
#endif

            if (gm.ballIsAvailable)
            {
                if (ballIsLoadingVelocity)
                {
                    lastFrameVelocity = velocityToLoad;
                    rb2d.velocity = velocityToLoad;
                    cc2d.enabled = true;
                    velocityZeroLastFrame = false;
                    magnitudeSlowerLastFrame = false;
                    ballIsLoadingVelocity = false;
                    return;
                }

                if (!ballIsGonnaBeShot)
                {
                    if (lastFrameVelocity == Vector3.zero)
                    {
                        velocityZeroLastFrame = true;
                    }
                    else if (lastFrameVelocity.magnitude < gm.currentBallVelocity - 0.1f || lastFrameVelocity.magnitude > gm.currentBallVelocity + 0.1f)
                    {
                        magnitudeSlowerLastFrame = true;
                    }

                    if (velocityZeroLastFrame)
                    {
                        print("Velocity was ZERO, gave impulse to prevent ball getting stuck " + gameObject.name);
                        BallVerticalImpulse();
                        velocityZeroLastFrame = false;
                    }
                    else if (magnitudeSlowerLastFrame)
                    {
                        print("Velocity was below/above currentSpeed, ADJUSTING SPEED");
                        rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
                        magnitudeSlowerLastFrame = false;
                    }
                }
            }
        }

        if (phasingActive)
        {
            if(currentPhasingsLeft <= 0)
            {
                ResetPhasing();
            }
        }

        if (huggingLeftWall)
        {
            timeHugging += Time.deltaTime;

            if(timeHugging >= 0.5f)
            {
                readyToFixLeftHugging = true;              
            }
        }
        else if (huggingRightWall)
        {
            timeHugging += Time.deltaTime;

            if (timeHugging >= 0.5f)
            {
                readyToFixRightHugging = true;
            }
        }
    }

    private void LaunchOnMouseClick()
    {
        //With Choose Cards doesnt work... My guess is that the cards disappear too fast for the raycast to get it
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
                //touch = Input.GetTouch(lastTouchIndex);

                //We now raycast with this information. If we have hit something we can process it.
                RaycastHit2D hit = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward, Mathf.Infinity, layerToIgnoreOnLaunch);

                if (hit.collider == null)
                {
                    gm.gameStarted = true;
                    Singleton.Instance.saveWasLoaded_Temp = false;
                    LaunchBall(false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Camera.main.transform.forward, Mathf.Infinity, layerToIgnoreOnLaunch);

            if (hit.collider == null)
            {
                gm.gameStarted = true;
                Singleton.Instance.saveWasLoaded_Temp = false;
                LaunchBall(false);
            }
        }
    }

    public void LaunchBall(bool random)
    {
        transform.parent = null;

        if (pum.magnetIsActive) BounceOffPlayer();
        else
        {
            if (!random)
                BallVerticalImpulse();
            else
                BallRandomImpulse(-8f, 8f);
        }

        hasLaunched = true;
    }

    public void BallVerticalImpulse()
    {
        int ran = Random.Range(0, 2);

        rb2d.AddForce(new Vector2((ran == 0) ? negXPush : posiXPush, yPush), ForceMode2D.Impulse);
        rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
    }

    public void BallRandomImpulse(float ran1, float ran2)
    {
        float ran = Random.Range(ran1, ran2);
        rb2d.AddForce(new Vector2(ran, yPush), ForceMode2D.Impulse);
        rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
    }

    public void BallHorizontalImpulse(bool toRight)
    {
        rb2d.AddForce(new Vector2((toRight) ? posiXPush : negXPush, 0), ForceMode2D.Impulse);
        rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
    }

    private void LockBallToPaddle()
    {
        rb2d.velocity = Vector2.zero;
        transform.parent = paddle.transform;
        transform.localPosition = startLocalPos;
        //isLockedToPaddle = true;
        hasLaunched = false;
    }

    private void MagnetBallToPaddle(Collision2D paddle)
    {
        transform.parent = paddle.transform;
        rb2d.velocity = Vector2.zero;
        //isLockedToPaddle = true;
        hasLaunched = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TopBound"))
        {
            if (gm.gameStarted)
            {
                gm.gameStarted = false;
                gm.ballIsAvailable = false;
                gm.ChangeMainBall(this);
                gm.EndLevel(true);
                PlayChangeLevelAnimation();
            }
        }
        else if (other.CompareTag("BotBound"))
        {
            gameObject.SetActive(false);
            transform.position = gm.placeToHidePooled;
            currentPhasingsLeft = 0;
            ResetPhasing();

            if (gm.ItWasTheLastBall())
            {
                gm.hasDiedThisRun = true;
                gm.ChangeMainBall(this);
                uic.UpdateHealth(1, false);
                if (uic.GetCurrentHealth() >= 0)
                {
                    gm.ballIsResettingAfterDeath = true;
                    BallBackToPaddle();
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
            gm.RemoveBallFromLists(gameObject, rb2d, this);

            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            scoreMultiplier = 0;
            timesBouncedOffPlayer++;
            AchievementManager.instance.VerifyAchievementProgress(18, "BOUNCY", timesBouncedOffPlayer);
            AchievementManager.instance.VerifyAchievementProgress(19, "THE_FLOOR_IS_LAVA", timesBouncedOffPlayer);

            if (pum.magnetIsActive)
            {
                if (!hasLaunched) return;
                MagnetBallToPaddle(other);

                if(transform.position.y < other.transform.position.y)
                {
                    Vector3 temp = new Vector3(0, other.transform.position.y - transform.position.y) * 2f;
                    transform.position += temp;
                }

                if(other.collider.bounds.max.x < cc2d.bounds.min.x)
                {
                    Vector3 temp2 = new Vector3(other.collider.bounds.max.x, transform.position.y);
                    transform.position = temp2;
                }
                else if (other.collider.bounds.min.x > cc2d.bounds.max.x)
                {
                    Vector3 temp3 = new Vector3(other.collider.bounds.min.x, transform.position.y);
                    transform.position = temp3;
                }

                AchievementManager.instance.VerifyAchievementProgress(6, "KEEPING_THEM_CLOSE", gm.mp.CheckAmountOfChildren());
                AchievementManager.instance.VerifyAchievementProgress(7, "BULLET_HELL", gm.mp.CheckAmountOfChildren());
                return;
            }

            if(hasLaunched) BounceOffPlayer();
            return;
        }

        if(hasLaunched) Bounce(other.contacts[0].normal);

        if(other.gameObject.tag == "Breakable")
        {
            AddBreakableScore();

			//Code for walls color change
			for (int i = 0; i < walls.Length; i++)
			{
                switch (other.gameObject.GetComponent<BreakableInteraction>().block_Color)
                {
                    case "Red":
                        wallMaterial.SetColor("_MainColor", Color.red * gm.wallLightIntensity);
						//wallsSr[i].color = Color.red * 2f;                                    //Bar stays lighted up
						//gm.backgroundMaterial.color = new Color(56f / 255, 3f / 255, 0f / 255);
						gm.backgroundMaterial.color = new Color(123f / 255, 0f / 255, 2f / 255);
						uic.ChangeScoreHeaderTextColor(Color.red);
						break;
					case "Blue":
						wallMaterial.SetColor("_MainColor", Color.blue * gm.wallLightIntensity);
						//wallsSr[i].color = Color.blue * 2f;
						//gm.backgroundMaterial.color = new Color(0f/255, 42f/255, 65f/255);
						gm.backgroundMaterial.color = new Color(0f / 255, 34f / 255, 100f / 255);
						uic.ChangeScoreHeaderTextColor(Color.blue);
                        break;
					case "Purple":
						wallMaterial.SetColor("_MainColor", Color.magenta * gm.wallLightIntensity);
						//wallsSr[i].color = Color.magenta * 2f;
						//gm.backgroundMaterial.color = new Color(27f / 255, 0f / 255, 55f / 255);
						gm.backgroundMaterial.color = new Color(85f / 255, 0f / 255, 101f / 255);
						uic.ChangeScoreHeaderTextColor(Color.magenta);
                        break;
					case "Green":
						wallMaterial.SetColor("_MainColor", Color.green * gm.wallLightIntensity);
						//wallsSr[i].color = Color.green * 2f;
						//gm.backgroundMaterial.color = new Color(0f / 255, 26f / 255, 18f / 255);
						gm.backgroundMaterial.color = new Color(0f / 255, 101f / 255, 3f / 255);
						uic.ChangeScoreHeaderTextColor(Color.green);
                        break;
					case "Yellow":
						wallMaterial.SetColor("_MainColor", Color.yellow * gm.wallLightIntensity);
						//wallsSr[i].color = Color.yellow * 2f;
						//gm.backgroundMaterial.color = new Color(27f / 255, 29f / 255, 0f / 255);
						gm.backgroundMaterial.color = new Color(132f / 255, 111f / 255, 0f / 255);
						uic.ChangeScoreHeaderTextColor(Color.yellow);
                        break;
					case "LightBlue":
						wallMaterial.SetColor("_MainColor", Color.cyan * gm.wallLightIntensity);
						//wallsSr[i].color = Color.cyan * 2f;
						//gm.backgroundMaterial.color = new Color(0f / 255, 54f / 255, 65f / 255);
						gm.backgroundMaterial.color = new Color(0f / 255, 106f / 255, 100f / 255);
						uic.ChangeScoreHeaderTextColor(Color.cyan);
                        break;
					case "Orange":
						wallMaterial.SetColor("_MainColor", new Color(1, 165 / 255f, 0) * gm.wallLightIntensity);
						//wallsSr[i].color = new Color(1, 165 / 255f, 0) * 2f;                      //Bar stays lighted up
						//gm.backgroundMaterial.color = new Color(29f / 255, 17f / 255, 0f / 255);
						gm.backgroundMaterial.color = new Color(140f / 255, 77f / 255, 0f / 255);
						uic.ChangeScoreHeaderTextColor(new Color(1, 0.5f, 0));
                        break;
				}
			}
		}
        else if (other.gameObject.CompareTag("Wall"))
        {
            GameObject temp;

            if (other.transform.position.x > transform.position.x)
            {
                temp = PoolManager.current.GetPooledGameObject(PoolManager.current.leftSparkCollisionPool, leftSparkCollisionParticles);
                temp.transform.position = other.contacts[0].point;
                temp.SetActive(true);
            }
            else
            {
                temp = PoolManager.current.GetPooledGameObject(PoolManager.current.rightSparkCollisionPool, rightSparkCollisionParticles);
                temp.transform.position = other.contacts[0].point;
                temp.SetActive(true);
            }
            
            if (other.transform.position.x > transform.position.x)
            {
                huggingRightWall = true;
            }
            else if (other.transform.position.x <= transform.position.x)
            {
                huggingLeftWall = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            huggingRightWall = false;
            huggingLeftWall = false;
            readyToFixLeftHugging = false;
            readyToFixRightHugging = false;
            timeHugging = 0;
        }
    }

    private void Bounce(Vector3 colNormal)
    {
        Vector3 dir = Vector3.Reflect(lastFrameVelocity.normalized, colNormal);

        rb2d.velocity = dir * gm.currentBallVelocity;

        //Code to fix corner collision being too horizontal
        if (rb2d.velocity.y != 0)
        {
            if ((rb2d.velocity.y < 1) && (Mathf.Sign(rb2d.velocity.y) == 1))
            {
                rb2d.AddForce(new Vector2(0, minYVel), ForceMode2D.Impulse);
                rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
                print("Y push to avoid horizontal stuck!");
            }
            else if ((rb2d.velocity.y > -1) && (Mathf.Sign(rb2d.velocity.y) == -1))
            {
                rb2d.AddForce(new Vector2(0, -minYVel), ForceMode2D.Impulse);
                rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
                print("Y push to avoid horizontal stuck!");
            }
        }
        else
        {
            rb2d.AddForce(new Vector2(0, -minYVel), ForceMode2D.Impulse);
            rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
            print("Y velocity was 0, avoiding possible top Wall stuck");
        }

        if (readyToFixLeftHugging)
        {
            BallHorizontalImpulse(true);
            readyToFixLeftHugging = false;
            huggingLeftWall = false;
            timeHugging = 0;
        }
        else if (readyToFixRightHugging)
        {
            BallHorizontalImpulse(false);
            readyToFixRightHugging = false;
            huggingRightWall = false;
            timeHugging = 0;
        }
    }

    private void BounceOffPlayer()
    {
        if (readyToFixLeftHugging)
        {
            rb2d.AddForce(new Vector2(posiXPush, yPush), ForceMode2D.Impulse);
            rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
            readyToFixLeftHugging = false;
            huggingLeftWall = false;
            timeHugging = 0;
            return;
        }
        else if (readyToFixRightHugging)
        {
            rb2d.AddForce(new Vector2(negXPush, yPush), ForceMode2D.Impulse);
            rb2d.velocity = rb2d.velocity.normalized * gm.currentBallVelocity;
            readyToFixRightHugging = false;
            huggingRightWall = false;
            timeHugging = 0;
            return;
        }

        if (paddle.transform.position.y >= transform.position.y) print("Ball was below Pallet");
        Vector3 dir = (transform.position - paddle.position).normalized;

        rb2d.velocity = dir * gm.currentBallVelocity;    
    }

    private void PlayChangeLevelAnimation()
    {
        rb2d.velocity = Vector2.zero;
        //cc2d.enabled = false;
        //hasLaunched = false;
    }

    public void BallBackToPaddle()
    {
        LockBallToPaddle();
        gm.StartBallCreationParticles();
        //cc2d.enabled = true;
    }

    public void ResetPhasing()
    {
        phasingActive = false;
        gameObject.layer = 8;
        instantKillGo.SetActive(false);
    }

    public void AddBreakableScore()
    {
        gm.AddScore(100 + (int)(103 * scoreMultiplier));
        scoreMultiplier += 0.1f;
    }
}
