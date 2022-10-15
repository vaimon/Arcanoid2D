using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonusBaseScript : MonoBehaviour
{
    Color color = Color.yellow;
    String text = "+100";

    private PlayerScript _playerScript;
    private AudioSource _audioSource;

    private const int pointsPerActivation = 100;
    private const float deltaY = 0.01f;

    public virtual void BonusActivate()
    {
        _playerScript.AddPoints(pointsPerActivation);
    }

    void initializeBonus()
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
        var textComponent = gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = Color.black;
    }

    void Start()
    {
        _playerScript = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerScript>();
        _audioSource = gameObject.GetComponent<AudioSource>();
        initializeBonus();
    }

    void Update()
    {
        var pos = transform.position;
        pos.y = gameObject.transform.position.y - deltaY;
        transform.position = pos;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerScript.playBonusSound();
            BonusActivate();
            Destroy(gameObject);
        }
    }
}