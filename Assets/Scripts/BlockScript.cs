using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour
{
    public GameObject textObject;
    TMP_Text textComponent;
    public int hitsToDestroy;
    public int points;
    public bool isBonusBlock;
    public bool isMovingBlock;
    
    private PlayerScript _playerScript;
    private int deltaDirection = 1;

    private const float deltaX = 0.02f;

    void Start()
    {
        _playerScript = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerScript>();
        if (textObject != null)
        {
            textComponent = textObject.GetComponent<TMP_Text>();
            textComponent.text = hitsToDestroy.ToString();
        }
    }

    private void Update()
    {
        if (!(Time.timeScale > 0) || !isMovingBlock) return;
        var pos = transform.position;
        pos.x = gameObject.transform.position.x + deltaX * deltaDirection;
        transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Для двигающихся блоков при коллизии только со стенами и блоками меняме направление
        if (collision.gameObject.CompareTag("Block") || collision.gameObject.CompareTag("Wall"))
        {
            deltaDirection *= -1;
            return;
        }
        hitsToDestroy--;
        if (hitsToDestroy == 0)
        {
            if (isBonusBlock)
            {
                _playerScript.SpawnBonus(transform.position);
            }
            Destroy(gameObject);
            _playerScript.OnBlockDestroyed(points);
        }
        else if (textComponent != null)
            textComponent.text = hitsToDestroy.ToString();
    }
}