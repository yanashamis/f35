using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drag_on_screen : MonoBehaviour
{
    public int order;
    private bool dragging = false;
    private bool done = false;
    private float distance;
    public GameObject targetplace;
    public GameObject namebar;
    public bool stickToTarget = false;
    public float movespeed = 20f;

    private LookOrbit LookOrbitHere;

    private GameObject objectStiching;

    private listhandler listhandlerHere;

    public List<GameObject> outlineObject;
    public GameObject outlineTarget;



    private void OnMouseEnter()
    {
        if ((!done) && listhandlerHere.previousChecked(order))
            foreach(GameObject _out in outlineObject)
                _out.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = true;
    }

    private void OnMouseExit()
    {
        foreach (GameObject _out in outlineObject)
            _out.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = false;
    }

    private void Start()
    {
        LookOrbitHere = Camera.main.GetComponent<LookOrbit>();
        listhandlerHere = GameObject.Find("_listhandler").GetComponent<listhandler>();
        stickToTarget = false;
    }
    void OnMouseDown()
    {
        if ((!done) && (listhandlerHere.previousChecked(order)))
            {
                //LookOrbitHere.FreezeControls();
                LookOrbitHere.FreezeRotation();
                distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                dragging = true;
                outlineTarget.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = true;
                namebar.gameObject.SetActive(true);
            }
    }

    void OnMouseUp()
    {
        if ((!done) &&(listhandlerHere.previousChecked(order)))
            {
                //LookOrbitHere.UnfreezeControls();
                LookOrbitHere.UnfreezeRotation();
                dragging = false;
                if (shootTestBeam())
                {
                    //Debug.Log("hit that");
                    stickToTarget = true;
            }
        }
    }
    bool shootTestBeam()
    {
        var hitTargetplace = false;
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        hits = Physics.RaycastAll(ray, 100.0F);

        //Debug.Log(hits);
        //Debug.Log(hits.Length);

        foreach (RaycastHit h in hits)
        {
            //Debug.Log(h);
            //Debug.Log(h.transform.name);

            if(h.transform.name == targetplace.transform.name){
                hitTargetplace = true;
                objectStiching = h.transform.gameObject;
            }
        }



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
            //Debug.Log(dist);
            if (dist < 0.001f)
            {
                stickToTarget = false;
                stiched();
                done = true;
                namebar.gameObject.SetActive(false);
                outlineTarget.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = false;
            }
        }
    }
    private void stiched()
    {
        listhandlerHere.selectAccordingToText(objectStiching.name);
    }
}
