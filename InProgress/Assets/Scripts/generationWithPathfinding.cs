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
    private int currentScore = 0;
    private int currentHeight = 15;

    //Store previous position to avoid stacking objects
    private List<Vector2> prevPos = new List<Vector2>();

    private int offset = 95;

    private int heightOffset = 0;

    private static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public GameObject levelCompleteUI;

    public Component[] findPlayer;

    private GameObject floorTemp;

    //Populate the level and check to verify it is "complete"
    public void populateLevelAndCheck()
    {
      Debug.Log(currentScore);
      // Debug.Log(player.GetComponent<Transform>().position);
      // Start by spawning in the grid for the current level
      Transform floorTransform = floor.GetComponent<Transform>();

      // Set the scale to 20 x 20 using step 1.
      var floorScale = floorTransform.localScale;

      floorScale.x = gridSize * 2;
      floorScale.z = gridSize * 2;

      floorTransform.localScale = floorScale;

      // Make floor spawn and add it to list in case we need it later.
      floorTemp = Instantiate(floor, new Vector3(0, 0, 0), Quaternion.identity, gameObject.GetComponent<Transform>());

      objectsInScene.Add(floorTemp);

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

      var newPlayer = Instantiate(player, new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), Quaternion.identity, gameObject.GetComponent<Transform>());

      Destroy(player);

      player = newPlayer;

      //Center the floor under the player
      Transform floor2Transform = floorTemp.GetComponent<Transform>();
      posPlayer.y = 0f;
      floor2Transform.position = posPlayer;

      // Add in the first object where the player is on top of it.
      objectsInScene.Add(Instantiate(basicTree, new Vector3(playerTransform.position.x, currentHeight, playerTransform.position.z), Quaternion.identity, gameObject.GetComponent<Transform>()));

      //Use current score to increase difficulty of the level and number of objects being placed.
      int tempScore = currentScore + 5;

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
      int boundXLow = -20;
      int boundXHigh = 20;

      int boundZLow = -200;
      int boundZHigh = 200;

      //Used in loop to check valid spawn points
      Vector2 currentLocation = startingLocation;

      //Stores next direction we can't go and avoids it.
      int invalidLocation = 0;

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
          if(lastLastPlaced == moveBranchTree)
          {
            nextTree = 3;
          }
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

        List<Vector2> validCoords = new List<Vector2>();

        //Used to calculate the direction we moved and make sure we don't backtrack
        Vector2 tempLocation = new Vector2(0, 0);

        //The fun begins.
        //Calculate the current tree picked and then based off of previous trees
        //find new valid coordinates, pick one at random that doesn't overlap with previous.
        //Add object to scene. If the height has scaled with a climb tree we need to increment our Y location and Scale.
        //Update variables and location for next pass to avoid overlap and incomplete level creation.
        switch(nextTree)
        {
          //BASIC TREE
          case 1:
            //If the lastplaced is a moveBranch make sure to account for extra distance we can travel.
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true, invalidLocation);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false, invalidLocation);
            }

            int pickAPlace = validCoords.Count;

            pickAPlace = Random.Range(0, pickAPlace);

            Vector2 itemLocation = validCoords[pickAPlace];

            GameObject newObj = Instantiate(basicTree, new Vector3((itemLocation.x * 10) - offset, currentHeight, (itemLocation.y * 10) - offset),
                    Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj);

            if(heightOffset >= 1)
            {
              Transform newObjTransform = newObj.GetComponent<Transform>();

              var tempPosition = newObjTransform.position;

              tempPosition.y = tempPosition.y + (heightOffset * 5);

              var tempScale = newObjTransform.localScale;

              tempScale.y = tempPosition.y * 2;

              newObjTransform.localScale = tempScale;
              newObjTransform.position = tempPosition;
            }

            tempLocation = currentLocation;

            prevPos.Add(itemLocation);
            lastLastPlaced = lastPlaced;
            lastPlaced = basicTree;
            currentLocation = itemLocation;
            break;

          //MOVEBRANCHTREE
          case 2:
            //If a move branch is being placed then we only need to check the farthest distance
            validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true, invalidLocation);

            int pickAPlace2 = validCoords.Count;

            pickAPlace2 = Random.Range(0, pickAPlace2);

            Vector2 itemLocation2 = validCoords[pickAPlace2];

            GameObject newObj2 = Instantiate(moveBranchTree, new Vector3((itemLocation2.x * 10) - offset, currentHeight, (itemLocation2.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj2);

            if(heightOffset >= 1)
            {
              Transform newObjTransform2 = newObj2.GetComponent<Transform>();

              var tempPosition2 = newObjTransform2.position;

              tempPosition2.y = tempPosition2.y + (heightOffset * 5);

              var tempScale2 = newObjTransform2.localScale;

              tempScale2.y = tempPosition2.y * 2;

              newObjTransform2.localScale = tempScale2;
              newObjTransform2.position = tempPosition2;
            }

            tempLocation = currentLocation;

            prevPos.Add(itemLocation2);
            lastLastPlaced = lastPlaced;
            lastPlaced = moveBranchTree;
            currentLocation = itemLocation2;
            break;

          //CLIMBTREE
          case 3:
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true, invalidLocation);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false, invalidLocation);
            }

            int pickAPlace3 = validCoords.Count;

            pickAPlace3 = Random.Range(0, pickAPlace3);

            Vector2 itemLocation3 = validCoords[pickAPlace3];

            GameObject newObj3 = Instantiate(climbTree, new Vector3((itemLocation3.x * 10) - offset, currentHeight, (itemLocation3.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj3);

            if(heightOffset >= 1)
            {
              Transform newObjTransform3 = newObj3.GetComponent<Transform>();

              var tempPosition3 = newObjTransform3.position;

              tempPosition3.y = tempPosition3.y + (heightOffset * 5);

              var tempScale3 = newObjTransform3.localScale;

              tempScale3.y = tempPosition3.y * 2;

              newObjTransform3.localScale = tempScale3;
              newObjTransform3.position = tempPosition3;
            }

            tempLocation = currentLocation;

            heightOffset++;

            prevPos.Add(itemLocation3);
            lastLastPlaced = lastPlaced;
            lastPlaced = climbTree;
            currentLocation = itemLocation3;
            break;

          //ENDTREE
          case 4:
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true, invalidLocation);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false, invalidLocation);
            }

            int pickAPlace4 = validCoords.Count;

            pickAPlace4 = Random.Range(0, pickAPlace4);

            Vector2 itemLocation4 = validCoords[pickAPlace4];

            GameObject newObj4 = Instantiate(endTree, new Vector3((itemLocation4.x * 10) - offset, currentHeight, (itemLocation4.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj4);

            if(heightOffset >= 1)
            {
              Transform newObjTransform4 = newObj4.GetComponent<Transform>();

              var tempPosition4 = newObjTransform4.position;

              tempPosition4.y = tempPosition4.y + (heightOffset * 5);

              var tempScale4 = newObjTransform4.localScale;

              tempScale4.y = tempPosition4.y * 2;

              newObjTransform4.localScale = tempScale4;
              newObjTransform4.position = tempPosition4;
            }

            tempLocation = currentLocation;

            prevPos.Add(itemLocation4);
            lastLastPlaced = lastPlaced;
            lastPlaced = endTree;
            currentLocation = itemLocation4;
            break;
        }

        //After the first tree is placed make sure to figure out the next blocked direction
        tempLocation = currentLocation - tempLocation;
        int direction;

        //Based off of the largest value comparing the previous location and the new location
        //we can determine which direction not to spawn next iteration.
        if(Mathf.Abs(tempLocation.x) > Mathf.Abs(tempLocation.y))
        {
          direction = (int)tempLocation.x;

          if(direction > 0)
          {
            invalidLocation = 3;
          }
          else
          {
            invalidLocation = 1;
          }
        }
        else
        {
          direction = (int)tempLocation.y;
          if(direction > 0)
          {
            invalidLocation = 4;
          }
          else
          {
            invalidLocation = 2;
          }
        }

        tempScore--;
      }

    }

    //Based on all possible moves generate the locations for a new item
    public List<Vector2> generatePossibleMove(Vector2 currPoss, int xLow, int xHigh, int zLow, int zHigh, bool isMove, int invalidLocation)
    {
      List<Vector2> temp = new List<Vector2>();

      invalidLocation = 4;

      //Set variables based on the next item being placed. Need to account for proper space to jump.
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
      bool notBlocked = true;
      int blockedCount = 1;
      //It's only bad practice if it doesn't work haha ha ha :{
      while(true)
      {
        if(blockedCount == invalidLocation)
        {
          //Should be false
          notBlocked = true;
        }
        else
        {
          //Should be true
          notBlocked = false;
        }
        //Exit if we reach all spots checked
        if(overAll == total)
        {
          break;
        }
        //At halfway switch variables to reduce code
        if(overAll % (total / 2) == 0 && count > 0)
        {
          change--;
          int tempChange = change;
          change = step;
          step = tempChange * -1;
          change *= -1;
          flip = !flip;
          count = 0;
          blockedCount++;
        }
        //Switch x and z searching
        if(count % (total / 4) == 0 && overAll != (total / 2) && count > 0)
        {
          step--;
          int tempStep = step;
          step = change;
          change = tempStep * -1;
          flip = !flip;
          count = 0;
          blockedCount++;
        }
        if(notBlocked)
        {
          //Get the new locations based off the current location
          float tempX = currPoss.x + change;
          float tempZ = currPoss.y + step;

          //Check if valid X
          if(tempX > xLow && tempX < xHigh)
          {
            //Check if valid Y
            if(tempZ > zLow && tempZ < zHigh)
            {
              //Create coord if this far
              Vector2 tempCoordNew = new Vector2(tempX, tempZ);

              //Compare coord to make sure nothing placed here yet then add to possible locations of new piece
              if(!prevPos.Contains(tempCoordNew))
              {
                temp.Add(tempCoordNew);
              }
            }
          }
        }

        //For keeping single statements in lines above we use a flip to increment variables depending on how
        //far we are in the search for valid locations
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

      //Return all places we can go.
      return temp;
    }

    // Convert an integer to a (x, y) vector coordinate for starting location
    public Vector2 convertLocationToXY(int location)
    {
      float x = Mathf.Floor(location/gridSize);
      float y = Mathf.Floor(location - (x * gridSize));

      return new Vector2(x, y);
    }

    //RUN THE ENTIRE LEVEL
    void Start()
    {
      //Verify path finding
      this.populateLevelAndCheck();

      // Update currentScore to increase difficulty next iteration.
      currentScore += 1;
    }

    //Constantly run to find the pause indicator if paused
    void Update()
    {
      Transform currPlayerPlace = player.GetComponent<Transform>();

      Transform floorTransform = floorTemp.GetComponent<Transform>();

      var tempLocation = floorTransform.position;

      tempLocation.x = currPlayerPlace.position.x;
      tempLocation.z = currPlayerPlace.position.z;

      floorTransform.position = tempLocation;

      //Pause menu if needed
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

    //Restart the game after pause
    public void Resume()
    {
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1.0f;
      GameIsPaused = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    //Free cursor and stop time
    void Pause()
    {
      pauseMenuUI.SetActive(true);
      Time.timeScale = 0.0f;
      GameIsPaused = true;
      Cursor.lockState = CursorLockMode.Confined;
    }

    //Exit to main menu and return from action.
    public void Quit()
    {
      pauseMenuUI.SetActive(false);
      Time.timeScale = 1.0f;
      GameIsPaused = false;
      SceneManager.LoadScene("menuScene");
    }

    public void nextLevelComplete()
    {
      levelCompleteUI.SetActive(false);
      Time.timeScale = 1.0f;
      SceneManager.LoadScene("loadingScene");
    }

    // When leaving this scene store the current score for the level generation
    void OnDisable()
    {
      PlayerPrefs.SetInt("score", currentScore);
    }

    //On start get the current score.
    void OnEnable()
    {
      currentScore  =  PlayerPrefs.GetInt("score");
    }
}
