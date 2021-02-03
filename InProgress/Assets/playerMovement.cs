using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
  }

  private void OnTriggerEnter(Collider other)
  {
    levelCompleteUI.SetActive(true);
    Time.timeScale = 0.0f;
    Cursor.lockState = CursorLockMode.Confined;
  }
}
