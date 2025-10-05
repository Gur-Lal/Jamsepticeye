using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float RestartHoldTimeRequired = 3f;
    [SerializeField] GameObject InfoText;
    bool useInfoText = false;
    float timeCounter = 0f;
    void Start()
    {
        if (InfoText != null)
        {
            useInfoText = true;
            InfoText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.input.Player.Restart.IsPressed())
        {
            timeCounter += Time.deltaTime;
            if (useInfoText) InfoText.SetActive(true);
        }
        else { timeCounter = 0f; InfoText.SetActive(false); }

        if (timeCounter > RestartHoldTimeRequired)
        {
            timeCounter = 0f;
            Debug.Log("[Restart Level Script] RESTARTING SCENE");

            InfoText.SetActive(false);

            PlayerController.input.Disable();

            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }
    }
}
