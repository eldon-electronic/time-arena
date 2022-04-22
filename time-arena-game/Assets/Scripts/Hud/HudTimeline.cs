using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeline : MonoBehaviour
{
    [SerializeField] private GameObject _timeline;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Image _timelineFill;
    [SerializeField] private PhotonView _view;
    [SerializeField] private Sprite[] _teamIcons;
    private SceneController _sceneController;
    private TimeLord _timeLord;
    private Dictionary<int, Slider> _players;
    private Dictionary<int, string> _viewIDtoUserID;
    private Dictionary<string, string> _iconAssignment;

    void Awake()
    {
        _players = new Dictionary<int, Slider>();
        _viewIDtoUserID = new Dictionary<int, string>();
        _iconAssignment = new Dictionary<string, string>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            string userID = player.GetComponent<PhotonView>().Owner.UserId;
            PhotonView playerView = player.GetComponent<PhotonView>();
            _viewIDtoUserID.Add(playerView.ViewID, userID);
        }
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
        GameController.newTimeLord += SetTimeLord;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
        GameController.newTimeLord -= SetTimeLord;
    }

    void Start()
    {
        GameObject pregame = GameObject.FindWithTag("PreGameController");
        _sceneController = pregame.GetComponent<PreGameController>();
        _timeLord = _sceneController.GetTimeLord();

        if (!PhotonNetwork.IsMasterClient) {
            Debug.Log("Requesting icons from master");
            _view.RPC("RPC_getIconAssignment", RpcTarget.MasterClient);
        }
    }

    void LateUpdate()
    {
        SetTimeBarPosition();
        
        // We set the visibility of all icons off here in case someone leaves the game.
        foreach (var icon in _players)
        {
            icon.Value.gameObject.SetActive(false);
        }
        
        //SetIconPositions();
    }


    // ------------ PRIVATE METHODS ------------

    private void OnGameActive(GameController game) { _sceneController = game; }

    private void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }

    private void SetTimeBarPosition()
    {
        int frame = _timeLord.GetCurrentFrame();
        int totalFrames = _timeLord.GetTotalFrames();
        _timelineFill.fillAmount = (float) frame / (float) totalFrames;
    }

    private Slider InstantiateIcon(string iconName)
    {
        Sprite teamIcon = null;
        foreach (var icon in _teamIcons) {
            if (icon.name == iconName) teamIcon = icon;
        }

        // Instantiate, set its parent to be the timeline and specify which icon to use
        GameObject newIcon = Instantiate(_iconPrefab, _iconContainer);
        newIcon.GetComponent<TimelineSliderItem>().SetUp(teamIcon);

        return newIcon.GetComponent<Slider>();
    }

    private bool AddNewIcon(int playerID)
    {
        try
        {
            string userID = _viewIDtoUserID[playerID];
            string iconName = _iconAssignment[userID];
            Slider icon = InstantiateIcon(iconName);
            _players.Add(playerID, icon);
            return true;
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError($"Error: {e}");
            return false;
        }
    }

    private void SetIconPositions()
    {
        List<(int id, int frame)> players = _timeLord.GetPerceivedFrames();
        int totalFrames = _timeLord.GetTotalFrames();
        int frame;
        float position;

        foreach (var player in players)
        {
            // This skips this player if they're not in our dictionary and can't be added.
            if (!_players.ContainsKey(player.id) && !AddNewIcon(player.id)) continue;
            frame = player.frame;
            position = (float) frame / (float) totalFrames;
            _players[player.id].value = position;
            _players[player.id].gameObject.SetActive(true);
        }
    }

    // ------------ RPC ------------

    // Only called on Master Client (they've got the PlayerPrefs).
    [PunRPC] void RPC_getIconAssignment() { 
        Debug.Log("Getting icons from PlayerPrefs");
        _iconAssignment.Clear();
        foreach (KeyValuePair<int, string> pair in _viewIDtoUserID) {
            _iconAssignment.Add(pair.Value, PlayerPrefs.GetString(pair.Value));
        }
        Debug.Log("Sending them over");
        _view.RPC("RPC_sendIconAssignment", RpcTarget.All, _iconAssignment);
    }

    [PunRPC] void RPC_sendIconAssignment(Dictionary<string, string> iconAssignment) {
        _iconAssignment.Clear();
        _iconAssignment = iconAssignment;
    }

}
