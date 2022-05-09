using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class WinScreenController : MonoBehaviour
{

    public static WinScreenController Instance;

    [SerializeField] private Transform _guardianContainer;
    [SerializeField] private Transform _minerContainer;
    [SerializeField] private GameObject _statListItem;
    [SerializeField] private TMP_Text _winText;
    [SerializeField] private GameObject _winUI;
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioClip _winClip;
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
        _winUI.SetActive(false);
    }

    private void OnGameEnded(Constants.Team winningTeam) {
        _winUI.SetActive(true);
        SetWinText(winningTeam);
        _soundSource.PlayOneShot(_winClip);
        _sceneController = FindObjectOfType<SceneController>();
        _miners = _sceneController.GetMinerControllers();
        _guardians = _sceneController.GetGuardianControllers();

        List<Statistics> minerStats = GetMinerStats();
        List<Statistics> guardianStats = GetGuardianStats();

        InstantiateItems(minerStats, _minerContainer);
        InstantiateItems(guardianStats, _guardianContainer);
    }

    private List<Statistics> GetMinerStats() {
        List<Statistics> minerStats = new List<Statistics>();
        foreach (var minerController in _miners.Values) {
            minerStats.Add(new Statistics{Nickname = minerController.Nickname, Team = Constants.Team.Miner, Score = minerController.Score});
        }
        return minerStats;
    }

    private List<Statistics> GetGuardianStats() {
        List<Statistics> guardianStats = new List<Statistics>();
        foreach (var guardianController in _guardians.Values) {
            guardianStats.Add(new Statistics{Nickname = guardianController.Nickname, Team = Constants.Team.Guardian, Score = guardianController.Score});
        }
        return guardianStats;
    }

    private void SetWinText(Constants.Team winningTeam) {
        _winText.text = (winningTeam == Constants.Team.Miner) ? "MINERS WIN!" : "GUARDIANS WIN!";
    }

    private void InstantiateItems(List<Statistics> teamStatistics, Transform teamContainer) {
        // Sort by score
        teamStatistics.Sort(delegate(Statistics s1, Statistics s2) { return s2.Score.CompareTo(s1.Score); });
        foreach (var teamStat in teamStatistics) {
            Instantiate(_statListItem, teamContainer)
                .GetComponent<StatListItem>()
                .SetUp(teamStat.Nickname, teamStat.Team, teamStat.Score);
        }
    }
}
