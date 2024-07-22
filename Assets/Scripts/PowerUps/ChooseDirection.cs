using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDirection : MonoBehaviour
{
    //references to other Scripts
    GameManager gm;
    PowerUpManager pum;
    MovePaddle mp;

    private LineRenderer _lineRenderer;

    private Vector2 _initialPosition;
    private Vector2 _currentPosition;
    
    [HideInInspector] public bool isDragging = false;
    public bool allBallsAtZeroSpeed = false;
    public bool isShooting = false;

    Vector3 touchPosWorld;
    TouchPhase touchPhase = TouchPhase.Began;

    //Inspector accesible variables
    public float launchInterval = 0.25f;

    //temp
    public List<Ball> currentActiveBallScripts = new List<Ball>();
    public List<Rigidbody2D> currentActiveBallRb2ds = new List<Rigidbody2D>();

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        pum = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerUpManager>();
        mp = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePaddle>();

        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        if(_lineRenderer == null) _lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    public void Start()
    {
        _lineRenderer.startWidth = 0.02f;
        _lineRenderer.endWidth = 0.02f;
        //_lineRenderer.SetWidth(0.1f, 0.1f);
        _lineRenderer.enabled = false;
    }

    public void FixedUpdate()
    {
        if (pum.chooseDirectionIsActive)
        {
            if (!isShooting) //!allBallsAtZeroSpeed
            {
                for (int i = 0; i < currentActiveBallRb2ds.Count; i++)
                {
                    currentActiveBallRb2ds[i].velocity = currentActiveBallRb2ds[i].velocity * 0.9f;

                    if (currentActiveBallRb2ds[currentActiveBallRb2ds.Count - 1].velocity.magnitude < 0.05f)
                    {
                        //print("Ball Magnitude its: " + ballRb2ds[ballRb2ds.Count - 1].velocity.magnitude);
                        for (int j = 0; j < currentActiveBallRb2ds.Count; j++)
                        {
                            currentActiveBallRb2ds[j].velocity = Vector2.zero;
                        }
                        //allBallsAtZeroSpeed = true;
                    }
                }
            }
        }
    }

    public void Update()
    {
        if (!gm.gameIsPaused)
        {
            if (pum.chooseDirectionIsActive)
            {
                //We check if we have more than one touch happening.
                //We also check if the first touches phase is Ended (that the finger was lifted)
                if (Input.touchCount > 0)
                {
                    touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                    if (Input.GetTouch(0).phase == touchPhase)
                    {
                        //We now raycast with this information. If we have hit something we can process it.
                        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                        if (hitInformation.collider != null)
                        {
                            if (hitInformation.collider.CompareTag("DragPanel"))
                            {
                                pum.ActivateChooseDirectionIndicators(false);
                                gm.ShowInwardsParticles(touchPosWorld2D);
                                isDragging = true;
                                _initialPosition = touchPosWorld2D;
                                MoveBallsToMouseDown();
                                _lineRenderer.SetPosition(0, _initialPosition);
                                //_lineRenderer.SetVertexCount(1);
                                _lineRenderer.positionCount = 1;
                                _lineRenderer.enabled = true;
                            }
                            else
                            {
                                print("NO HIT");
                                return;
                            }
                        }
                    }

                    if (isDragging)
                    {
                        _currentPosition = touchPosWorld2D;
                        //_lineRenderer.SetVertexCount(2);
                        _lineRenderer.positionCount = 2;
                        _lineRenderer.SetPosition(1, _currentPosition);

                        if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            var releasePosition = touchPosWorld2D;
                            var direction = releasePosition - _initialPosition;

                            if (direction == Vector2.zero) ShootBallsOnMouseUp(Vector2.up, gm.currentBallVelocity, launchInterval);
                            else ShootBallsOnMouseUp(direction, gm.currentBallVelocity, launchInterval);

                            DisableChooseDirection();

                            //Debug.Log("Process direction " + direction);
                        }
                    }
                }

#if UNITY_EDITOR
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Camera.main.transform.forward);

                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("DragPanel"))
                        {
                            pum.ActivateChooseDirectionIndicators(false);
                            gm.ShowInwardsParticles(mousePos);
                            isDragging = true;
                            _initialPosition = GetCurrentMousePosition().GetValueOrDefault();
                            MoveBallsToMouseDown();
                            _lineRenderer.SetPosition(0, _initialPosition);
                            //_lineRenderer.SetVertexCount(1);
                            _lineRenderer.positionCount = 1;
                            _lineRenderer.enabled = true;
                        }
                        else
                        {
                            print("NO HIT");
                            return;
                        }
                    }
                }

                if (isDragging)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _currentPosition = GetCurrentMousePosition().GetValueOrDefault();
                        //_lineRenderer.SetVertexCount(2);
                        _lineRenderer.positionCount = 2;
                        _lineRenderer.SetPosition(1, _currentPosition);

                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        var releasePosition = GetCurrentMousePosition().GetValueOrDefault();
                        var direction = releasePosition - _initialPosition;

                        if (direction == Vector2.zero) ShootBallsOnMouseUp(Vector2.up, gm.currentBallVelocity, launchInterval);
                        else ShootBallsOnMouseUp(direction, gm.currentBallVelocity, launchInterval);

                        DisableChooseDirection();

                        //Debug.Log("Process direction " + direction);
                    }
                }
