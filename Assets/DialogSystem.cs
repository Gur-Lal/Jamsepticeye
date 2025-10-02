using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;

    private string[] dialogLines;
    private int currentLine = 0;
    private bool isActive = false;

    public void StartDialog(string[] newLines)
    {
        dialogLines = newLines;
        currentLine = 0;
        dialogPanel.SetActive(true);
        isActive = true;
        ShowLine();
    }

    public void ShowLine()
    {
        if (currentLine < dialogLines.Length)
        {
            dialogText.text = dialogLines[currentLine];
        }
        else
        {
            EndDialog();
        }
    }

    private void NextLine()
    {
        currentLine++;
        ShowLine();
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);
        isActive = false;
    }

    public void OnNextDialog(InputAction.CallbackContext context)
    {
        if (isActive && context.performed)
        {
            NextLine();
        }
    }
}
