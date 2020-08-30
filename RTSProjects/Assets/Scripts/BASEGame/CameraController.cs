using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float panSpeed;
    public float rotateAmount;
    public float rotateSpeed;

    Quaternion rotation;

    public float panDetect;
    float minHeight = 15f;
    float maxHeight = 80f;
    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.rotation = rotation;
        }
    }
    private void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetMouseButton(2))
        {
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * rotateSpeed);
        }
    }

    private void Start()
    {
        rotation = Camera.main.transform.rotation;
    }

    void MoveCamera()
    {
        float moveY = Camera.main.transform.position.y;
        float moveX = Camera.main.transform.position.x;
        float moveZ = Camera.main.transform.position.z;


        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;


        if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect)
        {
            moveX -= panSpeed;
        }
        if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect)
        {
            moveX += panSpeed;
        }
        if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect)
        {
            moveZ += panSpeed;
        }
        if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect)
        {
            moveZ -= panSpeed;
        }

        moveY -= Input.GetAxis("Mouse ScrollWheel") * (panSpeed * 20);

        moveY = Mathf.Clamp(moveY, minHeight, maxHeight);

        Vector3 newPos = new Vector3(moveX, moveY, moveZ);

        Camera.main.transform.position = newPos;
    }

    private void Awake()
    {
        instance = this;
    }
    public void FocusOnPosition (Vector3 pos)
    {
        transform.position = pos;
    }
}