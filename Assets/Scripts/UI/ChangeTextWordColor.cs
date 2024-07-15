using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeTextWordColor : MonoBehaviour
{
    //Code for Change Color Overtime
    [Header("Color over time variables")]
    [SerializeField] bool changeColorOverTime;
    [SerializeField] float timeToChangeColor = 1f;
    List<Color32> tempColorList = new List<Color32>();
    Color32[] finalColors;
    float currentTimer = 0;

    //Code to change the color once
    [Header("Important Variables")]
    public TMP_Text _text;
    public int wordIndex;

    public Color32[] myColor32 = new Color32[4];
    TMP_WordInfo info;


    private void Start()
    {
        _text.ForceMeshUpdate();
        wordIndex = GetIndexOfWord("RNG");
        tempColorList.AddRange(myColor32);
        finalColors = new Color32[myColor32.Length];
        RandomizeColor32();
        currentTimer = timeToChangeColor;
    }

    void Update()
    {        
        if (changeColorOverTime)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= timeToChangeColor)
            {
                info = _text.textInfo.wordInfo[wordIndex];
                for (int i = 0; i < info.characterCount; ++i)
                {
                    RandomizeColor32();
                    int charIndex = info.firstCharacterIndex + i;
                    int meshIndex = _text.textInfo.characterInfo[charIndex].materialReferenceIndex;
                    int vertexIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;

                    Color32[] vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;
                    vertexColors[vertexIndex + 0] = finalColors[0];
                    vertexColors[vertexIndex + 1] = finalColors[1];
                    vertexColors[vertexIndex + 2] = finalColors[2];
                    vertexColors[vertexIndex + 3] = finalColors[3];
                }

                _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                //wordIndex = 12;
                currentTimer = 0;
            }
        }
        //if (changeColorOverTime)
        //{
        //    currentTimer += Time.deltaTime;
        //
        //    if(currentTimer >= timeToChangeColor)
        //    {
        //        //RandomizeColor32();
        //        currentTimer = 0;
        //    }
        //}
    }

    public void RandomizeColor32()
    {        
        int tempRandom;

        for (int i = 0; i < finalColors.Length; i++)
        {
            tempRandom = Random.Range(0, tempColorList.Count);
            finalColors[i] = tempColorList[tempRandom];
            tempColorList.RemoveAt(tempRandom);
        }

        tempColorList.AddRange(myColor32);
    }

    public int GetIndexOfWord(string word)
    {
        int wordCount = _text.textInfo.wordCount;

        for (int i = 0; i < wordCount; i++)
        {
            TMP_WordInfo wordInfo = _text.textInfo.wordInfo[i];

            if(wordInfo.GetWord().ToUpper().CompareTo(word.ToUpper()) == 0)
            {
                return i;
            }
            //int cc = wordInfo.characterCount;
            //
            //for (int j = 0; j < cc; j++)
            //{
            //    if(wordInfo.)
            //}
        }

        print("The word was NOT found - Could NOT retrieve word INDEX");
        return 0;
    }
}
