using UnityEngine;
using UnityEngine.AI;


public class SimpleWASDController : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float animationSpeed = 3.5f;
    [SerializeField] float rotationSpeed = 20f;


    private NavMeshAgent agent;
    private Camera mainCamera;
    private Animator animator;
    private float x, z;
    private Vector3 movement;
    public float currentSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        agent.updateRotation = false;
        animator.SetFloat("percentSpeed", 0);
    }

    void Update()
    {
        Movement();
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        animator.SetFloat("percentSpeed", speed);
    }

    private void Movement()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 right = mainCamera.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        movement = forward * z + right * x;

        if (movement != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(movement);
            Quaternion slerpedRotation = Quaternion.Slerp(transform.rotation,
                desiredRotation, movement.magnitude * rotationSpeed * Time.deltaTime);

            transform.rotation = slerpedRotation;
        }

        agent.velocity = movement * speed;
        currentSpeed = movement.magnitude * animationSpeed;
    }
}
