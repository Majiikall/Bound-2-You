using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class generationWithPathfinding : MonoBehaviour
{
    // The floor of the scene
    public GameObject floor;

    // Tree obstacles to spawn
    public GameObject basicTree;
    public GameObject endTree;
    public GameObject moveBranchTree;
    public GameObject climbTree;

    // Set player location
    public GameObject player;

    // Used to constrain grid for spawning
    private int gridSize = 20;

    // Can access what is in the scene
    private List<GameObject> objectsInScene = new List<GameObject>();

    private int currentScore = 1;
    private int currentHeight = 15;

    public void populateLevelAndCheck()
    {
      // Start by spawning in the grid for the current level
      Transform floorTransform = floor.GetComponent<Transform>();

      // Set the scale to 20 x 20 using step 1.
      var floorScale = floorTransform.localScale;

      floorScale.x = gridSize;
      floorScale.z = gridSize;

      floorTransform.localScale = floorScale;

      // Make floor spawn and add it to list in case we need it later.
      objectsInScene.Add(Instantiate(floor, new Vector3(0, 0, 0), Quaternion.identity, gameObject.GetComponent<Transform>()));

      int convPassIn = Random.Range(0, gridSize * gridSize);

      Vector2 startingLocation = convertLocationToXY(convPassIn);

      Transform playerTransform = player.GetComponent<Transform>();

      var posPlayer = playerTransform.position;
      posPlayer.x = (-100 + (startingLocation.x * 10)) + 5;
      posPlayer.z = (-100 + (startingLocation.y * 10)) + 5;
      posPlayer.y = 30f;
      playerTransform.position = posPlayer;

      objectsInScene.Add(Instantiate(basicTree, new Vector3(playerTransform.position.x, currentHeight, playerTransform.position.z), Quaternion.identity, gameObject.GetComponent<Transform>()));

    }

    // Convert an integer to a (x, y) vector coordinate for starting location
    public Vector2 convertLocationToXY(int location)
    {
      float x = Mathf.Floor(location/gridSize);
      float y = Mathf.Floor(location - (x * gridSize));

      return new Vector2(x, y);
    }

    void Start()
    {
      //Verify path finding
      this.populateLevelAndCheck();

      // Update currentScore to increase difficulty next iteration.
      currentScore += 1;
    }

    //TESTING
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.T))
      {
        Quit();
      }
    }

    // Exit scene for TESTING
    public void Quit()
    {
      Cursor.lockState = CursorLockMode.Confined;
      SceneManager.LoadScene("menuScene");
    }


    // Store score for multiple iterations of the game. Reset if return to menu
    void OnDisable()
    {
      PlayerPrefs.SetInt("score", currentScore);
    }

    void OnEnable()
    {
      currentScore  =  PlayerPrefs.GetInt("score");
    }
}
