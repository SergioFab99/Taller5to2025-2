using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    private Resolution[] resolutions;
    private List<Resolution> selectedResolutionList = new List<Resolution>();
    int indexResolution;
    void Start()
    {
        FullScreen();
        CheckResolution();
    }
    void Update()
    {
        
    }

    void FullScreen()
    {
        if (Screen.fullScreen)
        {
            fullScreenToggle.isOn = true;
        }
        else
        {
            fullScreenToggle.isOn = false;
        }
    }

    public void ActivateFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    void CheckResolution()
    {
        resolutions = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> options = new List<string>();
        int actualResolution = 0;
        int resCount = 0;
        foreach(Resolution res in resolutions)
        {
            resCount++;
            string option = res.width.ToString() + " x " + res.height.ToString();
            if (!options.Contains(option))
            {
                options.Add(option);
                selectedResolutionList.Add(res);
            }

            if (Screen.fullScreen && res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                actualResolution = resCount;
            }
        }
        /*for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option))
            {
                options.Add(option);
                selectedResolutionList.Add()
            }
            
            if(Screen.fullScreen && resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                actualResolution = i;
            }
        }*/
        
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = actualResolution;
        resolutionDropDown.RefreshShownValue();
        resolutionDropDown.value = PlayerPrefs.GetInt("resolutionNumber", actualResolution);
    }

    public void ChangeResolution()
    {
        PlayerPrefs.SetInt("resolutionNumber", resolutionDropDown.value);
        indexResolution = resolutionDropDown.value;
        Screen.SetResolution(selectedResolutionList[indexResolution].width, selectedResolutionList[indexResolution].height, Screen.fullScreen);
        /*Resolution resolution = resolutions[indexResolution];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);*/
    }
}
