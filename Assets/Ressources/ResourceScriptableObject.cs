using UnityEngine;

[CreateAssetMenu(fileName = "ResourceScriptableObject", menuName = "Scriptable Objects/ResourceScriptableObject")]
public class ResourceScriptableObject : ScriptableObject
{
    public ResourcesType resourcesType;
    public int value;
    public GameObject ressourcePrefab;
}
