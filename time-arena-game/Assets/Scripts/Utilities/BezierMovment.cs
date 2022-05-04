using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovment : CameraMovement
{
    [SerializeField]Vector3 bPoint;
    
    public BezierMovment(Vector3 sp, Vector3 ep, Quaternion sr, Quaternion er, int length, Vector3 bp) : base(sp, ep, sr, er, length){
        bPoint = bp;
    }

    public override (Vector3, Quaternion) GetCameraAtFrame(int frame){
        if(frame > frameLength) return (endPos, endRot);
        if(frame < 0) return (startPos, startRot);
        float p = (float)frame / (float)frameLength;
        Vector3 a = (1 - p) * ((1 - p) * startPos + p * bPoint) + p * ((1-p) * bPoint + p * endPos);
        return (a, Quaternion.Lerp(startRot, endRot, p));
    }
}
