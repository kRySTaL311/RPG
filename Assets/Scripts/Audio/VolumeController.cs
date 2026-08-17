using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
public class VolumeController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject audioSettingPanel;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
            SetSlider();
        }
        else if (PlayerPrefs.HasKey("SFXVolume"))
        {
            audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
            SetSlider();
        }
        else if (PlayerPrefs.HasKey("MusicVolume"))
        {
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
            SetSlider();
        }
        else
        {
            SetSlider();
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Escape))
        {
            audioSettingPanel.SetActive(true);

        }
    }

    void SetSlider()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void UpdateMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    public void UpdateSfxVolume()
    {
        audioMixer.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void UpdateMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void CloseAudioPanelWindow()
    {
        audioSettingPanel.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}
