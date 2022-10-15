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
    public bool isBonusBlock = false;
    private PlayerScript _playerScript;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
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