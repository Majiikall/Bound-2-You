using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class controlFlow : MonoBehaviour
{
    [SerializeField]
    [Range(5, 25)]
    public int gridSectionsPerRow;

    public GameObject levelPrefab;

    private float relativePos = 0.0f;

    private GameObject level1;
    private GameObject level2;
    private GameObject newLevel;
    private GameObject toDestroy;
    private GameObject prevLevel;

    private float destructionTime = 5.0f;
    private int destructionCount = 0;
    private int prevDestructionCount = 1;

    // Start is called before the first frame update
    void Start()
    {
      level1 = Instantiate(levelPrefab, new Vector3(0, 0, 0), Quaternion.identity, gameObject.GetComponent<Transform>());

      level1.GetComponent<populateMe>().populateLevel(gridSectionsPerRow, relativePos);

      toDestroy = level1;

      relativePos += 140.0f;

      level2 = Instantiate(levelPrefab, new Vector3(0, 0, relativePos), Quaternion.identity, gameObject.GetComponent<Transform>());

      level2.GetComponent<populateMe>().populateLevel(gridSectionsPerRow, relativePos);
    }

    // Update is called once per frame
    void Update()
    {
      if(destructionCount > prevDestructionCount)
      {
        newLevel = Instantiate(levelPrefab, new Vector3(0, 0, relativePos), Quaternion.identity, gameObject.GetComponent<Transform>());
        newLevel.GetComponent<populateMe>().populateLevel(gridSectionsPerRow + destructionCount, relativePos);

        toDestroy = prevLevel;
        prevLevel = newLevel;
        prevDestructionCount = destructionCount;
      }
    }

    private void OnTriggerExit(Collider other)
    {
      if(Time.realtimeSinceStartup > destructionTime)
      {
        if(destructionCount == 1)
        {
          Destroy(level2, 1.0f);
          destructionTime = Time.realtimeSinceStartup;
          prevDestructionCount = destructionCount;
          destructionCount++;
          relativePos += 140.0f;
        }
        else
        {
          Destroy(toDestroy, 1.0f);
          destructionTime = Time.realtimeSinceStartup;
          prevDestructionCount = destructionCount;
          destructionCount++;
          relativePos += 140.0f;
        }

        Debug.Log("New Difficulty Level: " + (gridSectionsPerRow + destructionCount));
      }
    }
}
