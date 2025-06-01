using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogger : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image starImage;
    [SerializeField] private Sprite completedStarImage;
    [SerializeField] private Sprite unCompletedStarImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject isCompleted;

    [SerializeField] private float fadeTime = 0.5f;

    public void PlayShowAnimation()
    {
        canvasGroup.DOFade(1, fadeTime);
    }

    public void PlayCompleteAnimation(Action onComplete = null)
    {
        canvasGroup.DOFade(0, fadeTime).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void UpdateUI(Quest quest)
    {
        nameText.text = quest.questName;
        infoText.text = quest.info;
        isCompleted.SetActive(quest.isCompleted);
        starImage.sprite = quest.isCompleted ? completedStarImage : unCompletedStarImage;
    }
}
