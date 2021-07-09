using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class listhandler : MonoBehaviour
{
    public List<GameObject> raw;

    public void checkRaw(int n)
    {
        raw[n].transform.Find("check").gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
