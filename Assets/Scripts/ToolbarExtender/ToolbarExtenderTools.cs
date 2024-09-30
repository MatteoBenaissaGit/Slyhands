using System.Collections.Generic;
using Card.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[InitializeOnLoad]
public class ScenePanelToolbar
{
#if UNITY_EDITOR

    private static SceneList _sceneList;

    static ScenePanelToolbar()
    {
        UnityToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(OnScenePanelGUI);
    }

    private static void OnScenePanelGUI()
    {
        GUILayout.FlexibleSpace();

        var sceneImage = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image;
        if (GUILayout.Button(new GUIContent("GameScene", sceneImage, "Start GameScene")))
        {
            if (Event.current.button == 0)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
            }
        }

        if (GUILayout.Button(new GUIContent("LevelEditorScene", sceneImage, "Start LevelEditorScene")))
        {
            if (Event.current.button == 0)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/LevelEditorScene.unity");
            }
        }

        DraggableSceneSpace();
    }

    private static void DraggableSceneSpace()
    {
        //get scene list
        if (_sceneList ==  null)
        {
            _sceneList = AssetDatabase.LoadAssetAtPath<SceneList>("Assets/Scripts/ToolbarExtender/SceneList.asset");
            if (_sceneList == null)
            {
                EditorGUILayout.HelpBox("SceneList asset not found!", MessageType.Error, true);
                return;
            }
        }

        _sceneList.Scenes.RemoveAll(x => x == null);

        if (_sceneList.Scenes.Count > 0)
        {
            EditorGUILayout.Space(20);
        }

        //draw scenes
        List<int> scenesToRemove = new();
        var sceneImage = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image;
        for (int i = 0; i < _sceneList.Scenes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            string name = _sceneList.Scenes[i].name;
            if (GUILayout.Button(new GUIContent(name, sceneImage, $"Open {name}")))
            {
                if (Event.current.button == 0)
                {
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_sceneList.Scenes[i]));
                }
                else
                {
                    scenesToRemove.Add(i);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        foreach (int index in scenesToRemove)
        {
            _sceneList.Scenes.RemoveAt(index);
        }

        // draggable space
        EditorGUILayout.Space(20);
        SceneAsset newScene =
            (SceneAsset)EditorGUILayout.ObjectField(null, typeof(SceneAsset), false, GUILayout.Width(150));

        if (newScene != null)
        {
            _sceneList.Scenes.Add(newScene);
            EditorUtility.SetDirty(_sceneList);
            AssetDatabase.SaveAssetIfDirty(_sceneList);
        }
    }

    [InitializeOnLoad]
    public class ToolbarRightButtons
    {
        static ToolbarRightButtons()
        {
            UnityToolbarExtender.ToolbarExtender.RightToolbarGUI.Add(OnButtonsGUI);
        }

        private static void OnButtonsGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Card Designer", EditorGUIUtility.IconContent("d_Grid.BoxTool").image)))
            {
                CardDesignerWindow.OpenWindow();
            }

            EditorGUILayout.Space(30);

            if (GUILayout.Button(new GUIContent("Editor icons", EditorGUIUtility.IconContent("d_CustomTool").image)))
            {
                Plugins.IconWindows.OpenWindow();
            }

            EditorGUILayout.Space(30);

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Settings"))))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Settings"))))
            {
                SettingsService.OpenUserPreferences();
            }

            EditorGUILayout.Space(30);
        }
    }
#endif
}