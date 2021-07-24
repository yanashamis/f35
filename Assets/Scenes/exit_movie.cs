using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class exit_movie : MonoBehaviour
{



    public GameObject button;


    public GameObject timeline;

    private void OnMouseDown()
    {
        timeline.GetComponent<PlayableDirector>().enabled = false;
    }

    //public void exiMovie()
    //{
    //    gameObject.GetComponent<PlayableDirector>().enabled = false;
    //}

}
