using UnityEngine;

public interface IInteractable
{
    string Prompt { get; }
    bool CanInteract();
    void Interact();
}