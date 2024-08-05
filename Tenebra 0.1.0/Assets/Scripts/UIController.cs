
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
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

    public GameObject drawCardButton, endTurnButton, drawPileButton, discardPileButton;

    public UIDamageIndicator playerDamage, enemyDamage;

    public GameObject battleEndedScreen;
    public TMP_Text battleResultText;

    public string mainMenuScene, battleSelectScene;

    public GameObject pauseScreen, areYouSurePanel;

    public GameObject drawPilePanel, discardPilePanel;
    public bool drawPileOpen, discardPileOpen = false;

    public Vector2 drawPileOpenPosition, drawPileClosedPosition;
    public Vector2 discardPileOpenPosition, discardPileClosedPosition;

    public TMP_Text drawPileCount, discardPileCount;

    public GameObject coins;
    public TMP_Text coinAmountText;
    private int coinAmount = 0;

    void Start()
    {
        drawPilePanel.GetComponent<RectTransform>().anchoredPosition = drawPileClosedPosition;
        discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileClosedPosition;

        coinAmount = 0;
        UpdateCoinAmountText();
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
        Time.timeScale = 1f;
    }

    public void MainMenuYes()
    {
        SceneManager.LoadScene(mainMenuScene);
        FindObjectOfType<GameController>().SaveGame();
        Time.timeScale = 1f;
    }

    public void MainMenuNo()
    {
        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ChooseNewBattle()
    {
        SceneManager.LoadScene(battleSelectScene);
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
            drawPileButton.GetComponent<Button>().interactable = false;
            discardPileButton.GetComponent<Button>().interactable = false;
            drawPileOpen = true;
        }
        else
        {
            drawPilePanel.GetComponent<RectTransform>().anchoredPosition = drawPileClosedPosition;
            drawPileButton.GetComponent<Button>().interactable = true;
            discardPileButton.GetComponent<Button>().interactable = true;
            drawPileOpen = false;
        }
    }

    public void OpenDiscardPile()
    {
        if (discardPileOpen == false)
        {
            discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileOpenPosition;
            discardPileButton.GetComponent<Button>().interactable = false;
            drawPileButton.GetComponent<Button>().interactable = false;
            discardPileOpen = true;
        }
        else
        {
            discardPilePanel.GetComponent<RectTransform>().anchoredPosition = discardPileClosedPosition;
            discardPileButton.GetComponent<Button>().interactable = true;
            drawPileButton.GetComponent<Button>().interactable = true;
            discardPileOpen = false;
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
        FindObjectOfType<GameController>().SaveGame();
    }

    public void OnLoadButtonClick()
    {
        FindObjectOfType<GameController>().LoadGame();
    }
}
