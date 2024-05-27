
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DeveloperConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;

    private float pausedTimeScale;

    private UserInput userInput;
    private GameSession gameSession;

    private static DeveloperConsoleBehaviour instance;

    private DeveloperConsole developerConsole;
    private DeveloperConsole DeveloperConsole
    {
        get
        {
            if (developerConsole != null) { return developerConsole; }
            return developerConsole = new DeveloperConsole(prefix, commands);
        }
    }


    private void Start()
    {
        SetUpInputEvents();
        GameSession.Instance.developerConsole = this;
    }


    void SetUpInputEvents()
    {
        gameSession = GameSession.Instance;
        userInput = gameSession.userInput;
        userInput.onToggleConsole.AddListener(Toggle);
    }

    public void Toggle()
    {
        Debug.Log("CONSOLE Toggle");
        if (uiCanvas.activeSelf)
        {
            inputField.text = string.Empty;
            Time.timeScale = pausedTimeScale;
            uiCanvas.SetActive(false);
        }
        else
        {
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        DeveloperConsole.ProcessCommand(inputValue);

        inputField.text = string.Empty;
    }
}

