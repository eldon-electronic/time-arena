using UnityEngine;
using UnityEngine.UI;

public class HudTeam : MonoBehaviour
{
    [SerializeField] private Text _teamDisplay;

    public void SetTeam(Constants.Team team)
    {
        switch (team)
        {
            case Constants.Team.Guardian: _teamDisplay.text = "GUARDIAN"; break;
            case Constants.Team.Miner: _teamDisplay.text = "MINER"; break;
            default: Debug.LogError("Invalid team"); break;
        }
    }
}
