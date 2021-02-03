using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class load : MonoBehaviour
{
  void Start()
  {
      //Start the coroutine we define below named ExampleCoroutine.
      StartCoroutine(ExampleCoroutine());
  }

  IEnumerator ExampleCoroutine()
  {
      //yield on a new YieldInstruction that waits for 5 seconds.
      yield return new WaitForSeconds(5);

      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }
}
