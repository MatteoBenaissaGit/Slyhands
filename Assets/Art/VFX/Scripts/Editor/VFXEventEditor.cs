using System;
using System.Collections.Generic;
using System.Linq;
using Art.VFX.Scripts.Effects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Art.VFX.Scripts.Editor
{
    [CustomEditor(typeof(VFXEvent))]
    public class VFXEventEditor : UnityEditor.Editor
    {
        private SerializedProperty _effects;
        private ReorderableList _list;
        private Type[] _types = new Type[]{};
        
        private void OnEnable()
        {
            _effects = serializedObject.FindProperty("VFXEffects");

            _types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetTypes()
                    where assemblyType.IsSubclassOf(typeof(VFXEffect))
                    select assemblyType).ToArray();

            _list = new ReorderableList(serializedObject, _effects, true, true, true, true);
            _list.drawElementCallback = DrawListItems;
            _list.drawHeaderCallback = DrawHeader;
            _list.onAddDropdownCallback = AddDropDown;
        }

        public override void OnInspectorGUI()
        {
            VFXEvent gameEvent = target as VFXEvent;
            
            _list.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            VFXEvent vfxEvent = target as VFXEvent;
            if (vfxEvent == null) return;

            VFXEffect effect = vfxEvent.VFXEffects[index];
            if (effect == null) return;
            effect.VFXEvent = vfxEvent;

            rect.x += 1;
            rect.width = 6;
            EditorGUI.DrawRect(rect, effect.EffectColorInEditor);
            
            SerializedProperty element = _effects.GetArrayElementAtIndex(index);
            SerializedProperty enabledProperty = element.FindPropertyRelative("Enabled");
            rect.x += 15;
            enabledProperty.boolValue = EditorGUI.Toggle(rect, GUIContent.none, enabledProperty.boolValue);

            rect.x += 30;
            rect.width *= 2;
            string label = vfxEvent.VFXEffects[index].ToString();
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.normal.textColor = effect.EffectColorInEditor;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            EditorGUI.LabelField(rect, label, null, labelStyle);

            if (isFocused == false && isActive == false)
            {
                return;
            }

            foreach (SerializedProperty child in GetChildren(element))
            {
                EditorGUILayout.PropertyField(child);
            }
        }

        private IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextProperty = property.Copy();
            nextProperty.Next(false);

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextProperty)) break;
                    yield return currentProperty;
                } 
                while (currentProperty.Next(false));
            }
        }

        private void DrawHeader(Rect rect)
        {
            //TODO
        }

        private void AddDropDown(Rect rect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Type type in _types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    _effects.arraySize++;
                    SerializedProperty newProp = _effects.GetArrayElementAtIndex(_effects.arraySize - 1);
                    newProp.managedReferenceValue = Activator.CreateInstance(type);
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }
}