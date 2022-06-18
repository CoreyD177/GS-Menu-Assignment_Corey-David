using UnityEngine; //Required for unity connection
using UnityEngine.SceneManagement; //Allows us to check which scene we are in

public class GameManager : MonoBehaviour
{
    #region Variables
    //GameObject variables to hold player and ai agents so we can store and change their Transforms
    [Header("GameObjects")]
    [Tooltip("Add the player character from the Hierarchy here in Game Scene. Ignore in Menu Scene")]
    public GameObject erika;
    [Tooltip("Add the ogre character from the Hierarchy here in Game Scene. Ignore in Menu Scene")]
    public GameObject ogre;
    [Tooltip("Add the mutant character from the Hierarchy here in Game Scene. Ignore in Menu Scene")]
    public GameObject mutant;
    [Tooltip("Add the human character from the Hierarchy here in Game Scene. Ignore in Menu Scene")]
    public GameObject human;
    //References to their scripts so we can modify their states and other components
    [Tooltip("Add the mutant character from the Hierarchy here in Game Scene so we can grab its attached script. Ignore in Menu Scene")]
    public MutantHandler mutantHandler;
    [Tooltip("Add the human character from the Hierarchy here in Game Scene so we can grab its attached script. Ignore in Menu Scene")]
    public HumanHandler humanHandler;
    [Tooltip("Add the ogre character from the Hierarchy here in Game Scene so we can grab its attached script. Ignore in Menu Scene")]
    public OgreHandler ogreHandler;
    [Tooltip("Add the player character from the Hierarchy here in Game Scene so we can grab its attached script. Ignore in Menu Scene")]
    public PlayerMovement erikaHandler;
    //Reference to the MenuHandler class so we can retrieve and modify its settings values
    private MenuHandler _menuHandler;
    #endregion
    #region Initial Load
    public void Start()
    {       
        //Retrieve the MenuHandler class from the optionsholder object
        _menuHandler = GameObject.Find("OptionsHolder").GetComponent<MenuHandler>();
        //If we are in the game scene check if we are loading and load the appropriate file based on index saved in PlayerPrefs
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            ogreHandler = GameObject.Find("Ogre").GetComponent<OgreHandler>();
            //If we are loading a game
            if (PlayerPrefs.HasKey("isLoading"))
            {
                //If isLoading = 0 we are loading the last save file created based off LastSave value in playerprefs
                if (PlayerPrefs.GetInt("isLoading") == 0)
                {
                    LoadGame(PlayerPrefs.GetInt("LastSave"));
                }
                //Else load the file that corresponds to the isLoading value
                else
                {
                    LoadGame(PlayerPrefs.GetInt("isLoading"));
                }
                //Delete the isLoading key so we don't load again if new game is selected
                PlayerPrefs.DeleteKey("isLoading");
            }
        }
    }
    #endregion
    #region Settings
    public void SaveSettings()
    {
        //Save each of the available settings in PlayerPrefs using the name of the object as a key
        PlayerPrefs.SetFloat(_menuHandler.masterVolume.name, _menuHandler.masterVolume.value);
        PlayerPrefs.SetFloat(_menuHandler.musicVolume.name, _menuHandler.musicVolume.value);
        PlayerPrefs.SetFloat(_menuHandler.sfxVolume.name, _menuHandler.sfxVolume.value);
        //Bools have to be converted to int
        PlayerPrefs.SetInt(_menuHandler.masterMute.name, (_menuHandler.masterMute.isOn ? 1 : 0));
        PlayerPrefs.SetInt(_menuHandler.musicMute.name, (_menuHandler.musicMute.isOn ? 1 : 0));
        PlayerPrefs.SetInt(_menuHandler.sfxMute.name, (_menuHandler.sfxMute.isOn ? 1 : 0));
        PlayerPrefs.SetInt(_menuHandler.fullscreen.name, (_menuHandler.fullscreen.isOn ? 1 : 0));
        //Save the int value of the option in the dropdown menus
        PlayerPrefs.SetInt(_menuHandler.resolution.name, _menuHandler.resolution.value);
        PlayerPrefs.SetInt(_menuHandler.quality.name, _menuHandler.quality.value);
        PlayerPrefs.Save();
    }
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(_menuHandler.masterVolume.name))
        {
            //Load each of the values from PlayerPrefs that matches the name of the corresponding object
            _menuHandler.masterVolume.value = PlayerPrefs.GetFloat(_menuHandler.masterVolume.name);
            _menuHandler.musicVolume.value = PlayerPrefs.GetFloat(_menuHandler.musicVolume.name);
            _menuHandler.sfxVolume.value = PlayerPrefs.GetFloat(_menuHandler.sfxVolume.name);
            //Convert int for toggles back to bool
            _menuHandler.masterMute.isOn = ((PlayerPrefs.GetInt(_menuHandler.masterMute.name) != 0) ? true : false);
            _menuHandler.musicMute.isOn = ((PlayerPrefs.GetInt(_menuHandler.musicMute.name) != 0) ? true : false);
            _menuHandler.sfxMute.isOn = ((PlayerPrefs.GetInt(_menuHandler.sfxMute.name) != 0) ? true : false);
            _menuHandler.fullscreen.isOn = ((PlayerPrefs.GetInt(_menuHandler.fullscreen.name) != 0) ? true : false);
            _menuHandler.resolution.value = PlayerPrefs.GetInt(_menuHandler.resolution.name);
            _menuHandler.quality.value = PlayerPrefs.GetInt(_menuHandler.quality.name);
        }
    }
    #endregion
    #region Game Saves
    public void SaveGame(int saveIndex)
    {
        //Call the SaveGameData function from BinarySave to save the current positions and states of the characters using the index passed from the button as a save index
        BinarySave.SaveGameData(erika.transform, mutant.transform, human.transform, ogre.transform, mutantHandler.mutantState, humanHandler.humanState, ogreHandler.ogreState, saveIndex);
        //Set LastSave value in playerprefs to this index so we load this file on continue
        PlayerPrefs.SetInt("LastSave", saveIndex);
    }
    public void LoadGame(int saveIndex)
    {
        //Disabled CharacterController on player character so we have no issues changing her position
        erika.GetComponent<CharacterController>().enabled = false;
        //Call LoadPlayerData function from BinarySave so we can load the saved values corresponding to the passed index and change the characters position and state accordingly
        BinarySave.LoadPlayerData(erika.transform, mutant.transform, human.transform, ogre.transform, mutantHandler.mutantState, humanHandler.humanState, ogreHandler.ogreState, saveIndex);
        //Warp the navmesh agents to the newly loaded position.
        mutantHandler.mutantAgent.Warp(mutant.transform.position);
        humanHandler.humanAgent.Warp(human.transform.position);
        ogreHandler.ogreAgent.Warp(ogre.transform.position);
        //Reenable player characters CharacterController
        erika.GetComponent<CharacterController>().enabled = true;
        //Change NavMesh agents states as required
        mutantHandler.SelectState(mutantHandler.mutantState);
        if (mutantHandler.mutantState == "Raid" || mutantHandler.mutantState == "Escape")
        {
            //Disable Colliders and NavMesh Obstacles on swing gate to avoid blocking
            mutantHandler.obstacles[2].carving = false;
            mutantHandler.obstacles[3].carving = false;
            GameObject.Find("Gate03").GetComponent<BoxCollider>().enabled = false;
            GameObject.Find("Gate04").GetComponent<BoxCollider>().enabled = false;
            humanHandler.SelectState(humanHandler.humanState);
            ogreHandler.SelectState(ogreHandler.ogreState);            
        }
        //Reenable time as it will have been stopped if loading from pause menu
        Time.timeScale = 1;
    }
    #endregion
}
