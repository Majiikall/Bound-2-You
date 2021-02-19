using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalOscillate : MonoBehaviour
{
    float value = 0;
    float scale = 0.0035f;
 
    void Start()
    {
        value = this.GetComponent<Transform>().position.y;
    }

    void Update()
    {
        Transform goalTransform = this.GetComponent<Transform>();

        var oscil = goalTransform.position;

        oscil.y -= scale;

        goalTransform.position = oscil;
        
        if(goalTransform.position.y > (value * 1.015f) || goalTransform.position.y < (value * 0.95f))
        {
            scale *= -1;
        }
    }
}
