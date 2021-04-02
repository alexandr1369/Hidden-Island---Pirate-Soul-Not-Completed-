using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.Playables;

public class ShipController : MonoBehaviour
{
    #region Singleton
    public static ShipController instance;
    void Awake()
    {
        instance = this;
    }
    #endregion

    // check for movement permission
    public bool isMovementAllowed;
    public bool isShootingAllowed;

    // check whether ship is moving or not
    public bool isMoving;

    // swipes
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private float minDistanceForSwipe = 20f;

    // current ship speed
    public float animVelocity;

    // bomb prefab(bullet)
    public GameObject bombPrefab;
    private float shootCoolDown;

    // ship movement
    public Transform shipPanel; // obj ship for moving

    public List<float> yPosList; // ship positions for Y
    public List<float> xPosList; // ship positions for X

    public int yCurrentPos; // current y ship pos(0-2)
    public int xCurrentPos; // current x ship pos(0-4)[BOSS]

    public float xShipStartPos; // start ship position(on X)
    public Vector2 worldResolution; // screen resolution

    private int yPaths = 3; // available paths for X
    private int xPaths = 5; // available paths for Y

    private float moveSpeed; // ship movement speed

    void Start()
    {
        Init();
    }
    void Update()
    {
        // decrease shoot cool down
        if(shootCoolDown > 0)
            shootCoolDown -= Time.deltaTime;

        // shoot controller
        if (Input.GetMouseButton(0) && BossManager.instance.currentBoss.IsActive && shootCoolDown <= 0)
            Shoot();

        // movement controller
        if (isMovementAllowed)
        {
            // touch movement contoller
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    fingerUpPosition = touch.position;
                    fingerDownPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDownPosition = touch.position;
                    DetectSwipe();
                }
            }

