using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuControls : MonoBehaviour
{
   public AudioSource audioSource;
   public Slider musicSlider;
   public Slider soundSlider;
   public Toggle musicToggle;
   public Toggle soundToggle;
   public Button quitBtn;
   
   public GameDataScript gameData;

   public void PlayPressed()
   {
      SceneManager.LoadScene("MainScene");
      // if (!gameData.endMenu)
      // {
      //    GameObject.Find("Canvas").SetActive(false);
      //    Cursor.visible = false;
      //    quitBtn.gameObject.SetActive(true);
      // }
      // else
      // {
      //    GameObject.Find("Canvas").SetActive(false);
      //    gameData.Reset();
      //    SceneManager.LoadScene("MainScene");
      // }
   }
   
   public void MusicVolume()
   {
      audioSource.volume = musicSlider.value;
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
         audioSource.volume = musicSlider.value;
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
