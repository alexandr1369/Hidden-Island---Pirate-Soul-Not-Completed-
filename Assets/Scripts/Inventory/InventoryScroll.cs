using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScroll : MonoBehaviour
{
    #region Singleton
    public static InventoryScroll instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Image infoPanel; // information panel(selected or not)
    public GameObject selectBtn; // select button(active or not)

    public GameObject[] visibleShipPanels; // -3 -2 -1  0  +1 +2 3

    private List<ShipInfo> ownedShips; // bought ships
    private int currentIndex; // current index

    private Animator animator; // current animator

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private float minDistanceForSwipe = 0f;

    private void Start()
    {
        // data loading
        LoadData();

        // getting main inventory animator
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // TODO: добавить управление свапами
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;

                // TODO: реализовать красивое движение картинок
                // на текущий момент срабатывает двойной вызов триггера, пофиксить
                DetectSwipe();
            }
        }

        // для теста управление стрелками на клавиатуре
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentIndex == 0 || animator.IsInTransition(0)) return;
            animator.SetTrigger("MoveLeft");
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentIndex == ownedShips.Count - 1 || animator.IsInTransition(0)) return;
            animator.SetTrigger("MoveRight");
        }
    }
    // set active ship panel as selected ship
    public void MakeCurrentShipSelected()
    {
        // set current ship
        PlayerManager.instance.SetCurrentShip(ownedShips[currentIndex].label);

        // update info panel display
        UpdateInfoPanel();

        // update Ship Display in Lobby
        ShipManager.instance.LoadShipData();
    }
    // initial settings
    public void LoadData()
    {
        // get owned ships
        ownedShips = PlayerManager.instance.GetPurchasedShips();
        ownedShips.Sort((x, y) => string.Compare(x.label, y.label));

        // get current ship index
        for (int i = 0; i < ownedShips.Count; i++)
            if (ownedShips[i] == PlayerManager.instance.currentShip)
                currentIndex = i;

        // деактивировать кнопку(т.к. текущая панель - выбранный корабль)
        selectBtn.GetComponent<InteractableButton>().isInteractable = false;
            
        UpdateData();
    }

    // будет вызываться на конце анимации пролистывания
    // side - сторона скролла
    // обновляет информацию о короблях(иконки), Info Panel и кнопку Select
    private void MoveInventoryScroll(string side)
    {
        if (side == "right")
            currentIndex -= 1;
        else if (side == "left")
            currentIndex += 1;
        else return;
    }
    // go through all owned ships and load data
    private void UpdateData()
    {
        // i - visible panel array
        // j - owned ships
        for (int i = 0, j = currentIndex - 3; i <= 6; i++, j++)
        {
            try
            {
                ShipInfo ship = ownedShips[j];

                visibleShipPanels[i].GetComponent<InventorySlot>().ship = ship;
                visibleShipPanels[i].GetComponent<InventorySlot>().UpdateInfo();
                visibleShipPanels[i].SetActive(true);
            }
            catch
            {
                visibleShipPanels[i].SetActive(false);
            }
        }
    }
    private void UpdateInfoPanel()
    {
        // если текущая центральная панель с короблем является выбранным кораблем
        if(ownedShips[currentIndex] == PlayerManager.instance.currentShip)
        {
            // деактивироать кнопку выбора
            selectBtn.GetComponent<InteractableButton>().isInteractable = false;
            selectBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);

            // изменить информацию
            infoPanel.sprite = Resources.Load<Sprite>(@"Textures\Inventory\SelectedText");
        }
        else
        {
            // активироать кнопку выбора
            selectBtn.GetComponent<InteractableButton>().isInteractable = true;
            selectBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            // изменить информацию
            infoPanel.sprite = Resources.Load<Sprite>(@"Textures\Inventory\SelectText");
        }
    }
    // swipe utils
    private void DetectSwipe()
    {
        SwipeDirection dir = GetSwipeDirection();
        if (dir == SwipeDirection.Left && currentIndex != ownedShips.Count - 1 && !animator.IsInTransition(0))
        {
            animator.SetTrigger("MoveRight");
        }
        else if (dir == SwipeDirection.Right && currentIndex != 0 && !animator.IsInTransition(0))
        {
            animator.SetTrigger("MoveLeft");
        }
    }
    private bool isSwipe()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x) > minDistanceForSwipe;
    }
    private SwipeDirection GetSwipeDirection()
    {
        if (isSwipe())
            return fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        return SwipeDirection.None;
    }
}
