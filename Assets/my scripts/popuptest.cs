using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popuptest : MonoBehaviour
{
    public GameObject dest;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  void uDestroy()
    {
        Destroy(dest);
        Debug.Log("click");
    }
}
