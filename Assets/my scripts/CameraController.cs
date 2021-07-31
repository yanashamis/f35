using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraOrbit;
    public Transform traget;

    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    // Start is called before the first frame update
    void Start()
    {
        //cameraOrbit.position = traget.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Zooming();
        //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 90f, 0);
        //cameraOrbit.position = traget.position;
        //transform.LookAt(traget.position);
    }


    void Zooming()
    {
        transform.LookAt(traget.position);
        Vector3 Scrolldirection = transform.position.normalized;

        float step = zoomSpeed * Time.deltaTime;

        // Allows zooming in and out via the mouse wheel
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Scrolldirection.y > minZoom)
        {
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Scrolldirection.y < maxZoom)
        {
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
        }
    }
}
