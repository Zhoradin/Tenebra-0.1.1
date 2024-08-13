using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public float transitionDuration = 1f;

    public TMP_Text moonPhaseText;
    public float fadeDuration = .25f;
    private Coroutine fadeCoroutine;

    public int moonPhaseTillNextRound;

    private Dictionary<MoonPhase, string> moonPhaseNames = new Dictionary<MoonPhase, string>
    {
        { MoonPhase.NewMoon, "New Moon" },
        { MoonPhase.WaxingCrescent, "Waxing Crescent" },
        { MoonPhase.FirstQuarter, "First Quarter" },
        { MoonPhase.WaxingGibbous, "Waxing Gibbous" },
        { MoonPhase.FullMoon, "Full Moon" },
        { MoonPhase.WaningGibbous, "Waning Gibbous" },
        { MoonPhase.LastQuarter, "Last Quarter" },
        { MoonPhase.WaningCrescent, "Waning Crescent" }
    };

    private void Start()
    {
        currentMoonPhase = BattleController.instance.currentMoonPhase;
        UpdateMoonPhaseImage();
        SetMoonPhaseText();
    }

    public void UpdateMoonPhase()
    {
        currentMoonPhase = BattleController.instance.currentMoonPhase;
        UpdateMoonPhaseImage();
        SetMoonPhaseText();
    }

    private void UpdateMoonPhaseImage()
    {
        int moonPhaseIndex = (int)currentMoonPhase;
        if (moonPhaseIndex >= 0 && moonPhaseIndex < moonPhaseImages.Count)
        {
            Sprite newSprite = moonPhaseImages[moonPhaseIndex];
            if (moonPhaseImage.sprite != newSprite)
            {
                StartCoroutine(TransitionToNewImage(newSprite));
            }
        }
    }

    private IEnumerator TransitionToNewImage(Sprite newSprite)
    {
        // Fade out the current image
        yield return StartCoroutine(FadeOutImage());

        // Set the new sprite
        moonPhaseImage.sprite = newSprite;

        // Fade in the new image
        yield return StartCoroutine(FadeInImage());
    }

    private IEnumerator FadeOutImage()
    {
        float elapsedTime = 0f;
        Color color = moonPhaseImage.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / transitionDuration);
            moonPhaseImage.color = color;
            yield return null;
        }

        color.a = 0f;
        moonPhaseImage.color = color;
    }

    private IEnumerator FadeInImage()
    {
        float elapsedTime = 0f;
        Color color = moonPhaseImage.color;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / transitionDuration);
            moonPhaseImage.color = color;
            yield return null;
        }

        color.a = 1f;
        moonPhaseImage.color = color;
    }

    public void AdvanceMoonPhase()
    {
        switch (BattleController.instance.moonPhaseCount)
        {
            case 0:
                BattleController.instance.currentMoonPhase = MoonPhase.NewMoon;
                moonPhaseTillNextRound = 1;
                break;

            case int count when (count >= 1 && count <= 3):
                BattleController.instance.currentMoonPhase = MoonPhase.WaxingCrescent;
                moonPhaseTillNextRound = 4 - count;
                break;

            case 4:
                BattleController.instance.currentMoonPhase = MoonPhase.FirstQuarter;
                moonPhaseTillNextRound = 1;
                break;

            case int count when (count >= 5 && count <= 7):
                BattleController.instance.currentMoonPhase = MoonPhase.WaxingGibbous;
                moonPhaseTillNextRound = 8 - count;
                break;

            case 8:
                BattleController.instance.currentMoonPhase = MoonPhase.FullMoon;
                moonPhaseTillNextRound = 1;
                break;

            case int count when (count >= 9 && count <= 11):
                BattleController.instance.currentMoonPhase = MoonPhase.WaningGibbous;
                moonPhaseTillNextRound = 12 - count;
                break;

            case 12:
                BattleController.instance.currentMoonPhase = MoonPhase.LastQuarter;
                moonPhaseTillNextRound = 1;
                break;

            case int count when (count >= 13 && count <= 15):
                BattleController.instance.currentMoonPhase = MoonPhase.WaningCrescent;
                moonPhaseTillNextRound = 16 - count;
                break;

            default:
                Debug.Log("Returning to New Moon");
                moonPhaseTillNextRound = 0; // Reset for new cycle
                break;
        }

        SetMoonPhaseText();
    }


    public void SetMoonPhaseText()
    {
        string turnText = moonPhaseTillNextRound == 1 ? "turn" : "turns";
        string moonPhaseName = moonPhaseNames[BattleController.instance.currentMoonPhase];
        moonPhaseText.text = moonPhaseName + "\n" + moonPhaseTillNextRound + " " + turnText + "\n" + "till next round";
    }

    public void ShowMoonPhase()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeTextToFullAlpha());
    }

    public void HideMoonPhase()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeTextToZeroAlpha());
    }

    private IEnumerator FadeTextToFullAlpha()
    {
        moonPhaseText.gameObject.SetActive(true);
        Color color = moonPhaseText.color;
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeDuration;
            color.a = Mathf.Clamp01(alpha);
            moonPhaseText.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeTextToZeroAlpha()
    {
        Color color = moonPhaseText.color;
        float alpha = color.a;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / fadeDuration;
            color.a = Mathf.Clamp01(alpha);
            moonPhaseText.color = color;
            yield return null;
        }

        moonPhaseText.gameObject.SetActive(false);
    }
}
