using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BarController : MonoBehaviour
{
    public static BarController instance;

    public Vector2 shopPanelClosedPosition, shopPanelOpenPosition;

    public GameObject shopPanel, dialoguePanel;
    public GameObject shopButton, dialogueButton;

    public bool shopPanelOpen, dialoguePanelOpen = false;

    public TextAsset inkJSONOne, inkJSONTwo;
    

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
        dialoguePanel.SetActive(false);
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
            dialogueButton.GetComponent<Button>().interactable = false;
        }
        else if (shopPanelOpen == true)
        {
            shopPanel.transform.localPosition = shopPanelClosedPosition;
            shopPanelOpen = false;
            dialogueButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OpenDialoguePanelOne()
    {
        if (dialoguePanelOpen == false)
        {
            dialoguePanelOpen = true;
            shopButton.GetComponent<Button>().interactable = false;
            DialogueManager.instance.EnterDialogueMode(inkJSONOne);
        }
        else if (dialoguePanelOpen == true)
        {
            dialoguePanel.SetActive(false);
            dialoguePanelOpen = false;
            shopButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OpenDialoguePanelTwo()
    {
        if (dialoguePanelOpen == false)
        {
            dialoguePanelOpen = true;
            shopButton.GetComponent<Button>().interactable = false;
            DialogueManager.instance.EnterDialogueMode(inkJSONTwo);
        }
        else if (dialoguePanelOpen == true)
        {
            dialoguePanel.SetActive(false);
            dialoguePanelOpen = false;
            shopButton.GetComponent<Button>().interactable = true;
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
