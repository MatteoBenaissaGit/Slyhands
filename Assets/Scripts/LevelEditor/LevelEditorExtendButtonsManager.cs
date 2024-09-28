using Board;
using LevelEditor;
using Slots;
using UnityEngine;

public class LevelEditorExtendButtonsManager : MonoBehaviour
{
    [SerializeField] private LevelEditorExtendButtonController[] _extendButtons;

    private BoardController _boardController;
    
    public void Initialize(BoardController boardController)
    {
        _boardController = boardController;
        
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            //event
            button.OnExtend += () => _boardController.ExtendBoard(button.Orientation);

            //position
            Vector2Int direction = WorldOrientation.GetDirection(button.Orientation);
            
            Vector3 boardCenter = _boardController.WorldCenter;
            Vector3 boardSize = _boardController.Data.BoardSize;

            float xOffset = direction.x * boardSize.x / 2f + direction.x;
            float zOffset = direction.y * boardSize.z / 2f + direction.y;
            Vector3 newPosition = boardCenter + new Vector3(xOffset, 0, zOffset);
            button.transform.position = newPosition; 
        }
        
        LevelEditorManager.Instance.UI.OnHeightChanged += SetButtonsHeight;
        SetButtonsHeight(0);
    }

    public void SetButtonsHeight(int height)
    {
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            Vector3 position = button.transform.position;
            float heightWorld =  _boardController.GetCoordinatesToWorldPosition(new Vector3(0,height,0)).y;
            position.y = heightWorld;
            button.transform.position = position;
        }
    }
}
