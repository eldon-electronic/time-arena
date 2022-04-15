using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudStartTimer : MonoBehaviour
{
    [SerializeField] private GameObject _startTimer;
    [SerializeField] private Text _text;

    void OnEnable()
    {
        GameController.gameActive += SetGame;
        GameController.gameStarted += OnGameStarted;
        GameController.countDown += OnCountDown;
    }

    void OnDisable()
    {
        GameController.gameActive -= SetGame;
        GameController.gameStarted -= OnGameStarted;
        GameController.countDown -= OnCountDown;
    }

    void Start() { _startTimer.SetActive(false); }

    private void SetGame(GameController game) { _startTimer.SetActive(true); }

    private void OnGameStarted() { _startTimer.SetActive(false); }

    private void OnCountDown(float seconds) { _text.text = $"{(int) seconds}"; }
}
