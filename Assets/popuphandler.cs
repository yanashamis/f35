using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popuphandler : MonoBehaviour
{
    // Start is called before the first frame update
    public listhandler listhandlerHere;
    public GameObject popupcontainer;
    public List<GameObject> texts;
    private int currentN;
    private void popupInitSetup()
    {
        foreach (var text in texts)
        {
            text.SetActive(false);
        }
        closepopup();
    }
    public void showPopup(int textn)
    {
        currentN = textn;
        popupcontainer.SetActive(true);

        foreach (var text in texts)
        {
            text.SetActive(false);
        }
        texts[currentN].SetActive(true);
    }
    public void closepopup()
    {
        //listhandlerHere.checkRaw(currentN);
        popupcontainer.SetActive(false);
    }
    void Start()
    {
        popupInitSetup();
        showPopup(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
