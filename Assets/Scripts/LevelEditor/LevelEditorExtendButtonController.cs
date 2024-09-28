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
    
        [SerializeField] private Color _baseColor, _hoveredColor;

        private SpriteRenderer _spriteRenderer;
        private LevelEditorExtendButtonsManager _manager;
    
        public void Initialize(LevelEditorExtendButtonsManager manager)
        {
            _manager = manager;
        }
        
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
            _manager.OnExtend?.Invoke(Orientation);
            
            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        }
    }
}
