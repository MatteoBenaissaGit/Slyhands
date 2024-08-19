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
            int rows = Mathf.FloorToInt(windowHeight / IconButtonSize) - 2;
            int iconsPerPage = rows * columns;

            int totalPages = (_guids.Length + iconsPerPage - 1) / iconsPerPage;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous") && _currentPage > 0)
            {
                _currentPage--;
            }

            if (GUILayout.Button("Next") && _currentPage < totalPages - 1)
            {
                _currentPage++;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField($"Page {_currentPage + 1} of {totalPages}");

            int start = _currentPage * iconsPerPage;
            int end = Mathf.Min(start + iconsPerPage, _guids.Length);

            EditorGUILayout.BeginVertical();
            for (int i = start; i < end; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(_guids[i]);
                Texture icon = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                GUIContent content = new GUIContent(icon);

                if (i % columns == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(content, GUILayout.Width(IconButtonSize), GUILayout.Height(IconButtonSize)))
                {
                    _selectedIconName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                }

                if (i % columns == columns - 1)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();

            if (string.IsNullOrEmpty(_selectedIconName) == false)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_restore"), GUILayout.Width(30)))
                {
                    EditorGUIUtility.systemCopyBuffer = $"EditorGUIUtility.IconContent(\"{_selectedIconName}\")";
                }

                EditorGUILayout.LabelField(_selectedIconName);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}