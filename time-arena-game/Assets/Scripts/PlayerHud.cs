using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public GameController game;
	public PhotonView view;
    public Canvas UI;
    public Text teamDispl;
	public Text debugPanelText;
	public Text masterClientOpts;
	public Text ab1Cooldown_displ;
	public Text ab2Cooldown_displ;
	public Text ab3Cooldown_displ;
	public Text timeDispl;
	public Text startTimeDispl;
	public Text winningDispl;
	private float secondsTillGame;
	private bool isCountingTillGameStart;
    public Slider elapsedTimeSlider;
    public Slider playerIcon0;
    public Slider playerIcon1;
    public Slider playerIcon2;
    public Slider playerIcon3;
    public Slider playerIcon4;
	private Slider[] playerIcons;
	public Image Forward;
	public Image ForwardPressed;
	public Image ForwardUnable;

    private Hashtable debugItems;
    private float[] abilities;


    void Start()
    {
        // The first Slider in the array corresponds to this player
        playerIcons = new Slider[]{playerIcon0, playerIcon1, playerIcon2, playerIcon3, playerIcon4};

        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            Destroy(UI);
        }
    }


    // ------------ LATE UPDATE HELPER FUNCTIONS ------------

    private void LateUpdateMasterClientOptions()
    {
        // If master client, show 'press e to start' text or 'starting in' text
        masterClientOpts.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name == "PreGameScene" && PhotonNetwork.IsMasterClient);

        if (isCountingTillGameStart)
        {
            masterClientOpts.text = "Starting in " + System.Math.Round (secondsTillGame, 0) + "s";
            if (System.Math.Round (secondsTillGame, 0) <= 0.0f)
            {
                masterClientOpts.text = "Loading...";
            }
        }
    }


    private void LateUpdateStartTimeDisplay()
    {
        startTimeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            startTimeDispl.transform.parent.gameObject.SetActive(!game.gameStarted);
            if (!game.gameStarted && !game.gameEnded)
            {
                startTimeDispl.text = "" + (5-(int)(game.timeElapsedInGame+0.9f));
            }
        }
    }


    private void LateUpdateTimeDisplay()
    {
        timeDispl.transform.parent.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        if (SceneManager.GetActiveScene().name == "GameScene" && !game.gameEnded)
        {
            if (game.gameStarted)
            {
                float t = game.gameLength - game.timeElapsedInGame;
                timeDispl.text = (int)(t/60) + ":" + ((int)(t%60)).ToString().PadLeft(2, '0') + ":" + (((int)(((t%60)-(int)(t%60))*100))*60/100).ToString().PadLeft(2, '0');
            }
            else
            {
                timeDispl.text = "0:00:00";
            }
        }
    }


    private void LateUpdateTimeline()
    {
        // Set visibility of timeline and player icons
        elapsedTimeSlider.gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        playerIcons[0].gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene");
        for (int i=1; i < 5; i++)
        {
            playerIcons[i].gameObject.SetActive(SceneManager.GetActiveScene().name != "PreGameScene" && game.otherPlayersElapsedTime.Count >= i + 1);
        }

        // Set player icon positions
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (game.gameStarted && !game.gameEnded)
            {
                elapsedTimeSlider.value = game.timeElapsedInGame / game.gameLength;
                int n = 0;
                List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
                foreach(int key in keys)
                {
                    playerIcons[n].value = game.otherPlayersElapsedTime[key];
                    n++;
                }
            }
            else if (!game.gameStarted && !game.gameEnded)
            {
                playerIcons[0].value = 0;
                int n = 0;
                List<int> keys = new List<int>(game.otherPlayersElapsedTime.Keys);
                foreach(int key in keys){
                    playerIcons[n].value = 0;
                    n++;
                }
            }
        }
    }


    private void LateUpdateDebugPanel()
    {
        System.String debugText = "";
        foreach (DictionaryEntry de in debugItems)
        {
            debugText += $"{de.Key}: {de.Value}\n";
        }
        debugPanelText.text = debugText;
    }

    private void LateUpdateAbilities()
    {
        ab1Cooldown_displ.text = abilities[0].ToString();
        ab2Cooldown_displ.text = abilities[1].ToString();
        ab3Cooldown_displ.text = abilities[2].ToString();
    }

    private void LateUpdateWinningDisplay()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && game.gameEnded)
        {
            winningDispl.transform.parent.gameObject.SetActive(true);
            winningDispl.text = (game.winningTeam == 1) ? "HIDERS WIN!" : "SEEKERS WIN!";
        }
    }


    // ------------ UPDATE METHODS ------------

    void Update()
    {
        // if counting, reduce timer
        if (PhotonNetwork.IsMasterClient && isCountingTillGameStart) {
            secondsTillGame -= Time.deltaTime;
            if (secondsTillGame <= 0) {
                PhotonNetwork.LoadLevel("GameScene");
                isCountingTillGameStart = false;
            }
        }
    }


    void LateUpdate()
    {
        if (!view.IsMine) return;

        LateUpdateMasterClientOptions();
        LateUpdateStartTimeDisplay();
        LateUpdateTimeDisplay();
        LateUpdateTimeline();
        LateUpdateDebugPanel();
        LateUpdateAbilities();
        LateUpdateWinningDisplay();
    }


    // ------------ PUBLIC METHODS ------------


        public void SetTeam(System.String teamName)
    {
        teamDispl.text = teamName;
    }


    public void StartCountingDown()
    {
        if (isCountingTillGameStart) return;
        
        isCountingTillGameStart = true;
        secondsTillGame = 5.0f;
    }


    public void StopCountingDown()
    {
        isCountingTillGameStart = false;
        secondsTillGame = 0.0f;
    }


    public void SetDebugValues(Hashtable items)
    {
        debugItems = items;
    }

    public void SetAbilityValues(float[] items)
    {
        abilities = items;
    }
}
