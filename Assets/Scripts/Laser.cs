using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10.0f;

    void Update()
    {

        transform.Translate(Vector2.up * _speed * Time.deltaTime);
        if (transform.position.y > 7.5f || transform.position.x > 11.0f || transform.position.x < -11.0f)
        // x & y checks added to cope with StarBurst powerup
        {
            Destroy(this.gameObject);
        }
 
    }
}
