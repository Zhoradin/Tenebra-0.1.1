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
}
