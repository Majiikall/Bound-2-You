using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
  public void PlayGame()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void QuitGame()
  {
    Debug.Log("QUIT functioning");
    Application.Quit();
  }

  public void TestGame()
  {
    SceneManager.LoadScene("idealScene");
  }

  void OnDisable()
  {
    PlayerPrefs.SetInt("score", 1);
  }
}
