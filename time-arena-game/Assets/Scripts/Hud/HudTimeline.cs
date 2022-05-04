using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeline : MonoBehaviour
{
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Image _timelineFill;
    [SerializeField] private Sprite _yourMinerIcon;
    [SerializeField] private Sprite _yourGuardianIcon;
    [SerializeField] private Sprite _minerIcon;
    [SerializeField] private Sprite _guardianIcon;
    [SerializeField] private PhotonView _view;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private Sprite[] _teamIcons;
    [SerializeField] private Sprite _backupIcon;
    private SceneController _sceneController;
    private TimeLord _timeLord;
    private Dictionary<int, Slider> _players;

    void Awake()
    {
        _players = new Dictionary<int, Slider>();
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
        _sceneController = FindObjectOfType<SceneController>();
        _timeLord = _sceneController.GetTimeLord();
    }

    void LateUpdate()
    {
        SetTimeBarPosition();
        
        // We set the visibility of all icons off here in case someone leaves the game.
        foreach (var icon in _players)
        {
            icon.Value.gameObject.SetActive(false);
        }
        
        SetIconPositions();
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

    private Slider InstantiateIcon(Sprite icon, bool isMe)
    {
        // Instantiate and set its parent to be the timeline.
        GameObject newIcon = Instantiate(_iconPrefab, _iconContainer);
        newIcon.gameObject.GetComponent<TimelineSliderItem>().SetUp(icon, isMe);
        
        return newIcon.GetComponent<Slider>();
    }

    private bool AddNewIcon(int playerID)
    {
        try
        {
            Sprite playerIcon = GetIcon(_sceneController.GetIconString(playerID));
            Slider iconSlider = InstantiateIcon(playerIcon, playerID == _view.ViewID);
            _players.Add(playerID, iconSlider);
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

    private Sprite GetIcon(string iconName) {
        foreach (var icon in _teamIcons) {
            if (icon.name == iconName) return icon;
        } return _backupIcon;
    }
}
