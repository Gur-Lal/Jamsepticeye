using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartLevel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float RestartHoldTimeRequired = 3f;
    [SerializeField] GameObject InfoText;
    [SerializeField] Image ProgressWheel;
    bool useInfoText = false;
    bool useProgressWheel = false;
    float timeCounter = 0f;
    void Start()
    {
        if (InfoText != null)
        {
            useInfoText = true;
            InfoText.SetActive(false);
        }

        if (ProgressWheel != null)
        {
            useProgressWheel = true;
            ProgressWheel.fillAmount = 0f;
            ProgressWheel.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.input.Player.Restart.IsPressed())
        {
            timeCounter += Time.deltaTime;
            if (useInfoText) InfoText.SetActive(true);

            if (useProgressWheel)
            {
                ProgressWheel.gameObject.SetActive(true);
                ProgressWheel.fillAmount = timeCounter / RestartHoldTimeRequired;
            }
        }
        else
        {
            timeCounter = 0f; 
            if (useInfoText) InfoText.SetActive(false);

            if (useProgressWheel)
            {
                ProgressWheel.fillAmount = 0f;
                ProgressWheel.gameObject.SetActive(false);
            }
        }

        if (timeCounter > RestartHoldTimeRequired)
        {
            timeCounter = 0f;

            InfoText.SetActive(false);

            if(useProgressWheel)
            {
                ProgressWheel.fillAmount = 0f;
                ProgressWheel.gameObject.SetActive(false);
            }
            PlayerController.input.Disable();

            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }
    }
}
