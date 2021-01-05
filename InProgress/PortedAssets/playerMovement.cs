using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class playerMovement : MonoBehaviour
{
  [SerializeField]
  public float speed;

  private Vector3 playerVelocity;
  private bool groundedPlayer;
  private float playerSpeed = 2.0f;
  private float jumpHeight = 3.0f;
  private float gravityValue = -9.81f;

  public Camera c1;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    var transform = this.GetComponent<Transform>();
    var oldPos = transform.localPosition;
    if (Input.GetKey(KeyCode.A)){
      transform.Rotate(-Vector3.up * speed * 17 * Time.deltaTime);
    }

    if (Input.GetKey(KeyCode.D)){
      transform.Rotate(Vector3.up * speed * 17 * Time.deltaTime);
    }

    Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    input = Vector2.ClampMagnitude(input, 1);

    var camTransform = c1.GetComponent<Transform>();
    Vector3 camF = camTransform.forward;
    Vector3 camR = camTransform.right;

    camF.y = 0;
    camR.y = 0;

    camF = camF.normalized;
    camR = camR.normalized;

    var newPos = (camF*input.y + camR*input.x)*speed;

    var characterController = this.GetComponent<CharacterController>();
    characterController.SimpleMove(newPos);

    groundedPlayer = characterController.isGrounded;
    if (groundedPlayer && playerVelocity.y < 0)
    {
        playerVelocity.y = 0f;
    }

    if (Input.GetButtonDown("Jump") && groundedPlayer)
    {
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    playerVelocity.y += gravityValue * Time.deltaTime;
    characterController.Move(playerVelocity * Time.deltaTime);

    var newPoss = transform.localPosition;

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
