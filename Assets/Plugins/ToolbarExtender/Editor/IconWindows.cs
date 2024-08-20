using UnityEditor;
using UnityEngine;

namespace Plugins
{
    internal class IconWindows : EditorWindow
    {
        private string _selectedIconName = "";
        private int _currentPage = 0;
        private const int IconButtonSize = 50;
        private string[] _guids;

        [MenuItem("Tools/Icon Windows")]
        public static void OpenWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(IconWindows));
        }

        private void OnEnable()
        {
            _guids = AssetDatabase.FindAssets("", new[] { "Assets/Plugins/Icons/icons" });
        }

        private void OnGUI()
        {
            float windowWidth = position.width;
            int columns = Mathf.FloorToInt(windowWidth / IconButtonSize) - 1;
            float windowHeight = position.height;
            int rows = Mathf.FloorToInt(windowHeight / (IconButtonSize + 3)) - 1;
            int iconsPerPage = rows * columns;

            int totalPages = (_guids.Length + iconsPerPage - 1) / iconsPerPage;

            //--- next / previous buttons ---
            var centerStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.BeginHorizontal(centerStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Previous") && _currentPage > 0)
            {
                _currentPage--;
            }
            var labelStyle = new GUIStyle() { 
                alignment = TextAnchor.MiddleCenter, 
                fontStyle = FontStyle.Bold, 
                normal = new GUIStyleState(){textColor = new Color(0.83f, 0.83f, 0.83f)}
            };
            EditorGUILayout.LabelField($"{_currentPage + 1} / {totalPages}", labelStyle, GUILayout.Width(80));
            if (GUILayout.Button("Next") && _currentPage < totalPages - 1)
            {
                _currentPage++;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            //--- icons ---
            
            int start = _currentPage * iconsPerPage;
            int end = Mathf.Min(start + iconsPerPage, _guids.Length);

            EditorGUILayout.BeginVertical(centerStyle);
            for (int i = start; i < end; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(_guids[i]);
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                GUIContent content = new GUIContent(icon);

                if (i % columns == 0)
                {
                    EditorGUILayout.BeginHorizontal(centerStyle);
                    GUILayout.FlexibleSpace();
                }

                if (GUILayout.Button(content, GUILayout.Width(IconButtonSize), GUILayout.Height(IconButtonSize)))
                {
                    _selectedIconName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                }

                if (i % columns == columns - 1 || i == end - 1)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            // -- name & copy button ---
            EditorGUILayout.BeginVertical();
            if (string.IsNullOrEmpty(_selectedIconName) == false)
            {
                EditorGUILayout.BeginHorizontal(centerStyle);
                GUILayout.FlexibleSpace();

                // Draw a button with a black background
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.black;
                GUIContent boxContent = new GUIContent() { image = EditorGUIUtility.IconContent(_selectedIconName).image };
                GUILayout.Button(boxContent, GUILayout.Width(IconButtonSize), GUILayout.Height(IconButtonSize));
                GUI.backgroundColor = Color.white;
                GUILayout.Button(boxContent, GUILayout.Width(IconButtonSize), GUILayout.Height(IconButtonSize));
                GUI.backgroundColor = defaultColor;
                
                // copy button
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_selectedIconName);
                GUIContent content = new() { image = EditorGUIUtility.IconContent("d_winbtn_win_restore").image, text = "Copy to clipboard" };
                if (GUILayout.Button(content, GUILayout.Width(150)))
                {
                    EditorGUIUtility.systemCopyBuffer = $"EditorGUIUtility.IconContent(\"{_selectedIconName}\")";
                }
                EditorGUILayout.EndVertical();
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}