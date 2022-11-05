using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseControls : MonoBehaviour
{
    public static bool flagPauseMenu = true;
    public AudioSource audioSource;
    public Slider musicPauseSlider;
    public Slider soundPauseSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    
    public Toggle musicPauseToggle;
    public Toggle soundPauseToggle;
    public Toggle soundToggle;
    
    public GameDataScript gameData;

    public void Start()
    {
        musicPauseToggle.isOn = gameData.music;
        MusicPauseToggle();
        soundPauseToggle.isOn = gameData.sound;
        SoundPauseToggle();
        musicPauseSlider.value = gameData.musicValue;
        soundPauseSlider.value = gameData.soundValue;
    }
    
    public void PlayPausePressed()
    {
        GameObject.Find("PauseCanvas").SetActive(false);
        flagPauseMenu = false;
    }
    
    public void NewGamePressed()
    {
        GameObject.Find("PauseCanvas").SetActive(false);
        flagPauseMenu = false;
        gameData.Reset();
        SceneManager.LoadScene("MainScene");
    }
    public void QuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    public void MusicPauseVolume()
    {
        audioSource.volume = musicPauseSlider.value * 0.1f;
        gameData.musicValue = musicPauseSlider.value;
    }
   
    public void SoundPauseVolume()
    {
        PlayerScript.volumeScale = soundPauseSlider.value * 0.1f;
        gameData.soundValue = soundPauseSlider.value;
    }
    
    public void MusicPauseToggle()
    {
        if (!musicPauseToggle.isOn)
        {
            audioSource.volume = 0;
            gameData.music = false;
            musicPauseSlider.interactable = false;
        }
        else
        {
            audioSource.volume = musicPauseSlider.value * 0.1f;
            gameData.music = true;
            musicPauseSlider.interactable = true;
        }
    }
   
    public void SoundPauseToggle()
    {
        if (!soundPauseToggle.isOn)
        {
            PlayerScript.volumeScale = 0;
            gameData.sound = false;
            soundPauseSlider.interactable = false;
        }
        else
        {
            PlayerScript.volumeScale = soundPauseSlider.value * 0.1f;
            gameData.sound = true;
            soundPauseSlider.interactable = true;
        }
    }
}
