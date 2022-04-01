using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameController : MonoBehaviour
{
    private TimeLord _timeLord;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FrameRate;

        int totalFrames = Constants.FrameRate * 60 * 2;
        _timeLord = new TimeLord(totalFrames);
    }

    void Update()
    {
        _timeLord.Tick();
    }

    public void Register(PlayerController pc)
    {
        pc.SetTimeLord(_timeLord);
    }
}
