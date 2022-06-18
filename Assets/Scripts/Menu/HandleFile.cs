using UnityEngine; //Resources.Load
using System.IO; //have access to characters from a byte stream

public class HandleFile : MonoBehaviour
{
    #region Variables
    //The location of the default keybinds text
    public static string path = Path.Combine(Application.streamingAssetsPath, "Settings/Keybinds.txt");
    #endregion
    #region Write
    public static void WriteSaveFile()
    {
        //Create a new writer that will overwrite existing file
        StreamWriter writer = new StreamWriter(path, false);
        //write each of our keys to the file with key index and value separated by a colon
        foreach (var key in Keybinds.keys)
        {
            writer.WriteLine(key.Key + ":" + key.Value.ToString());
        }
        //Close the writer
        writer.Close();
        //Reload the file so we have the correct version
        TextAsset asset = Resources.Load(path) as TextAsset;
    }
    #endregion
    #region Read
    public static void ReadSaveFile()
    {
        //Create a new reader to read from the file
        StreamReader reader = new StreamReader(path);
        //Create a string from the line we are currently reading
        string line;
        //While loop to run for each line until the line is empty
        while ((line = reader.ReadLine()) != null)
        {
            //Create a string array with the first index being the string value from the file and the second index being the keycode
            string[] parts = line.Split(':');
            //If we already have keybinds in the keys dictionary change the value in the dictionary
            if (Keybinds.keys.Count > 0)
            {
                Keybinds.keys[parts[0]] = (KeyCode)System.Enum.Parse(typeof(KeyCode), parts[1]);
            }
            //else we need add the keys to the dictionary
            else
            {
                Keybinds.keys.Add(parts[0], (KeyCode)System.Enum.Parse(typeof(KeyCode), parts[1]));
            }
        }
        //Close the reader
        reader.Close();
    }
    #endregion
}
