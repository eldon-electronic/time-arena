using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterial : MonoBehaviour
{
    [SerializeField] private GameObject _playerBody;
    [SerializeField] private GameObject _playerArm;
    [SerializeField] private GameObject _playerHand;
    [SerializeField] private Material _guardianMat;
    [SerializeField] private Material _minerMat;
    [SerializeField] private PhotonView _view;
    [SerializeField] private PlayerController _player;
    private Dictionary<Constants.Team, Material> _materials;
    

    void Awake()
    {
        _materials = new Dictionary<Constants.Team, Material>();
        _materials.Add(Constants.Team.Guardian, _guardianMat);
        _materials.Add(Constants.Team.Miner, _minerMat);
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
    }

    void OnDisable()
    {
        GameController.gameActive -= OnGameActive;
    }

    void Start()
    {   
        if (!_view.IsMine) _playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        else _playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        _minerMat.SetFloat("_CutoffHeight", 50.0f);
		_guardianMat.SetFloat("_CutoffHeight", 50.0f);

        SetMaterial(_player.Team);
    }

    void Update()
    {
        if (!_view.IsMine)
        {
            _playerBody.SetActive(gameObject.layer == Constants.LayerPlayer);
            _playerArm.SetActive(gameObject.layer == Constants.LayerPlayer);
            _playerHand.SetActive(gameObject.layer == Constants.LayerPlayer);
        }
    }

    private void OnGameActive(GameController game)
    {
        _playerArm.SetActive(_player.Team == Constants.Team.Guardian);
        _playerHand.SetActive(_player.Team == Constants.Team.Guardian);
    }

    private void SetMaterial(Constants.Team team)
    {
        _playerBody.GetComponent<Renderer>().material = _materials[team];
        _playerArm.GetComponent<Renderer>().material = _materials[team];
        foreach (Transform armPart in _playerHand.transform)
        {
            armPart.gameObject.GetComponent<Renderer>().material = _materials[team];
        }
    }
}
