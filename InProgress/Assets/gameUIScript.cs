using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class gameUIScript : MonoBehaviour
{
    public Text text;

    private string[] displayOptions = {"B", "Y"};

    private int curr = 0;

    public TextMeshProUGUI timeToNight;
    private int step = 1;

    void start()
    {

    }

    void Update()
    {
      if(Time.time / 5 > step)
      {
        Color32[] temp = timeToNight.textInfo.meshInfo[0].colors32;
        temp[0] = new Color(1, 1, 1, 1);
        timeToNight.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        step += 1;
      }
      if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
      {
        if(curr == 0)
        {
          curr = 1;
        }
        else
        {
          curr = 0;
        }
        this.text.text = displayOptions[curr];
      }
    }


}
