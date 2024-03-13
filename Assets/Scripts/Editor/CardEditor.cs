using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    SerializedProperty _cardTypeProp;

    public List<SerializedProperty> CommonProp = new List<SerializedProperty>();

    public List<SerializedProperty> VialsProp = new List<SerializedProperty>();
    public List<SerializedProperty> ParchmentsProp = new List<SerializedProperty>();
    public List<SerializedProperty> UtilitiesProp = new List<SerializedProperty>();
    public List<SerializedProperty> EnchantmentsProp = new List<SerializedProperty>();

    void OnEnable()
    {
        //Card Type Property
        _cardTypeProp = serializedObject.FindProperty("CardType");

        //Common Properties
        CommonProp.Add(serializedObject.FindProperty("CardTitle"));
        CommonProp.Add(serializedObject.FindProperty("CardSprite"));
        CommonProp.Add(serializedObject.FindProperty("CardDescription"));
        CommonProp.Add(serializedObject.FindProperty("CardPrice"));

        //Vial Properties
        VialsProp.Add(serializedObject.FindProperty("TestVariableVial1"));
        VialsProp.Add(serializedObject.FindProperty("TestVariableVial2"));

        //Parchment Properties
        ParchmentsProp.Add(serializedObject.FindProperty("TestVariableParchment1"));
        ParchmentsProp.Add(serializedObject.FindProperty("TestVariableParchment2"));
        ParchmentsProp.Add(serializedObject.FindProperty("TestVariableParchment3"));

        //Utilities Properties
        UtilitiesProp.Add(serializedObject.FindProperty("TestVariableUtility1"));
        UtilitiesProp.Add(serializedObject.FindProperty("TestVariableUtility2"));

        //Enchantment Properties
        EnchantmentsProp.Add(serializedObject.FindProperty("TestVariableEnchantment1"));
        EnchantmentsProp.Add(serializedObject.FindProperty("TestVariableEnchantment2"));
        EnchantmentsProp.Add(serializedObject.FindProperty("TestVariableEnchantment3"));
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_cardTypeProp);

        CardType type = (CardType)_cardTypeProp.enumValueIndex;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Commons Variables", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        foreach (var property in CommonProp)
            EditorGUILayout.PropertyField(property, true);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Specifics Variables", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        switch (type)
        {
            case CardType.Vial:
                foreach (var property in VialsProp)
                    EditorGUILayout.PropertyField(property, true);
                break;
            case CardType.Parchment:
                foreach (var property in ParchmentsProp)
                    EditorGUILayout.PropertyField(property, true);
                break;
            case CardType.Utility:
                foreach (var property in UtilitiesProp)
                    EditorGUILayout.PropertyField(property, true);
                break;
            case CardType.Enchantment:
                foreach (var property in EnchantmentsProp)
                    EditorGUILayout.PropertyField(property, true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}