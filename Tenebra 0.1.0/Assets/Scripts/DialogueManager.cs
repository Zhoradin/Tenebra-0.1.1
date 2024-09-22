using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Params")]
    public float typingSpeed = 0.04f;
    public float MIN_ALPHA = 0.2f;
    public float fadeDuration = 0.5f;

    [Header("Load Globals Ink File")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Dialogue UI")]
    public TMP_Text dialogueText;
    public TMP_Text leftDisplayNameText, rightDisplayNameText;
    public Animator portraitAnimator;
    public Animator layoutAnimator;
    public GameObject continueIcon;
    public Animator leftPortraitAnimator, rightPortraitAnimator;
    public CanvasGroup leftPortraitGroup, rightPortraitGroup;

    public Story currentStory;

    private bool leftPortrait, rightPortrait;
    private int left, right = 0;

    private bool dialogueIsPlaying;
    private bool canContinueToNextLine = false;
    private bool skipLine = false;

    private Coroutine displayLineCoroutine;

    private const string LAYOUT_TAG = "layout";
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private DialogueVariables dialogueVariables;

    private void Awake()
    {
        instance = this;
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    private void Start()
    {
        dialogueIsPlaying = false;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        leftPortraitGroup.alpha = 0f;
        rightPortraitGroup.alpha = 0f;
        leftDisplayNameText.alpha = 0f;
        rightDisplayNameText.alpha = 0f;

        leftPortraitGroup.interactable = false;
        leftPortraitGroup.blocksRaycasts = false;
        rightPortraitGroup.interactable = false;
        rightPortraitGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!canContinueToNextLine && displayLineCoroutine != null)
            {
                skipLine = true;
            }
            else if (canContinueToNextLine && currentStory.currentChoices.Count == 0)
            {
                ContinueStory();
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        BarController.instance.dialoguePanel.SetActive(true);
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;

        dialogueVariables.StartListening(currentStory);

        leftDisplayNameText.text = "???";
        rightDisplayNameText.text = "???";

        ContinueStory();
    }

    public IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        BarController.instance.dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));

            HandleTags(currentStory.currentTags);
        }
        else if (currentStory.currentChoices.Count == 0)
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;
        skipLine = false;

        bool isAddingRichTextTag = false;

        foreach (char letter in line.ToCharArray())
        {
            if (skipLine)
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
                continue;
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case LAYOUT_TAG:
                    if (tagValue == "left")
                    {
                        if(left == 0)
                        {
                            left++;
                        }

                        StartCoroutine(FadePortrait(leftPortraitGroup, leftDisplayNameText, true));

                        if (right > 0)
                        {
                            StartCoroutine(FadePortrait(rightPortraitGroup, rightDisplayNameText, false));
                        }
                        
                        leftPortrait = true;
                        rightPortrait = false;
                    }
                    else if (tagValue == "right")
                    {
                        if(right == 0)
                        {
                            right++;
                        }

                        StartCoroutine(FadePortrait(rightPortraitGroup, rightDisplayNameText, true));

                        if(left > 0)
                        {
                            StartCoroutine(FadePortrait(leftPortraitGroup, leftDisplayNameText, false));
                        }

                        leftPortrait = false;
                        rightPortrait = true;
                    }
                    break;

                case SPEAKER_TAG:
                    if (leftPortrait)
                    {
                        leftDisplayNameText.text = tagValue;
                    }
                    else if (rightPortrait)
                    {
                        rightDisplayNameText.text = tagValue;
                    }
                    break;

                case PORTRAIT_TAG:
                    if (leftPortrait)
                    {
                        leftPortraitAnimator.Play(tagValue);
                    }
                    else if (rightPortrait)
                    {
                        rightPortraitAnimator.Play(tagValue);
                    }
                    break;

                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private IEnumerator FadePortrait(CanvasGroup portraitGroup, TMP_Text displayNameText, bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : MIN_ALPHA;
        float startAlpha = portraitGroup.alpha;
        float duration = fadeDuration;

        if (fadeIn)
        {
            portraitGroup.interactable = true;
            portraitGroup.blocksRaycasts = true;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            portraitGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            displayNameText.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        portraitGroup.alpha = targetAlpha;
        displayNameText.alpha = targetAlpha;

        if (!fadeIn)
        {
            portraitGroup.interactable = false;
            portraitGroup.blocksRaycasts = false;
        }
    }

    public void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can supoort. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;

        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }
}