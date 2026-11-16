
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IsometricCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField]
    private Vector3 offset = new Vector3(-12f, 18f, -12f);

    [SerializeField] private float followSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            t
        );

        transform.LookAt(target.position);
    }
}
