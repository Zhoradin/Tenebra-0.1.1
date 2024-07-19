using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonPhaseController : MonoBehaviour
{
    public static MoonPhaseController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Sprite> moonPhaseImages = new List<Sprite>();

    public Image moonPhaseImage;

    public MoonPhase currentMoonPhase;

    public float transitionDuration = 1f; // Geçiþ süresi

    private void Start()
    {
        currentMoonPhase = BattleController.instance.currentMoonPhase;
        UpdateMoonPhaseImage();
    }

    public void UpdateMoonPhase()
    {
        currentMoonPhase = BattleController.instance.currentMoonPhase;
        UpdateMoonPhaseImage();
    }

    private void UpdateMoonPhaseImage()
    {
        int moonPhaseIndex = (int)currentMoonPhase;
        if (moonPhaseIndex >= 0 && moonPhaseIndex < moonPhaseImages.Count)
        {
            StartCoroutine(TransitionToNewImage(moonPhaseImages[moonPhaseIndex]));
        }
    }

    private IEnumerator TransitionToNewImage(Sprite newSprite)
    {
        float elapsedTime = 0f;
        Sprite originalSprite = moonPhaseImage.sprite;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            moonPhaseImage.sprite = newSprite;
            moonPhaseImage.color = new Color(1f, 1f, 1f, t); // Sprite'in opacity'sini deðiþtir

            yield return null;
        }

        moonPhaseImage.sprite = newSprite;
        moonPhaseImage.color = new Color(1f, 1f, 1f, 1f); // Geçiþi tamamladýktan sonra tam opak yap
    }

    public void AdvanceMoonPhase()
    {
        if(BattleController.instance.moonPhaseCount == 0)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.NewMoon;
        }
        else if(BattleController.instance.moonPhaseCount <= 3)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.WaxingCrescent;
        }
        else if(BattleController.instance.moonPhaseCount == 4)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.FirstQuarter;
        } 
        else if(BattleController.instance.moonPhaseCount <= 7)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.WaxingGibbous;
        }
        else if(BattleController.instance.moonPhaseCount == 8)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.FullMoon;
        }
        else if(BattleController.instance.moonPhaseCount <= 11)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.WaningGibbous;
        }
        else if(BattleController.instance.moonPhaseCount == 12)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.LastQuarter;
        }
        else if(BattleController.instance.moonPhaseCount <= 15)
        {
            BattleController.instance.currentMoonPhase = MoonPhase.WaningCrescent;
        }
        else
        {
            Debug.Log("New Moona geri dönülüyor");

        }
    }
}
