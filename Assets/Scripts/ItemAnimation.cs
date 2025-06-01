using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemAnimation : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private Vector3 targetScale;
    [SerializeField] private float moveTime;
    [SerializeField] private float scaleTime;
    [SerializeField] private UnityEvent endAction;

    public void Play()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(itemTransform.DOMove(targetPos, moveTime));
        sequence.Join(itemTransform.DOScale(targetScale, scaleTime));
        sequence.OnComplete(() => endAction?.Invoke());
    }
}
