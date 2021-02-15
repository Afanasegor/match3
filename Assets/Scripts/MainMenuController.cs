using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioMixerGroup audioMixer;
    [SerializeField] private Image musicImg, soundImg;
    [SerializeField] private Sprite musicOn, musicOff, soundOn, soundOff;

    private int musicCheck, soundCheck; // 0 - Off, 1 - On;

    private void Start()
    {
        CheckAudio();

        musicImg = musicImg.GetComponent<Image>();
        StartCoroutine(StartExplosionEffect());
    }

    public void LoadTheGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadTheRecordsPage()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadAboutPage()
    {
        SceneManager.LoadScene(3);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
    
    // Анимация взрыва при старте игры в главном меню
    IEnumerator StartExplosionEffect()
    {
        yield return new WaitForSeconds(0.5f);
        explosionEffect.SetActive(true);
        explosionEffect.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// проверяет сохранение музыки и звуков в начале игры, вызвать метод в Start();
    /// </summary>
    private void CheckAudio()
    {
        if (!PlayerPrefs.HasKey("Music"))
        {
            musicCheck = 1;
            PlayerPrefs.SetInt("Music", musicCheck);
        }
        else
        {
            musicCheck = PlayerPrefs.GetInt("Music");
            switch (musicCheck)
            {
                case 0:
                    musicImg.sprite = musicOff;
                    audioMixer.audioMixer.SetFloat("MusicVolume", -80);
                    break;
                case 1:
                    musicImg.sprite = musicOn;
                    audioMixer.audioMixer.SetFloat("MusicVolume", 0);
                    break;
                default:
                    break;
            }
        }

        if (!PlayerPrefs.HasKey("Sound"))
        {
            soundCheck = 1;
            PlayerPrefs.SetInt("Sound", soundCheck);
        }
        else
        {
            soundCheck = PlayerPrefs.GetInt("Sound");
            switch (soundCheck)
            {
                case 0:
                    soundImg.sprite = soundOff;
                    audioMixer.audioMixer.SetFloat("SoundVolume", -80);
                    break;
                case 1:
                    soundImg.sprite = soundOn;
                    audioMixer.audioMixer.SetFloat("SoundVolume", 0);
                    break;
                default:
                    break;
            }
        }
    }

    // TODO: добавить методы на кнопку музыки
    public void SwitchMusic()
    {
        switch (musicCheck)
        {
            case 0:
                musicCheck = 1;
                PlayerPrefs.SetInt("Music", musicCheck);
                musicImg.sprite = musicOn;
                audioMixer.audioMixer.SetFloat("MusicVolume", 0);
                break;
            case 1:
                musicCheck = 0;
                PlayerPrefs.SetInt("Music", musicCheck);
                musicImg.sprite = musicOff;
                audioMixer.audioMixer.SetFloat("MusicVolume", -80);
                break;
            default:
                break;
        }
    }

    // TODO: добавить методы на кнопку звука
    public void SwitchSound()
    {
        switch (soundCheck)
        {
            case 0:
                soundCheck = 1;
                PlayerPrefs.SetInt("Sound", soundCheck);
                soundImg.sprite = soundOn;
                audioMixer.audioMixer.SetFloat("SoundVolume", 0);
                break;
            case 1:
                soundCheck = 0;
                PlayerPrefs.SetInt("Sound", musicCheck);
                soundImg.sprite = soundOff;
                audioMixer.audioMixer.SetFloat("SoundVolume", -80);
                break;
            default:
                break;
        }
    }

    // TODO: удалить в последующем - метод разработчика
    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}