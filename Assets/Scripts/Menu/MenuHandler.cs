using System.Collections.Generic; //Required for dictionaries
using UnityEngine; //Required for Unity Connection
using UnityEngine.Audio; //Required to access audio options
using UnityEngine.SceneManagement; //Required to change scenes
using UnityEngine.UI; //Required to access UI Canvas elements

public class MenuHandler : MonoBehaviour
{
    #region Variables
    [Header("Audio")]
    [Tooltip("Add the AudioMixer from your assets here")]
    public AudioMixer masterAudio;
    //Variable for name of current volume slider user is adjusting
    [HideInInspector] public string currentSlider;
    //Variable to store current slider when volume is muted so user can't change value until unmuting
    private Slider _tempSlider;
    [Header("Video")]
    //An array we will use to store the resolutions available on the users computer
    public Resolution[] resolutions;
    //Add UI elements so we can retrieve their values adn enable/disable as required
    [Header("UI")]
    [Tooltip("Add Continue button from PlayMenu here. Ignore in Game scene")]
    public Button continueButton;
    [Tooltip("Add Load button from PlayMenu here. Ignore in Game scene")]
    public Button loadButton;
    [Header("Settings Objects")]
    [Tooltip("Add MasterVolume slider from SettingsMenu here.")]
    public Slider masterVolume;
    [Tooltip("Add MusicVolume slider from SettingsMenu here.")]
    public Slider musicVolume;
    [Tooltip("Add SFXVolume slider from SettingsMenu here.")]
    public Slider sfxVolume;
    [Tooltip("Add MasterMute toggle from SettingsMenu here.")]
    public Toggle masterMute;
    [Tooltip("Add MusicMute toggle from SettingsMenu here.")]
    public Toggle musicMute;
    [Tooltip("Add SFXMute toggle from SettingsMenu here.")]
    public Toggle sfxMute;
    [Tooltip("Add Fullscreen toggle from SettingsMenu here.")]
    public Toggle fullscreen;
    [Tooltip("Add Resolution dropdown from SettingsMenu here.")]
    public Dropdown resolution;
    [Tooltip("Add quality dropdown from SettingsMenu here.")]
    public Dropdown quality;
    //Reference to the GameManager script so we can activate its functions
    private GameManager _gameManager;
    public GameObject settingsMenu;
    #endregion
    #region Initial Setup
    private void Start()
    {
        //Retrieve the GameManager class from its object in the heirarchy
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        #region Resolutions Setup
        //Fill the resolutions array with the available resolutions on the computer in use
        resolutions = Screen.resolutions;
        //Clear the options from the Resolution dropdown menu
        resolution.ClearOptions();
        //Create a new list we will store the resolutions in so we can update the dropdown menu
        List<string> options = new List<string>();
        //Set a default value of 0 to current resolution index value
        int currentResolutionIndex = 0;
        //For each resolution in the array add a string value representing the resolution to the options list
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            //If our current resolution matches the resolution at the current index in the array set the current res index value to the index of this iteration
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        //Refresh the dropdown menu with the options we have stored in the list
        resolution.AddOptions(options);
        //Change the current value on the dropdown menu to match our current resolution using the current res index
        resolution.value = currentResolutionIndex;
        //Refresh the dropdown menu so it shows the correct current value
        resolution.RefreshShownValue();
        #endregion
        //If we have a positive value for mute in PlayerPrefs set that sliders value to -80 to be muted
        if (PlayerPrefs.GetInt("MasterMute") == 1)
        {
            masterAudio.SetFloat("masterVolume", -80);
        }
        if (PlayerPrefs.GetInt("MusicMute") == 1)
        {
            masterAudio.SetFloat("musicVolume", -80);
        }
        if (PlayerPrefs.GetInt("SFXMute") == 1)
        {
            masterAudio.SetFloat("sfxVolume", -80);
        }
    }
    #endregion
    #region Audio
    public void GetSlider(Slider slider)
    {
        //Set our temporary slider to the Slider that was passed from the mute toggle user clicked
        _tempSlider = slider;
    }

    public void MuteToggle(bool isMuted)
    {
        //If mute is toggled on set volume on the current slider to lowest level and make slider inactive
        if (isMuted)
        {
            masterAudio.SetFloat(currentSlider, -80);
            _tempSlider.interactable = false;
        }
        //Else return the volume to value of slider and make slider active again
        else
        {
            masterAudio.SetFloat(currentSlider, _tempSlider.value);
            _tempSlider.interactable = true;
        }
    }
    public void CurrentSlider(string sliderName)
    {
        //Store the name of the current slider user is changing
        currentSlider = sliderName;
    }
    public void ChangeVolume(float volume)
    {
        //Adjust volume based on value of slider user is changing
        masterAudio.SetFloat(currentSlider, volume);
    }
    #endregion
    #region Resolution
    public void FullScreenToggle(bool isFullscreen)
    {
        //If fullscreen toggle is on then make game fullscreen otherwise make it not fullscreen
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        //Store the resolution from the resolution array from the index that has been passed through by the dropdown menu
        Resolution res = resolutions[resolutionIndex];
        //Change resolution based on the value stored in res
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
    #endregion
    #region Graphics
    public void Quality(int qualityIndex)
    {
        //Update the quality level based off value passed from dropdown
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    #endregion
    #region Transitions
    public void ChangeScene(int scene)
    {
        //Load scene that corresponds to the value passed to this function
        SceneManager.LoadScene(scene);
    }
    public void Settings()
    {
        //Load settings when settings menu is activated
        if (settingsMenu.activeInHierarchy)
        {
            _gameManager.LoadSettings();
        }
        //Save settings when it is deactivated
        else
        {
            _gameManager.SaveSettings();
        }        
    }
    public void Quit()
    {
        //Exit play mode in Unity Editor when function is activated
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //Quit the game if not in Unity Editor
        Application.Quit();
    }
    public void PlayMenu()
    {
        //If we have a LastSave key in playerprefs we have save files we can load, make the buttons interactable
        if (PlayerPrefs.HasKey("LastSave"))
        {
            continueButton.interactable = true;
            loadButton.interactable = true;
        }
        //Else no save file exists, make the buttons non-interactable
        else
        {
            continueButton.interactable = false;
            loadButton.interactable = false;
        }
    }
    public void IsLoading(int saveIndex)
    {
        //Set isLoading value in PlayerPrefs to match value passed from button. 0 means we are continuing last save
        PlayerPrefs.SetInt("isLoading", saveIndex);
    }
    #endregion
}
