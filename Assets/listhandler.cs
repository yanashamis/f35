using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class listhandler : MonoBehaviour
{
    public List<GameObject> raw;
    private popuphandler popuphandlerHere;

    public void selectAccordingToText(string text)
    {
        switch (text)
        {


            case "part_nosecone_place":
                checkRaw(0);
                return;
            case "fins_place":
                checkRaw(1);
                return;
            case "Steering fins_place":
                checkRaw(2);
                return;
        }
    }
    private bool allChecked()
    {
        foreach (GameObject _r in raw)
        {
            if (!_r.transform.Find("check").gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }
    public void checkRaw(int n)
    {
        Debug.Log("checking raw " + n);
        raw[n].transform.Find("check").gameObject.SetActive(true);

        
        if (allChecked())
        {
            popuphandlerHere.showPopup(1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        popuphandlerHere = GameObject.Find("_popuphandler").GetComponent<popuphandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
