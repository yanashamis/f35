using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class exit_movie : MonoBehaviour
{

    public GameObject movie;

    public GameObject button;


    public GameObject timeline;

    private void OnMouseDown()
    {
        exiMovie();
        timeline.GetComponent<PlayableDirector>().enabled = false;
    }

    public void exiMovie()
    {
        movie.SetActive(false);
        Destroy(movie);
        movie.GetComponent<GameObject>().SetActive(false);
    }

}
