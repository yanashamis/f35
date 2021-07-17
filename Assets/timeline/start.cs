using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class start : MonoBehaviour
{
    private PlayableDirector director;
    public GameObject controlPanel;
    public Cinemachine.CinemachineBrain CinemachineBrainHere;
    public GameObject button;


    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.played += Director_Played;
        director.stopped += Director_stopped;

    }

    private void Director_stopped(PlayableDirector obj)

    {
        controlPanel.SetActive(true);
    }

    private void Director_Played(PlayableDirector obj)

    {
        controlPanel.SetActive(true);
    }

    public void StartTimeline()
    {
        director.Play();
    }

    public void disableCinemachine() 
    {
        CinemachineBrainHere.enabled = false;
        //Destroy(button);

    }

}
