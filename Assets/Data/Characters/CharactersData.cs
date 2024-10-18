using System;
using System.Collections.Generic;
using Board.Characters;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
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
        [field:SerializeField] public bool IsDetected { get; set; }
    }
    
    [Serializable]
    public class CharacterData
    {
        [field:SerializeField] public CharacterType Type { get; private set; }    
        [field:SerializeField] public int Life { get; private set; }    
        [field:SerializeField] public int MovementPoints { get; private set; }    
        [field:SerializeField] public int FootPrintLength { get; private set; }

        [field:SerializeField, BoxGroup("View")] public bool HasViewDetection { get; private set; } = true;
        
        [field:SerializeField, ShowIf("HasViewDetection"), BoxGroup("View")] public Vector2Int ViewDetectionSize { get; private set; }

        [ShowInInspector, DoNotDrawAsReference, ShowIf("HasViewDetection"), BoxGroup("View")]
        [TableMatrix(HorizontalTitle = "View Detection", DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, SquareCells = true, Transpose = true)]
        public bool[,] Transposed 
        {
            get => ViewDetection;
            set
            {
                ViewDetection = value;
                ViewDetectionList ??= new List<ViewDetectionSquare>();
                ViewDetectionList.Clear();
                for (int x = 0; x < value.GetLength(0); x++)
                {
                    for (int y = 0; y < value.GetLength(1); y++)
                    {
                        int yPosition = ViewDetectionCenter.x - x; 
                        int xPosition = ViewDetectionCenter.y - y; 
                        ViewDetectionList.Add(new ViewDetectionSquare(new Vector2Int(xPosition, yPosition), value[x, y]));
                    }
                }
            }
        }
        
        public bool[,] ViewDetection;
        [SerializeField, HideInInspector] public List<ViewDetectionSquare> ViewDetectionList;

        [field: SerializeField, BoxGroup("Sound")] public bool DoMakeSoundMoving { get; private set; } = false; 
        [field: SerializeField, BoxGroup("Sound"), ShowIf("DoMakeSoundMoving")] public int MoveSoundRange { get; private set; } = 2; 
        [field: SerializeField, BoxGroup("Sound")] public int EarSoundRange { get; private set; } = 0; 
        
        private Vector2Int ViewDetectionCenter => new(ViewDetection.GetLength(0) / 2, ViewDetection.GetLength(1) / 2);

#if UNITY_EDITOR
        private static bool DrawColoredEnumElement(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.71f, 0.71f, 0.71f) : new Color(0, 0, 0, 0.5f));
            if (value)
            {
                EditorGUI.DrawTextureAlpha(rect.Padding(6), EditorGUIUtility.IconContent("d_animationvisibilitytoggleon").image);
            }

            return value;
        }

        [OnInspectorInit]
        private void OnInspectorInitializeSetVisionDetectionData()
        {
            SetVisionDetectionData(false);
        }
        
        public void SetVisionDetectionData(bool canClearList = true)
        {
            if (HasViewDetection == false) return;
            
            if (ViewDetection != null)
            {
                ViewDetection[ViewDetectionCenter.x, ViewDetectionCenter.y] = true;
            }
            
            if (ViewDetection != null && ViewDetection.GetLength(0) == ViewDetectionSize.x && ViewDetection.GetLength(1) == ViewDetectionSize.y)
            {
                UpdateArrayFromDetectionList();
                return;
            }
            
            
            Vector2Int detectionSize = ViewDetectionSize;
            detectionSize.x = detectionSize.x % 2 == 0 ? detectionSize.x + 1 : detectionSize.x;
            detectionSize.y = detectionSize.y % 2 == 0 ? detectionSize.y + 1 : detectionSize.y;
            ViewDetectionSize = detectionSize;
            
            ViewDetection = new bool[detectionSize.x, detectionSize.y];
            ViewDetection[ViewDetectionCenter.x, ViewDetectionCenter.y] = true;
            
            UpdateArrayFromDetectionList();
        }

        private void UpdateArrayFromDetectionList()
        {
            if (ViewDetectionList == null)
            {
                return;
            }
            
            foreach (ViewDetectionSquare square in ViewDetectionList)
            {
                if (square.Position == Vector2Int.zero)
                {
                    square.IsDetected = true;
                }

                int xPosition = ViewDetectionCenter.x - square.Position.y;
                int yPosition = ViewDetectionCenter.y - square.Position.x;
                if (xPosition >= ViewDetection.GetLength(0) || yPosition >= ViewDetection.GetLength(1) || xPosition < 0 || yPosition < 0)
                {
                    continue;
                }
                ViewDetection[xPosition, yPosition] = square.IsDetected;
            }
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
            Characters.ForEach(x => x.SetVisionDetectionData());
            EditorUtility.SetDirty(this);
        }
    }
}