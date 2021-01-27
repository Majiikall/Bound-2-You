using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class searchAlgorithm
{
    public void checkPathComplete(List<GameObject> objsInScene, int gridRows, Transform floorSize, Vector2[,] grid, int playerLocation)
    {
      
    }

    private Vector2 convertToCoord(int toConv, int gridSectionsPerRow)
    {
      float row = Mathf.Floor(toConv / gridSectionsPerRow);
      float column = toConv - (row * gridSectionsPerRow);

      return new Vector2((int)row, (int)column);
    }

}
