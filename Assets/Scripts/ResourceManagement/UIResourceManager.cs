using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using UnityEditor;
using UnityEngine;

namespace ResourceManagement
{
    public class UIResourceManager : MonoBehaviour
    {
        [field:Header("Sprites")] [field:SerializeField] public List<Sprite> Sprites { get; private set; } = new List<Sprite>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            string[] enumEntries = Sprites.Select(x => x.name).ToArray();
            string enumName = "UIResourceEnum";
            string filePathAndName = "Assets/Scripts/ResourceManagement/" + enumName + ".cs";

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + enumName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < enumEntries.Length; i++)
                {
                    streamWriter.WriteLine("	" + enumEntries[i] + ",");
                }

                streamWriter.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
#endif
        
        public Sprite GetSprite(UIResourceEnum resource)
        {
            Sprite sprite = Sprites[(int)resource];
            if (sprite == null)
            {
                throw new Exception("no resource for this");
            }
            return sprite;
        }
    }
}