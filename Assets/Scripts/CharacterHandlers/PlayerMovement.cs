using UnityEngine; //Required for Unity connection

//Require a Character controller component to be attached to the game object for movement
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Variable to allow us to activate the pause menu when pause button is pressed
    [Header("GameObjects")]
    [Tooltip("Add PauseMenu canvas here")]
    public GameObject pauseMenu;
    //GameObject components to allow us to modify position and animation
    [Header("Character Components")]
    [Tooltip("You can drag the Animator attached to this character here, but it will be grabbed automatically anyway")]
    public Animator erikaAnimator;
    [Tooltip("You can drag the Character Controller attached to this character here, but it will be grabbed automatically anyway")]
    public CharacterController charC;
    [Header("Speeds")]
    //Set the different speeds for the player
    public float moveSpeed = 0f;
    public float walkSpeed = 5f, runSpeed = 10f, crouchSpeed = 2.5f;
    public float jumpSpeed = 10f, gravity = 20f;
    //A Vector2 to store the X and Y position input values for movement
    private Vector2 _input;
    //A variable to store our movement direction to use for movement
    private Vector3 _moveDir;
    private Vector3 _pos;
    //Variable to control animation for crouching and swimming
    private float _isCrouching = 1f;
    private bool _swimming = false;
    #endregion

    void Start()
    {
        //Assign the PauseMenu object to the variable on start so we can manipulate it. Pause holder allows us to find actual pause menu even though it is deactivated at start
        pauseMenu = GameObject.Find("PauseHolder").transform.GetChild(0).gameObject;
        //Time may have been pause from previous return to main menu, reactivate on load
        Time.timeScale = 1;
        //Start the game with cursor locked and hidden
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Retrieve the required components from the GameObject this script is attached to
        charC = GetComponent<CharacterController>();
        erikaAnimator = GetComponent<Animator>();
        //If we are playing in unity editor run the ReadSaveFile function from HandleFile to store keybinds so we can move
#if UNITY_EDITOR
        HandleFile.ReadSaveFile();
#endif
    }

    void Update()
    {
        #region Player Input
        //If player presses pause button run the pause function
        if (Input.GetKeyDown(Keybinds.keys["Pause"]))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                Pause();
            }
        }
        //If our player is touching the ground or water we are allowed to control movement
        if (charC.isGrounded || _swimming == true)
        {
            //Store our Y axis input in the input variable
            _input.y = Input.GetKey(Keybinds.keys["Forward"]) ? 1 : Input.GetKey(Keybinds.keys["Backward"]) ? -1 : 0;
            //Store our X axis input in the input variable
            _input.x = Input.GetKey(Keybinds.keys["Right"]) ? 1 : Input.GetKey(Keybinds.keys["Left"]) ? -1 : 0;
            //Set our movement speed based off whether we are pressing any of the modifier keys
            moveSpeed = Input.GetKey(Keybinds.keys["Sprint"]) ? runSpeed : Input.GetKey(Keybinds.keys["Crouch"]) ? crouchSpeed : walkSpeed;
            //Store our input movement vectors in the direction variable so we have a direction to move toward
            _moveDir = transform.TransformDirection(new Vector3(_input.x, 0, _input.y));
            //Multiply our direction by the speed of movement so we will move the appropriate distance
            _moveDir *= moveSpeed;
            //If we press the jump key add the jump value to our y position and trigger the animation
            if (Input.GetKey(Keybinds.keys["Jump"]))
            {
                erikaAnimator.SetBool("Jumping", true);
                _moveDir.y = jumpSpeed;
            }
            else
            {
                erikaAnimator.SetBool("Jumping", false);
            }
            //If we press the crouch key set the animator variable to reflect that
            if (Input.GetKey(Keybinds.keys["Crouch"]))
            {
                _isCrouching = 0f;
            }
            else
            {
                _isCrouching = 1f;
            }
        }
        //Allow gravity to pull us down regardless of us being on the ground or not
        _moveDir.y -= gravity * Time.deltaTime;
        //Apply our calculated movement to the character
        charC.Move(_moveDir * Time.deltaTime);
        //Make sure we can't sink lower than surface of water
        _pos = transform.position;
        _pos.y = Mathf.Clamp(_pos.y, -1.6f, Mathf.Infinity);
        transform.position = _pos;
        #endregion
        #region Animation
        //Apply speed value to the animator
        if (_input.y == 0 && _input.x == 0)
        {
            erikaAnimator.SetFloat("Speed", 0);
        }
        else
        {
            erikaAnimator.SetFloat("Speed", moveSpeed);
        }
        //Apply leftRight value to the animator
        erikaAnimator.SetFloat("LeftRight", _input.x);
        //Apply forwardBack value to the animator
        erikaAnimator.SetFloat("ForwardBack", _input.y);
        //Apply IsCrouching value to the animator
        erikaAnimator.SetFloat("IsCrouching", _isCrouching);
        //Apply swimming value to animator
        erikaAnimator.SetBool("Swimming", _swimming);
        #endregion
    }
    #region Control Swimming
    //On coliiding with water set swimming to true
    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.layer == 4)
        {
            _swimming = true;
        }
    }
    //On exiting water collision set swimming to false
    private void OnTriggerExit(Collider other)
    {        
        if (other.gameObject.layer == 4)
        {
            _swimming = false;
        }
    }
    #endregion
    #region Pause
    public void Pause()
    {
        //If game is running, pause the timescale, reactivate cursor and enable pause menu
        if (!pauseMenu.activeInHierarchy)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
        }
        //Else hide and lock cursor and resume timescale
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    #endregion
}