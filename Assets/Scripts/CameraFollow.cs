using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Stroller;
    [SerializeField] private Vector3 offset = new Vector3(0.61f, 2.02f, -2.68f);

    // Update is called once per frame


    void LateUpdate()
    {
        transform.position = Stroller.transform.position + offset;
    }
}
