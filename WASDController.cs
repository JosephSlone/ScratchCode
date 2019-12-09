using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using UnityEngine.EventSystems;
using RPG.Dialogue;
using RPG.InventorySystem;
using RPG.Saving;
using System;

namespace RPG.Control
{
    public class WASDController : MonoBehaviour, IAction, ISaveable
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
            animator.SetFloat("animationSpeed", animationSpeed);
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
            animator.SetFloat("forwardSpeed", speed);
        }

        private void Movement()
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");

            Vector3 right = mainCamera.transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);

            movement = forward * z + right * x;

            if(movement != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(movement);
                Quaternion slerpedRotation = Quaternion.Slerp(transform.rotation, 
                    desiredRotation, movement.magnitude * rotationSpeed * Time.deltaTime);

                transform.rotation = slerpedRotation;
            }            
            agent.velocity = movement * speed;

            currentSpeed = movement.magnitude * animationSpeed;
        }

        private void FixedUpdate()
        {
            animator.SetFloat("forwardSpeed", currentSpeed);
        }

        public void Cancel()
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.Log("Null Agent: " + name);
                throw new NotImplementedException();
            }
            agent.enabled = false;
            transform.position = position.ToVector();
            agent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void Step()
        {

        }
    }
}
