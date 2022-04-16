using UnityEngine;
using UnityEngine.UI;

public class HudTeam : MonoBehaviour
{
    [SerializeField] private Text _teamDisplay;
    [SerializeField] private PlayerController _player;

    public void Start()
    {
        switch (_player.Team)
        {
            case Constants.Team.Guardian: _teamDisplay.text = "GUARDIAN"; break;
            case Constants.Team.Miner: _teamDisplay.text = "MINER"; break;
            default: Debug.LogError("Invalid team"); break;
        }
    }
}
