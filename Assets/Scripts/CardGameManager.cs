using DG.Tweening;
using Naninovel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform puzzleField;
    [SerializeField] private GridLayoutGroup fieldGrid;
    [SerializeField] private GameObject puzzlePrefab;
    [SerializeField] private PlayScript playScript;

    [Header("Variables")]
    [SerializeField] private Vector2 puzzleSize;
    [SerializeField] private Vector2 puzzleSpace;
    [SerializeField] private int columnCount;
    [SerializeField] private int size;
    [SerializeField] private float timeToCheck;
    [SerializeField] private float rotationTime = 0.25f;

    [Header("Sprites")]
    [SerializeField] private Sprite[] puzzles;
    [SerializeField] private Sprite[] backSprites;

    [Header("Lists")]
    [SerializeField] private List<Sprite> gamePuzzles = new List<Sprite>();
    [SerializeField] private List<Button> btns = new List<Button>();

    [Header("SFX")]
    [SerializeField] private string correctGuessSoundName;
    [SerializeField] private string wrongGuessSoundName;
    [SerializeField] private string gameoverSoundName;
    [SerializeField] private string pickCardSoundName;
    [SerializeField] private string unPickCardSoundName;

    private Sprite firstGuestBackSprite;
    private Sprite secondGuestBackSprite;

    private bool firstGuess, secondGuess;

    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private bool isGameOver = false;

    private bool canPickPuzzle = true;

    private bool isCardsCreated = false;

    public void RestartGame()
    {
        isGameOver = false;
        canPickPuzzle = true;
        firstGuess = secondGuess = false;
        countCorrectGuesses = 0;

        if (size % 2 != 0)
        {
            size++;
            Debug.LogWarning("The size cannot be an odd number. It has been adjusted to the next even number: " + size);
        }

        if (!isCardsCreated)
        {
            AddButtons();
            AddGamePuzzles();
        }

        SetBackSprites();

        foreach (Button button in btns)
        {
            button.interactable = true;
        }

        Shuffle(gamePuzzles);

        gameGuesses = (gamePuzzles.Count) / 2;
        Debug.Log("Game Guessed: " + gameGuesses);
    }

    private void AddButtons()
    {
        fieldGrid.constraintCount = columnCount;
        fieldGrid.cellSize = puzzleSize;
        fieldGrid.spacing = puzzleSpace;

        for (int i = 0; i < size; i++)
        {
            GameObject puzzleObject = Instantiate(puzzlePrefab, puzzleField);
            puzzleObject.name = $"{i}";
            Button puzzleButton = puzzleObject.GetComponent<Button>();
            puzzleButton.onClick.AddListener(PizkPuzzle);
            btns.Add(puzzleButton);
        }

        isCardsCreated = true;
    }

    private void SetBackSprites()
    {
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].image.sprite = backSprites[i % 2];
        }
    }

    private void PizkPuzzle()
    {
        if (isGameOver || !canPickPuzzle)
            return;

        string cardName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        if (!firstGuess)
        {
            firstGuess = true;

            firstGuessIndex = int.Parse(cardName);

            btns[firstGuessIndex].interactable = false;

            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;

            firstGuestBackSprite = btns[firstGuessIndex].image.sprite;

            TurnOverPuzzle(btns[firstGuessIndex].GetComponent<RectTransform>(), btns[firstGuessIndex].image, gamePuzzles[firstGuessIndex]);
            PlaySFX(pickCardSoundName);
        }
        else if (!secondGuess)
        {
            secondGuess = true;

            secondGuessIndex = int.Parse(cardName);

            btns[secondGuessIndex].interactable = false;

            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

            secondGuestBackSprite = btns[secondGuessIndex].image.sprite;

            TurnOverPuzzle(btns[secondGuessIndex].GetComponent<RectTransform>(), btns[secondGuessIndex].image, gamePuzzles[secondGuessIndex], () => StartCoroutine(CheckPuzzles()));
            PlaySFX(pickCardSoundName);
        }
    }

    private void TurnOverPuzzle(RectTransform buttonRectTransform, Image buttonImage, Sprite puzzleSprite, Action onCompleteAction = null)
    {
        canPickPuzzle = false;
        buttonRectTransform.DORotate(new Vector3(0, 90f, 0), rotationTime).OnComplete(() =>
        {
            buttonImage.sprite = puzzleSprite;
            buttonRectTransform.DORotate(Vector3.zero, rotationTime).OnComplete(() =>
            {
                canPickPuzzle = true;
                onCompleteAction?.Invoke();
            });
        });
    }

    IEnumerator CheckPuzzles()
    {
        yield return new WaitForSeconds(timeToCheck);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(0.5f);

            countCorrectGuesses++;

            if (countCorrectGuesses == gameGuesses)
            {
                PlaySFX(gameoverSoundName);
                GameOver();
            }
            else
            {
                PlaySFX(correctGuessSoundName);
            }
                
        }
        else
        {
            PlaySFX(wrongGuessSoundName);

            PlaySFX(unPickCardSoundName);

            TurnOverPuzzle(btns[firstGuessIndex].GetComponent<RectTransform>(), btns[firstGuessIndex].image, firstGuestBackSprite, () => btns[firstGuessIndex].interactable = true);
            TurnOverPuzzle(btns[secondGuessIndex].GetComponent<RectTransform>(), btns[secondGuessIndex].image, secondGuestBackSprite, () => btns[secondGuessIndex].interactable = true);
        }

        yield return new WaitForSeconds(0.5f);

        firstGuess = secondGuess = false;
    }

    private void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        ICustomVariableManager vars = Engine.GetService<ICustomVariableManager>();
        vars.TrySetVariableValue("isCardGameCompleted", true);
        playScript.Play();
    }

    private void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;

        for (int i = 0; i < looper; i += 2)
        {
            if (index >= puzzles.Length)
                index = 0;

            gamePuzzles.Add(puzzles[index]);
            if (i + 1 < looper)
            {
                gamePuzzles.Add(puzzles[index]);
            }

            index++;
        }
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public async void PlaySFX(string clipName)
    {
        var audioManager = Engine.GetService<IAudioManager>();
        await audioManager.PlaySfxAsync(clipName);
    }
}
