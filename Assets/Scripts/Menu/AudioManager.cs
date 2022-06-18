using UnityEngine; //Required for connection to unity

public class AudioManager : MonoBehaviour
{
    //Create a static instance of AudioManager
    public static AudioManager audioInstance;
    //A bool to check if dictionary is full
    public static bool dictionaryFull = false;
    private void Awake()
    {
        //Don't destroy this gameobject when we move to new scene
        DontDestroyOnLoad(this.gameObject);
        //If we already have an active instance that isn't this one, destroy this instance
        if (audioInstance != null && audioInstance != this)
        {
            Destroy(this.gameObject);
        }
        //Else this is the AudioManager instance
        else
        {
            audioInstance = this;
        }
        
    }

}
