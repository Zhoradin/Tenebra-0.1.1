using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HubController : MonoBehaviour
{
    public GameObject soliriaButton, caerulisnButton, amarunisButton, poulviButton, arstelloButton, logiumButton, rohvButton, tenebraButton, abororButton;

    // Start is called before the first frame update
    void Start()
    {
        caerulisnButton.GetComponent<Button>().interactable = true;
        logiumButton.GetComponent<Button>().interactable = true;
        arstelloButton.GetComponent<Button>().interactable = true;

        soliriaButton.GetComponent<Button>().interactable = false;
        amarunisButton.GetComponent<Button>().interactable = false;
        poulviButton.GetComponent<Button>().interactable = false;
        rohvButton.GetComponent<Button>().interactable = false;
        tenebraButton.GetComponent<Button>().interactable = false;
        abororButton.GetComponent<Button>().interactable = false;

        CheckTowerActiveness();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckTowerActiveness()
    {
        if (DataCarrier.instance.hasCaerulisnBadge)
        {
            amarunisButton.GetComponent<Button>().interactable = true;
        }
        if (DataCarrier.instance.hasLogiumBadge)
        {
            poulviButton.GetComponent<Button>().interactable = true;
        }
        if (DataCarrier.instance.hasArstelloBadge)
        {
            rohvButton.GetComponent<Button>().interactable = true;
        }

        if(DataCarrier.instance.hasAmarunisBadge && DataCarrier.instance.hasArstelloBadge && DataCarrier.instance.hasCaerulisnBadge && DataCarrier.instance.hasCaerulisnBadge && DataCarrier.instance.hasLogiumBadge
            && DataCarrier.instance.hasPoulviBadge && DataCarrier.instance.hasRohvBadge)
        {
            soliriaButton.GetComponent<Button>().interactable = true;
            tenebraButton.GetComponent<Button>().interactable = true;
        }

        if (DataCarrier.instance.hasAmarunisBadge && DataCarrier.instance.hasArstelloBadge && DataCarrier.instance.hasCaerulisnBadge && DataCarrier.instance.hasCaerulisnBadge && DataCarrier.instance.hasLogiumBadge
            && DataCarrier.instance.hasPoulviBadge && DataCarrier.instance.hasRohvBadge && DataCarrier.instance.hasSoliriaBadge && DataCarrier.instance.hasTenebraBadge)
        {
            abororButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OnBarClicked()
    {
        SceneManager.LoadScene("Bar");
    }

    public void OnSoliriaClicked()
    {
        SceneManager.LoadScene("Pathway Soliria");
        FindObjectOfType<DataCarrier>().lastGod = "Soliria";
    }

    public void OnCaerulisnClicked()
    {
        SceneManager.LoadScene("Pathway Caerulisn");
        FindObjectOfType<DataCarrier>().lastGod = "Caerulisn";
    }

    public void OnAmarunisClicked()
    {
        SceneManager.LoadScene("Pathway Amarunis");
        FindObjectOfType<DataCarrier>().lastGod = "Amarunis";
    }

    public void OnPoulviClicked()
    {
        SceneManager.LoadScene("Pathway Poulvi");
        FindObjectOfType<DataCarrier>().lastGod = "Poulvi";
    }

    public void OnArstelloClicked()
    {
        SceneManager.LoadScene("Pathway Arstello");
        FindObjectOfType<DataCarrier>().lastGod = "Arstello";
    }

    public void OnLogiumClicked()
    {
        SceneManager.LoadScene("Pathway Logium");
        FindObjectOfType<DataCarrier>().lastGod = "Logium";
    }

    public void OnRohvClicked()
    {
        SceneManager.LoadScene("Pathway Rohv");
        FindObjectOfType<DataCarrier>().lastGod = "Rohv";
    }

    public void OnTenebraClicked()
    {
        SceneManager.LoadScene("Pathway Tenebra");
        FindObjectOfType<DataCarrier>().lastGod = "Tenebra";
    }

    public void OnAbororClicked()
    {
        SceneManager.LoadScene("Pathway Aboror");
        FindObjectOfType<DataCarrier>().lastGod = "Aboror";
    }
}
