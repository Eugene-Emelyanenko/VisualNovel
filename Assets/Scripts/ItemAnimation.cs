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

    private Vector3 startPos;
    private Vector3 startScale;

    private Sequence sequence;

    private void Start()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    public void OnShow()
    {
        transform.position = startPos;
        transform.localScale = startScale;
    }

    public void OnHide()
    {
        sequence.Kill();
    }

    public void Play()
    {       
        sequence = DOTween.Sequence();
        sequence.Append(itemTransform.DOMove(targetPos, moveTime));
        sequence.Join(itemTransform.DOScale(targetScale, scaleTime));
        sequence.OnComplete(() =>
        {
            endAction?.Invoke();
            sequence = null;
        });
    }
}
