using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameDataScript : ScriptableObject
{
    public bool resetOnStart;
    public int level = 1;
    public int balls = 6;
    public int points = 0;
    public int pointsToBall = 0;
    //включены ли музыка и звук
    public bool music = true;
    public bool sound = true;
    //значение громкости музыки и звуков
    public float musicValue = 1f;
    public float soundValue = 10f;
    // public bool playFirst = false; //для блокировки пробела при открытом паузном меню
    public bool endMenu = false; //для проверки, что главное меню теперь заключительное
    /// <summary>
    /// Мягкая перезагрузка между играми
    /// </summary>
    public void Reset()
    {
        level = 1;
        balls = 6;
        points = 0;
        pointsToBall = 0;
    }
    /// <summary>
    /// Полная перезагрузка, в т.ч. настроек
    /// </summary>
    public void FullReset()
    {
        level = 1;
        balls = 6;
        points = 0;
        pointsToBall = 0;
        music = true;
        sound = true;
        musicValue = .5f;
        soundValue = 5f;
    }
    
    /*public void Load()
    {
        level = PlayerPrefs.GetInt("level", 1);
        balls = PlayerPrefs.GetInt("balls", 6);
        points = PlayerPrefs.GetInt("points", 0);
        pointsToBall = PlayerPrefs.GetInt("pointsToBall", 0);
        music = PlayerPrefs.GetInt("music", 1) == 1;
        sound = PlayerPrefs.GetInt("sound", 1) == 1;
    }*/
    
    /*public void Save()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("balls", balls);
        PlayerPrefs.SetInt("points", points);
        PlayerPrefs.SetInt("pointsToBall", pointsToBall);
        PlayerPrefs.SetInt("music", music ? 1 : 0);
        PlayerPrefs.SetInt("sound", sound ? 1 : 0);
    }*/

}