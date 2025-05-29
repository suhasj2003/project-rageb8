using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallexEffectX;
    public float parallexEffectY;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallexEffectX));
        float distx = (cam.transform.position.x * parallexEffectX);
        float disty = (cam.transform.position.y * parallexEffectY);

        transform.position = new Vector3(startpos + distx, startpos + disty, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}