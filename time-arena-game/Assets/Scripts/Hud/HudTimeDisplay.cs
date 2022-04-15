using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeDisplay : MonoBehaviour
{
    [SerializeField] private Text _text;
    private TimeLord _timeLord;

    void Start() { _text.text = "0:00"; }

    void LateUpdate()
    {
        int frame = _timeLord.GetCurrentFrame();
        float time = (float) frame / (float) Constants.FrameRate;
        float t = Constants.GameLength - time;
        int minutes = (int) (t / 60);
        int seconds = (int) (t % 60);
        _text.text = minutes.ToString() + ":" + seconds.ToString().PadLeft(2, '0');
    }

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }
}
