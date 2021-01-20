using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.P))
      {
        if(GameIsPaused)
        {
          Resume();
        }
        else
        {
          Pause();
        }
      }
    }

    public void Resume()
    {
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1.0f;
      GameIsPaused = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
      pauseMenuUI.SetActive(true);
      Time.timeScale = 0.0f;
      GameIsPaused = true;
      Cursor.lockState = CursorLockMode.Confined;
    }

    public void Quit()
    {
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1.0f;
      GameIsPaused = false;
      SceneManager.LoadScene("menuScene");
    }
}
