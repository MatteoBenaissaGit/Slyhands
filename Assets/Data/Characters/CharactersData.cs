using System;
using System.Collections.Generic;
using Board.Characters;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Data.Characters
{
    [Serializable]
    public class ViewDetectionSquare
    {
        public ViewDetectionSquare(Vector2Int position, bool isDetected)
        {
            Position = position;
            IsDetected = isDetected;
        }
        
        [field:SerializeField] public Vector2Int Position { get; private set; }
        [field:SerializeField] public bool IsDetected { get; private set; }
    }
    
    [Serializable]
    public class CharacterData
    {
        [field:SerializeField] public CharacterType Type { get; private set; }    
        [field:SerializeField] public int Life { get; private set; }    
        [field:SerializeField] public int MovementPoints { get; private set; }    
        [field:SerializeField] public int FootPrintLength { get; private set; }

        [field:SerializeField] public bool HasViewDetection { get; private set; } = true;
        
        [field:SerializeField, ShowIf("HasViewDetection")] public Vector2Int ViewDetectionSize { get; private set; }

        [ShowInInspector, DoNotDrawAsReference, ShowIf("HasViewDetection")]
        [TableMatrix(HorizontalTitle = "View Detection", DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, SquareCells = true, Transpose = true)]
        public bool[,] Transposed 
        {
            get => ViewDetection;
            set
            {
                ViewDetection = value;
                ViewDetectionList.Clear();
                for (int x = 0; x < value.GetLength(0); x++)
                {
                    for (int y = 0; y < value.GetLength(1); y++)
                    {
                        ViewDetectionList.Add(new ViewDetectionSquare(new Vector2Int(x, y), value[x, y]));
                    }
                }
            }
        }
        
        public bool[,] ViewDetection;
        public List<ViewDetectionSquare> ViewDetectionList = new();

#if UNITY_EDITOR
        private static bool DrawColoredEnumElement(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));

            return value;
        }

        [OnInspectorInit]
        public void CreateData()
        {
            if (ViewDetection != null)
            {
                ViewDetection[ViewDetection.GetLength(0) / 2, ViewDetection.GetLength(1) / 2] = true;
            }
            
            if (ViewDetection != null && ViewDetection.GetLength(0) == ViewDetectionSize.x && ViewDetection.GetLength(1) == ViewDetectionSize.y)
            {
                if (ViewDetectionList != null)
                {
                    foreach (ViewDetectionSquare square in ViewDetectionList)
                    {
                        if (square.Position.x >= ViewDetection.GetLength(0) || square.Position.y >= ViewDetection.GetLength(1)) continue;
                        ViewDetection[square.Position.x, square.Position.y] = square.IsDetected;
                    }
                }
                return;
            }
            
            
            Vector2Int detectionSize = ViewDetectionSize;
            detectionSize.x = detectionSize.x % 2 == 0 ? detectionSize.x + 1 : detectionSize.x;
            detectionSize.y = detectionSize.y % 2 == 0 ? detectionSize.y + 1 : detectionSize.y;
            ViewDetectionSize = detectionSize;
            
            ViewDetection = new bool[detectionSize.x, detectionSize.y];
            ViewDetection[ViewDetection.GetLength(0) / 2, ViewDetection.GetLength(1) / 2] = true;
        }
#endif
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "Data/Characters", order = 1)]
    public class CharactersData : ScriptableObject
    {
        [SerializeField] private List<CharacterData> Characters = new List<CharacterData>();

        public CharacterData GetCharacterData(CharacterType type)
        {
            CharacterData character = Characters.Find(x => x.Type == type);
            if (character == null)
            {
                throw new Exception($"CharacterData not found for type {type}");
            }
            return character;
        }

        private void OnValidate()
        {
            Characters.ForEach(x => x.CreateData());
        }
    }
}