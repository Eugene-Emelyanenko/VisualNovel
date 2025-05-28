using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Variables")]
    [SerializeField] private Vector2 puzzleSize;
    [SerializeField] private Vector2 puzzleSpace;
    [SerializeField] private int columnCount;
    [SerializeField] private int size;
    [SerializeField] private float timeToCheck;
    [SerializeField] private float rotationTime = 0.25f;

    [Header("Sprites")]
    [SerializeField] private Sprite[] puzzles;
    [SerializeField] private Sprite backSprite;

    [Header("Lists")]
    [SerializeField] private List<Sprite> gamePuzzles = new List<Sprite>();
    [SerializeField] private List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;

    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private bool isGameOver = false;

    private bool canPickPuzzle = true;

    private void Start()
    {
        if (size % 2 != 0)
        {
            size++;
            Debug.LogWarning("The size cannot be an odd number. It has been adjusted to the next even number: " + size);
        }

        AddButtons();
        AddGamePuzzles();
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

            TurnOverPuzzle(btns[firstGuessIndex].GetComponent<RectTransform>(), btns[firstGuessIndex].image, gamePuzzles[firstGuessIndex]);
        }
        else if(!secondGuess)
        {
            secondGuess = true;

            secondGuessIndex = int.Parse(cardName);

            btns[secondGuessIndex].interactable = false;

            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

            TurnOverPuzzle(btns[secondGuessIndex].GetComponent<RectTransform>(), btns[secondGuessIndex].image, gamePuzzles[secondGuessIndex], () => StartCoroutine(CheckPuzzles()));
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

        if(firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(0.5f);

            countCorrectGuesses++;

            if (countCorrectGuesses == gameGuesses)
                GameOver();
        }
        else
        {
            TurnOverPuzzle(btns[firstGuessIndex].GetComponent<RectTransform>(), btns[firstGuessIndex].image, backSprite, () => btns[firstGuessIndex].interactable = true);
            TurnOverPuzzle(btns[secondGuessIndex].GetComponent<RectTransform>(), btns[secondGuessIndex].image, backSprite, () => btns[secondGuessIndex].interactable = true);
        }

        yield return new WaitForSeconds(0.5f);

        firstGuess = secondGuess = false;
    }

    private void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
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
}
