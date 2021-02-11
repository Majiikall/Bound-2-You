using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class testPlayer : MonoBehaviour
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

  // NEW //

  public int scalingFrames = 0;
  public float nextValueScale = 0.0f;
  public float nextValuePos = 0.0f;

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
      //Add in the code for the UI to change to the tree. Signaling
      //that we can now connect to the trees.

      //Check for user input to grow
      if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
      {
        GameObject parent = other.gameObject.transform.parent.gameObject;
        nextValueScale = parent.GetComponent<Transform>().localScale.y + 10.0f;
        nextValuePos = parent.GetComponent<Transform>().position.y + 5.0f;
        scalingFrames = 75;
        Debug.Log(scalingFrames);
        Debug.Log("registered input");
      }

      if(scalingFrames > 0)
      {
        GameObject parent = other.gameObject.transform.parent.gameObject;
        Transform objectTransform = parent.GetComponent<Transform>();

        var tempScale = objectTransform.localScale;
        var tempPos = objectTransform.position;

        tempScale.y = Mathf.Lerp(objectTransform.localScale.y, nextValueScale, 1.0f / scalingFrames);
        tempPos.y = Mathf.Lerp(objectTransform.position.y, nextValuePos, 1.0f / scalingFrames);

        objectTransform.localScale = tempScale;
        objectTransform.position = tempPos;
        scalingFrames--;
      }

    }

    private void OnTriggerExit(Collider other)
    {
      Debug.Log("Trigger over");
    }
}
