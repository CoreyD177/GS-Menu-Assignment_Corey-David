using System.Collections.Generic; //Allow us to access and use Dictionaries
using UnityEngine; //Connects to Unity
using UnityEngine.UI; //Allows us to access and use Canvas UI Elements
using System.IO; //Allows us to check if a file exists

public class Keybinds : MonoBehaviour
{
    #region Variables
    //A dictionary holding the values of our movement keybinds. Allow for use in other scripts
    [SerializeField] public static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    //Create a struct so we can store Name of key, Text field from button it relates to and the Key it is set to
    [System.Serializable]
    public struct KeyUISetup
    {
        public string keyName;
        public Text keyDisplayText;
        public string defaultKey;
    }
    //An array to store the values of the struct and so we can edit in Unity
    [Header("Default Keys")]
    [Tooltip("Setup the name and default key for each movement type and add the text box that corresponds to the movements button")]
    public KeyUISetup[] baseSetup;
    [Header("Colours")]
    //An array of buttons so we can change their colour based on their state
    [Tooltip("Add each of the buttons you want to be able to modify the colour for")]
    public GameObject[] buttonUIColor;
    //Colour variables for each state so we can modify button colours
    [Tooltip("Set the colour to change the button to when the key has successfully changed")]
    public Color32 changedKey = new Color32(39, 26, 56, 255);
    [Tooltip("Set the colour to change the button to when a button is selected")]
    public Color32 selectedKey = new Color32(83, 4, 140, 255);
    [Tooltip("Set the colour to change the button to when the key needs to be reset to default")]
    public Color32 resetKey = new Color32(172, 4, 240, 255);
    [Tooltip("Set the colour to change the button to when a duplicate key has been found")]
    public Color32 errorKey = new Color32(255, 0, 0, 255);
    //Store the value of the current button that has been pressed
    public GameObject currentKey;
    #endregion
    #region Initial Setup
    void Start()
    {
        //If dictionaryFull value from AudioManager is false we have not filled dictionary yet
        if (AudioManager.dictionaryFull == false)
        {
            //If player prefs does not have forward key and a TXT file does not exist load defaults set in our baseSetup array
            if (!PlayerPrefs.HasKey("Forward") && !File.Exists(HandleFile.path))
            {
                //Add the default keys from our baseSetup array to the dictionary
                for (int i = 0; i < baseSetup.Length; i++)
                {
                    keys.Add(baseSetup[i].keyName, (KeyCode)System.Enum.Parse(typeof(KeyCode), baseSetup[i].defaultKey));
                }
                //For each entry in our dictionary set the value in PlayerPrefs to match our dictionary
                foreach (var keyEntry in keys)
                {
                    PlayerPrefs.SetString(keyEntry.Key, keyEntry.Value.ToString());
                }
                //Save our changes to PlayerPrefs
                PlayerPrefs.Save();
                //Write the dictionary values to a save file
                HandleFile.WriteSaveFile();
            }
            //Else we have saved values to read from, read from PlayerPrefs first with TXT as redundancy
            else
            {
                if (PlayerPrefs.HasKey("Forward"))
                {
                    for (int i = 0; i < baseSetup.Length; i++)
                    {
                        keys.Add(baseSetup[i].keyName, (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(baseSetup[i].keyName)));
                    }
                }
                else
                {
                    HandleFile.ReadSaveFile();
                }
            }
            //Change dictionaryFull to true so we don't try to load dictionary again when loading a new scene
            AudioManager.dictionaryFull = true;
        }
    }
    #endregion
    #region UI Input and Changes
    public void UpdateUI()
    {
        //Change the text elements for all keybind buttons to the values we have just set up in the dictionary
        for (int i = 0; i < baseSetup.Length; i++)
        {
            baseSetup[i].keyDisplayText.text = keys[baseSetup[i].keyName].ToString();
        }
    }
    
    public void ChangeKey(GameObject clickedKey)
    {
        //Set current key to the button user has clicked
        currentKey = clickedKey;
        //if user has selected a key change its button colour to the selected colour and reset any other button to default colour
        if (clickedKey != null)
        {
            //change colour of the key to the selected key colour
            clickedKey.GetComponent<Image>().color = selectedKey;
            //Change the other buttons to default colour and display the current key value they hold
            for (int i = 0; i < buttonUIColor.Length; i++)
            {
                if (buttonUIColor[i] != currentKey)
                {
                    buttonUIColor[i].GetComponent<Image>().color = resetKey;
                    baseSetup[i].keyDisplayText.text = keys[baseSetup[i].keyName].ToString();
                }
            }
        }
    }
    public void SaveKeys()
    {
        //For each entry in our dictionary set the value in PlayerPrefs to match our dictionary
        foreach (var keyEntry in keys)
        {
            PlayerPrefs.SetString(keyEntry.Key, keyEntry.Value.ToString());
        }
        //Save our changes to PlayerPrefs
        PlayerPrefs.Save();
        //Write the values to the additional save file as well
        HandleFile.WriteSaveFile();
        //For each button in our array reset the colour to it's default value
        for (int i = 0; i < buttonUIColor.Length; i++)
        {
            //GameObject currentButton = (GameObject)buttonUIColor[i];
            buttonUIColor[i].GetComponent<Image>().color = resetKey;
        }
        //For each key in our dictionary update its corresponding button text to match the new value
        for (int i = 0; i < baseSetup.Length; i++)
        {
            baseSetup[i].keyDisplayText.text = keys[baseSetup[i].keyName].ToString();
        }

    }
    private void OnGUI()
    {
        //Create a variable to use as a reference for our current key
        string newKey = "";
        //Create a variable to use as a reference to the current event
        Event e = Event.current;
        //if user has selected a button
        if (currentKey != null)
        {
            //If the event is a key press store the keycode pressed in the newKey reference
            //Shift keys need to be added seperately because Unity being Unity
            if (e.isKey)
            {
                newKey = e.keyCode.ToString();
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                newKey = "LeftShift";
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                newKey = "RightShift";
            }
            //If the event is a mouse button store the button in the newKey reference
            if (e.isMouse)
            {
                newKey = e.button.ToString();
            }
            //If we have set a new key
            if (newKey != "")
            {
                //Boolean to identify whether we have found a duplicate value
                bool foundDuplicate = false;
                //Check each entry in the keys dictionary to check if the value already exists
                foreach (var keyEntry in keys)
                {
                    if (keys.ContainsValue(e.keyCode))
                    {
                        //Indicate to user that a duplicate has been found using text and colour and change boolean value to true
                        foundDuplicate = true;
                        currentKey.GetComponent<Image>().color = errorKey;
                        currentKey.GetComponentInChildren<Text>().text = "Duplicate";
                    }
                }
                //If we haven't found a duplicate in the dictionary store the new keycode and indicate the new key has been stored using text and colour
                if (foundDuplicate == false)
                {

                    keys[currentKey.name] = (KeyCode)System.Enum.Parse(typeof(KeyCode), newKey);
                    currentKey.GetComponentInChildren<Text>().text = newKey;
                    currentKey.GetComponent<Image>().color = changedKey;
                }
                //forget the key we are editing
                currentKey = null;
            }
        }

    }
    #endregion
}