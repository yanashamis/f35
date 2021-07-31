using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class attack : MonoBehaviour
{
    public void GoToGameplay()
    {
        SceneManager.LoadScene("sky");
    }
}
