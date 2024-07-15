using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePaddle : MonoBehaviour
{
    GameManager gm;
    PowerUpManager pum;
    CapsuleCollider2D cc2d;
    SpriteRenderer sr;
    Camera main;

    public ShootPowerUp spu;

    public GameObject magnetChildGO = null;
    [HideInInspector] public Animator magnetAnim;

    //Different pallet sizes variables
    public Sprite[] spriteSizes = null;
    [SerializeField] SpriteRenderer glowRenderer;
    public Sprite[] glowSpriteSizes = null;
    float[] palletSizes = { 0.8f, 1.25f, 1.9f, 2.54f };

    //float[] minX = { -3.65f, -3.34f, -3.02f };
    float[] maxX = { 3.92f, 3.65f, 3.34f, 3.02f };

    float xLimit = 3.65f;

    public bool canMove = false;

	//New Lines Added - Shinno
	public bool ExtraGlow;
	public GameObject extra_glowRenderer;
	[SerializeField] Sprite[] extra_glowSpriteSizes = null;
	[HideInInspector]public int currentSize;
	//

	void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        cc2d = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        magnetAnim = magnetChildGO.GetComponent<Animator>(); 
        main = Camera.main;
    }

    private void Start()
    {
        //sr.sprite = spriteSizes[1];
        //glowRenderer.sprite = glowSpriteSizes[1];
        //
		////New Lines Added - Shinno
		//if (ExtraGlow)extra_glowRenderer.GetComponent<SpriteRenderer>().sprite = extra_glowSpriteSizes[1];
		////
	}

	void Update()
    {
        if (Time.timeScale != 0)
        {
            if (canMove)
            {
                if (Input.touchCount > 0)
                {
                    Vector2 paddlePos = new Vector2(transform.position.x, transform.position.y);
                    paddlePos.x = Mathf.Clamp(GetXPos(), -xLimit, xLimit);

                    transform.position = paddlePos;
                }

#if UNITY_EDITOR
                Vector2 paddlePos2 = new Vector2(transform.position.x, transform.position.y);
                paddlePos2.x = Mathf.Clamp(main.ScreenToWorldPoint(Input.mousePosition).x, -xLimit, xLimit);

                transform.position = paddlePos2;
#endif
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 14) //Layer 14 = PowerUp
        {
            gm.totalBubblesPicked++;
            gm.AddScore(gm.bubbleScoreMultiplier);
            gm.bubbleScoreMultiplier += 200;

            pum.ActivateBubblePowerUp(other.GetComponent<PowerUpIndex>().index);
            GameObject temp = PoolManager.current.GetPooledGameObject(PoolManager.current.bubbleExplosionParticlesPool, gm.bubbleExplosionParticle);
            temp.transform.position = other.transform.position;
            temp.SetActive(true);
            pum.currentActiveBubbles.Remove(other.gameObject);
            other.gameObject.SetActive(false);
        }
        //else if(other.gameObject.layer == 12) //Layer 12 = BlockDestroyed
        //{
        //    gm.AddMoney(1);
        //    other.gameObject.SetActive(false);
        //}        
    }

    private float GetXPos()
    {
        Touch touch = Input.GetTouch(0);

        return main.ScreenToWorldPoint(touch.position).x;
        //return touch.position.x / Screen.width * 3.375f;
    }

    public void ChangePaddleSize(int size) //small = 0, default = 1, medium = 2, large = 3
    {
        if (size > 3) size = 3;
        else if (size < 0) size = 0;

        pum.currentPaddleSize = size;
        sr.sprite = spriteSizes[size];
        glowRenderer.sprite = glowSpriteSizes[size];
        xLimit = maxX[size];
        cc2d.size = new Vector2(palletSizes[size], cc2d.size.y);

        if(magnetAnim.isActiveAndEnabled)
            magnetAnim.SetInteger("currentSize", size);

        spu.UpdateToPaddleSize();

		//New Line Added - Shinno
		if (ExtraGlow)
		{
			extra_glowRenderer.GetComponent<SpriteRenderer>().sprite = extra_glowSpriteSizes[size];
			currentSize = size;
		}
		//
    }

    public int CheckAmountOfChildren()
    {
        return transform.childCount;
    }
}
