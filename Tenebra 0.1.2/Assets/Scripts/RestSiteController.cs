using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestSiteController : MonoBehaviour
{
    public TMP_Text healthText, coinText;
    public GameObject restButton, returnButton;
    public string whichTower;

    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        returnButton.GetComponent<Button>().interactable = false;

        UpdateHealth();
        coinText.text =  DataCarrier.instance.playerCoin.ToString();

        whichTower = "Pathway " + DataCarrier.instance.lastGod;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth()
    {
        healthText.text = DataCarrier.instance.playerHealth + "/" + DataCarrier.instance.playerMaxHealth;
    }

    public void OnRestClicked()
    {
        DataCarrier.instance.playerHealth += DataCarrier.instance.playerMaxHealth / 3;
        UpdateHealth();
        gameController.SaveGame();

        restButton.GetComponent<Button>().interactable = false;
        returnButton.GetComponent<Button>().interactable = true;
    }

    public void OnCraftClicked()
    {

    }

    public void OnReturnClicked()
    {
        SceneManager.LoadScene(whichTower);
    }
}
