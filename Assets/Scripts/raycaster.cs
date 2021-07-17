using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycaster : MonoBehaviour
{
    public int clickstate = 1;
    public listhandler listhandlerHere;
    public popuphandler popuphandlerHere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                //Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
                clickObj(hit.transform.name);
            }
        }
    }

    void clickObj(string objname)
    {
        switch (objname)
        {
                case "Cube_1":
                if (clickstate == 1)
                {
                    //listhandlerHere.checkRaw(clickstate - 1);
                    popuphandlerHere.showPopup(clickstate - 1);
                    clickstate++;
                }
                return;
                case "Cube_2":
                if (clickstate == 2)
                {
                    //listhandlerHere.checkRaw(clickstate - 1);
                    popuphandlerHere.showPopup(clickstate - 1);
                    clickstate++;
                }
                return;
                case "Cube_3":
                if (clickstate == 3)
                {
                    //listhandlerHere.checkRaw(clickstate - 1);
                    popuphandlerHere.showPopup(clickstate - 1);
                    clickstate++;
                }
                return;

            }
    }
}
