using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class playerMovement : MonoBehaviour
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

  bool isGrounded;

  public GameObject levelCompleteUI;

  public GameObject mainUI;

  private float scalingFrames = 0;
  private float nextValueScale = 0.0f;
  private float nextValuePos = 0.0f;

  private int curr = 0;

  private bool isActive = false;
  private bool branchActive = false;

  private GameObject parent;
  private GameObject parent2;

  private bool pressedYet = false;

  private Camera skyCam;
  public Camera playerCam;

  private int altSelection = 0;

  public Light playerLight;

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

    var newPoss = transformHold.localPosition;
    var delta = (newPoss - oldPos).magnitude;
    var animation = this.GetComponent<Transform>().GetComponentsInChildren<Animator>()[0];

    if (delta > 0.01)
    {
      animation.Play("Walk State");
    }
    else
    {
      animation.Play("Idle State");
    }

    bool mousePressedYet = Input.GetMouseButtonDown(0);

    if(Input.GetButtonDown("Submit") || mousePressedYet)
    {
      if(mousePressedYet)
      {
        pressedYet = true;
      }
      else if(branchActive)
      {
        skyCam.depth *= -1;
        playerCam.depth *= -1;
        pressedYet = !pressedYet;
      }
      else
      {
        pressedYet = true;
      }
    }
    if (pressedYet && isActive)
    {
        runIt();
    }
    if(pressedYet && branchActive)
    {
      branchIt();
    }
  }

  private void runIt()
  {
    if(scalingFrames > 0)
    {
      Transform objectTransform = parent.transform.parent.GetComponent<Transform>();

      var tempScale = objectTransform.localScale;
      var tempPos = objectTransform.position;

      tempScale.y = Mathf.Lerp(objectTransform.localScale.y, nextValueScale, 1.0f / scalingFrames);
      tempPos.y = Mathf.Lerp(objectTransform.position.y, nextValuePos, 1.0f / scalingFrames);

      objectTransform.localScale = tempScale;
      objectTransform.position = tempPos;
      scalingFrames--;
    }
  }

  private void branchIt()
  {
    if(skyCam.depth > 0)
    {
      bool south = Input.GetKeyDown(KeyCode.Alpha1);
      bool east = Input.GetKeyDown(KeyCode.Alpha2);
      bool north = Input.GetKeyDown(KeyCode.Alpha3);
      bool west = Input.GetKeyDown(KeyCode.Alpha4);
      if(Input.GetMouseButtonDown(0))
      {
        altSelection++;
        if(altSelection > 4)
        {
          altSelection = 1;
        }
      }

      if(south || altSelection == 1)
      {
        Component[] transform;
        transform = parent2.GetComponentsInChildren<Transform>();
        Transform toUpdate = null;
        foreach(Transform child in transform)
        {
          if(child.name == "branch")
          {
            toUpdate = child;
          }
        }
        Transform parentCoords = parent2.GetComponent<Transform>();

        var updatePosition = toUpdate.position;
        var updateScale = toUpdate.localScale;

        updatePosition.z = parentCoords.position.z - parentCoords.localScale.z;
        updatePosition.x = parentCoords.position.x;
        updateScale.x = 0.65f;
        updateScale.z = 1.0f;

        toUpdate.position = updatePosition;
        toUpdate.localScale = updateScale;

        altSelection = 1;
      }

      //NEXT
      if(east || altSelection == 2)
      {
        Component[] transform;
        transform = parent2.GetComponentsInChildren<Transform>();
        Transform toUpdate = null;
        foreach(Transform child in transform)
        {
          if(child.name == "branch")
          {
            toUpdate = child;
          }
        }
        Transform parentCoords = parent2.GetComponent<Transform>();

        var updatePosition = toUpdate.position;
        var updateScale = toUpdate.localScale;

        updatePosition.x = parentCoords.position.x + parentCoords.localScale.x;
        updatePosition.z = parentCoords.position.z;
        updateScale.z = 0.65f;
        updateScale.x = 1.0f;

        toUpdate.position = updatePosition;
        toUpdate.localScale = updateScale;

        altSelection = 2;
      }
      if(north || altSelection == 3)
      {
        Component[] transform;
        transform = parent2.GetComponentsInChildren<Transform>();
        Transform toUpdate = null;
        foreach(Transform child in transform)
        {
          if(child.name == "branch")
          {
            toUpdate = child;
          }
        }
        Transform parentCoords = parent2.GetComponent<Transform>();

        var updatePosition = toUpdate.position;
        var updateScale = toUpdate.localScale;

        updatePosition.z = parentCoords.position.z + parentCoords.localScale.z;
        updatePosition.x = parentCoords.position.x;
        updateScale.z = 1.0f;
        updateScale.x = 0.65f;

        toUpdate.position = updatePosition;
        toUpdate.localScale = updateScale;

        altSelection = 3;
      }
      if(west || altSelection == 4)
      {
        Component[] transform;
        transform = parent2.GetComponentsInChildren<Transform>();
        Transform toUpdate = null;
        foreach(Transform child in transform)
        {
          if(child.name == "branch")
          {
            toUpdate = child;
          }
        }
        Transform parentCoords = parent2.GetComponent<Transform>();

        var updatePosition = toUpdate.position;
        var updateScale = toUpdate.localScale;

        updatePosition.x = parentCoords.position.x - parentCoords.localScale.x;
        updatePosition.z = parentCoords.position.z;
        updateScale.z = 0.65f;
        updateScale.x = 1.0f;

        toUpdate.position = updatePosition;
        toUpdate.localScale = updateScale;

        altSelection = 4;
      }
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if(other.name == "growerTrigger")
    {
      mainUI.SetActive(true);
      isActive = true;
      parent = other.gameObject.transform.parent.gameObject;
      nextValueScale = parent.transform.parent.GetComponent<Transform>().localScale.y + 10.0f;
      nextValuePos = parent.transform.parent.GetComponent<Transform>().position.y + 5.0f;
      scalingFrames = 10 / Time.deltaTime;
    }
    else if(other.name == "endTrigger")
    {
      levelCompleteUI.SetActive(true);
      Time.timeScale = 0.0f;
      Cursor.lockState = CursorLockMode.Confined;
    }
    else if(other.name == "branchTrigger")
    {
      parent2 = other.gameObject.transform.parent.gameObject;
      mainUI.SetActive(true);
      branchActive = true;

      foreach(Camera cam in parent2.GetComponentsInChildren<Camera>())
      {
        skyCam = cam;
      }
      playerLight.intensity = 12;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.name == "growerTrigger" || other.name == "branchTrigger")
    {
      mainUI.SetActive(false);

      if(branchActive)
      {
        if(skyCam.depth == 1)
        {
          skyCam.depth *= -1;
          playerCam.depth *= -1;
        }
        altSelection = 0;
        playerLight.intensity = 0;
      }
      isActive = false;
      pressedYet = false;
      branchActive = false;
    }
  }
}
