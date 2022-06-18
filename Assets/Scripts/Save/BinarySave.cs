using UnityEngine; //Required for Unity connection
using System.IO; //Allows us to use FileStream to read from and save to files
using System.Runtime.Serialization.Formatters.Binary; //Allows use of the BinaryFormatter

public class BinarySave
{
    //Save function requires Player transforms, states and a save index to be fed to it
    public static void SaveGameData(Transform erikaTransform, Transform mutantTransform, Transform humanTransform, Transform ogreTransform, string mutantState,string humanState, string ogreState, int saveIndex)
    {
        //Retrieve SaveData information so we can save it to file
        SaveData data = new SaveData(erikaTransform, mutantTransform, humanTransform, ogreTransform, mutantState, humanState, ogreState);
        //Create a new BinaryFormatter so we can convert SaveData information
        BinaryFormatter formatter = new BinaryFormatter();
        //Create a path for the new save file to live in appending the passed through save index to the name of the file
        string path = Application.persistentDataPath + "/SaveFile" + saveIndex + ".bin";
        //Open a new FileStream to stream the data to the file
        FileStream stream = new FileStream(path, FileMode.Create);
        //Use formatter to convert stream to binary
        formatter.Serialize(stream, data);
        //Close FileStream
        stream.Close();
    }
    //Load function  requires Player transforms, states and a save index to be fed to it when calling so it will change the values. Interacts with SaveData file.
    public static SaveData LoadPlayerData(Transform erikaTransform, Transform mutantTransform, Transform humanTransform, Transform ogreTransform, string mutantState, string humanState, string ogreState, int saveIndex)
    {
        //Create path to existing save file using the passed through save index to identify the correct file to read
        string path = Application.persistentDataPath + "/SaveFile" + saveIndex + ".bin";
        //If the file exists at that path
        if (File.Exists(path))
        {
            //Create a new BinaryFormatter so we can convert back from binary
            BinaryFormatter formatter = new BinaryFormatter();
            //OOpen a new FileStream to read from file
            FileStream stream = new FileStream(path, FileMode.Open);
            //New SaveData reference to store our deserialized data into for loading
            SaveData data = (SaveData)formatter.Deserialize(stream);
            //Close the stream
            stream.Close();
            //Call the LoadPlayerData function in SaveData to store correct values in the arrays and variables so it can be passed to GameManager
            data.LoadPlayerData(erikaTransform, mutantTransform, humanTransform, ogreTransform, mutantState, humanState, ogreState);
            //Return the data
            return data;
        }
        //If the specified file doesn't exist return nothing
        else
        {
            return null;
        }
    }
}
