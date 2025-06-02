using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    private float lengthx, lengthy, startposx, startposy;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallexEffectX;
    [SerializeField] private float parallexEffectY;

    [SerializeField] private bool repeating = true;


    public void Start()
    {
        startposx = transform.position.x;
        startposy = transform.position.y;
        lengthx = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    public void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallexEffectX));
        float distx = (cam.transform.position.x * parallexEffectX);
        float disty = (cam.transform.position.y * parallexEffectY);

        transform.position = new Vector3(startposx + distx, startposy + disty, transform.position.z);

        if (repeating)
        {
            if (temp > startposx + lengthx / 2)
            {
                startposx += lengthx;
            }
            else if (temp < startposx - lengthx / 2)
            {
                startposx -= lengthx;

            }
        }
    }
}