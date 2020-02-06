using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    [SerializeField]
    private int _powerUpID;

    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7.0f)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player p = other.transform.GetComponent<Player>();

            if(p != null)
            {
                switch(this._powerUpID)
                {
                    case 0:
                        p.TripleShotActive();
                        break;
                    case 1:
                        p.SpeedBoostActive();
                        break;
                    case 2:
                        p.ShieldActive();
                        break;
                    default:
                        Debug.LogError("PowerUP CS: Unknown _powerUpID" + this._powerUpID);
                        break;
                }

                
            }

            Destroy(this.gameObject);
        }
    }
}
