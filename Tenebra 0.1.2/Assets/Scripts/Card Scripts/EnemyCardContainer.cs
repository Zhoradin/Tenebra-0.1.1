using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCardContainer : MonoBehaviour
{
    public static EnemyCardContainer instance;

    private void Awake()
    {
        instance = this;
    }

    public Image cardTypeFrame;
}
