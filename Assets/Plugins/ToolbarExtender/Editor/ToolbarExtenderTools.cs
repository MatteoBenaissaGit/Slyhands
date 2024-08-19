using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace Plugins.unity_toolbar_extender_master.Editor
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        private static SceneList _sceneList;
        
        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if(GUILayout.Button(new GUIContent("GameScene", "Start GameScene")))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
            }

            if(GUILayout.Button(new GUIContent("LevelEditorScene", "Start LevelEditorScene")))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/LevelEditorScene.unity");
            }
            
            DraggableSceneSpace();
        }

        private static void DraggableSceneSpace()
        {
            //get scene list
            if (_sceneList == null)
            {
                _sceneList = AssetDatabase.LoadAssetAtPath<SceneList>("Assets/Plugins/ToolbarExtender/Editor/SceneList.asset");
                if (_sceneList == null)
                {
                    EditorGUILayout.HelpBox("SceneList asset not found!", MessageType.Error, true);
                    return;
                }
            }

            _sceneList.Scenes.RemoveAll(x => x == null);

            //draw scenes
            List<int> scenesToRemove = new();
            for (int i = 0; i < _sceneList.Scenes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(_sceneList.Scenes[i].name))
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
            SceneAsset newScene = (SceneAsset)EditorGUILayout.ObjectField(null, typeof(SceneAsset), false);
            if (newScene != null)
            {
                _sceneList.Scenes.Add(newScene);
            }
        }
    }
}