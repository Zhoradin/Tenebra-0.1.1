using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public GameObject pauseScreen;

    public GameObject drawPilePanel, discardPilePanel;
    public bool drawPileOpen ,discardPileOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(essenceWarningCounter > 0)
        {
            essenceWarningCounter -= Time.deltaTime;

            if(essenceWarningCounter <= 0)
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
        if(drawPileOpen == false)
        {
            drawPilePanel.SetActive(true);
            drawPileButton.GetComponent<Button>().interactable = false;
            drawPileOpen = true;
        }
        else
        {
            drawPilePanel.SetActive(false);
            drawPileButton.GetComponent<Button>().interactable = true;
            drawPileOpen = false;
        }
    }
    public void OpenDiscardPile()
    {
        if (discardPileOpen == false)
        {
            discardPilePanel.SetActive(true);
            discardPileButton.GetComponent<Button>().interactable = false;
            discardPileOpen = true;
        }
        else
        {
            discardPilePanel.SetActive(false);
            discardPileButton.GetComponent<Button>().interactable = true;
            discardPileOpen = false;
        }
    }
}
