using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag_on_screen : MonoBehaviour
{
    private bool dragging = false;
    private float distance;
    public GameObject targetplace;
    public bool stickToTarget = false;
    public float movespeed = 0.01f;

    private void Start()
    {
        stickToTarget = false;
    }
    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        dragging = false;
        if (shootTestBeam())
        {
            Debug.Log("hit that");
        }
    }
    bool shootTestBeam()
    {
        var hitTargetplace = false;
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        hits = Physics.RaycastAll(ray, 100.0F);

        Debug.Log(hits);
        Debug.Log(hits.Length);

        

        return hitTargetplace;
    }
    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }

        if (stickToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, targetplace.transform.position, Time.deltaTime * movespeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetplace.transform.rotation, Time.deltaTime * movespeed);

            float dist = Vector3.Distance(transform.position, targetplace.transform.position);
            Debug.Log(dist);
            if (dist < 0.001f)
            {
                stickToTarget = false;
            }
        }
    }
}
