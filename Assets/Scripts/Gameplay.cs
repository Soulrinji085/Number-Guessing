using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Tilemaps;
using System.Collections;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttempts = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool GameActive;

    void InitializedUI()
    { 
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onEndEdit.AddListener(delegate { SubmitGuess(); });

    }

    void SubmitGuess()
    {
        if (!GameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int .TryParse(input, out guess)) 
        { 
            gameState.text = "Please enter a valid number.";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"Plaese enter a number between {minNumber} - {maxNumber}.";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";

    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player Turn" : "Computer Turn";

        gameLog.text += $"{playerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameState.text = $"{playerName} got it right\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttempts)
        {
            //Lose
            gameLog.text += $"Game Over! The Correct number was {targetNumber}\n";
            EndGame();
        }
        else 
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Lower!" : "Higher!";
            gameLog.text += $"{hint}\n";

            //Switch turns
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player Turn" : "Computer Turn";
            attempsLeft.text = $"Attempts Left: {maxAttempts - currentAttemps}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn(bool targetISHigher)
    {
        yield return new WaitForSeconds(1f); // Simulate thinking time
        if (!GameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        GameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Click New Game to start again";
        Canvas.ForceUpdateCanvases(); // Ensure UI updates immediately
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        GameActive = true;

        currentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attempts Left: {maxAttempts}";
        gameLog.text = "== Game Log == \n";
        gameState.text = "New game started!";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = "";
        guessInputField.Select();
        guessInputField.ActivateInputField();

    }

    void Start()
    {
        InitializedUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
