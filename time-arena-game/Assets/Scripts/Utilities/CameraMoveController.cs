using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveController : MonoBehaviour
{
    GameObject movingCam;
    [SerializeField] GameObject player;
    [Tooltip("The camera to return to after the movement")]
    [SerializeField] Camera afterCam;
    [SerializeField] List<CameraMovement> moves;
    private List<int> starts;
    [SerializeField] private int startFrame;
    private int endFrame;
    [SerializeField] private bool fromPlayer = true;
    [SerializeField] private float distance = 3;
    // Start is called before the first frame update
    void Start()
    {
        if(moves == null) moves = new List<CameraMovement>();
        if(fromPlayer) moves.Add(CreatePlayerTrack());
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

    BezierMovment CreatePlayerTrack(){
        Vector3 bp = new Vector3(player.transform.position.x, player.transform.position.y + 5, player.transform.position.z);
        Vector3 r = player.transform.rotation.eulerAngles + new Vector3(40, 0, 0);
        return new BezierMovment(new Vector3(0, player.transform.position.y + 8, 0) + player.transform.position / distance, afterCam.transform.position, Quaternion.Euler(r), player.transform.rotation, 300, bp);
    }
}
