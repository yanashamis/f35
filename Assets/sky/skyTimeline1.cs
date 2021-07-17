using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class skyTimeline1 : MonoBehaviour
{
    private PlayableDirector director;
    public GameObject controlPanel;
    public Cinemachine.CinemachineBrain CinemachineBrainHere;
    public GameObject gameobject;
    public GameObject plan;
    //public Animator arrow;

    private void Awake()
    {
        //arrow = GetComponent<Animator>();
        director = GetComponent<PlayableDirector>();
        director.played += Director_Played;
        director.stopped += Director_stopped;

    }

    private void Director_stopped(PlayableDirector obj)

    {
        controlPanel.SetActive(true);
        disableCinemachine();
        //arrow.gameObject.GetComponent<Animation>().Play("pointeranimation");
    }

    private void Director_Played(PlayableDirector obj)

    {
        controlPanel.SetActive(true);
    }

    public void StartTimeline()
    {
        // enable cinemachine component

        director.Play();
        TornOnCinemachine();
    }

    public void disableCinemachine()
    {
        CinemachineBrainHere.enabled = false;
    }

    public void TornOnCinemachine()
    {
        CinemachineBrainHere.enabled = true;
    }

    public void destroyplan()
    {
        Destroy(plan);
    }
    public void destroyGameObject()
    {
           Destroy(gameobject);
    }
}
