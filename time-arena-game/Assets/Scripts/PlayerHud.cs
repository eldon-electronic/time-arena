using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class PlayerHud : MonoBehaviour
{
    public GameController Game;
	public PhotonView View;
    public Text TeamDispl;
    public CanvasGroup DebugCanvasGroup;
	public Text DebugPanelText;
	public Text MasterClientOpt;
	public Slider ForwardCooldownSlider;
	public Slider BackCooldownSlider;
	public Text TimeDispl;
	public Text StartTimeDispl;
  public Text WinningDispl;
  public Text ScoreDispl;
    public CanvasGroup TimelineCanvasGroup;
    public Slider ElapsedTimeSlider;
    public Slider PlayerIcon0;
    public Slider PlayerIcon1;
    public Slider PlayerIcon2;
    public Slider PlayerIcon3;
    public Slider PlayerIcon4;
	public Image ForwardJumpIcon;
    public Image BackJumpIcon;
    public Sprite RedPressedSprite;
    public Sprite GreenPressedSprite;
    public Sprite GreenUnpressedSprite;
    public GameObject ArrowImage;
    public GameObject PopUpText;
    public GameObject PopUp;
    private float _secondsTillGame;
	private bool _isCountingTillGameStart;
    private Slider[] _playerIcons;
    private Hashtable _debugItems;
    private float[] _cooldowns;
    private bool _debug;
    private bool _canJumpForward;
    private bool _canJumpBack;
    private string _message;
    private int score;

    private Dictionary<string, Vector2> _uiPositions;
    private Dictionary<string, Vector3> _uiRotations;


    void Start()
    {


        //vector2 forwardJumpPos = new vector2(-400f,-125f);
        //vector2 backJumpPos = new vector2{-400f,-125f};
       // vector2 forwardJumpPos = new vector2{-400f,-125f};

        _uiPositions = new Dictionary<string, Vector2>();
        _uiPositions.Add("forwardJump",new Vector2(381f,-76f));
        _uiPositions.Add("backJump",new Vector2(-397f,-82f));
        _uiPositions.Add("timebar",new Vector2(-342f,-122f));
        _uiPositions.Add("timer",new Vector2(-290f,105f));
        _uiPositions.Add("team",new Vector2(284f,104f));


        _uiRotations = new Dictionary<string, Vector3>();
        _uiRotations.Add("forwardJump",new Vector3(-10.1f,-720.2f,18.5f));
        _uiRotations.Add("backJump",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timebar",new Vector3(-10.1f,-535.5f,4.7f));
        _uiRotations.Add("timer",new Vector3(-10.1f,-535.5f,-150f));
        _uiRotations.Add("team",new Vector3(-10.1f,-355.5f,-150f));




        if (View.IsMine)

        {
            // The first Slider in the array corresponds to this player.
            _playerIcons = new Slider[]{PlayerIcon0, PlayerIcon1, PlayerIcon2, PlayerIcon3, PlayerIcon4};

            // Link SceneChange event to OnSceneChange.
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        _debugItems = new Hashtable();
        _cooldowns = new float[2];
        _debug = false;
        DebugCanvasGroup.alpha = 0.0f;
        _canJumpForward = false;
        _canJumpBack = false;


    }

    void OnSceneChange(Scene current, Scene next)
    {
        if (next.name == "GameScene")
        {
            Game = FindObjectOfType<GameController>();
        }
    }

    // ------------ LATE UPDATE HELPER FUNCTIONS ------------

    private void LateUpdateMasterClientOptions()
    {
        // If master client, show 'press e to start' text or 'starting in' text.
        MasterClientOpt.transform.parent.gameObject.SetActive(
            SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient
        );

        if (_isCountingTillGameStart)
        {
            var timeLeft = System.Math.Round(_secondsTillGame, 0);
            MasterClientOpt.text = $"Starting in {timeLeft}s";
            if (System.Math.Round (_secondsTillGame, 0) <= 0.0f)
            {
                MasterClientOpt.text = "Loading...";
            }
        } else {
          MasterClientOpt.text = "Press F to Start";

        }
    }


    private void LateUpdateStartTimeDisplay()
    {
        StartTimeDispl.transform.parent.gameObject.SetActive(
            SceneManager.GetActiveScene().name != "PreGameScene"
        );



        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if(Game == null){
              Game = FindObjectOfType<TimeLord>().GetComponent<GameController>();
            }
            StartTimeDispl.transform.parent.gameObject.SetActive(!Game.GameStarted);
            if (!Game.GameStarted && !Game.GameEnded)
            {
                var elapsed = 5 - (int) (Game.TimeElapsedInGame + 0.9f);
                StartTimeDispl.text = $"{elapsed}";
            }
        }
    }


    private void LateUpdateTimeDisplay()
    {
       // TimeDispl.transform.parent.gameObject.SetActive(
           // SceneManager.GetActiveScene().name != "PreGameScene"
        //);

        if (SceneManager.GetActiveScene().name == "GameScene" && !Game.GameEnded)
        {
            if (Game.GameStarted)
            {
                float t = Game.GameLength - Game.TimeElapsedInGame;
                int minutes = (int) (t / 60);
                int seconds = (int) (t % 60);
                TimeDispl.text = minutes.ToString() + ":" + seconds.ToString().PadLeft(2, '0');
            } else {
                TimeDispl.text = "0:00:00";
            }
        }
    }


    private void LateUpdateTimeline()
    {
        // Set visibility of timeline, player icons and jump cooldowns.
       // TimelineCanvasGroup.alpha = (SceneManager.GetActiveScene().name != "PreGameScene") ? 1.0f: 0.0f;
      //  ElapsedTimeSlider.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
       // _playerIcons[0].gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        for (int i=1; i < 5; i++)
        {
            _playerIcons[i].gameObject.SetActive(
                SceneManager.GetActiveScene().name != "PreGameScene" &&
                Game.OtherPlayersElapsedTime.Count >= i + 1
            );
        }

        // Set player icon positions.
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (Game.GameStarted && !Game.GameEnded)
            {
                ElapsedTimeSlider.value = Game.TimeElapsedInGame / Game.GameLength;
                int n = 0;
                List<int> keys = new List<int>(Game.OtherPlayersElapsedTime.Keys);
                foreach(int key in keys)
                {
                    _playerIcons[n].value = Game.OtherPlayersElapsedTime[key];
                    n++;
                }
            } else if (!Game.GameStarted && !Game.GameEnded) {
                _playerIcons[0].value = 0;
                int n = 0;
                List<int> keys = new List<int>(Game.OtherPlayersElapsedTime.Keys);
                foreach(int key in keys){
                    _playerIcons[n].value = 0;
                    n++;
                }
            }
        }
    }



    private void LateUpdateDebugPanel()
    {
        if (_debug)
        {
            System.String debugText = "";
            foreach (DictionaryEntry de in _debugItems)
            {
                debugText += $"{de.Key}: {de.Value}\n";
            }
            DebugPanelText.text = debugText;
            DebugCanvasGroup.alpha = 1.0f;
        }
        else
        {
            DebugCanvasGroup.alpha = 0.0f;
        }
    }

    private void LateUpdateCooldowns()
    {
        ForwardCooldownSlider.value = _cooldowns[0];
        BackCooldownSlider.value = _cooldowns[1];

        if (_canJumpForward) ForwardJumpIcon.sprite = GreenUnpressedSprite;
        else ForwardJumpIcon.sprite = RedPressedSprite;
        if (_canJumpBack) BackJumpIcon.sprite = GreenUnpressedSprite;
        else BackJumpIcon.sprite = RedPressedSprite;
    }

    private void LateUpdateWinningDisplay()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && Game.GameEnded)
        {
            WinningDispl.transform.parent.gameObject.SetActive(true);
            WinningDispl.text = (Game.WinningTeam == Constants.Team.Miner) ? "HIDERS WIN!" : "SEEKERS WIN!";
        }
    }

    private void LateUpdateTutorial()
    {
        if (SceneManager.GetActiveScene().name != "PreGameScene")
        {
            PopUp.gameObject.SetActive(false);

        }
    }

    private void LateUpdateScore(){
      if (SceneManager.GetActiveScene().name == "GameScene")
      {
        ScoreDispl.transform.parent.gameObject.SetActive(true);

          ScoreDispl.text = score + "";
      } else if(SceneManager.GetActiveScene().name == "PreGameScene") {
        ScoreDispl.transform.parent.gameObject.SetActive(false);
      }
    }


    // ------------ UPDATE METHODS ------------

    void Update()
    {
        // If counting, reduce timer.
        if (PhotonNetwork.IsMasterClient && _isCountingTillGameStart && View.IsMine) {
            _secondsTillGame -= Time.deltaTime;
            if (_secondsTillGame <= 0) {
                PhotonNetwork.LoadLevel("GameScene");
                _isCountingTillGameStart = false;
            }
        }
    }


    void LateUpdate()
    {
        if (!View.IsMine) return;

        LateUpdateMasterClientOptions();
        LateUpdateStartTimeDisplay();
        LateUpdateTimeDisplay();
        LateUpdateTimeline();
        LateUpdateDebugPanel();
        LateUpdateCooldowns();
        LateUpdateWinningDisplay();
        LateUpdateTutorial();
        LateUpdateScore();

    }


    // ------------ PUBLIC METHODS ------------

    public void setScore(int a){
      score = a;
    }
    public int getScore(){
      return score;
    }

    public void SetTeam(System.String teamName)
    {
        if (View.IsMine) TeamDispl.text = teamName;
    }


    public void StartCountingDown()
    {
        if (_isCountingTillGameStart) return;

        _isCountingTillGameStart = true;
        _secondsTillGame = 5.0f;
    }


    public void StopCountingDown()
    {
        _isCountingTillGameStart = false;
        _secondsTillGame = 0.0f;
    }


    public void SetDebugValues(Hashtable items)
    {
        _debugItems = items;
    }

    public void SetCooldownValues(float[] items)
    {
        // Each item should be a float between 0.0f (empty) and 1.0f (full).
        _cooldowns = items;
    }

    public void ToggleDebug()
    {
        _debug = !_debug;
    }

    public void PressForwardJumpButton()
    {
        if (!View.IsMine) return;

        ForwardJumpIcon.sprite = GreenPressedSprite;
    }

    public void PressBackJumpButton()
    {
        if (!View.IsMine) return;
        BackJumpIcon.sprite = GreenPressedSprite;
    }

    public void SetCanJump(bool forward, bool back)
    {
        _canJumpForward = forward;
        _canJumpBack = back;
    }

    public void SetArrowPosition(string uiElement){

        if (!View.IsMine) return;

        ArrowImage.GetComponent<RectTransform>().anchoredPosition = _uiPositions[uiElement];
        ArrowImage.GetComponent<RectTransform>().eulerAngles = _uiRotations[uiElement];
    }

    public void SetMessage(string tutorialMessage){

        if (!View.IsMine) return;

        _message = tutorialMessage;
        PopUpText.GetComponent<TextMeshProUGUI>().text = _message;

    }

    public void SetArrowVisibility(bool arrowVisibility){

        if (!View.IsMine) return;

        ArrowImage.gameObject.SetActive(arrowVisibility);
    }


}
