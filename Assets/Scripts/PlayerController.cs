using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private CharacterController controller;
    public float moveSpeed = 5f;

    void Start()
    {
    controller = GetComponent<CharacterController>();
    }
    void Update()
    {
    Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
    direction = Vector3.ClampMagnitude(direction, 1f);
    controller.Move(direction * Time.deltaTime * moveSpeed);
    }

    void OnMove(InputValue value)
    {
    moveInput = value.Get<Vector2>();
    }
}