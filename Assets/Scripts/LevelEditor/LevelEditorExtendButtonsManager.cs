using System;
using Board;
using LevelEditor;
using Slots;
using UnityEngine;

public class LevelEditorExtendButtonsManager : MonoBehaviour
{
    public Action<WorldOrientation.Orientation> OnExtend { get; set; }

    [SerializeField] private LevelEditorExtendButtonController[] _extendButtons;

    private BoardController _boardController;
    private int _height = 0;

    private void Awake()
    {
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void Initialize(BoardController boardController)
    {
        _boardController = boardController;
        OnExtend = null;
        OnExtend += _boardController.ExtendBoard;
        OnExtend += ExtendButtonClicked;
        
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            //event
            button.gameObject.SetActive(true);
            button.Initialize(this);

            //position
            SetButtonPosition(button);
        }
        
        LevelEditorManager.Instance.UI.OnHeightChanged += SetButtonsHeight;
        SetButtonsHeight(0);
    }

    private void SetButtonPosition(LevelEditorExtendButtonController button)
    {
        Vector2Int direction = WorldOrientation.GetDirection(button.Orientation);
        Vector3 boardCenter = _boardController.WorldCenter;
        boardCenter.y = _boardController.GetCoordinatesToWorldPosition(new Vector3(0,_height,0)).y;
        Vector3 boardSize = _boardController.Data.Size;
        float xOffset = direction.x * boardSize.x / 2f + direction.x;
        float zOffset = direction.y * boardSize.z / 2f + direction.y;
        Vector3 newPosition = boardCenter + new Vector3(xOffset, 0, zOffset);
        button.transform.position = newPosition;
    }

    private void SetButtonsHeight(int height)
    {
        _height = height;
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            Vector3 position = button.transform.position;
            float heightWorld = _boardController.GetCoordinatesToWorldPosition(new Vector3(0,_height,0)).y;
            position.y = heightWorld;
            button.transform.position = position;
        }
    }

    private void ExtendButtonClicked(WorldOrientation.Orientation orientation)
    {
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            SetButtonPosition(button);
        }
    }
}
