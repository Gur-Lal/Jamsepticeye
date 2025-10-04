using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DialogSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Button continueButton;
    
    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private string npcName = "NPC";
    
    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public bool Busy = false;

    void Start()
    {
        dialogPanel.SetActive(false);
    
    }

    public void StartDialog(string[] lines)
    {
        if (Busy == true) return;
        Busy = true;
        if (lines == null || lines.Length == 0)
            return;

        currentLines = lines;
        currentLineIndex = 0;
        
        dialogPanel.SetActive(true);
        
        if (npcNameText != null)
            npcNameText.text = npcName;
        
        DisplayLine();
    }

    void DisplayLine()
    {
        if (currentLineIndex < currentLines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
                
            typingCoroutine = StartCoroutine(TypeText(currentLines[currentLineIndex]));
        }
        else
        {
            EndDialog();
        }
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogText.text = "";
        
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        isTyping = false;
    }

    public void OnContinueClicked()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = currentLines[currentLineIndex];
            isTyping = false;
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
            if (PlayerController.input.Interaction)
            {
                OnContinueClicked();
            }
            else if (Keyboard.current != null && 
                    (|| 
                     Keyboard.current.enterKey.wasPressedThisFrame))
            {
                OnContinueClicked();
            }
        }
    }

    public void EndDialog()
    {
        dialogPanel.SetActive(false);
        currentLines = null;
        currentLineIndex = 0;
        Busy = false;
    }

    public void SetNPCName(string name)
    {
        npcName = name;
    }
}