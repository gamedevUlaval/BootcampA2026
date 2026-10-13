using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveInput;
    private CharacterController controller;
    public float moveSpeed = 5f;
    public float turnSpeed = 12f;

    [Tooltip("Corrects the model facing if it points the wrong way, try 180 if it walks backwards.")]
    public float modelYawOffset = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        direction = Vector3.ClampMagnitude(direction, 1f);

        controller.Move(direction * Time.deltaTime * moveSpeed);

        FaceDirection(direction);
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up)
                                 * Quaternion.Euler(0f, modelYawOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Mathf.Clamp01(turnSpeed * Time.deltaTime));
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}