using UnityEngine; //Required for unity connection

//Allow the class to be serialized
[System.Serializable]
public class SaveData
{
    #region Variables
    //Float arrays to store the Vector3 and Quaternion values for each character
    public float[] erikaPosition;
    public float[] erikaRotation;
    public float[] mutantPosition;
    public float[] mutantRotation;
    public float[] humanPosition;
    public float[] humanRotation;
    public float[] ogrePosition;
    public float[] ogreRotation;
    //String variables to store the state of each NavMesh Agent
    public string mutantSaveState;
    public string humanSaveState;
    public string ogreSaveState;
    #endregion

    public SaveData(Transform erikaTransform, Transform mutantTransform, Transform humanTransform, Transform ogreTransform, string mutantState, string humanState, string ogreState)
    {
        //Store the current values for each player which was passed from GameManager so BinarySave can save them to file
        erikaPosition = new float[] {erikaTransform.position.x,erikaTransform.position.y,erikaTransform.position.z};
        erikaRotation = new float[] {erikaTransform.rotation.x, erikaTransform.rotation.y, erikaTransform.rotation.z, erikaTransform.rotation.w};
        mutantPosition = new float[] { mutantTransform.position.x, mutantTransform.position.y, mutantTransform.position.z };
        mutantRotation = new float[] { mutantTransform.rotation.x, mutantTransform.rotation.y, mutantTransform.rotation.z, mutantTransform.rotation.w };
        humanPosition = new float[] { humanTransform.position.x, humanTransform.position.y, humanTransform.position.z };
        humanRotation = new float[] { humanTransform.rotation.x, humanTransform.rotation.y, humanTransform.rotation.z, humanTransform.rotation.w };
        ogrePosition = new float[] { ogreTransform.position.x, ogreTransform.position.y, ogreTransform.position.z };
        ogreRotation = new float[] { ogreTransform.rotation.x, ogreTransform.rotation.y, ogreTransform.rotation.z, ogreTransform.rotation.w };
        mutantSaveState = mutantState;
        humanSaveState = humanState;
        ogreSaveState = ogreState;
    }

    public void LoadPlayerData(Transform erikaTransform, Transform mutantTransform, Transform humanTransform, Transform ogreTransform, string mutantState, string humanState, string ogreState)
    {
        //Set Transform and State data to that which was returned from BinarySave so we can change the characters position and state on load using GameManager.
        erikaTransform.position = new Vector3(erikaPosition[0], erikaPosition[1], erikaPosition[2]);
        erikaTransform.rotation = new Quaternion(erikaRotation[0], erikaRotation[1], erikaRotation[2], erikaRotation[3]);
        mutantTransform.position = new Vector3(mutantPosition[0], mutantPosition[1], mutantPosition[2]);
        mutantTransform.rotation = new Quaternion(mutantRotation[0], mutantRotation[1], mutantRotation[2], mutantRotation[3]);
        humanTransform.position = new Vector3(humanPosition[0], humanPosition[1], humanPosition[2]);
        humanTransform.rotation = new Quaternion(humanRotation[0], humanRotation[1], humanRotation[2], humanRotation[3]);
        ogreTransform.position = new Vector3(ogrePosition[0], ogrePosition[1], ogrePosition[2]);
        ogreTransform.rotation = new Quaternion(ogreRotation[0], ogreRotation[1], ogreRotation[2], ogreRotation[3]);
        mutantState = mutantSaveState;
        humanState = humanSaveState;
        ogreState = humanSaveState;
    }
}
