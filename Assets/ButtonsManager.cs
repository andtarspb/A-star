using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    [SerializeField]
    Image modeImg;

    PathFinding pf;

    private void Start()
    {
        pf = FindObjectOfType < PathFinding>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ButtonObstacles();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ButtonEraser();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ButtonStart();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ButtonEnd();
        }
    }

    public void ButtonObstacles()
    {
        pf.mode = "obstacle";
        modeImg.color = Color.black;
    }

    public void ButtonEraser()
    {
        pf.mode = "eraser";
        modeImg.color = Color.white;
    }

    public void ButtonStart()
    {
        pf.mode = "start";
        modeImg.color = Color.green;
    }

    public void ButtonEnd()
    {
        pf.mode = "end";
        modeImg.color = Color.blue;
    }
}
