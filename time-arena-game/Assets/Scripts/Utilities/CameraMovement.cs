using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] protected Vector3 startPos; //camera's initial position
    [SerializeField] protected Vector3 endPos; //camera's final position
    [SerializeField] protected Quaternion startRot; //camera's initial rotation
    [SerializeField] protected Quaternion endRot; //camera's final rotation
    [SerializeField] protected int frameLength; //length of movement in frames

    public virtual (Vector3, Quaternion) GetCameraAtFrame(int frame){
        if(frame > frameLength) return (endPos, endRot);
        if(frame < 0) return (startPos, startRot);
        float percentage = (float)frame / (float)frameLength;
        return (Vector3.Lerp(startPos, endPos, percentage), Quaternion.Lerp(startRot, endRot, percentage));
    }

    public int GetLength(){
        return frameLength;
    }
}