using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
