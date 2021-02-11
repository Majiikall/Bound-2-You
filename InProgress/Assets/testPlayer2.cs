using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class testPlayer2 : MonoBehaviour
{
  [SerializeField]
  public float speed = 10.0f;

  [SerializeField]
  public float gravity = -9.81f;

  public CharacterController  controller;

  Vector3 velocity;

  public Transform groundCheck;
  public float groundDistance = 0.4f;
  public LayerMask groundMask;
  public float jumpHeight = 3.0f;

  //NEW//
  public Camera skyCam;
  public Camera playerCam;

  bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
      if(isGrounded && velocity.y < 0)
      {
        velocity.y = -2.0f;
      }
      var transformHold = this.GetComponent<Transform>();
      var oldPos = transformHold.localPosition;
      float x = Input.GetAxis("Horizontal");
      float z = Input.GetAxis("Vertical");

      Vector3 move = transform.right * x + transform.forward * z * 1.1f;

      controller.Move(move * speed * Time.deltaTime);

      if(Input.GetButtonDown("Jump") && isGrounded)
      {
        velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
      }

      velocity.y += gravity * Time.deltaTime;

      controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
      if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
      {
        skyCam.depth *= -1;
        playerCam.depth *= -1;
      }

      if(skyCam.depth > 0)
      {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
          GameObject parent = other.gameObject.transform.parent.gameObject;
          Component[] transform;
          transform = parent.GetComponentsInChildren<Transform>();
          Transform toUpdate = null;
          foreach(Transform child in transform)
          {
            if(child.name == "branch")
            {
              toUpdate = child;
            }
          }
          Debug.Log(toUpdate);
          Transform parentCoords = parent.GetComponent<Transform>();

          var updatePosition = toUpdate.position;
          var updateScale = toUpdate.localScale;

          updatePosition.z = parentCoords.position.z - parentCoords.localScale.z;
          updatePosition.x = parentCoords.position.x;
          updateScale.x = 0.65f;
          updateScale.z = 1.0f;

          toUpdate.position = updatePosition;
          toUpdate.localScale = updateScale;
          Debug.Log("Here1");
        }

        //NEXT
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
          GameObject parent = other.gameObject.transform.parent.gameObject;
          Component[] transform;
          transform = parent.GetComponentsInChildren<Transform>();
          Transform toUpdate = null;
          foreach(Transform child in transform)
          {
            if(child.name == "branch")
            {
              toUpdate = child;
            }
          }
          Debug.Log(toUpdate);
          Transform parentCoords = parent.GetComponent<Transform>();

          var updatePosition = toUpdate.position;
          var updateScale = toUpdate.localScale;

          updatePosition.x = parentCoords.position.x + parentCoords.localScale.x;
          updatePosition.z = parentCoords.position.z;
          updateScale.z = 0.65f;
          updateScale.x = 1.0f;

          toUpdate.position = updatePosition;
          toUpdate.localScale = updateScale;
          Debug.Log("Here2");
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
          GameObject parent = other.gameObject.transform.parent.gameObject;
          Component[] transform;
          transform = parent.GetComponentsInChildren<Transform>();
          Transform toUpdate = null;
          foreach(Transform child in transform)
          {
            if(child.name == "branch")
            {
              toUpdate = child;
            }
          }
          Debug.Log(toUpdate);
          Transform parentCoords = parent.GetComponent<Transform>();

          var updatePosition = toUpdate.position;
          var updateScale = toUpdate.localScale;

          updatePosition.z = parentCoords.position.z + parentCoords.localScale.z;
          updatePosition.x = parentCoords.position.x;
          updateScale.z = 1.0f;
          updateScale.x = 0.65f;

          toUpdate.position = updatePosition;
          toUpdate.localScale = updateScale;
          Debug.Log("Here3");
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
          GameObject parent = other.gameObject.transform.parent.gameObject;
          Component[] transform;
          transform = parent.GetComponentsInChildren<Transform>();
          Transform toUpdate = null;
          foreach(Transform child in transform)
          {
            if(child.name == "branch")
            {
              toUpdate = child;
            }
          }
          Debug.Log(toUpdate);
          Transform parentCoords = parent.GetComponent<Transform>();

          var updatePosition = toUpdate.position;
          var updateScale = toUpdate.localScale;

          updatePosition.x = parentCoords.position.x - parentCoords.localScale.x;
          updatePosition.z = parentCoords.position.z;
          updateScale.z = 0.65f;
          updateScale.x = 1.0f;

          toUpdate.position = updatePosition;
          toUpdate.localScale = updateScale;
          Debug.Log("Here4");
        }
      }
    }
}
