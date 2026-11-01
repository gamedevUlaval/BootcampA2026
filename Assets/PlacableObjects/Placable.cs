using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

public class Placable : MonoBehaviour
{
    public PlacableObjectData data;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 textPosition = transform.position + Vector3.up * 1.5f;

        Handles.Label(textPosition, gameObject.name);
    }
#endif
}
