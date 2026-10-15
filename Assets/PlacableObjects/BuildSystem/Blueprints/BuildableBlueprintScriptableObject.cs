using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildableBlueprintScriptableObject", menuName = "Scriptable Objects/BuildableBlueprintScriptableObject")]
public class BuildableBlueprintScriptableObject : ScriptableObject
{
    public string displayName;
    public List<ResourceEntry> recipe;
    public GameObject prefab;
}
