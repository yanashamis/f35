using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;



public class TimelineControler : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public GameObject Redbuttom;

    private void OnMouseDown()
    {
        if (Redbuttom.tag == "redbutton")
        {
            playableDirector.Play();
        }
    }


}
