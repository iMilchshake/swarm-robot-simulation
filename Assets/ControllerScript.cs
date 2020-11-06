using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    public static ControllerScript ctrlScript;
    public int fps;
    public float communicateRange;

    void Start()
    {
        //make this single instance of ControllerScript accessible to everyone
        ctrlScript = this;

        //set target fps
        Application.targetFrameRate = fps;
    }
}
