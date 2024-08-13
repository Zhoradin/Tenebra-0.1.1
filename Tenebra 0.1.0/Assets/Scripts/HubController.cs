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
        SceneManager.LoadScene("Battle 1");
    }

    public void OnCaerulisnClicked()
    {
        SceneManager.LoadScene("Battle 2");
    }

    public void OnAmarunisClicked()
    {
        SceneManager.LoadScene("Battle 3");
    }
}
