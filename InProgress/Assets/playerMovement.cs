using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    Debug.Log(isGrounded);
    Debug.Log(velocity.y);
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
    // if (Input.GetKey(KeyCode.A)){
    //   transform.Rotate(-Vector3.up * speed * 17 * Time.deltaTime);
    // }
    //
    // if (Input.GetKey(KeyCode.D)){
    //   transform.Rotate(Vector3.up * speed * 17 * Time.deltaTime);
    // }
    //
    // Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    // input = Vector2.ClampMagnitude(input, 1);
    //
    // var camTransform = c1.GetComponent<Transform>();
    // Vector3 camF = camTransform.forward;
    // Vector3 camR = camTransform.right;
    //
    // camF.y = 0;
    // camR.y = 0;
    //
    // camF = camF.normalized;
    // camR = camR.normalized;
    //
    // var newPos = (camF*input.y + camR*input.x)*speed;
    //
    // var characterController = this.GetComponent<CharacterController>();
    // characterController.SimpleMove(newPos);
    //
    // groundedPlayer = characterController.isGrounded;
    // if (groundedPlayer && playerVelocity.y < 0)
    // {
    //     playerVelocity.y = 0f;
    // }
    //
    // if (Input.GetButtonDown("Jump") && groundedPlayer)
    // {
    //     playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    // }
    //
    // playerVelocity.y += gravityValue * Time.deltaTime;
    // characterController.Move(playerVelocity * Time.deltaTime);
    //
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
}
