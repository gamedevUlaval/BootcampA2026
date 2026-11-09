using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIState<RepresentedObject> : State where RepresentedObject : MonoBehaviour
{
    [SerializeField] protected RepresentedObject representedObject;
    [SerializeField] protected EventSystem eventSystem;
    [SerializeField] protected GameObject firstSelected;
    [SerializeField] protected AudioClip selectedSound;

    public override void StateEnter(object data, Type lastState)
    {
        base.StateEnter(data, lastState);
        if (eventSystem)
        {
            eventSystem.SetSelectedGameObject(firstSelected);
        }
    }
}
