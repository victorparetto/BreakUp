using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardChestBehaviour : MonoBehaviour
{
    MenuManager mm;

    [SerializeField] float transitionDuration = 2f;
    [SerializeField] Button panelButton;

    [Header("Chest Animation")]
    [SerializeField] RectTransform chestAnimationRT = null;
    [SerializeField] Animator chestAnim;
    [SerializeField] Button chestBtn;

    [SerializeField] GameObject chestObjects = null;
    [SerializeField] GameObject pulseParticles = null;

    [Header("Reward Animation")]
    [SerializeField] GameObject rewardObjects = null;
    [SerializeField] RectTransform moneyPrizeAnimationRT = null;
    [SerializeField] TMP_Text rewardAmount;
    [SerializeField] bool isRewardScaling = false;

    Vector2 chestAnimationScaleStart;
    Vector2 rewardAnimationScaleStart;

    private void Awake()
    {
        mm = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        chestBtn.interactable = false;

        chestAnimationScaleStart = chestAnimationRT.localScale;
        rewardAnimationScaleStart = moneyPrizeAnimationRT.localScale;
    }

    private void OnEnable()
    {
        chestAnimationRT.localScale = chestAnimationScaleStart;
        moneyPrizeAnimationRT.localScale = rewardAnimationScaleStart;

        rewardAmount.text = mm.selectedAchievementMoneyReward.ToString();
        panelButton.interactable = false;
        isRewardScaling = false;
        chestBtn.interactable = false;
        chestObjects.SetActive(true);
        pulseParticles.SetActive(true);

        StartCoroutine(MoveUISmoothly(chestAnimationRT, Vector3.zero));
        StartCoroutine(ScaleUISmoothly(chestAnimationRT));
    }

    private void Update()
    {
        if (chestAnim.GetCurrentAnimatorStateInfo(0).IsName("Exploded"))
        {
            isRewardScaling = true;
            rewardObjects.SetActive(true);
            StartCoroutine(ScaleUISmoothly(moneyPrizeAnimationRT));
        }

        if (chestAnim.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
        {
            print("Animation Finished");
            chestObjects.SetActive(false);
        }
    }

    public IEnumerator MoveUISmoothly(RectTransform rt, Vector3 target)
    {
        float currentTime = 0;
        //float duration = 0.5f;

        Vector3 origin = rt.anchoredPosition;

        while (currentTime <= transitionDuration)
        {
            currentTime += Time.deltaTime;
            float percent = Mathf.Clamp01(currentTime / transitionDuration);

            float smooth = percent * percent * (3f - 2f * percent);
            rt.anchoredPosition = Vector3.Lerp(origin, target, smooth);
            yield return null;
        }
    }

    public IEnumerator ScaleUISmoothly(RectTransform rt)
    {
        float currentTime = 0;
        Vector3 origin;

        origin = rt.localScale;

        while (currentTime <= transitionDuration)
        {
            currentTime += Time.deltaTime;
            float percent = Mathf.Clamp01(currentTime / transitionDuration);

            float smooth = percent * percent * (3f - 2f * percent);
            rt.localScale = Vector3.Lerp(origin, Vector3.one, smooth);
            yield return null;
        }

        chestBtn.interactable = true;

        if (isRewardScaling)
        {
            isRewardScaling = false;
            panelButton.interactable = true;
        }
    }

    public void OpenChest()
    {
        chestAnim.SetTrigger("open");
    }
}
