using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;

    public float minZoomDistance;
    public float maxZoomDistance;

    private Camera cam;

    public static CameraController instance;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * zInput + transform.right * xInput;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float distance = Vector3.Distance(transform.position, cam.transform.position);

        if (distance < minZoomDistance && scrollInput > 0.0f)
            return;
        else if (distance > maxZoomDistance && scrollInput < 0.0f)
            return;

        cam.transform.position += cam.transform.forward * scrollInput * zoomSpeed;
    }

    public void FocusOnPosition(Vector3 position)
    {
        transform.position = position;
    }
}
