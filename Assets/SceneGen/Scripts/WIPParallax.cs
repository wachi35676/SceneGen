using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIPParallax : MonoBehaviour
{

    public Camera cam;

    public Transform subject;

    private Vector2 startPosition;

    private float startZ;

    private Vector2 travel => (Vector2)cam.transform.position - startPosition;

    private float distanceFromSubject => transform.position.z;

    private float clippingPlane =>
        (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    private float parallaxFactor => Math.Abs(distanceFromSubject) / clippingPlane;
    // Start is called before the first frame update
    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
        
    }

    // Update is called once per frame
    public void Update()
    {
        Vector2 newPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);

    }
}
