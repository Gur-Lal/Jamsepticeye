using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public int CurrentLineID = 0;
    [Header("Settings")]
    [SerializeField] private bool PausePlayerControl = false;
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextShakeScript textShaker;
    [SerializeField] List<IDialogueAction> DialogueActions;
    
    private float typeSpeed = 0.05f;

    private PlayerController pcontr;

    private List<DialogLine> currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool IsFirstLine;

    public bool Busy = false;
    NPC source;

    void Start()
    {
        dialogPanel.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);
        pcontr = FindFirstObjectByType<PlayerController>();
    
    }

    public void StartDialog(List<DialogLine> lines, NPC source_)
    {
        if (Busy == true) return;
        source = source_;
        Busy = true;
        if (lines == null || lines.Count == 0)
            return;

        if (PausePlayerControl && pcontr != null) pcontr.Incapacitate();

        currentLines = lines;
        currentLineIndex = 0;
        
        dialogPanel.SetActive(true);
        if (portraitImage) portraitImage.gameObject.SetActive(true);

        if (speakerNameText != null)
            speakerNameText.text = lines[currentLineIndex].speakerData.speakerName;
        else speakerNameText.text = "???";

        IsFirstLine = true;
        StartCoroutine(InputDelay());

        DisplayLine();
    }

    IEnumerator InputDelay()
    {
        yield return new WaitForSeconds(0.2f); //0.2f second delay before first line of dialogue can be skipped so they dont skip it accidentally
        IsFirstLine = false;
    }

    void DisplayLine()
    {
        if (currentLineIndex < currentLines.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            CurrentLineID = currentLineIndex;

            //Update speaker name and font
            speakerNameText.text = currentLines[currentLineIndex].speakerData.speakerName;
            typeSpeed = currentLines[currentLineIndex].speakerData.secondsPerLetterTyped;
            dialogText.font = currentLines[currentLineIndex].speakerData.font;
            speakerNameText.font = currentLines[currentLineIndex].speakerData.font;
            if (portraitImage != null) portraitImage.sprite = currentLines[currentLineIndex].speakerData.portrait;

            bool textShake = currentLines[currentLineIndex].speakerData.textShake;
            if (textShake && textShaker != null) textShaker.StartShake();
            else if (textShaker != null) textShaker.StopShake();

            foreach (var ping in DialogueActions) ping.Ping(currentLineIndex); //activate all relevant dialogue actions

            typingCoroutine = StartCoroutine(TypeText(currentLines[currentLineIndex]));
        }
        else
        {
            EndDialog();
        }
    }

    IEnumerator TypeText(DialogLine line)
    {
        isTyping = true;
        dialogText.text = line.Line;

        int totalChars = line.Line.Length;

        for (int i = 0; i <= totalChars; i++)
        {
            dialogText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeSpeed);
        }

        
        isTyping = false;
    }

    public void OnContinueClicked()
    {
        if (isTyping && !IsFirstLine)
        {//Allow text skipping?
            StopCoroutine(typingCoroutine);
            dialogText.maxVisibleCharacters = currentLines[currentLineIndex].Line.Length;
            isTyping = false;
        }
        else if (!isTyping) 
        {
            // Move to next line
            currentLineIndex++;
            DisplayLine();
        }
    }

    void Update()
    {
        if (dialogPanel.activeSelf)
        {
            if (PlayerController.input.Player.Interact.triggered)
            {
                OnContinueClicked();
            }
        }
    }

    public void EndDialog()
    {
        currentLines = null;
        currentLineIndex = 0;
        Busy = false;

        if (dialogPanel == null) return;

        if (PausePlayerControl && pcontr != null) pcontr.Deincapacitate();
        dialogPanel.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);

        foreach (var ping in DialogueActions) ping.Ping(-1); //end all relevant dialogue actions

        source.EndOfDialogue();
    }
}