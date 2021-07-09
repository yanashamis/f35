using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneScriptMenu : MonoBehaviour
{



    public void GoToGameplay()
    {
        SceneManager.LoadScene("menu");
     }
}
