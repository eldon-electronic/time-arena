using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class WinScreenController : MonoBehaviour
{

    public static WinScreenController Instance;

    [SerializeField] private Transform _statsContainer;
    [SerializeField] private GameObject _statListItem;
    [SerializeField] private TMP_Text _winText;
    private Dictionary<int, PlayerMinerController> _miners;
	private Dictionary<int, PlayerGuardianController> _guardians;
    private SceneController _sceneController;

    class Statistics
    {
        public string Nickname;
        public Constants.Team Team;
        public int Score;
    }

    void OnEnable() 
    {
        GameController.gameEnded += OnGameEnded;
    }

    void OnDisable()
    {
        GameController.gameEnded -= OnGameEnded;
    }

    void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    void Start() {
        
    }

    private void OnGameEnded(Constants.Team winningTeam) {
        _sceneController = FindObjectOfType<SceneController>();
        _miners = _sceneController.GetMinerControllers();
        _guardians = _sceneController.GetGuardianControllers();
        SetWinText(winningTeam);
        List<Statistics> playerStats = GetPlayerStats();
        foreach (var playerStat in playerStats) {
            Instantiate(_statListItem, _statsContainer)
                .GetComponent<StatListItem>().SetUp(playerStat.Nickname, playerStat.Team, playerStat.Score);
        }
        this.gameObject.SetActive(true);
    }

    private List<Statistics> GetPlayerStats() {
        List<Statistics> playerStats = new List<Statistics>();
        foreach (var minerController in _miners.Values) {
            playerStats.Add(new Statistics{Nickname = minerController.Nickname, Team = Constants.Team.Miner, Score = minerController.Score});
        }
        foreach (var guardianController in _guardians.Values) { 
            playerStats.Add(new Statistics{Nickname = guardianController.Nickname, Team = Constants.Team.Guardian, Score = guardianController.Score});
        }
        return playerStats;
    }

    private void SetWinText(Constants.Team winningTeam) {
        switch (winningTeam) {
            case Constants.Team.Guardian: _winText.text = "Miners Win!"; break;
            case Constants.Team.Miner: _winText.text = "Guardians Win!"; break;
        }
    }
}
