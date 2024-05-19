using System;
using DG.Tweening;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [Header("Overed Behavior")] [SerializeField]
    private bool IsOvered;
    
    private Vector3 _idleScale;
    private Vector3 _overedScale;
    [SerializeField] private float _bumpScale;
    [SerializeField] private float _effectDuration;
    [SerializeField] private int _bumpNumber;

    [HideInInspector] public Vector3 IdlePosition;
    [SerializeField] private float _moveY;
    private float _overedPositionY;
    
    public Action MouseEnter;
    public Action MouseExit;

    private void Awake()
    {
        MouseEnter += DoEnterTween;
        MouseExit += DoExitTween;
    }

    private void Start()
    {
        IdlePosition = transform.position;
        _overedPositionY = IdlePosition.y + _moveY;

        _idleScale = transform.localScale;
        _overedScale = transform.localScale * _bumpScale;
    }

    private void OnMouseEnter()
    {
        if (IsOvered == false)
        {
            CardManager.Instance.CardSelected = gameObject;
            MouseEnter.Invoke();
        }
    }

    private void OnMouseExit()
    {
        MouseExit.Invoke();
    }
    
    private void DoEnterTween()
    {
        IsOvered = true;
        
        transform.DOPunchScale(_overedScale - (transform.localScale - _idleScale - _overedScale), _effectDuration, _bumpNumber);

        transform.DOLocalMoveY(_overedPositionY, _effectDuration);
    }

    private void DoExitTween()
    {
        IsOvered = false;

        transform.DOMove(IdlePosition, _effectDuration);
    }
}