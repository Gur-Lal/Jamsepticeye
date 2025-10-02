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
    [SerializeField] private InputAction continueAction;
    
    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialogPanel.SetActive(false);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
        
        // Enable the input action
        if (continueAction != null)
        {
            continueAction.Enable();
        }
    }

    void OnEnable()
    {
        if (continueAction != null)
            continueAction.Enable();
    }

    void OnDisable()
    {
        if (continueAction != null)
            continueAction.Disable();
    }

    public void StartDialog(string[] lines)
    {
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
            // Skip typing animation and show full text
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
        // Allow advancing with Space or Enter key using new Input System
        if (dialogPanel.activeSelf)
        {
            // Check if Input Action is set up and triggered
            if (continueAction != null && continueAction.triggered)
            {
                OnContinueClicked();
            }
            // Fallback: Direct keyboard check with new Input System
            else if (Keyboard.current != null && 
                    (Keyboard.current.spaceKey.wasPressedThisFrame || 
                     Keyboard.current.enterKey.wasPressedThisFrame))
            {
                OnContinueClicked();
            }
        }
    }

    void EndDialog()
    {
        dialogPanel.SetActive(false);
        currentLines = null;
        currentLineIndex = 0;
    }

    public void SetNPCName(string name)
    {
        npcName = name;
    }
}