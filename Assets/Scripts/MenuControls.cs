using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
   public static bool flagMenu = true;
   public AudioSource audioSource;
   public Slider musicSlider;
   public Slider soundSlider;
   public Toggle musicToggle;
   public Toggle soundToggle;
   public Button btn;
   
   public GameDataScript gameData;

   public bool firstOpen = true;
   
   public void Start()
   {
      if (firstOpen)
      {
         btn.gameObject.SetActive(false);
         firstOpen = false;
      }
      else btn.gameObject.SetActive(true);
   }
   
   public void PlayPressed()
   {
      GameObject.Find("Canvas").SetActive(false);
      flagMenu = false;
   }
   
   public void MusicVolume()
   {
      audioSource.volume = musicSlider.value * 0.1f;
      gameData.musicValue = musicSlider.value;
   }
   
   public void SoundVolume()
   {
      PlayerScript.volumeScale = soundSlider.value * 0.1f;
      gameData.soundValue = soundSlider.value;
   }

   public void MusicToggle()
   {
      if (!musicToggle.isOn)
      {
         audioSource.volume = 0;
         gameData.music = false;
         musicSlider.interactable = false;
      }
      else
      {
         audioSource.volume = musicSlider.value * 0.1f;
         gameData.music = true;
         musicSlider.interactable = true;
      }
   }
   
   public void SoundToggle()
   {
      if (!soundToggle.isOn)
      {
         PlayerScript.volumeScale = 0;
         gameData.sound = false;
         soundSlider.interactable = false;
      }
      else
      {
         PlayerScript.volumeScale = soundSlider.value * 0.1f;
         gameData.sound = true;
         soundSlider.interactable = true;
      }
   }
   
   public void QuitPressed()
   {
      Application.Quit();
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#endif
   }
}
