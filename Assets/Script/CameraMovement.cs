using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
This class is initially made after this video:
https://www.youtube.com/watch?v=pxS14VJ_eXQ
*/
public class CameraMovement : MonoBehaviour
{
    private const float cameraSpeed = 30f;
    private const float cameraZoomSpeed = 3000f;

    float speed = cameraSpeed;
    float zoomSpeed = cameraZoomSpeed;
    float rotateSpeed =20f;

    float maxHeight = 40f;
    float minHeight = 4f;

    Vector2 p1;
    Vector2 p2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //press left shift to move camera quickly
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = cameraSpeed;
            zoomSpeed = cameraZoomSpeed;
        }
        else
        {
            speed = cameraSpeed/2;
            zoomSpeed = cameraZoomSpeed/2;
        }

        //Buttons can be configured in edit->Project Settings->Input Manager
        float horizontalSpeed = Time.deltaTime * transform.position.y * speed * Input.GetAxis("Horizontal");// z/s
        float verticalSpeed = Time.deltaTime  * transform.position.y * speed * Input.GetAxis("Vertical");//q/d
        float scrollSpeed =  Time.deltaTime * Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

        //limite height
        if ((transform.position.y >= maxHeight) && (scrollSpeed > 0))
        {
            scrollSpeed = 0;
        }
        else if ((transform.position.y <= minHeight) && (scrollSpeed < 0))
        {
            scrollSpeed = 0;
        }

        //prevent overshooting the camera past the limits
        if (transform.position.y + scrollSpeed > maxHeight)
        {
            scrollSpeed = maxHeight - transform.position.y;
        }
        else if (transform.position.y + scrollSpeed < minHeight)
        {
            scrollSpeed = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSpeed, 0);
        Vector3 lateralMove = horizontalSpeed * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= verticalSpeed;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;

        GetCameraRotation();

    }

    void GetCameraRotation()
    {
        //2 is for middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            p1 = Input.mousePosition;

        }
        if (Input.GetMouseButton(2))
        {
            p2 = Input.mousePosition;

            float dx = Time.deltaTime * (p2 - p1).x * rotateSpeed;
            float dy = Time.deltaTime * (p2 - p1).y * rotateSpeed;

            transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));
            //GetChild fonctionne correctement parce CameraParent n'a qu'un enfent (mainCamera)
            transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(-dy,0,0));
            p1 = p2;
        }
    }
}
