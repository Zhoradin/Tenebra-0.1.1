using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class RandomEventController : MonoBehaviour
{
    public string whichTower;
    public TMP_Text eventTitle, eventText;
    public TMP_Text eventButtonText1, eventButtonText2, eventButtonText3;
    public GameObject eventButton1, eventButton2, eventButton3;
    public GameObject leaveButton;
    public Image eventImage;

    public string lastStory;
    public int storyNumber = 1;

    public RandomEventSO randomEventData;    

    // Start is called before the first frame update
    void Start()
    {
        leaveButton.GetComponent<Button>().interactable = false;
        whichTower = "Pathway " + DataCarrier.instance.lastGod;
        SetupRandomEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupRandomEvent()
    {
        eventTitle.text = randomEventData.title.ToString();
        eventText.text = randomEventData.text1.ToString();
        if(randomEventData.text1Button1 != "")
        {
            eventButtonText1.text = randomEventData.text1Button1.ToString();
        }
        else
        {
            eventButton1.gameObject.SetActive(false);
        }
        if (randomEventData.text1Button2 != "")
        {
            eventButtonText2.text = randomEventData.text1Button2.ToString();
        }
        else
        {
            eventButton2.gameObject.SetActive(false);
        }
        if (randomEventData.text1Button3 != "")
        {
            eventButtonText3.text = randomEventData.text1Button3.ToString();
        }
        else
        {
            eventButton3.gameObject.SetActive(false);
        }

        eventImage.sprite = randomEventData.eventSprite;
        lastStory = "text" + storyNumber;
    }

    public void ContinueEvent1()
    {
        if(lastStory == "text1")
        {
            eventText.text = randomEventData.text2.ToString();
            if(randomEventData.text2Button1.ToString() != "")
            {
                Debug.Log("buton dolu");
                eventButtonText1.text = randomEventData.text2Button1.ToString();
                eventButtonText2.text = randomEventData.text2Button2.ToString();
            }
            else
            {
                Debug.Log("buton boþ");
                eventButton1.gameObject.SetActive(false);
                eventButton2.gameObject.SetActive(false);
                leaveButton.GetComponent<Button>().interactable = true;
            }

            lastStory = "text2";
        }
        else
        {
            leaveButton.GetComponent<Button>().interactable = true;
        }
    }

    public void ContinueEvent2()
    {
        if (lastStory == "text1")
        {
            eventText.text = randomEventData.text3.ToString();
            if (randomEventData.text3Button1.ToString() != "")
            {
                eventButtonText1.text = randomEventData.text3Button1.ToString();
                eventButtonText2.text = randomEventData.text3Button2.ToString();
            }
            else
            {
                eventButton1.gameObject.SetActive(false);
                eventButton2.gameObject.SetActive(false);
                leaveButton.GetComponent<Button>().interactable = true;
            }
            lastStory = "text3";
        }
        else
        {
            leaveButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OnLeaveClicked()
    {
        SceneManager.LoadScene(whichTower);
    }
}
