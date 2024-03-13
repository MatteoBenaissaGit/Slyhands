using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    SerializedProperty cardTypeProp;

    public List<SerializedProperty> CommonProp = new List<SerializedProperty>();

    public List<SerializedProperty> VialsProp = new List<SerializedProperty>();
    public List<SerializedProperty> ParchmentsProp = new List<SerializedProperty>();
    public List<SerializedProperty> UtilitiesProp = new List<SerializedProperty>();
    public List<SerializedProperty> EnchantmentsProp = new List<SerializedProperty>();

    void OnEnable()
    {
        //Card Type Property
        cardTypeProp = serializedObject.FindProperty("CardType");

        //Common Properties
        CommonProp.Add(serializedObject.FindProperty("CardTitle"));
        CommonProp.Add(serializedObject.FindProperty("CardSprite"));
        CommonProp.Add(serializedObject.FindProperty("CardDescription"));
        CommonProp.Add(serializedObject.FindProperty("CardPrice"));

        //Vial Properties
        VialsProp.Add(serializedObject.FindProperty("VialString"));

        //Parchment Properties
        ParchmentsProp.Add(serializedObject.FindProperty("ParchmentInt"));

        //Utilities Properties
        UtilitiesProp.Add(serializedObject.FindProperty("UtilityBool"));

        //Enchantment Properties
        EnchantmentsProp.Add(serializedObject.FindProperty("EnchantmentFloat"));
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(cardTypeProp);

        CardType type = (CardType)cardTypeProp.enumValueIndex;

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