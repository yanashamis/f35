using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouse : MonoBehaviour
{

    public GameObject myObjectsToLight;


    private void OnMouseExit()
    {


        Debug.Log("exit");


        myObjectsToLight.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = false;
       

       

    }

    private void OnMouseEnter()
    {



        Debug.Log("enter");

        myObjectsToLight.GetComponent<Knife.HDRPOutline.Core.OutlineObject>().enabled = true;


       

    }

}
