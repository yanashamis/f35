using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVideo : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(SelfDestruct());
    }


    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(16f);
        Destroy(gameObject);
    }

}
