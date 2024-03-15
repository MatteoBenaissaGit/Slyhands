﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using Inputs;
using Sirenix.OdinInspector;
using Slots;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LevelEditor
{
    /// <summary>
    /// This class manage all the action buttons in the level editor
    /// </summary>
    public class LevelEditorUIActionButtons : MonoBehaviour
    {
        [SerializeField, Required] private LevelEditorActionButtonController _selectionButton;
        [SerializeField, Required] private LevelEditorActionButtonController _paintButton;

        private List<LevelEditorActionButtonController> _buttons;
        private LevelEditorActionButtonController _currentButton;
        private bool _isHoldingClick;
        private RaycastHit[] _mouseClickRaycastHits ;
        
        private void Awake()
        {
            _mouseClickRaycastHits = new RaycastHit[32];
            
            _buttons = new List<LevelEditorActionButtonController>()
            {
                _selectionButton,
                _paintButton
            };
            
            _selectionButton.Button.onClick.AddListener(() => SetCurrentButton(_selectionButton));
            _paintButton.Button.onClick.AddListener(() => SetCurrentButton(_paintButton));

            _currentButton = _selectionButton;
            _currentButton.SetSelected(true, true);
        }

        private void OnDestroy()
        {
            _selectionButton.Button.onClick.RemoveListener(() => SetCurrentButton(_selectionButton));
            _paintButton.Button.onClick.RemoveListener(() => SetCurrentButton(_paintButton));
        }

        private void Start()
        {
            InputManager.Instance.LevelEditorInput.OnClickTap += ClickTapAction;
            InputManager.Instance.LevelEditorInput.OnClickHold += (bool doHold) => _isHoldingClick = doHold;
        }

        private void Update()
        {
            if (_isHoldingClick)
            {
                ClickHoldAction();
            }
        }

        /// <summary>
        /// Set the current used button
        /// </summary>
        /// <param name="buttonToSet">The button to set as current</param>
        public void SetCurrentButton(LevelEditorActionButtonController buttonToSet)
        {
            _currentButton.SetSelected(false);
            _currentButton = buttonToSet;
            _currentButton.SetSelected(true);
        }

        /// <summary>
        /// Handle the action made when the user tap click 
        /// </summary>
        private void ClickTapAction()
        {
            SlotLocation slotLocation = GetClickedSlotLocation();
            if (_currentButton == null || slotLocation == null)
            {
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    break;
                case LevelEditorActionButtonType.Paint:
                    LevelEditorManager.Instance.Board.CreateSlotAt(slotLocation.Coordinates);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handle the action made when the user hold click
        /// </summary>
        private void ClickHoldAction()
        {
            SlotLocation slotLocation = GetClickedSlotLocation();
            if (_currentButton == null || slotLocation == null)
            {
                return;
            }
            
            switch (_currentButton.Type)
            {
                case LevelEditorActionButtonType.Selection:
                    break;
                case LevelEditorActionButtonType.Paint:
                    LevelEditorManager.Instance.Board.CreateSlotAt(slotLocation.Coordinates);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private SlotLocation GetClickedSlotLocation()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return null;
            }
            
            Ray ray = LevelEditorManager.Instance.Camera.Camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _mouseClickRaycastHits);
            for (int i = 0; i < hits; i++)
            {
                if (_mouseClickRaycastHits[i].collider.TryGetComponent(out SlotLocation location) == false)
                {
                    continue;
                }
                return location;
            }

            return null;
        }
    }
}