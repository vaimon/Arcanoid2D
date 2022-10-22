using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public Vector2 ballInitialForce;
    public AudioClip hitSound;
    public AudioClip loseSound;
    public GameDataScript gameData;

    Rigidbody2D rb;
    GameObject playerObj;
    float deltaX;
    AudioSource audioSrc;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        deltaX = transform.position.x;
        audioSrc = Camera.main.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (rb.isKinematic)
            if (Input.GetButtonDown("Fire1"))
            {
                rb.isKinematic = false;
                rb.AddForce(ballInitialForce);
            }
            else
            {
                var pos = transform.position;
                pos.x = playerObj.transform.position.x + deltaX;
                transform.position = pos;
            }
        if (!rb.isKinematic && Input.GetKeyDown(KeyCode.J))
        {
            var v = rb.velocity;
            rb.velocity = Quaternion.AngleAxis(Random.Range(1,359), Vector3.forward) * v;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameData.sound)
            audioSrc.PlayOneShot(loseSound, 5);
        Destroy(gameObject);
        playerObj.GetComponent<PlayerScript>().OnBallDestroyed();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameData.sound)
            audioSrc.PlayOneShot(hitSound, 5);
    }
}