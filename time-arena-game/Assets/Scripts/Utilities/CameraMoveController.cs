using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveController : MonoBehaviour
{
    [SerializeField] GameObject cameraPrefab;
    [SerializeField] float d1;
    [SerializeField] float d2;
    [SerializeField] float d3;
    [SerializeField] int w1;
    [SerializeField] int w2;
    [SerializeField] int w3;
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
        if(fromPlayer) moves = CreatePlayerTrack();
        starts = new List<int>();
        startFrame += Time.frameCount;
        if(afterCam == null) afterCam = Camera.main;
        movingCam = Instantiate(cameraPrefab);
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
            afterCam.transform.rotation = movingCam.transform.rotation;
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

    List<CameraMovement> CreatePlayerTrack(){
        List<CameraMovement> o = new List<CameraMovement>();
        Vector3 r = player.transform.rotation.eulerAngles + new Vector3(40, 0, 0);
        Vector3 istart = new Vector3(0, player.transform.position.y + 8, 0) + player.transform.position / distance;
        Vector3 cstart = istart;
        Vector3 stop = istart + ((afterCam.transform.position - istart) * d1);
        Vector3 bp = (cstart + stop) / 2;
        o.Add(new BezierMovment(istart, stop, Quaternion.Euler(r), Quaternion.Euler(r), 100, bp));
        o.Add(new CameraMovement(stop, stop, Quaternion.Euler(r), Quaternion.Euler(r), w1));
        cstart = stop;
        stop = istart + ((afterCam.transform.position - istart) * d2);
        bp = (cstart + stop) / 2;
        o.Add(new BezierMovment(cstart, stop, Quaternion.Euler(r), Quaternion.Euler(r), 100, bp));
        o.Add(new CameraMovement(stop, stop, Quaternion.Euler(r), Quaternion.Euler(r), w2));
        cstart = stop;
        stop = istart + ((afterCam.transform.position - istart) * d3);
        bp = (cstart + stop) / 2;
        o.Add(new BezierMovment(cstart, stop, Quaternion.Euler(r), afterCam.transform.rotation, 100, bp));
        o.Add(new CameraMovement(stop, stop, afterCam.transform.rotation, afterCam.transform.rotation, w3));
        return o;
    }
}
