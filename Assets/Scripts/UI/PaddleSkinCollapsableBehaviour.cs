using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaddleSkinCollapsableBehaviour : MonoBehaviour
{
    MenuManager mm;
    Animator anim;

    [Header("Important Variables")]
    [SerializeField] int paddleIndex = 0;
    [SerializeField] GameObject[] paddlesPrefabs = null;
    [SerializeField] bool isBuyable = false;
    [SerializeField] int cost = 0;

    int currentPaddleImageID = 0;
    int alternatePaddle1ImageID = 1;
    int alternatePaddle2ImageID = 2;

    [SerializeField] bool isEquipped = false;
    //[SerializeField] bool hasAlternateSkins = true;
    public bool isExpanded = false;

    Sprite tempSprite = null;

    [Header("Permanent Variables")]
    [SerializeField] Image[] allPaddleImages = null;
    Sprite[] allpaddleSprites = null;
    [SerializeField] Image expandedCollapsedImage = null;
    [SerializeField] Sprite[] expandArrowSprites = null;
    [SerializeField] Image equippedImage = null;
    [SerializeField] Sprite[] equippedSprites = null;
    [SerializeField] TMP_Text costText = null;

    [Header("Buy Variables")]
    [SerializeField] Animator buyButAnim;
    [SerializeField] bool isOnConfirmation = false;
    [SerializeField] GameObject buyGos = null;
    [SerializeField] GameObject equipGos = null;
    [SerializeField] GameObject buyText = null;
    [SerializeField] GameObject confirmationText = null;

    void Awake()
    {
        anim = GetComponent<Animator>();
        mm = GameObject.Find("MenuManager").GetComponent<MenuManager>();
    }

    void Start()
    {
        allpaddleSprites = new Sprite[allPaddleImages.Length];
        for (int i = 0; i < allPaddleImages.Length; i++)
        {
            allpaddleSprites[i] = allPaddleImages[i].sprite;
        }

        if (isBuyable)
        {
            costText.text = "<size=70><sprite=0></size>" + cost.ToString();

            if (MenuDataManager.Instance.boughtPaddles[paddleIndex])
            {
                ActivateEquipGOsForBuyables();
            }
        }

        CheckIfIsCurrentlyEquipped(); //Comes after Load on MenuManager
        ForceCanvasUpdate();
    }

    public void ForceCanvasUpdate()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void CheckIfIsCurrentlyEquipped()
    {
        if(MenuDataManager.Instance.currentPaddleIndex == paddleIndex)
        {
            equippedImage.sprite = equippedSprites[1];
            isEquipped = true;
            mm.ChangeCurrentEquippedPaddleBehaviour(this);

            if (MenuDataManager.Instance.currentPaddleColorID == 0)
            {
                CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[0]);
            }
            else if(MenuDataManager.Instance.currentPaddleColorID == 1)
            {
                if (paddlesPrefabs[1] != null)
                {
                    int tempID;

                    tempSprite = allpaddleSprites[currentPaddleImageID];
                    tempID = currentPaddleImageID;
                    currentPaddleImageID = alternatePaddle1ImageID;
                    alternatePaddle1ImageID = tempID;

                    allPaddleImages[0].sprite = allpaddleSprites[currentPaddleImageID];
                    allPaddleImages[1].sprite = tempSprite;

                    CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[currentPaddleImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Paddle Index");
                    CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[0]);
                }
            } 
            else if(MenuDataManager.Instance.currentPaddleColorID == 2)
            {
                if(paddlesPrefabs[2] != null)
                {
                    int tempID;

                    tempSprite = allpaddleSprites[currentPaddleImageID];
                    tempID = currentPaddleImageID;
                    currentPaddleImageID = alternatePaddle2ImageID;
                    alternatePaddle2ImageID = tempID;

                    allPaddleImages[0].sprite = allpaddleSprites[currentPaddleImageID];
                    allPaddleImages[1].sprite = tempSprite;

                    CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[currentPaddleImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Paddle Index");
                    CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[0]);
                }
            }
        }
    }

    public void ExpandOrCollapse()
    {
        if (isExpanded)
        {
            anim.SetTrigger("collapse");
            isExpanded = false;
            expandedCollapsedImage.sprite = expandArrowSprites[0];        
        }
        else
        {
            anim.SetTrigger("expand");
            isExpanded = true;
            expandedCollapsedImage.sprite = expandArrowSprites[1];

            mm.ChangeOpenPaddleBehaviour(this);
        }
    }

    public void EquipButton()
    {
        if (mm.IsTheCurrentPaddleBehaviour(this)) return;

        equippedImage.sprite = equippedSprites[1];
        isEquipped = true;
        //mm.currentPaddleIndex = paddleIndex;

        mm.ChangeCurrentEquippedPaddleBehaviour(this);

        CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[currentPaddleImageID]);
        MenuDataManager.Instance.currentPaddleIndex = paddleIndex;
        MenuDataManager.Instance.currentPaddleColorID = currentPaddleImageID;
        MenuDataManager.Instance.Save();
    }

    public void UnequipPaddle()
    {
        equippedImage.sprite = equippedSprites[0];
        isEquipped = false;
    }

    public void AlternateSkin1Button()
    {
        int tempID;

        tempSprite = allpaddleSprites[currentPaddleImageID];
        tempID = currentPaddleImageID;
        currentPaddleImageID = alternatePaddle1ImageID;
        alternatePaddle1ImageID = tempID;

        allPaddleImages[0].sprite = allpaddleSprites[currentPaddleImageID];
        allPaddleImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[currentPaddleImageID]);            
            MenuDataManager.Instance.currentPaddleColorID = currentPaddleImageID;
            MenuDataManager.Instance.Save();
        }
    }

    public void AlternateSkin2Button()
    {
        int tempID;

        tempSprite = allpaddleSprites[currentPaddleImageID];
        tempID = currentPaddleImageID;
        currentPaddleImageID = alternatePaddle2ImageID;
        alternatePaddle2ImageID = tempID;

        allPaddleImages[0].sprite = allpaddleSprites[currentPaddleImageID];
        allPaddleImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            CustomizableManager.SetCurrentPaddleGameObject(paddlesPrefabs[currentPaddleImageID]);
            MenuDataManager.Instance.currentPaddleColorID = currentPaddleImageID;
            MenuDataManager.Instance.Save();
        }
    }

    public void ActivateEquipGOsForBuyables()
    {
        buyGos.SetActive(false);
        equipGos.SetActive(true);
    }

    public void ActivateConfirmationBeforeBuy(bool activate)
    {
        if (activate)
        {
            buyText.SetActive(false);
            confirmationText.SetActive(true);
            isOnConfirmation = true;
        }
        else
        {
            buyText.SetActive(true);
            confirmationText.SetActive(false);
            isOnConfirmation = false;
        }
    }

    public void BuyButton()
    {
        if (isOnConfirmation)
        {
            if(MenuDataManager.Instance.currentMoney >= cost)
            {
                MenuDataManager.Instance.boughtPaddles[paddleIndex] = true;
                MenuDataManager.Instance.currentMoney -= cost;
                MenuDataManager.Instance.Save();

                mm.UpdateMoneyTopBar();
                buyButAnim.enabled = true;
                buyButAnim.SetTrigger("bought");
            }
            else
            {
                //Play buzzing sound
            }
        }
        else
        {
            ActivateConfirmationBeforeBuy(true);
        }
        //MenuDataManager.Instance.currentMoney -= cost;
    }
}
