using System;
using System.Collections.Generic;
using UnityEngine;

public class Detector<T> : MonoBehaviour
{
    public Action<T> OnDetected;
    public Action<T> OnLost;
    public List<T> DetectedList;

    protected void Awake()
    {
        DetectedList = new List<T>();
    }

    protected void OnDisable()
    {
        DetectedList.Clear();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        T detected = other.GetComponent<T>();

        if (detected != null && !DetectedList.Contains(detected))
        {
            DetectedList.Add(detected);
            OnDetected?.Invoke(detected);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        T detected = other.GetComponent<T>();

        if (detected != null && DetectedList.Contains(detected))
        {
            DetectedList.Remove(detected);
            OnLost?.Invoke(detected);
        }
    }

    public void Clean()
    {
        DetectedList.RemoveAll(x => x == null);
    }
}