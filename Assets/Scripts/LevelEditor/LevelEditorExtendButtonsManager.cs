using Board;
using LevelEditor;
using Slots;
using UnityEngine;

public class LevelEditorExtendButtonsManager : MonoBehaviour
{
    [SerializeField] private LevelEditorExtendButtonController[] _extendButtons;

    public void Initialize(BoardController boardController)
    {
        foreach (LevelEditorExtendButtonController button in _extendButtons)
        {
            //event
            button.OnExtend += () => boardController.ExtendBoard(button.Orientation);

            //position
            Vector2Int direction = WorldOrientation.GetDirection(button.Orientation);
            transform.position += new Vector3(direction.x, 0, direction.y);
        }
    }
}
