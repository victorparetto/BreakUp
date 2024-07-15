using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallSkinCollapsableBehaviour : MonoBehaviour
{
    MenuManager mm;
    Animator anim;

    [Header("Important Variables")]
    [SerializeField] int ballIndex = 0;
    [SerializeField] GameObject[] ballPrefabs = null;
    [SerializeField] bool isBuyable = false;
    [SerializeField] int cost = 0;

    int currentBallImageID = 0;
    int alternateBall1ImageID = 1;
    int alternateBall2ImageID = 2;

    [SerializeField] bool isEquipped = false;
    //[SerializeField] bool hasAlternateSkins = true;
    public bool isExpanded = false;

    Sprite tempSprite = null;

    [Header("Permanent Variables")]
    [SerializeField] Image[] allBallImages = null;
    Sprite[] allBallSprites = null;
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
        allBallSprites = new Sprite[allBallImages.Length];
        for (int i = 0; i < allBallImages.Length; i++)
        {
            allBallSprites[i] = allBallImages[i].sprite;
        }

        if (isBuyable)
        {
            costText.text = "<size=70><sprite=0></size>" + cost.ToString();

            if (MenuDataManager.Instance.boughtPaddles[ballIndex])
            {
                ActivateEquipGOsForBuyables();
            }
        }

        CheckIfIsCurrentlyEquipped(); //Comes after Load on MenuManager
    }

    public void CheckIfIsCurrentlyEquipped()
    {
        if(MenuDataManager.Instance.currentBallIndex == ballIndex)
        {
            equippedImage.sprite = equippedSprites[1];
            isEquipped = true;
            mm.ChangeCurrentEquippedBallBehaviour(this);

            if (MenuDataManager.Instance.currentBallColorID == 0)
            {
                CustomizableManager.SetCurrentBallGameObject(ballPrefabs[0]);
            }
            else if(MenuDataManager.Instance.currentBallColorID == 1)
            {
                if (ballPrefabs[1] != null)
                {
                    int tempID;

                    tempSprite = allBallSprites[currentBallImageID];
                    tempID = currentBallImageID;
                    currentBallImageID = alternateBall1ImageID;
                    alternateBall1ImageID = tempID;

                    allBallImages[0].sprite = allBallSprites[currentBallImageID];
                    allBallImages[1].sprite = tempSprite;

                    CustomizableManager.SetCurrentBallGameObject(ballPrefabs[currentBallImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Ball Index");
                    CustomizableManager.SetCurrentBallGameObject(ballPrefabs[0]);
                }
            } 
            else if(MenuDataManager.Instance.currentBallColorID == 2)
            {
                if(ballPrefabs[2] != null)
                {
                    int tempID;

                    tempSprite = allBallSprites[currentBallImageID];
                    tempID = currentBallImageID;
                    currentBallImageID = alternateBall2ImageID;
                    alternateBall2ImageID = tempID;

                    allBallImages[0].sprite = allBallSprites[currentBallImageID];
                    allBallImages[1].sprite = tempSprite;

                    CustomizableManager.SetCurrentBallGameObject(ballPrefabs[currentBallImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Ball Index");
                    CustomizableManager.SetCurrentBallGameObject(ballPrefabs[0]);
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

            mm.ChangeOpenBallBehaviour(this);
        }
    }

    public void EquipButton()
    {
        if (mm.IsTheCurrentBallBehaviour(this)) return;

        equippedImage.sprite = equippedSprites[1];
        isEquipped = true;
        //mm.currentPaddleIndex = paddleIndex;

        mm.ChangeCurrentEquippedBallBehaviour(this);

        CustomizableManager.SetCurrentBallGameObject(ballPrefabs[currentBallImageID]);
        MenuDataManager.Instance.currentBallIndex = ballIndex;
        MenuDataManager.Instance.currentBallColorID = currentBallImageID;
        MenuDataManager.Instance.Save();
    }

    public void UnequipBall()
    {
        equippedImage.sprite = equippedSprites[0];
        isEquipped = false;
    }

    public void AlternateSkin1Button()
    {
        int tempID;

        tempSprite = allBallSprites[currentBallImageID];
        tempID = currentBallImageID;
        currentBallImageID = alternateBall1ImageID;
        alternateBall1ImageID = tempID;

        allBallImages[0].sprite = allBallSprites[currentBallImageID];
        allBallImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            CustomizableManager.SetCurrentBallGameObject(ballPrefabs[currentBallImageID]);            
            MenuDataManager.Instance.currentBallColorID = currentBallImageID;
            MenuDataManager.Instance.Save();
        }
    }

    public void AlternateSkin2Button()
    {
        int tempID;

        tempSprite = allBallSprites[currentBallImageID];
        tempID = currentBallImageID;
        currentBallImageID = alternateBall2ImageID;
        alternateBall2ImageID = tempID;

        allBallImages[0].sprite = allBallSprites[currentBallImageID];
        allBallImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            CustomizableManager.SetCurrentBallGameObject(ballPrefabs[currentBallImageID]);
            MenuDataManager.Instance.currentBallColorID = currentBallImageID;
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
            if (MenuDataManager.Instance.currentMoney >= cost)
            {
                MenuDataManager.Instance.boughtPaddles[ballIndex] = true;
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
