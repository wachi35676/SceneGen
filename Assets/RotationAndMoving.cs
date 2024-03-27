using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAndMoving : MonoBehaviour
{
    public int RotationSpeed = 100;
    public float MovingSpeed = 10;
    
    void Update()
    {
        // rotate the sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.transform.Rotate(Vector3.forward * Time.deltaTime * RotationSpeed);
        
        //move in the x direction
        transform.position += new Vector3(MovingSpeed * Time.deltaTime, 0, 0);
    }
}
