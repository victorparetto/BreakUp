using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrailSkinCollapsableBehaviour : MonoBehaviour
{
    MenuManager mm;
    Animator anim;

    [Header("Important Variables")]
    [SerializeField] int trailIndex = 0;
    [SerializeField] GameObject[] trailPrefabs = null;
    [SerializeField] bool isBuyable = false;
    [SerializeField] int cost = 0;

    int currentTrailImageID = 0;
    int alternateTrail1ImageID = 1;
    int alternateTrail2ImageID = 2;

    [SerializeField] bool isEquipped = false;
    //[SerializeField] bool hasAlternateSkins = true;
    public bool isExpanded = false;

    Sprite tempSprite = null;

    [Header("Permanent Variables")]
    [SerializeField] Image[] allTrailImages = null;
    Sprite[] allTrailSprites = null;
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
        allTrailSprites = new Sprite[allTrailImages.Length];
        for (int i = 0; i < allTrailImages.Length; i++)
        {
            allTrailSprites[i] = allTrailImages[i].sprite;
        }

        if (isBuyable)
        {
            costText.text = "<size=70><sprite=0></size>" + cost.ToString();

            if (MenuDataManager.Instance.boughtPaddles[trailIndex])
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
        if(MenuDataManager.Instance.currentTrailIndex == trailIndex)
        {
            equippedImage.sprite = equippedSprites[1];
            isEquipped = true;
            mm.ChangeCurrentEquippedTrailBehaviour(this);

            if (MenuDataManager.Instance.currentTrailColorID == 0)
            {
                if (trailPrefabs.Length > 0)
                    CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[0]);
                else
                    CustomizableManager.SetCurrentTrailGameObject(null);
            }
            else if(MenuDataManager.Instance.currentTrailColorID == 1)
            {
                if (trailPrefabs[1] != null)
                {
                    int tempID;

                    tempSprite = allTrailSprites[currentTrailImageID];
                    tempID = currentTrailImageID;
                    currentTrailImageID = alternateTrail1ImageID;
                    alternateTrail1ImageID = tempID;

                    allTrailImages[0].sprite = allTrailSprites[currentTrailImageID];
                    allTrailImages[1].sprite = tempSprite;

                    if (trailPrefabs.Length > 0)
                        CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[currentTrailImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Trail Index");
                    if (trailPrefabs.Length > 0)
                        CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[0]);
                }
            } 
            else if(MenuDataManager.Instance.currentTrailColorID == 2)
            {
                if(trailPrefabs[2] != null)
                {
                    int tempID;

                    tempSprite = allTrailSprites[currentTrailImageID];
                    tempID = currentTrailImageID;
                    currentTrailImageID = alternateTrail2ImageID;
                    alternateTrail2ImageID = tempID;

                    allTrailImages[0].sprite = allTrailSprites[currentTrailImageID];
                    allTrailImages[1].sprite = tempSprite;

                    if (trailPrefabs.Length > 0)
                        CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[currentTrailImageID]);
                }
                else
                {
                    Debug.LogError("There is no Alternate Color for this Trail Index");
                    if (trailPrefabs.Length > 0)
                        CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[0]);
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

            mm.ChangeOpenTrailBehaviour(this);
        }
    }

    public void EquipButton()
    {
        if (mm.IsTheCurrentTrailBehaviour(this)) return;

        equippedImage.sprite = equippedSprites[1];
        isEquipped = true;
        //mm.currentPaddleIndex = paddleIndex;

        mm.ChangeCurrentEquippedTrailBehaviour(this);

        if (trailPrefabs.Length > 0)
            CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[currentTrailImageID]);
        else
            CustomizableManager.SetCurrentTrailGameObject(null);

        MenuDataManager.Instance.currentTrailIndex = trailIndex;
        MenuDataManager.Instance.currentTrailColorID = currentTrailImageID;
        MenuDataManager.Instance.Save();
    }

    public void UnequipTrail()
    {
        equippedImage.sprite = equippedSprites[0];
        isEquipped = false;
    }

    public void AlternateSkin1Button()
    {
        int tempID;

        tempSprite = allTrailSprites[currentTrailImageID];
        tempID = currentTrailImageID;
        currentTrailImageID = alternateTrail1ImageID;
        alternateTrail1ImageID = tempID;

        allTrailImages[0].sprite = allTrailSprites[currentTrailImageID];
        allTrailImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            if (trailPrefabs.Length > 0)
                CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[currentTrailImageID]);       
            else
                CustomizableManager.SetCurrentTrailGameObject(null);
            MenuDataManager.Instance.currentTrailColorID = currentTrailImageID;
            MenuDataManager.Instance.Save();
        }
    }

    public void AlternateSkin2Button()
    {
        int tempID;

        tempSprite = allTrailSprites[currentTrailImageID];
        tempID = currentTrailImageID;
        currentTrailImageID = alternateTrail2ImageID;
        alternateTrail2ImageID = tempID;

        allTrailImages[0].sprite = allTrailSprites[currentTrailImageID];
        allTrailImages[1].sprite = tempSprite;

        if (isEquipped)
        {
            if (trailPrefabs.Length > 0)
                CustomizableManager.SetCurrentTrailGameObject(trailPrefabs[currentTrailImageID]);
            MenuDataManager.Instance.currentTrailColorID = currentTrailImageID;
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
                MenuDataManager.Instance.boughtPaddles[trailIndex] = true;
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
