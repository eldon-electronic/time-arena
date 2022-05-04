using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]Vector3 startPos; //camera's initial position
    [SerializeField]Vector3 endPos; //camera's final position
    [SerializeField]Quaternion startRot; //camera's initial rotation
    [SerializeField]Quaternion endRot; //camera's final rotation
    [SerializeField]int frameLength; //length of movement in frames

    CameraMovement(Vector3 sp, Vector3 ep, Quaternion sr, Quaternion er, int length){
        startPos = sp;
        endPos = ep;
        startRot = sr;
        endRot = er;
        frameLength = length;
    }

    public (Vector3, Quaternion) GetCameraAtFrame(int frame){
        if(frame > frameLength) return (endPos, endRot);
        if(frame < 0) return (startPos, startRot);
        float percentage = (float)frame / (float)frameLength;
        return (Vector3.Lerp(startPos, endPos, percentage), Quaternion.Lerp(startRot, endRot, percentage));
    }

    public int GetLength(){
        return frameLength;
    }
}