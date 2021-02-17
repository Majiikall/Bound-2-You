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

    private GameObject parent;

    private bool pressedYet = false;

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
        if(Input.GetButtonDown("Submit"))
        {
            pressedYet = true;
        }
        if (pressedYet && isActive)
        {
            runIt();
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

    private void OnTriggerEnter(Collider other)
  {
        if(other.name != "growerTrigger")
        {
            levelCompleteUI.SetActive(true);
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if(other.name == "growerTrigger")
        {
            mainUI.SetActive(true);
            isActive = true;
            parent = other.gameObject.transform.parent.gameObject;
            nextValueScale = parent.transform.parent.GetComponent<Transform>().localScale.y + 10.0f;
            nextValuePos = parent.transform.parent.GetComponent<Transform>().position.y + 5.0f;
            scalingFrames = 10 / Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "growerTrigger")
        {
            mainUI.SetActive(false);
            isActive = false;
            pressedYet = false;
        }
    }
}
