using UnityEngine; //Required for connection to Unity
public class MouseLook : MonoBehaviour
{
    #region Variables
    //An enum to contain the two mouse axis as options
    public enum RotationalAxis
    {
        MouseX,
        MouseY
    }
    //A variable to allow us to choose between the options in the enum
    [Header("Rotation")]
    public RotationalAxis axis;
    //Sensitivity of mouse movement in turning the camera
    [Header("Sensitivity")]
    [Range(0, 500)]
    public float sensitivity = 2f;
    //Values to hold the y position of the camera between a certain range
    [Header("Rotational Clamp")]
    public float minY = -60f;
    public float maxY = 60f;
    //A variable to store the value of our Y axis mouse movement
    private float m_rotY;
    #endregion
    void Start()
    {
        //If the object is the camera set the rotational axis to Y to enable looking up and down
        if (GetComponent<Camera>())
        {
            axis = RotationalAxis.MouseY;
        }
    }
    void Update()
    {
        //If we are not paused allow rotation
        if (Time.timeScale == 1)
        {
            #region Mouse X
            //If we are rotating on the X
            if (axis == RotationalAxis.MouseX)
            {
                //Transform our Y rotation based on our X-Axis mouse input to turn side to side
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
            }
            #endregion
            #region Mouse Y
            //Else we are only rotating on Y
            else
            {
                //Add our y axis input to our Y rotation variable
                m_rotY += Input.GetAxis("Mouse Y") * sensitivity;
                //Clamp the Y rotation variable to make sure it stays within the desired range
                m_rotY = Mathf.Clamp(m_rotY, minY, maxY);
                //transform our X rotation angle based on the Y rotation variable
                transform.localEulerAngles = new Vector3(-m_rotY, 0, 0);
            }
            #endregion
        }
    }
}
