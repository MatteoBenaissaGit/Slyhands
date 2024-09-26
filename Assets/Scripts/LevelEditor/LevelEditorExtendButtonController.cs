using System;
using DG.Tweening;
using Slots;
using UnityEngine;

namespace LevelEditor
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    public class LevelEditorExtendButtonController : MonoBehaviour
    {
        [field:SerializeField] public WorldOrientation.Orientation Orientation { get; private set; }
        public Action OnExtend { get; set; }
    
        [SerializeField] private Color _baseColor, _hoveredColor;

        private SpriteRenderer _spriteRenderer;
    
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = _baseColor;
        }

        private void OnMouseEnter()
        {
            _spriteRenderer.color = _hoveredColor;
        }

        private void OnMouseExit()
        {
            _spriteRenderer.color = _baseColor;
        }

        private void OnMouseDown()
        {
            OnExtend?.Invoke();
            
            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            
            //TODO reposition the button at right place +1
        }
    }
}