            // поднятие вверх коробля
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveShipUp();
            }
            // погружение вниз коробля
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveShipDown();
            }
            // движение влево корабля
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (yCurrentPos == yPaths - 1)
                    MoveShipLeft();
            }
            // движение вправо корабля
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (yCurrentPos == yPaths - 1)
                    MoveShipRight();
            }
        }

        // check for current moving(какая-то хуйня с float, не равняется друг другу при одинаковых значениях)
        //isMoving = (yPosList.FindIndex(t => t.ToString("0.00") == shipPanel.position.y.ToString("0.00")) >= 0) ? false : true;
        isMoving = (yPosList.FindIndex(t => t == shipPanel.position.y) >= 0) ? false : true;
    }

    #region Utils

    /// <summary>
    /// Init settings of ship transitions
    /// </summary>
    void Init()
    {
        // allow movement and forbid shooting
        isMovementAllowed = true;
        isShootingAllowed = false;

        // getting the transform of shipPanel to move it in future
        shipPanel = GameObject.Find("ShipPanel").transform;

        // init settings
        Vector2 worldRes = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));    
        yPosList = new List<float>() { worldRes.y / 1.65f, 0f, -worldRes.y / 1.65f };
        // N = 10
        // S = N / 11 = N/11(каждый сектор)
        // part pos1 part pos2 part |pos3| part pos4 part pos5 part (11)
        // get 9 parts of width
        float part = worldRes.x * 2 / 11; // 10 / 11 ~ 0.9
        xPosList = new List<float>() { -4 * part, -2 * part, 0, 2 * part, 4 * part };

        // set current position
        yCurrentPos = 1;
        xCurrentPos = 0;

        // get world map resolution
        worldResolution = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        // get ship position
        xShipStartPos = -worldResolution.x * .8f;

        // set ship pos
        shipPanel.position = new Vector3(xShipStartPos, yPosList[yCurrentPos], shipPanel.position.z);

        // set anim and move speed
        SetVelocity(PlayerManager.instance.currentMapStage.minVelocity);
    }
    // update anim and move speed
    public void SetVelocity(float velocity)
    {
        animVelocity = velocity;
        ShipManager.instance.SetAnimVelocity(animVelocity);
        moveSpeed = animVelocity * 4;
    }
    // move up animation with curve
    public void MoveShipUp()
    {
        if (yCurrentPos != 0)
        {
            // get down position
            float upPosY = yPosList[yCurrentPos - 1];

            // move to it
            iTween.MoveTo(shipPanel.gameObject, iTween.Hash(
                "position", new Vector3(shipPanel.position.x, upPosY, shipPanel.position.z),
                "speed", moveSpeed,
                "easetype", iTween.EaseType.linear
            ));

            // increment current position
            yCurrentPos--;
        }

    }
    // move down animation with curve
    public void MoveShipDown()
    {
        if (yCurrentPos != yPaths - 1)
        {
            // get down position
            float downPosY = yPosList[yCurrentPos + 1];

            // move to it
            iTween.MoveTo(shipPanel.gameObject, iTween.Hash(
                "position", new Vector3(shipPanel.position.x, downPosY, shipPanel.position.z),
                "speed", moveSpeed,
                "easetype", iTween.EaseType.linear
            ));

            // increment current position
            yCurrentPos++;
        }
    }
    // move left animation with curve
    public void MoveShipLeft()
    {
        if (xCurrentPos != 0)
        {
            // get down position
            float leftPosX = xPosList[xCurrentPos - 1];

            // move to it
            iTween.MoveTo(shipPanel.gameObject, iTween.Hash(
                "position", new Vector3(leftPosX, shipPanel.position.y, shipPanel.position.z),
                "speed", moveSpeed,
                "easetype", iTween.EaseType.linear
            ));

            // increment current position
            xCurrentPos--;
        }
    }
    // move right animation with curve
    public void MoveShipRight()
    {
        if (xCurrentPos != xPaths - 1)
        {
            // get down position
            float rightPosX = xPosList[xCurrentPos + 1];

            // move to it
            iTween.MoveTo(shipPanel.gameObject, iTween.Hash(
                "position", new Vector3(rightPosX, shipPanel.position.y, shipPanel.position.z),
                "speed", moveSpeed,
                "easetype", iTween.EaseType.linear
            ));

            // increment current position
            xCurrentPos++;
        }
    }
    // shooting(boss stage)
    public void Shoot()
    {
        // if movement is not allowed -> can't shot
        if (!isShootingAllowed) return;

        // spawn bomb prefab
        Vector3 bombPosition = shipPanel.position;
        GameObject bomb = Instantiate(bombPrefab, bombPosition, Quaternion.identity, GameObject.Find("BackUI").transform);

        // begin bomb shooting animation
        bomb.GetComponentInChildren<ShipAttack>().Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // set shoot cool down
        shootCoolDown = 1f;
    }

    #endregion

    #region swipe utils

    private void DetectSwipe()
    {
        SwipeDirection dir = GetSwipeDirection();
        if (dir == SwipeDirection.Up && yCurrentPos != 0)
        {
            MoveShipUp();
        }
        else if (dir == SwipeDirection.Down && yCurrentPos != yPaths - 1)
        {
            MoveShipDown();
        }
        else if (dir == SwipeDirection.Left && xCurrentPos != 0)
        {
            MoveShipLeft();
        }
        else if (dir == SwipeDirection.Right && xCurrentPos != xPaths - 1)
        {
            MoveShipRight();
        }
    }
    private bool isSwipe()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y) > minDistanceForSwipe ||
            Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) > minDistanceForSwipe;
    }
    private SwipeDirection GetSwipeDirection()
    {
        if (isSwipe())
        {
            if (BossManager.instance.currentBoss.IsActive)
            {
                if (fingerDownPosition.y - fingerUpPosition.y > 0)
                    return SwipeDirection.Up;
                else if (fingerDownPosition.y - fingerUpPosition.y < 0)
                    return SwipeDirection.Down;
            }
            else
            {
                if (fingerDownPosition.x - fingerUpPosition.x > 0)
                    return SwipeDirection.Right;
                else if (fingerDownPosition.x - fingerUpPosition.x < 0)
                    return SwipeDirection.Left;
            }
        }
        return SwipeDirection.None;
    }

    #endregion
}
enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right,
    None
}
enum ShipMovementDirection
{
    Up,
    Down
}
