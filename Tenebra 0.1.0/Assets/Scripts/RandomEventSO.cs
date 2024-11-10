using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Event", menuName = "Scriptable Object/Random Event", order = 1)]
public class RandomEventSO : ScriptableObject
{
    public string title;
    [TextArea]
    public string text1;
    public string text1Button1, text1Button2, text1Button3;

    [TextArea]
    public string text2;
    public string text2Button1, text2Button2, text2Button3;

    [TextArea]
    public string text3;
    public string text3Button1, text3Button2, text3Button3;

    public Sprite eventSprite;
}
