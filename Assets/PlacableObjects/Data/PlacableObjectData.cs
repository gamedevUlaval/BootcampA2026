using UnityEngine;

[CreateAssetMenu(fileName = "PlacableObjectData", menuName = "Scriptable Objects/PlacableObjectData")]
public class PlacableObjectData : ScriptableObject
{
    public string displayName;
    public int footprintRadius;
}
