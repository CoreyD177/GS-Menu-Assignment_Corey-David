using System.Collections; //Allows use of IEnumerators
using UnityEngine; //Required for Unity connection
using UnityEngine.AI; //Allows use of Unity AI NavMesh features

public class MutantHandler : MonoBehaviour
{
    #region Variables
    //Variables for mutant character. Agent needs to be puglic for access in GameManager
    public NavMeshAgent mutantAgent;
    private Animator _mutantAnim;
    //Variables to control speed when mutant touches the water terrain
    private int _waterSpeed;
    private NavMeshHit _mutantHit;
    //Variables for various animators
    [Header("Animators")]
    [Tooltip("Add the BreakWall game object here to grab its animator")]
    [SerializeField] private Animator _breakWall;
    [Tooltip("Add the SwingGate02 game object here to grab its animator")]
    [SerializeField] private Animator _swingGate;
    [Tooltip("Add the DropGate02 game object here to grab its animator")]
    [SerializeField] private Animator _dropGate;
    [Tooltip("Add the Crypt game object here to grab its animator")]
    [SerializeField] private Animator _crypt;
    //Array of NavMesh Obstacles so they can be disabled when gates open as they were creating issues when activated
    [Header("NavMesh Obstacles")]
    [Tooltip("Add the Swing Gate objects containing the NavMesh obstacles here")]
    [SerializeField] public NavMeshObstacle[] obstacles;
    //Game Object variables to allow for location and to allow destroying
    [Header("Game Objects")]
    [Tooltip("Add the Key game object here")]
    [SerializeField] private GameObject key;
    [Tooltip("Add the ReleaseGate game object here")]
    [SerializeField] private GameObject _releaseGate;
    [Tooltip("Add the RaidGate game object here")]
    [SerializeField] private GameObject _raidGate;
    [Tooltip("Add the CryptRaid game object here")]
    [SerializeField] private GameObject _cryptRaid;
    [Tooltip("Add the Diamond game object here")]
    [SerializeField] private GameObject _diamond;
    [Tooltip("Add the EscapeGate game object here")]
    [SerializeField] private GameObject _escapeGate;
    [Tooltip("Add the CellWait game object here")]
    [SerializeField] private GameObject _cellWait;
    [Tooltip("Add the FinalDestination game object here")]
    [SerializeField] private GameObject _finalDestination;
    //Script references so we can trigger other AI Agents to start their behaviours
    [Tooltip("Add the Human character game object here to grab its script")]
    public HumanHandler humanHandler;
    [Tooltip("Add the Ogre character game object here to grab its script")]
    public OgreHandler ogreHandler;
    //Private animators, we will grab them using code
    private Animator _swingOpen;
    private Animator _escapeDropGate;
    //Variable to hold our current state, start by default, needs to be public for access from GameManager
    [HideInInspector]public string mutantState = "Start";
    #endregion
    void Start()
    {
        //Set _waterSpeed to the value of the NavMesh modifier mask for Water
        _waterSpeed = 1 << NavMesh.GetAreaFromName("Water");
        //Get componenets for Agent and Animators
        _swingOpen = GameObject.Find("SwingGate").GetComponent<Animator>();
        _escapeDropGate = GameObject.Find("DropGate").GetComponent<Animator>();
        _mutantAnim = GetComponent<Animator>();
        mutantAgent = GetComponent<NavMeshAgent>();
        //Find and set the Final Destination object
        _finalDestination = GameObject.Find("FinalDestination");
        //Run SelectState function based off our default _mutantState
        SelectState(mutantState);
    }
    void Update()
    {
        //Sample the Navmesh modifier our character is currently in contact with
        mutantAgent.SamplePathPosition(-1, 0.0f, out _mutantHit);
        //Change animations based off speed and which NavMesh we are on
        if (mutantAgent.velocity.magnitude < 0.01f)
        {
            _mutantAnim.SetBool("isWalking", false);
        }
        else
        {
            //If we are on water move slowly, trigger our swimming animation and reduce our Y position so we swim in water and not above it
            if (_mutantHit.mask == _waterSpeed)
            {
                mutantAgent.speed = 1.0f;
                Vector3 pos = transform.position;
                pos.y = -1.6f;
                transform.position = pos;
                _mutantAnim.SetBool("isSwimming", true);
                _mutantAnim.SetBool("isWalking", false);
            }
            //Else we are walking, set speed to walking speed and trigger walking animation
            else
            {
                mutantAgent.speed = 3.5f;
                _mutantAnim.SetBool("isWalking", true);
                _mutantAnim.SetBool("isSwimming", false);
            }
        }
    }
    #region Mutant States
    //Select state based off our _mutantState variable
    public void SelectState(string state)
    {
        switch (state)
        {
            case "Start":
                //Set destination to Key location
                mutantAgent.SetDestination(key.transform.position);
                StartCoroutine(MutantStart());
                break;
            case "Prisoner":
                //Set destination to location of main drop gate
                mutantAgent.SetDestination(_escapeGate.transform.position);
                StartCoroutine(FreePrisoner());
                break;
            case "Raid":
                //Set destination to drop gate guarding crypt
                mutantAgent.SetDestination(_raidGate.transform.position);
                StartCoroutine(StealTreasure());
                break;
            case "Escape":
                StartCoroutine(Escape());
                break;
            default:
                StartCoroutine(MutantStart());
                break;
        }
    }
    IEnumerator MutantStart()
    {
        //While we are in our first state check if we have reached the location of the key and if we have destroy the key and change state to Prisoner
        while (mutantState == "Start")
        {
            if (Vector3.Distance(transform.position, key.transform.position) < 0.6f)
            {
                Destroy(key);
                mutantState = "Prisoner";
            }
            yield return null;
        }
        //Run SelectState function based on our current _mutantState value
        SelectState(mutantState);
    }
    IEnumerator FreePrisoner()
    {
        //While we are in our prisoner state
        while (mutantState == "Prisoner")
        {
            //If we are close to the main gate open it and set new destination for prisoner's cell
            if (Vector3.Distance(transform.position, _escapeGate.transform.position) < 0.05f)
            {
                _escapeDropGate.SetTrigger("drop");
                mutantAgent.SetDestination(_releaseGate.transform.position);
            }
            //If we are close to prisoner's cell trigger punch animation, open gate and remove NavMesh obstacles for those gates
            //Change state for Ogre and Human so they start their actions and set Mutants new destination so he hides in his cell
            if (Vector3.Distance(transform.position, _releaseGate.transform.position) < 0.05f)
            {
                _mutantAnim.SetTrigger("punch");
                yield return new WaitForSecondsRealtime(0.5f);
                _swingGate.SetTrigger("open");
                //Deactivate Colliders and obstacles on second Swing gate to avoid blocking issues
                GameObject.Find("Gate03").GetComponent<BoxCollider>().enabled = false;
                GameObject.Find("Gate04").GetComponent<BoxCollider>().enabled = false;
                obstacles[2].carving = false;
                obstacles[3].carving = false;
                yield return new WaitForSecondsRealtime(1f);
                //Trigger other agents to start their behaviours and set new destination to hide in original cell
                humanHandler.SelectState("Investigate");
                ogreHandler.SelectState("Heist");
                mutantAgent.SetDestination(_cellWait.transform.position);
            }
            //If we are in our own cell, wait for other characters to go away and then open gates, remove NavMesh Obstacles and change state to Raid
            if (Vector3.Distance(transform.position, _cellWait.transform.position) < 0.05f)
            {
                yield return new WaitForSecondsRealtime(6f);
                _mutantAnim.SetTrigger("punch");
                yield return new WaitForSecondsRealtime(2f);
                //Disable colliders and obstacles on first swing gate to avoid blocking issues
                GameObject.Find("Gate01").GetComponent<BoxCollider>().enabled = false;
                GameObject.Find("Gate02").GetComponent<BoxCollider>().enabled = false;
                obstacles[0].carving = false;
                obstacles[1].carving = false;
                //Open swing gate and move to next state
                _swingOpen.SetTrigger("open");
                mutantState = "Raid";
            }
            yield return null;
        }
        //Run SelectState function based on our current _mutantState value
        SelectState(mutantState);
    }
    IEnumerator StealTreasure()
    {
        //While we are raiding
        while (mutantState == "Raid")
        {
            //If we are near the drop gate open it and set our new destination to the crypt
            if (Vector3.Distance(transform.position, _raidGate.transform.position) < 0.5f)
            {
                _dropGate.SetTrigger("drop");
                yield return new WaitForSecondsRealtime(2f);
                mutantAgent.SetDestination(_cryptRaid.transform.position);
            }
            //If we are near crypt, open it, steal the treasure and then change our state to Escape
            if (Vector3.Distance(transform.position, _cryptRaid.transform.position) < 0.5f)
            {
                _crypt.SetTrigger("slideOpen");
                yield return new WaitForSecondsRealtime(5f);
                Destroy(_diamond);
                mutantState = "Escape";
            }
            yield return null;
        }
        //Run SelectState function based on our current _mutantState value
        SelectState(mutantState);
    }
    IEnumerator Escape()
    {
        //Set destination to our final destination
        mutantAgent.SetDestination(_finalDestination.transform.position);
        yield return null;
    }
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        //Trigger the breakwall animation when we come into contact with the BreakWall
        if (other.transform.parent.name == "BreakWall")
        {
            _breakWall = other.gameObject.GetComponentInParent<Animator>();
            _breakWall.SetTrigger("break");
        }
    }
}
