using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    Rigidbody2D arrowRigidBody;
    PlayerMovement player;
    [SerializeField] float arrowLaunchVelocity = 10f;
    float arrowSpeed;
    float arrowDirection;
    // Start is called before the first frame update
    void Start()
    {
        arrowRigidBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        arrowSpeed = arrowLaunchVelocity * player.transform.localScale.x;
        arrowDirection = player.transform.localScale.x > Mathf.Epsilon ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        arrowRigidBody.velocity = new Vector2(arrowSpeed, 0);
        FlipSprite();
    }

    void FlipSprite()
    {

        transform.localScale = new Vector2(arrowDirection, transform.localScale.y);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.Log(other);
        if (other.tag == "Enemy") 
        {
            Destroy(other.gameObject);
            DestroyArrow();
        }
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        Invoke("DestroyArrow", 5f);
    }

    void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
