using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogSystem : MonoBehaviour
{
    public int CurrentLineID = 0;
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image portraitImage;
    
    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    
    private DialogLine[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public bool Busy = false;
    NPC source;

    void Start()
    {
        dialogPanel.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);
    
    }

    public void StartDialog(DialogLine[] lines, NPC source_)
    {
        if (Busy == true) return;
        source = source_;
        Busy = true;
        if (lines == null || lines.Length == 0)
            return;

        currentLines = lines;
        currentLineIndex = 0;
        
        dialogPanel.SetActive(true);
        if (portraitImage) portraitImage.gameObject.SetActive(true);

        if (speakerNameText != null)
            speakerNameText.text = lines[currentLineIndex].SpeakerName;
        else speakerNameText.text = "???";
        
        DisplayLine();
    }

    void DisplayLine()
    {
        if (currentLineIndex < currentLines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            CurrentLineID = currentLineIndex;

            //Update speaker name and font
            speakerNameText.text = currentLines[currentLineIndex].SpeakerName;
            dialogText.font = currentLines[currentLineIndex].Font;
            speakerNameText.font = currentLines[currentLineIndex].Font;
            if (portraitImage != null) portraitImage.sprite = currentLines[currentLineIndex].speakerImage;

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
        if (isTyping)
        {/* Disallow text skipping for now
            StopCoroutine(typingCoroutine);
            dialogText.text = currentLines[currentLineIndex].Line;
            isTyping = false;*/
        }
        else
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
        dialogPanel.SetActive(false);
        if (portraitImage) portraitImage.gameObject.SetActive(false);

        source.EndOfDialogue();
    }
}