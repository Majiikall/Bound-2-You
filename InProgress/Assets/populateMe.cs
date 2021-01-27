using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class populateMe : MonoBehaviour
{
    public GameObject levelLayout;
    private List<Transform> wallLocations = new List<Transform>();
    public GameObject obstacle;
    public GameObject enemy;

    private bool enemies = false;
    private bool obstacles = false;

    private bool prevObstacle = false;
    private GameObject prevEnemy;
    private int enemyCount = 0;

    private int currGridSize = 0;

    private List<GameObject> objectsInScene = new List<GameObject>();

    public GameObject player;

    // Start is called before the first frame update
    public void populateLevel(int gridSectionsPerRow, float relativeLevelPos)
    {
      currGridSize = gridSectionsPerRow * gridSectionsPerRow;

      Component[] features;
      features = levelLayout.GetComponentsInChildren<Transform>();

      foreach(Transform x in features)
      {
        if(x.name != "baseLevel")
        {
          wallLocations.Add(x);
        }
      }

      Transform floorSize = wallLocations[0];

      Transform obstacleTransform = obstacle.GetComponent<Transform>();

      var newScale = obstacleTransform.localScale;

      newScale.x = ((floorSize.localScale.x * 10)/gridSectionsPerRow);
      newScale.z = ((floorSize.localScale.z * 10)/gridSectionsPerRow);

      obstacleTransform.localScale = newScale;

      Vector2[,] grid = generateGrid(floorSize.localScale.x * 10, floorSize.localScale.z * 10, relativeLevelPos, gridSectionsPerRow);

      List<int> locations = new List<int>();

      for(int i = 1; i < currGridSize + 1; i++)
      {
        locations.Add(i);
      }

      int count = 1;
      // var objectTransform = cubeTest.GetComponent<Transform>();
      while(!enemies || !obstacles)
      {
        int currRan = Random.Range(0, locations.Count);
        while(currRan > currGridSize - gridSectionsPerRow || !locations.Contains(currRan))
        {
          currRan = Random.Range(0, locations.Count);
        }
        GameObject instantObj = gatherAssets();

        if(count == Mathf.Floor(currGridSize * 0.25f))
        {
          enemies = true;
          obstacles = true;
        }
        count = count + 1;
        Vector2 index = convertToCoord(currRan, gridSectionsPerRow);

        prevObstacle = (instantObj == obstacle);
        if(instantObj == enemy)
        {
          prevEnemy = instantObj;
        }

        locations.Remove(currRan);
        Vector2 positionsNew = grid[(int)index.x, (int)index.y];
        objectsInScene.Add(Instantiate(instantObj, new Vector3(positionsNew.x, 0, positionsNew.y), Quaternion.identity, gameObject.GetComponent<Transform>()));
      }

      searchAlgorithm search = new searchAlgorithm();

      // Remove the enemies in the scene
      for(int x = 0; x < objectsInScene.Count; ++x)
      {
         GameObject tempObj = objectsInScene[x];
         if(tempObj.name == "Sphere (1)(Clone)")
         {
           objectsInScene.Remove(tempObj);
         }
      }
      int currentLocation = findGridSpot(player);

      search.checkPathComplete(objectsInScene, gridSectionsPerRow, floorSize, grid, currentLocation);
    }

    private int findGridSpot(GameObject player)
    {
      Transform play = player.GetComponent<Transform>();
      // if(player.GetComponent<Transform>().positon.x == 0)
      // {
      //   return 0;
      // }
      // else if(player.GetComponent<Transform>().positon.x > 1)
      // {
      //   return 1;
      // }
      // else
      // {
      //   return -1;
      // }
      return 0;
    }

    private Vector2 convertToCoord(int toConv, int gridSectionsPerRow)
    {
      float row = Mathf.Floor(toConv / gridSectionsPerRow);
      float column = toConv - (row * gridSectionsPerRow);

      return new Vector2((int)row, (int)column);
    }

    private GameObject gatherAssets()
    {
      float prob = 100.0f;

      if(prevObstacle == true)
      {
        prob = prob * Random.Range(0.90f, 0.95f);
      }
      if(prevEnemy == enemy)
      {
        prob = prob * Random.Range(0.00f, 0.30f);
      }

      prob = prob - (((float)enemyCount/4.5f) * 100);
      if(prob < 0.0f)
      {
        prob = 0.0f;
      }

      bool finalChoice = (prob > Random.Range(-1, 100.0f));

      prevEnemy = null;
      prevObstacle = false;

      if(finalChoice == true)
      {
        enemyCount = enemyCount + 1;
        return enemy;
      }
      return obstacle;
    }

    private Vector2[,] generateGrid(float x, float y, float floorZ, int gridSectionsPerRow)
    {
      Vector2[,] toRet = new Vector2[gridSectionsPerRow, gridSectionsPerRow];

      Vector2 startPoint = new Vector2((-x / 2), floorZ + (y / 2));

      float stepX = (x / gridSectionsPerRow);
      float stepY = (y / gridSectionsPerRow);
      float currX = startPoint.x + (stepX / 2);
      float currY = startPoint.y - (stepY / 2);

      for(int i = 0; i < gridSectionsPerRow; i++)
      {
        float tempX = currX;

        for(int z = 0; z < gridSectionsPerRow; ++z)
        {
          toRet[i,z] = new Vector2(tempX, currY);
          tempX += stepX;
        }
        currY -= stepY;
      }
      return toRet;
    }

    public List<GameObject> returnSceneObjs()
    {
      return this.objectsInScene;
    }
}
