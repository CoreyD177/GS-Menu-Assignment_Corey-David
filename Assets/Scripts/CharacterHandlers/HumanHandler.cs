using System.Collections; //Allows use of IEnumerators
using UnityEngine; //Required for Unity connection
using UnityEngine.AI; //Allows use of Unity NavMesh features
public class HumanHandler : MonoBehaviour
{
    #region Variables
    //Animator for plank
    [Header("Animators")]
    [Tooltip("Add the Plank object here to grab it's animator")]
    [SerializeField]private Animator _plankAnim;
    //Variables for human character. Agent and State need to be public for access from GameManager
    private Animator _humanAnim;
    public NavMeshAgent humanAgent;
    [HideInInspector] public string humanState = "Investigate";
    //Variable for Ogre character to use as a proximity check
    [Header("Game Objects")]
    [Tooltip("Add the Ogre character object here")]
    [SerializeField]private GameObject _ogre;
    //Variables for Navpoint locations
    [Tooltip("Add the TrapPoint object here")]
    [SerializeField]private GameObject _trapPoint;
    [Tooltip("Add the RaidGate object here")]
    [SerializeField]private GameObject _investigatePoint;    
    #endregion
    private void Start()
    {
        //Get components for Agent and Animators
        _plankAnim = GameObject.Find("Plank").GetComponent<Animator>();
        _humanAnim = GetComponent<Animator>();
        humanAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        //Change human animations based off speed of movement
        if (humanAgent.velocity.magnitude < 0.01f)
        {
            _humanAnim.SetBool("isWalking", false);
            _humanAnim.SetBool("isRunning", false);
        }
        else if (humanAgent.velocity.magnitude < 5f)
        {
            _humanAnim.SetBool("isWalking", true);
            _humanAnim.SetBool("isRunning", false);
        }
        else
        {
            _humanAnim.SetBool("isWalking", false);
            _humanAnim.SetBool("isRunning", true);
        }
        //If we are chasing, continue to update our destination to where ogre currently is
        if (humanState == "Chase")
        {
            humanAgent.SetDestination(_ogre.transform.position);
        }
    }
    #region Human States
    //Select state based on _humanState value. Will be activated for first time from the MutantHandler class
    public void SelectState(string state)
    {
        switch (state)
        {
            case "Investigate":
                StartCoroutine(Investigating());
                break;
            case "Chase":
                StartCoroutine(Chasing());
                break;
            case "Trapped":
                StartCoroutine(Trapped());
                break;
            default:
                Debug.Log("Case Not Set");
                break;
        }
        
    }
    IEnumerator Investigating()
    {
        //Set destination to point near gate guarding the crypt
        humanAgent.SetDestination(_investigatePoint.transform.position);
        //Set speed to walking speed, walking slowly as he is investigating
        humanAgent.speed = 2.5f;
        //While we are investigating check for proximity of other character and change state to chase when he is close
        while (humanState == "Investigate")
        {
            if (Vector3.Distance(transform.position, _ogre.transform.position) < 10f)
            {
                humanState = "Chase";
            }
            yield return null;
        }
        //Run SelectState method based on current _humanState value
        SelectState(humanState);
    }
    IEnumerator Chasing()
    {        
        //Destination is already being handled in Update while we are in this state
        //Change speed to chasing speed, slightly slower than ogres speed so he can't actually catch him
        humanAgent.speed = 5.95f;
        //While we are chasing check for proximity to point where we will be trapped and when we get there trigger the plank to fall and change our state to trapped
        while (humanState == "Chase")
        {
            if (Vector3.Distance(_trapPoint.transform.position, transform.position) < 0.5f)
            {
                _plankAnim.SetTrigger("dropPlank");
                humanState = "Trapped";
            }
            yield return null;
        }
        //Run SelectState method based on current _humanState value
        SelectState(humanState);
    }
    IEnumerator Trapped()
    {
        //Set destination to where he is so he no longer tries to move
        humanAgent.SetDestination(_trapPoint.transform.position);
        //Change animation to idle as he has nowhere to go
        _humanAnim.SetBool("isWalking", false);
        _humanAnim.SetBool("isRunning", false);
        _humanAnim.SetBool("isKneeling", true);
        yield return null;
    }
    #endregion
}
