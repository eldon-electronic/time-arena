using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsernameManager : MonoBehaviour
{
    private string welcomeText;
    [SerializeField] private TMP_InputField _userInputField;
    [SerializeField] private Button _submitButton;
    [SerializeField] private TMP_Text _welcomeText;
    [SerializeField] private GameObject _welcomeTextContainer;
    [SerializeField] private GameObject _usernameInputContainer;
    [SerializeField] private Button _changeUsernameButton;
    [SerializeField] private TMP_Text _errorText;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("username")) {
            PlayerPrefs.DeleteAll();
        }

        DisplayUsernameInput();
    }

    // Helper function to disable error text display after a set amount of time has passed.
    private void disableErrorText() {
        _errorText.gameObject.SetActive(false);
    }

    public void SubmitUsername() {
        string userInput = _userInputField.text;
        if (!string.IsNullOrEmpty(userInput)) {
            PhotonNetwork.NickName = userInput;
            welcomeText = "Welcome to Time Arena " + userInput + ".";
            _welcomeText.text = welcomeText;
            displayWelcomeMessage();
        } else {
            _errorText.gameObject.SetActive(true);
            Invoke("disableErrorText", 3f);
        }
    }

    public void DisplayUsernameInput() {
        _welcomeTextContainer.gameObject.SetActive(false);
        _usernameInputContainer.gameObject.SetActive(true);
    }

    private void displayWelcomeMessage() {
        _usernameInputContainer.gameObject.SetActive(false);
        _welcomeTextContainer.gameObject.SetActive(true);
    }
}
