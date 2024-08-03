using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUIController : MonoBehaviour
{
    public static TestUIController instance;

    public Vector2 shopPanelClosedPosition, shopPanelOpenPosition;

    public GameObject shopPanel;

    public bool shopPanelOpen = false;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text playerHealth, playerCoin;

    public int testPlayerCoin = 99;

    // Start is called before the first frame update
    void Start()
    {
        SetPlayerHealth();
        SetPlayerCoin();

        shopPanel.GetComponent<RectTransform>().anchoredPosition = shopPanelClosedPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenShopPanel()
    {
        if(shopPanelOpen == false)
        {
            shopPanel.transform.localPosition = shopPanelOpenPosition;
            shopPanelOpen = true;
        }
        else if (shopPanelOpen == true)
        {
            shopPanel.transform.localPosition = shopPanelClosedPosition;
            shopPanelOpen = false;
        }
    }

    public void SetPlayerHealth()
    {
        playerHealth.text = BattleController.instance.playerHealth.ToString();
    }

    public void SetPlayerCoin()
    {
        playerCoin.text = testPlayerCoin.ToString();
    }
}
