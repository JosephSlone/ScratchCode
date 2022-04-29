using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PlayerMotor : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Camera mainCamera;

    [SerializeField] LayerMask walkableLayers;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float animationDamping = 0.2f;

    float distanceFactor;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (OkayToMoveForward())
            {
                Vector3 direction = transform.forward;
                agent.Move(distanceFactor * moveSpeed * Time.deltaTime * direction);
            }

            LookAtPointer();
        }
        else
        {
            distanceFactor = 0;
        }
            
        AnimateMovement();

    }

    private bool OkayToMoveForward()
    {
        Vector3 rayOrigin = transform.position;
        rayOrigin.y += 0.25f;
        Ray ray = new Ray(rayOrigin, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f))
        {
            return false;
        }
        return true;
    }

    private void AnimateMovement()
    {
        if(OkayToMoveForward())
        {
            animator.SetFloat("moveSpeed", distanceFactor, animationDamping, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("moveSpeed", 0, animationDamping, Time.deltaTime);
        }

    }

    private void LookAtPointer()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, walkableLayers))
        {
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            distanceFactor = Mathf.Clamp(Vector3.Distance(hit.point, transform.position), 0.2f, 5) / 5.0f;
        }
    }
}
