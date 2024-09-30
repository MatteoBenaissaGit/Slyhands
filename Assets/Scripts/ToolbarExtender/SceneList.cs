using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu]
public class SceneList : ScriptableObject
{
    [field:SerializeField] public List<SceneAsset> Scenes { get; set; } = new();
}