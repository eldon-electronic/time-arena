using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudTimeline : MonoBehaviour
{
    [SerializeField] private GameObject _timeline;
    [SerializeField] private GameObject _iconPrefab;
    [SerializeField] private Slider _elapsedTimeSlider;
    [SerializeField] private Sprite _yourIcon;
    [SerializeField] private Sprite _minerIcon;
    [SerializeField] private Sprite _guardianIcon;
    [SerializeField] private PlayerController _player;
    private SceneController _sceneController;
    private TimeLord _timeLord;
    private Dictionary<int, Slider> _players;

    void Awake()
    {
        _players = new Dictionary<int, Slider>();
    }

    void Start()
    {
        GameObject pregame = GameObject.FindWithTag("PreGameController");
        _sceneController = pregame.GetComponent<PreGameController>();

        SceneManager.activeSceneChanged += OnSceneChange;
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

    private void OnSceneChange(Scene current, Scene next)
    {
        GameObject game = GameObject.FindWithTag("GameController");
        _sceneController = game.GetComponent<GameController>();
    }

    private void SetTimeBarPosition()
    {
        int frame = _timeLord.GetCurrentFrame();
        int totalFrames = _timeLord.GetTotalFrames();
        _elapsedTimeSlider.value = (float) frame / (float) totalFrames;
    }

    private Slider InstantiateIcon(Constants.Team team, bool isMe)
    {
        // Instantiate and set its parent to be the timeline.
        GameObject newIcon = Instantiate(_iconPrefab);
        newIcon.transform.parent = _timeline.transform;

        // Reset its position and scale.
        newIcon.GetComponent<RectTransform>().localPosition = new Vector3(0, -180, 0);
        newIcon.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        // Set its icon image.
        GameObject handle = newIcon.transform.GetChild(0).GetChild(0).gameObject;
        if (isMe) handle.GetComponent<Image>().sprite = _yourIcon;
        else if (team == Constants.Team.Guardian)
        {
            handle.GetComponent<Image>().sprite = _guardianIcon;
        }
        else handle.GetComponent<Image>().sprite = _minerIcon;
        Debug.Log($"isMe: {isMe}, team: {team}");
        
        return newIcon.GetComponent<Slider>();
    }

    private bool AddNewIcon(int playerID)
    {
        try
        {
            Constants.Team team = _sceneController.GetTeam(playerID);
            Slider icon = InstantiateIcon(team, playerID == _player.GetID());
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


    // ------------ PUBLIC METHODS ------------

    public void SetTimeLord(TimeLord timeLord) { _timeLord = timeLord; }
}
