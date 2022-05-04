using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveController : MonoBehaviour
{
    GameObject movingCam;
    [Tooltip("The camera to return to after the movement")]
    [SerializeField] Camera afterCam;
    [SerializeField] List<CameraMovement> moves;
    private List<int> starts;
    [SerializeField] private int startFrame;
    private int endFrame;
    private bool camIsTemporary;
    // Start is called before the first frame update
    void Start()
    {
        starts = new List<int>();
        startFrame += Time.frameCount;
        if(afterCam == null) afterCam = Camera.main;
        movingCam = new GameObject("Camera");
        movingCam.AddComponent<Camera>();
        movingCam.GetComponent<Camera>().enabled = false;
        starts.Add(0);
        for(int i = 0; i < moves.Count - 1; i++){
            starts.Add(starts[i] + moves[i].GetLength());
        }
        endFrame = startFrame + starts[starts.Count - 1] + moves[moves.Count - 1].GetLength();
    }

    // Update is called once per frame
    void Update()
    {
        int t = Time.frameCount;
        //Debug.Log(t + " -> " + endFrame.ToString() + " " + (t == startFrame).ToString());
        if(t > endFrame){return;}
        if(t < startFrame){return;}
        if(t == startFrame){
            Debug.Log("Starting Movement 0");
            afterCam.enabled = false;
            movingCam.GetComponent<Camera>().enabled = true;
        }
        if(t == endFrame){
            movingCam.GetComponent<Camera>().enabled = false;
            afterCam.enabled = true;
            Destroy(movingCam);
        }
        int midFrame;
        CameraMovement current;
        (midFrame, current) = GetCurrentMovement(t - startFrame);
        (movingCam.transform.position, movingCam.transform.rotation) = current.GetCameraAtFrame(midFrame);
    }

    (int, CameraMovement) GetCurrentMovement(int frame){
        int lastStart = 0;
        foreach(int s in starts){
            if(s <= frame) lastStart = s;
            if(s > frame) break;
        }
        return (frame - lastStart, moves[starts.IndexOf(lastStart)]);
    }
}
