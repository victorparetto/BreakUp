using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStackManager : MonoBehaviour
{
    public static Stack<RectTransform> currentRectTransforms = new Stack<RectTransform>();
    public static Stack<bool> currentRtIsMoved = new Stack<bool>();

    public static RectTransform GetTopRectTransform()
    {
        return currentRectTransforms.Peek();
    }

    public static bool GetTopRtIsMoved()
    {
        return currentRtIsMoved.Peek();
    }

    public static void ClearStacks()
    {
        currentRectTransforms.Clear();
        currentRtIsMoved.Clear();
    }

    public static void PopOfEveryStack()
    {
        currentRectTransforms.Pop();
        currentRtIsMoved.Pop();
    }
    /*
    public void BackButton()
    {
        if (UIStackManager.currentRectTransforms.Count <= 0) return;

        if (UIStackManager.GetTopRtIsMoved()) StartCoroutine(MoveUISmoothly(UIStackManager.GetTopRectTransform(), HideBaseOnRT()));
        UIStackManager.GetTopRectTransform().gameObject.SetActive(false);
        UIStackManager.PopOfEveryStack();

        if (UIStackManager.GetTopRectTransform().gameObject.activeInHierarchy) return;

        //With a NEW item in the TOP of the stack, apply same logic as before to show it depending on if its moved/scaled.
        UIStackManager.GetTopRectTransform().gameObject.SetActive(true);

        if (UIStackManager.GetTopRectTransform() == rtPaddleSkins) currentPaddleShown.SetActive(true);
        else if (UIStackManager.GetTopRectTransform() == rtBallSkins) currentBallShown.SetActive(true);
        else
        {
            currentPaddleShown.SetActive(false);
            currentBallShown.SetActive(false);
        }

        if (UIStackManager.GetTopRtIsMoved())
        {
            StartCoroutine(MoveUISmoothly(UIStackManager.GetTopRectTransform(), Vector2.zero)); //replace Vecto2.Zero with the function to decide with Vector to use
        }
        else
        {
            StartCoroutine(ScaleUISmoothly(UIStackManager.GetTopRectTransform(), true, false));
        }

        //if current TOP of the Stack is rtMainPanel, hide BackPanel(contains backButton)
        if (UIStackManager.GetTopRectTransform() == rtMainPanel || UIStackManager.GetTopRectTransform() == rtCustomizePanel ||
            UIStackManager.GetTopRectTransform() == rtAchievementsPanel)
        {
            StartCoroutine(MoveUISmoothly(rtTabGroup, rtTabGroupPositionToGo));
            StartCoroutine(ScaleUISmoothly(rtBackPanel, false, false));
        }
        */
    }
