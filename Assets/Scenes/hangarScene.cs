using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hangarScene : MonoBehaviour
{
    public void GoToGameplay()
    {
        SceneManager.LoadScene("hangar_v_1");
    }
}
