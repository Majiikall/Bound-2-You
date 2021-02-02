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

    private int offset = 95;

    private int heightOffset = 0;

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

      Debug.Log(tempScore);

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

        List<Vector2> validCoords = new List<Vector2>();

        //The fun begins.
        //Calculate the current tree picked and then based off of previous trees
        //find new valid coordinates, pick one at random that doesn't overlap with previous.
        //then continue on to the next item.
        switch(nextTree)
        {
          case 1:
            //Debug.Log("basicTree");
            //If the lastplaced is a moveBranch make sure to account for extra distance we can travel.
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false);
            }

            int pickAPlace = validCoords.Count;

            pickAPlace = Random.Range(0, pickAPlace);

            Vector2 itemLocation = validCoords[pickAPlace];

            GameObject newObj = Instantiate(basicTree, new Vector3((itemLocation.x * 10) - offset, currentHeight, (itemLocation.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj);

            if(heightOffset >= 1)
            {
              Transform newObjTransform = newObj.GetComponent<Transform>();

              var tempScale = newObjTransform.localScale;

              tempScale.y = tempScale.y + (heightOffset * 5);

              var tempPosition = newObjTransform.position;

              tempPosition.y = tempPosition.y + (heightOffset * 5);

              newObjTransform.localScale = tempScale;
              newObjTransform.position = tempPosition;
            }

            prevPos.Add(itemLocation);
            lastLastPlaced = lastPlaced;
            lastPlaced = basicTree;
            currentLocation = itemLocation;
            break;

          case 2:
            //Debug.Log("moveBranchTree");
            //If a move branch is being placed then we only need to check the farthest distance
            validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true);

            int pickAPlace2 = validCoords.Count;

            pickAPlace2 = Random.Range(0, pickAPlace2);

            Vector2 itemLocation2 = validCoords[pickAPlace2];

            GameObject newObj2 = Instantiate(moveBranchTree, new Vector3((itemLocation2.x * 10) - offset, currentHeight, (itemLocation2.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj2);

            if(heightOffset >= 1)
            {
              Transform newObjTransform2 = newObj2.GetComponent<Transform>();

              var tempScale2 = newObjTransform2.localScale;

              tempScale2.y = tempScale2.y + (heightOffset * 5);

              var tempPosition2 = newObjTransform2.position;

              tempPosition2.y = tempPosition2.y + (heightOffset * 5);

              newObjTransform2.localScale = tempScale2;
              newObjTransform2.position = tempPosition2;
            }

            prevPos.Add(itemLocation2);
            lastLastPlaced = lastPlaced;
            lastPlaced = moveBranchTree;
            currentLocation = itemLocation2;
            break;

          case 3:
            //Debug.Log("climbTree");
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false);
            }

            int pickAPlace3 = validCoords.Count;

            pickAPlace3 = Random.Range(0, pickAPlace3);

            Vector2 itemLocation3 = validCoords[pickAPlace3];

            GameObject newObj3 = Instantiate(climbTree, new Vector3((itemLocation3.x * 10) - offset, currentHeight, (itemLocation3.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj3);

            if(heightOffset >= 1)
            {
              Transform newObjTransform3 = newObj3.GetComponent<Transform>();

              var tempScale3 = newObjTransform3.localScale;

              tempScale3.y = tempScale3.y + (heightOffset * 5);

              var tempPosition3 = newObjTransform3.position;

              tempPosition3.y = tempPosition3.y + (heightOffset * 5);

              newObjTransform3.localScale = tempScale3;
              newObjTransform3.position = tempPosition3;
            }

            heightOffset++;

            prevPos.Add(itemLocation3);
            lastLastPlaced = lastPlaced;
            lastPlaced = climbTree;
            currentLocation = itemLocation3;
            break;

          case 4:
            //Debug.Log("End Tree");
            if(lastPlaced == moveBranchTree)
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, true);
            }
            else
            {
              validCoords = generatePossibleMove(currentLocation, boundXLow, boundXHigh, boundZLow, boundZHigh, false);
            }

            int pickAPlace4 = validCoords.Count;

            pickAPlace4 = Random.Range(0, pickAPlace4);

            Vector2 itemLocation4 = validCoords[pickAPlace4];

            GameObject newObj4 = Instantiate(endTree, new Vector3((itemLocation4.x * 10) - offset, currentHeight, (itemLocation4.y * 10) - offset), Quaternion.identity, gameObject.GetComponent<Transform>());
            objectsInScene.Add(newObj4);

            if(heightOffset >= 1)
            {
              Transform newObjTransform4 = newObj4.GetComponent<Transform>();

              var tempScale4 = newObjTransform4.localScale;

              tempScale4.y = tempScale4.y + (heightOffset * 5);

              var tempPosition4 = newObjTransform4.position;

              tempPosition4.y = tempPosition4.y + (heightOffset * 5);

              newObjTransform4.localScale = tempScale4;
              newObjTransform4.position = tempPosition4;
            }

            prevPos.Add(itemLocation4);
            lastLastPlaced = lastPlaced;
            lastPlaced = endTree;
            currentLocation = itemLocation4;
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
      //It's only bad practice if it doesn't work haha ha ha :{
      while(true)
      {
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
        }
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
