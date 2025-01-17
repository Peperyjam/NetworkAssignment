using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public InputAction playerInput;
    Vector2 moveDirection;
    [SerializeField] AudioListener audioListener;
    [SerializeField] Camera cameraComponent;
    [SerializeField] Rigidbody2D rb;
    public float moveSpeed = 5f;
    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    void Update()
    {
        moveDirection = playerInput.ReadValue<Vector2>();

        if (IsOwner)
        {
            if (audioListener.enabled == false) audioListener.enabled = true;
            if (cameraComponent.enabled == false) cameraComponent.enabled = true;
        }
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

}
