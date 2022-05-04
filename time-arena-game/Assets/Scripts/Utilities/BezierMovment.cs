using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovment : CameraMovement
{
    [SerializeField]Vector3 bPoint;

    public override (Vector3, Quaternion) GetCameraAtFrame(int frame){
        if(frame > frameLength) return (endPos, endRot);
        if(frame < 0) return (startPos, startRot);
        float p = (float)frame / (float)frameLength;
        Vector3 a = (1 - p)*((1-p)*startPos + p*bPoint);
        Vector3 b = p*((1-p)*bPoint + p*endPos);
        return (a + b, Quaternion.Lerp(startRot, endRot, p));
    }
}
