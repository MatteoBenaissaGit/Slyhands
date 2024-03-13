using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using UnityEditor;
using UnityEngine;

namespace ResourceManagement
{
    public class AssetResourcesManager : MonoBehaviour
    {
        [field:Header("Resources")] [field:SerializeField] public List<GameObject> Resources { get; private set; } = new List<GameObject>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            string[] enumEntries = Resources.Select(x => x.name).ToArray();
            string enumName = "AssetResourceEnum";
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
        
        public GameObject InstantiateResource(AssetResourceEnum resource)
        {
            GameObject go = Resources[(int)resource];
            if (go == null)
            {
                throw new Exception("no resource for this");
            }
            return Instantiate(go);
        }
    }
}