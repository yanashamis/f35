using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjects : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float Depth = 1f;
    public float Displacement = 3f;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        if (transform.position.y < 0f)
        {
            float displacementMultiplier = Mathf.Clamp01(-transform.position.y / Depth) * Displacement;
            rigidbody.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), ForceMode.Acceleration);
        }
    }
}
