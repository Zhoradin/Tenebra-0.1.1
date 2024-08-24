
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour, IDataPersistence
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public TMP_Text playerEssenceText, playerHealthText, enemyHealthText, enemyEssenceText;

    public GameObject essenceWarning;
    public float essenceWarningTime;
    private float essenceWarningCounter;

    public GameObject drawCardButton, endTurnButton, drawPileButton, discardPileButton, graveyardPileButton, inventoryPanelButton;

    public UIDamageIndicator playerDamage, enemyDamage;

    public GameObject battleEndedScreen;
    public TMP_Text battleResultText;

    public string mainMenuScene;

    public GameObject pauseScreen, areYouSurePanel;

    public GameObject drawPilePanel, discardPilePanel, graveyardPilePanel, inventoryPanel;
    public bool drawPileOpen, discardPileOpen, graveyardPileOpen, inventoryPanelOpen = false;

    public Vector2 drawPileOpenPosition, drawPileClosedPosition;
    public Vector2 discardPileOpenPosition, discardPileClosedPosition;
    public Vector2 graveyardPileOpenPosition, graveyardPileClosedPosition;
    public Vector2 inventoryOpenPosition, inventoryClosedPosition;

    public TMP_Text drawPileCount, discardPileCount;

    public GameObject coins;
    public TMP_Text coinAmountText;
    private int coinAmount = 0;

    void Start()
    {
        coinAmount = DataCarrier.instance.playerCoin;
        UpdateCoinAmountText();

        drawPilePanel.GetComponent<RectTransform>().anchoredPosition = drawPileClosedPosition;
        discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileClosedPosition;
    }

    void Update()
    {
        if (essenceWarningCounter > 0)
        {
            essenceWarningCounter -= Time.deltaTime;

            if (essenceWarningCounter <= 0)
            {
                essenceWarning.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void SetPlayerEssenceText(int essenceAmount)
    {
        playerEssenceText.text = essenceAmount.ToString();
    }

    public void SetEnemyEssenceText(int essenceAmount)
    {
        enemyEssenceText.text = essenceAmount.ToString();
    }

    public void SetPlayerHealthText(int healthAmount)
    {
        playerHealthText.text = healthAmount.ToString();
    }

    public void SetEnemyHealthText(int healthAmount)
    {
        enemyHealthText.text = healthAmount.ToString();
    }

    public void ShowEssenceWarning()
    {
        essenceWarning.SetActive(true);
        essenceWarningCounter = essenceWarningTime;
    }

    public void DrawCard()
    {
        DeckController.instance.DrawCardForEssence();
    }

    public void EndPlayerTurn()
    {
        BattleController.instance.EndPlayerTurn();
    }

    public void MainMenu()
    {
        if (areYouSurePanel.activeSelf == false)
        {
            areYouSurePanel.SetActive(true);
        }
        else
        {
            areYouSurePanel.SetActive(false);
        }
    }

    public void SaveQuitYes()
    {
        BattleController.instance.UpdateDataCarrier();
        DataCarrier.instance.playerCoin = coinAmount;
        FindObjectOfType<GameController>().SaveGame();
        SceneManager.LoadScene("Hub");
        Time.timeScale = 1f;
    }

    public void SaveQuitCancel()
    {
        areYouSurePanel.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ChooseNewBattle()
    {
        SceneManager.LoadScene("Hub");
        Time.timeScale = 1f;
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void OpenDrawPile()
    {
        if (drawPileOpen == false)
        {
            drawPilePanel.GetComponent<RectTransform>().anchoredPosition = drawPileOpenPosition;
            graveyardPileButton.GetComponent<Button>().interactable = false;
            drawPileButton.GetComponent<Button>().interactable = false;
            discardPileButton.GetComponent<Button>().interactable = false;
            inventoryPanelButton.GetComponent<Button>().interactable = false;
            drawPileOpen = true;
        }
        else
        {
            drawPilePanel.GetComponent<RectTransform>().anchoredPosition = drawPileClosedPosition;
            graveyardPileButton.GetComponent<Button>().interactable = true;
            drawPileButton.GetComponent<Button>().interactable = true;
            discardPileButton.GetComponent<Button>().interactable = true;
            inventoryPanelButton.GetComponent<Button>().interactable = true;
            drawPileOpen = false;
        }
    }

    public void OpenDiscardPile()
    {
        if (discardPileOpen == false)
        {
            discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileOpenPosition;
            graveyardPileButton.GetComponent<Button>().interactable = false;
            discardPileButton.GetComponent<Button>().interactable = false;
            drawPileButton.GetComponent<Button>().interactable = false;
            inventoryPanelButton.GetComponent<Button>().interactable = false;
            discardPileOpen = true;
        }
        else
        {
            discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileClosedPosition;
            graveyardPileButton.GetComponent<Button>().interactable = true;
            discardPileButton.GetComponent<Button>().interactable = true;
            drawPileButton.GetComponent<Button>().interactable = true;
            inventoryPanelButton.GetComponent<Button>().interactable = true;
            discardPileOpen = false;
        }
    }

    public void OpenGraveyardPile()
    {
        if (graveyardPileOpen == false)
        {
            graveyardPilePanel.GetComponent<RectTransform>().anchoredPosition = graveyardPileOpenPosition;
            graveyardPileButton.GetComponent<Button>().interactable = false;
            discardPileButton.GetComponent<Button>().interactable = false;
            drawPileButton.GetComponent<Button>().interactable = false;
            inventoryPanelButton.GetComponent<Button>().interactable = false;
            graveyardPileOpen = true;
        }
        else
        {
            graveyardPilePanel.GetComponent<RectTransform>().anchoredPosition = graveyardPileClosedPosition;
            graveyardPileButton.GetComponent<Button>().interactable = true;
            discardPileButton.GetComponent<Button>().interactable = true;
            drawPileButton.GetComponent<Button>().interactable = true;
            inventoryPanelButton.GetComponent<Button>().interactable = true;
            graveyardPileOpen = false;
        }
    }

    public void OpenInventoryPanel()
    {
        if (inventoryPanelOpen == false)
        {
            inventoryPanel.transform.localPosition = inventoryOpenPosition;
            graveyardPileButton.GetComponent<Button>().interactable = false;
            discardPileButton.GetComponent<Button>().interactable = false;
            drawPileButton.GetComponent<Button>().interactable = false;
            inventoryPanelOpen = true;
        }
        else if (inventoryPanelOpen == true)
        {
            inventoryPanel.transform.localPosition = inventoryClosedPosition;
            graveyardPileButton.GetComponent<Button>().interactable = true;
            discardPileButton.GetComponent<Button>().interactable = true;
            drawPileButton.GetComponent<Button>().interactable = true;
            inventoryPanelOpen = false;
        }
    }

    public void ShowDrawPileCount()
    {
        drawPileCount.text = DrawPileController.instance.drawPile.Count.ToString();
    }

    public void ShowDiscardPileCount()
    {
        discardPileCount.text = DiscardPileController.instance.discardPile.Count.ToString();
    }

    public void AddCoins()
    {
        coinAmount += 6;
        UpdateCoinAmountText();

        BattleController.instance.StartCoroutine(BattleController.instance.ShowResultsCo());
    }

    private void UpdateCoinAmountText()
    {
        coinAmountText.text = coinAmount.ToString();
    }

    public void OnSaveButtonClick()
    {
        if (areYouSurePanel.activeSelf == false)
        {
            areYouSurePanel.SetActive(true);
        }
        else
        {
            areYouSurePanel.SetActive(false);
        }
    }

    public void LoadData(PlayerData data)
    {
        coinAmount = DataCarrier.instance.playerCoin;
    }

    public void SaveData(PlayerData data)
    {
        data.money = DataCarrier.instance.playerCoin;
    }
}
