using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public TMP_Text playerMaxHealth, playerCoin;

    public int testPlayerCoin = 99;

    // Start is called before the first frame update
    void Start()
    {
        testPlayerCoin = DataCarrier.instance.playerCoin;

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

    public void OnLeaveClicked()
    {
        SceneManager.LoadScene("Hub");
    }

    public void SetPlayerHealth()
    {
        playerMaxHealth.text = DataCarrier.instance.playerMaxHealth.ToString();
    }

    public void SetPlayerCoin()
    {
        playerCoin.text = testPlayerCoin.ToString();
        DataCarrier.instance.playerCoin = testPlayerCoin;
    }
}
