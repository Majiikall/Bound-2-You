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

    //Score, height and temp for proper placement
    private int currentScore = 1;
    private int currentHeight = 15;
    private int scoreForTesting = 1;

    //Store previous position to avoid stacking objects
    private List<Vector2> prevPos = new List<Vector2>();

    //Populate the level and check to verify it is "complete"
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

      // Pass in a random start location for the generation
      int convPassIn = Random.Range(0, gridSize * gridSize);

      Vector2 startingLocation = convertLocationToXY(convPassIn);

      Transform playerTransform = player.GetComponent<Transform>();

      // Reposition the player for the start of the game
      var posPlayer = playerTransform.position;
      posPlayer.x = (-100 + (startingLocation.x * 10)) + 5;
      posPlayer.z = (-100 + (startingLocation.y * 10)) + 5;
      posPlayer.y = 30f;
      playerTransform.position = posPlayer;

      // Add in the first object where the player is on top of it.
      objectsInScene.Add(Instantiate(basicTree, new Vector3(playerTransform.position.x, currentHeight, playerTransform.position.z), Quaternion.identity, gameObject.GetComponent<Transform>()));

      //Use current score to increase difficulty of the level and number of objects being placed.
      int tempScore = scoreForTesting * 5;

      if(tempScore > 25)
      {
        tempScore = 25;
      }

      //Set previous tree location
      prevPos.Add(startingLocation);

      //Define variables for generation.

      //What was the last placed object this will weigh into our selection process
      GameObject lastPlaced = basicTree;
      GameObject lastLastPlaced = null;

      //Set bounds for the available grid space
      int boundXLow = 0;
      int boundXHigh = 20;

      int boundZLow = 0;
      int boundZHigh = 20;

      Vector2 currentLocation = startingLocation;

      while(tempScore != 0)
      {
        //Pick next tree
        int nextTree = 0;

        //Limit selection to favor basicTree but make sure special trees are used
        //Special trees increase in amount as difficulty increases.
        if(lastPlaced == moveBranchTree || lastPlaced == climbTree)
        {
          nextTree = 1;
        }
        else if(lastPlaced == basicTree && lastLastPlaced != basicTree)
        {
          nextTree = Random.Range(1, 4);
        }
        else
        {
          if(tempScore % 2 == 0)
          {
            nextTree = 2;
          }
          else
          {
            nextTree = 3;
          }
        }

        //Make sure we end the level properly with the tree to switch states and continue the game
        if(tempScore == 1)
        {
          nextTree = 4;
        }

        //The fun begins.
        //Calculate the current tree picked and then based off of previous trees
        //find new valid coordinates, pick one at random that doesn't overlap with previous.
        //then continue on to the next item.
        switch(nextTree)
        {
          case 1:
            Debug.Log("BasicTree");
            List<Vector2> validCoords = new List<Vector2>();
            if(lastPlaced == moveBranchTree)
            {
              Debug.Log("Here with move");
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true);

              Debug.Log(validCoords.Count);
              foreach(Vector2 x in validCoords)
              {
                Instantiate(basicTree, new Vector3((x.x * 10) -95, currentHeight, (x.y * 10) -95), Quaternion.identity, gameObject.GetComponent<Transform>());
              }
            }
            else
            {
              Debug.Log("Here");
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false);

              foreach(Vector2 x in validCoords)
              {
                Instantiate(basicTree, new Vector3((x.x * 10) -95, currentHeight, (x.y * 10) -95), Quaternion.identity, gameObject.GetComponent<Transform>());
              }
            }

            lastLastPlaced = lastPlaced;
            lastPlaced = basicTree;
            tempScore = 1;
            break;
          case 2:
            //Debug.Log("moveBranchTree");


            lastLastPlaced = lastPlaced;
            lastPlaced = moveBranchTree;
            break;
          case 3:
            //Debug.Log("climbTree");


            lastLastPlaced = lastPlaced;
            lastPlaced = climbTree;
            break;
          case 4:
            //Debug.Log("End Tree");


            lastLastPlaced = lastPlaced;
            lastPlaced = endTree;
            break;
        }

        tempScore--;
        continue;
      }

    }

    //Based on all possible moves generate the locations for a new item
    public List<Vector2> generatePossibleMove(Vector2 currPoss, int xLow, int xHigh, int zLow, int zHigh, bool isMove)
    {
      List<Vector2> temp = new List<Vector2>();

      int count, change, step, total;
      if(isMove == true)
      {
        change = 3;
        step = -2;
        total = 20;
      }
      else
      {
        change = 2;
        step = -1;
        total = 12;
      }

      count = 0;
      int overAll = 0;
      bool flip = false;
      //It's only bad practice if it doesn't work haha ha ha :{
      while(true)
      {
        if(overAll == total)
        {
          break;
        }
        if(overAll % (total / 2) == 0 && count > 0)
        {
          change--;
          int tempChange = change;
          change = step;
          step = tempChange * -1;
          change *= -1;
          flip = !flip;
          count = 0;
        }
        if(count % (total / 4) == 0 && overAll != (total / 2) && count > 0)
        {
          step--;
          int tempStep = step;
          step = change;
          change = tempStep * -1;
          flip = !flip;
          count = 0;
        }
        float tempX = currPoss.x + change;
        float tempZ = currPoss.y + step;

        if(tempX >= xLow && tempX <= xHigh)
        {
          if(tempZ >= zLow && tempZ <= zHigh)
          {
            Vector2 tempCoordNew = new Vector2(tempX, tempZ);

            if(!prevPos.Contains(tempCoordNew))
            {
              temp.Add(tempCoordNew);
            }
          }
        }

        if(flip)
        {
          change++;
        }
        else
        {
          step++;
        }

        count++;
        overAll++;
      }

      return temp;
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
