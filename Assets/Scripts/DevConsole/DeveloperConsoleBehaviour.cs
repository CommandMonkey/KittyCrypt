using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeveloperConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField, TextArea] private string standardSuggestion;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private TMP_Text suggestionText = null;

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

    private int currentSuggestionIndex = -1;
    private List<string> currentSuggestions = new List<string>();

    private void Start()
    {
        SetUpInputEvents();
        GameSession.Instance.developerConsole = this;
    }

    private void Update()
    {
        HandleTabCompletion();
    }

    void SetUpInputEvents()
    {
        gameSession = GameSession.Instance;
        userInput = gameSession.userInput;
        userInput.onToggleConsole.AddListener(Toggle);
    }

    public void Toggle()
    {
        if (uiCanvas.activeSelf)
        {
            Debug.Log("Toggeling OFF console");
            inputField.text = string.Empty;
            Time.timeScale = pausedTimeScale;
            uiCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Toggeling ON console");
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    // Called by Unity's InputField
    public void TextChanged(string text)
    {
        if (text.Length < prefix.Length || !text.StartsWith(prefix))
        {
            // No Text
            suggestionText.text = standardSuggestion;
            return;
        }

        text = text.Substring(prefix.Length);

        if (text.Contains(' ')) // secondary stage (check for suggestions inside commands)
        {
            var parts = text.Split(new[] { ' ' }, 2);
            var command = parts[0];
            var followingText = parts.Length > 1 ? parts[1] : string.Empty;

            currentSuggestions = DeveloperConsole.GetSpecificCommandSuggestionList(command, followingText);
            currentSuggestionIndex = -1;

            string suggestions = "";
            foreach (string suggestion in currentSuggestions)
            {
                suggestions = prefix + command + " " + suggestion + "\n" + suggestions;
            }
            suggestionText.text = suggestions;

        }
        else // still first command (check for command suggestions)
        {
            currentSuggestions = DeveloperConsole.GetSuggestionList(text);
            currentSuggestionIndex = -1;

            string suggestions = "";
            foreach (string command in currentSuggestions)
            {
                suggestions = prefix + command + "\n" + suggestions;
            }
            suggestionText.text = suggestions;
        }
    }

    public void ProcessCommand(string inputValue)
    {
        if (DeveloperConsole.ProcessCommand(inputValue))
        {
            Toggle();
        }
        inputField.text = string.Empty;
    }

    private void HandleTabCompletion()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentSuggestions.Count == 0) return;

            currentSuggestionIndex = (currentSuggestionIndex + 1) % currentSuggestions.Count;
            string suggestion = currentSuggestions[currentSuggestionIndex];

            string text = inputField.text;
            if (text.StartsWith(prefix))
            {
                string[] parts = text.Substring(prefix.Length).Split(new[] { ' ' }, 2);
                string command = parts[0];
                string followingText = parts.Length > 1 ? parts[1] : string.Empty;

                if (text.Contains(' '))
                {
                    inputField.text = $"{prefix}{command} {suggestion}";
                }
                else
                {
                    inputField.text = $"{prefix}{suggestion}";
                }

                inputField.caretPosition = inputField.text.Length;
            }
        }
    }
}
