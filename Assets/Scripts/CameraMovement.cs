using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    public float sensitivity = 8f;
    public float maxAngle = 80f;

    private Vector2 currentRotation;
    private Quaternion oldRotation;
    private Quaternion newRotation;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * (speed / 100));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position - (transform.forward * (speed / 100));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position - (transform.right * (speed / 100));
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * (speed / 100));
        }
    }

    private void FixedUpdate()
    {
        oldRotation = transform.rotation;

        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxAngle, maxAngle);
        newRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);

        transform.rotation = Quaternion.Slerp(oldRotation, newRotation, sensitivity);
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(-20, 81, -24);
        transform.rotation = Quaternion.Euler(23, 41, 0);
    }

    public void SetCursorVisibility()
    {
        Cursor.visible = !enabled;
    }
}
