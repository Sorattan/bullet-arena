
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private Camera targetCamera;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 movement = Vector3.zero;

        if (targetCamera != null)
        {
            Vector3 forward = targetCamera.transform.forward;
            Vector3 right = targetCamera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            movement = (forward * input.y + right * input.x).normalized;
        }

        controller.SimpleMove(movement * moveSpeed);
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        if (targetCamera == null || Mouse.current == null)
            return;

        Ray ray = targetCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
