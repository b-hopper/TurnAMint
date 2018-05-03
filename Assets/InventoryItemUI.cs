using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour {
    public Sprite smallSprite, largeSprite, emptySprite;
    public Text countText;
    public GameObject detailGO;

    InventoryUI invUI;

    BoxCollider col;          // For some reason the GetComponent isn't working?

    RectTransform rTransform;

    Vector3 originalPosition;
    public Vector3 offset;

    bool cursorIsHolding;

    Color emptyColor, fullColor;

    internal ItemBase occupyingItem;

    public bool isOccupied;

    public int amount;

    Image thisImage, largeImage;
    public Color mouseoverColor;

    private void Awake()
    {
        invUI = GetComponentInParent<InventoryUI>();
        rTransform = GetComponent<RectTransform>();
        thisImage = GetComponent<Image>();
        emptyColor = thisImage.color;
        fullColor = new Color(1, 1, 1, 0.396f);
        originalPosition = rTransform.localPosition;
        col = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        invUI.OnItemPickedUpByCursor += OnItemPickedUpByCursor;
        invUI.OnItemDroppedByCursor += OnItemDroppedByCursor;
    }

    private void Update()
    {
        if (!cursorIsHolding) return;
        
        rTransform.position = GameManager.Instance.CamRefs.mainCam.ScreenToWorldPoint(Input.mousePosition - offset);
        if (Input.GetMouseButtonUp(0))
        {
            cursorIsHolding = false;
            rTransform.localPosition = originalPosition;
            invUI.TriggerItemMove(occupyingItem);
        }
        
    }

    void OnItemPickedUpByCursor()
    {
        if (col == null) return;

        col.enabled = false;
    }

    void OnItemDroppedByCursor()
    {
        if (col == null) return;

        col.enabled = true;
    }

    public void AddItemToSlot(ItemBase item)
    {
        amount += item.amount;
        SetImage(item.UI.SmallSprite, item.UI.LargeSprite);

        isOccupied = true;
        occupyingItem = item;

        if (amount > 1) countText.text = amount.ToString();
        else countText.text = "";

        col.enabled = true;
    }

    public void ClearItemFromSlot()
    {
        if (isOccupied)
        {
            ClearImage();
            amount = 0;
            countText.text = "";
            isOccupied = false;
            occupyingItem = null;
            col.enabled = false;
        }
    }

    public void RemoveSomeAmount(int amountRemoved)
    {
        amount -= amountRemoved;

        if (amount > 1) countText.text = amount.ToString();
        else countText.text = "";
    }

    void SetImage(Sprite newSmall, Sprite newLarge)
    {        
        if (newSmall != null)
        {
            Debug.Log(thisImage);
            thisImage.sprite = newSmall;
            thisImage.color = fullColor;
        }
        
        /*
        if (newLarge != null) largeSprite = newLarge;        
        else largeSprite = emptySprite;
        largeImage.sprite = largeSprite;*/
    }
    public void ClearImage()
    {
        smallSprite = null;
        largeSprite = null;
        thisImage.color = emptyColor;
    }

    public void OnMouseOver()
    {
        //detailGO.SetActive(!isEnding);
    }

    private void OnMouseEnter()
    {
        thisImage.color = isOccupied ? Color.white : mouseoverColor;
    }

    private void OnMouseExit()
    {
        thisImage.color = isOccupied ? fullColor : emptyColor;
    }

    private void OnMouseDown()
    {
        offset = Input.mousePosition - GameManager.Instance.CamRefs.mainCam.WorldToScreenPoint(rTransform.position);
        cursorIsHolding = true;
        invUI.SetCurrentlyHoldingObject(occupyingItem);
    }
}
