using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonusBaseScript : MonoBehaviour
{
    protected Color color = Color.yellow;
    protected Color textColor = Color.black;
    protected String text = "+100";
    protected PlayerScript _playerScript;

    private const int pointsPerActivation = 100;
    private const float deltaY = 0.02f;

    /// <summary>
    /// Действие при активации бонуса
    /// </summary>
    protected virtual void BonusActivate()
    {
        _playerScript.AddPoints(pointsPerActivation);
    }
    /// <summary>
    /// Инициализация внешнего вида бонуса
    /// </summary>
    protected virtual void initializeFields(){}

    void initializeBonus()
    {
        initializeFields();
        gameObject.GetComponent<SpriteRenderer>().color = color;
        var textComponent = gameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.color = textColor;
    }

    void Start()
    {
        _playerScript = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerScript>();
        // _audioSource = gameObject.GetComponent<AudioSource>();
        initializeBonus();
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            var pos = transform.position;
            pos.y = gameObject.transform.position.y - deltaY;
            transform.position = pos;
        }
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