#endif
            }
        }
    }

    private Vector2? GetCurrentMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return null;
    }

    public void StartChooseDirectionPowerUp()
    {
        currentActiveBallScripts.Clear();
        currentActiveBallRb2ds.Clear();

        pum.ActivateChooseDirectionIndicators(true);
        gm.ballIsAvailable = false;
        pum.chooseDirectionIsActive = true;

        currentActiveBallScripts.AddRange(gm.ballScripts);
        currentActiveBallRb2ds.AddRange(gm.ballRb2ds);

        Singleton.Instance.saveWasLoaded_Temp = false;

        for (int i = 0; i < currentActiveBallScripts.Count; i++)
        {
            if (gm.ballGos[i].transform.parent != null) { gm.ballGos[i].transform.parent = null; }
            gm.ballScripts[i].hasLaunched = false;
            //gm.ballScripts[i].isLockedToPaddle = false;
        }
    }

    private void MoveBallsToMouseDown()
    {
        gm.ballIsAvailable = false;
        StopAllCoroutines();
        mp.canMove = false;

        for (int i = 0; i < currentActiveBallScripts.Count; i++)
        {
            currentActiveBallScripts[i].transform.position = _initialPosition;
            currentActiveBallRb2ds[i].velocity = Vector2.zero;
        }
    }

    private void ShootBallsOnMouseUp(Vector3 direction, float speed, float interval)
    {        
        if (currentActiveBallScripts.Count > 1)
        {
            Singleton.Instance.lastDirectionX = direction.normalized.x;
            Singleton.Instance.lastDirectionY = direction.normalized.y;
            StartCoroutine(ShootBallsWithInterval(direction.normalized, speed, interval));
        }
        else
        {
            if (currentActiveBallScripts.Count < 1)
            {
                print("ERROR: Number of Current Balls = " + currentActiveBallScripts.Count);
                return;
            }
          
            currentActiveBallRb2ds[0].velocity = direction.normalized * speed;
            currentActiveBallScripts[0].hasLaunched = true;
            currentActiveBallRb2ds.Clear();
            currentActiveBallScripts.Clear();

            allBallsAtZeroSpeed = false;
            pum.chooseDirectionIsActive = false;
            gm.ballIsAvailable = true;            
        }
    } 

    public IEnumerator ShootBallsWithInterval(Vector2 direction, float speed, float interval)
    {
        if(currentActiveBallScripts.Count == 0) { print("No balls to shoot"); yield break; }
        //gm.ballIsAvailable = false;
        pum.chooseDirectionIsActive = false;
        isShooting = true;
        int temp = currentActiveBallScripts.Count;
        //Singleton.Instance.chooseDirectionPowerUpStillShooting = true;

        for (int i = 0; i < temp; i++)
        {
            if (currentActiveBallRb2ds[i] == null) continue;
            currentActiveBallRb2ds[i].velocity = direction * speed;
            currentActiveBallScripts[i].hasLaunched = true;
            currentActiveBallScripts[i].ballIsGonnaBeShot = false;

            if (i == temp - 1)
            {
                currentActiveBallRb2ds.Clear();
                currentActiveBallScripts.Clear();
                allBallsAtZeroSpeed = false;
                //pum.chooseDirectionIsActive = false;
                gm.ballIsAvailable = true;
                isShooting = false;
            }

            yield return new WaitForSeconds(interval);
        }
        //Singleton.Instance.chooseDirectionPowerUpStillShooting = false;
    }


    public void AddLoadedBallsToList(Rigidbody2D rb2d, Ball ballScript)
    {
        currentActiveBallRb2ds.Add(rb2d);
        currentActiveBallScripts.Add(ballScript);
    }

    public void DisableChooseDirection()
    {
        _lineRenderer.enabled = false;
        mp.canMove = true;
        isDragging = false;
    }
}