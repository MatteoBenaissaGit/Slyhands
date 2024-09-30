using UnityEngine;
using UnityEditor;

public class CardDesignerWindow : EditorWindow
{
    private Vector2 scrollPosition;

    private string cardName;
    private Font nameFont;
    private Color nameColor = Color.black;

    private Texture2D _background;

    private Texture2D _illustration;
    private int _illustrationPosX = 0;
    private int _illustrationPosY = 0;
    private int _illustrationSize = 32;

    private Texture2D typeIcon;
    private int _iconPosX = 0;
    private int _iconPosY = 0;
    private int _iconSize = 32;

    [MenuItem("Tools/Card Designer")]
    public static void ShowWindow()
    {
        GetWindow<CardDesignerWindow>("Card Designer");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width),
            GUILayout.Height(position.height));

        GUILayout.Label("Card Designer", EditorStyles.boldLabel);

        GUILayout.Space(20);
        cardName = EditorGUILayout.TextField("Card Name", cardName);
        nameFont = (Font)EditorGUILayout.ObjectField("Card Font", nameFont, typeof(Font), false);
        nameColor = EditorGUILayout.ColorField("Name Color", nameColor);

        GUILayout.Space(20);
        _background = (Texture2D)EditorGUILayout.ObjectField("Background", _background, typeof(Texture2D), false);

        GUILayout.Space(20);
        _illustration = (Texture2D)EditorGUILayout.ObjectField("Illustration", _illustration, typeof(Texture2D), false);
        _illustrationPosX = EditorGUILayout.IntField("Illu. Pos X", _illustrationPosX);
        _illustrationPosY = EditorGUILayout.IntField("Illu. Pos Y", _illustrationPosY);
        _illustrationSize = EditorGUILayout.IntField("Illu. Size", _illustrationSize);

        GUILayout.Space(20);
        typeIcon = (Texture2D)EditorGUILayout.ObjectField("Type Icon", typeIcon, typeof(Texture2D), false);
        _iconPosX = EditorGUILayout.IntField("Icon Pos X", _iconPosX);
        _iconPosY = EditorGUILayout.IntField("Icon Pos Y", _iconPosY);
        _iconSize = EditorGUILayout.IntField("Icon Size", _iconSize);

        GUILayout.Space(20);
        GUILayout.Label("Preview:", EditorStyles.boldLabel);

        if (_background != null)
        {
            float aspectRatio = (float)_background.width / _background.height;

            float previewWidth = 100f;
            float previewHeight = previewWidth / aspectRatio;

            Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
            GUI.DrawTexture(previewRect, _background, ScaleMode.ScaleToFit);

            Rect nameRect = new Rect(previewRect.width / 2 - (cardName.Length * 3), previewRect.y + 10,
                previewRect.width, 20);

            GUI.contentColor = nameColor;

            GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel);
            nameStyle.normal.textColor = nameColor;
            if (nameFont != null)
            {
                nameStyle.font = nameFont;
            }

            GUI.Label(nameRect, cardName, nameStyle);
            GUI.contentColor = Color.white;

            if (_illustration != null)
            {
                Rect illuRect = new Rect(previewRect.x + _illustrationPosX - (_illustrationSize / 2),
                    previewRect.y + _illustrationPosY - (_illustrationSize / 2), _illustrationSize, _illustrationSize);
                GUI.DrawTexture(illuRect, _illustration, ScaleMode.ScaleToFit);
            }

            if (typeIcon != null)
            {
                Rect iconRect = new Rect(previewRect.x + _iconPosX - (_iconSize / 2),
                    previewRect.y + _iconPosY - (_iconSize / 2), _iconSize, _iconSize);
                GUI.DrawTexture(iconRect, typeIcon, ScaleMode.ScaleToFit);
            }
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Create Card"))
        {
            CreateCard();
        }

        GUILayout.Space(20);

        EditorGUILayout.EndScrollView();
    }

    private void CreateCard()
    {
        GameObject cardObject = new GameObject(cardName);

        MeshRenderer renderer = cardObject.AddComponent<MeshRenderer>();
        MeshFilter filter = cardObject.AddComponent<MeshFilter>();

        Material cardMaterial = new Material(Shader.Find("Standard"));
        cardMaterial.mainTexture = _background;
        renderer.material = cardMaterial;

        Debug.Log($"Card '{cardName}' created with illustration and type icon.");
    }
}