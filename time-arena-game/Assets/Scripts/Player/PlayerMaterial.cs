using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterial : MonoBehaviour
{
    [SerializeField] private GameObject _playerBody;
    [SerializeField] private GameObject _playerArm;
    [SerializeField] private GameObject _handThumb;
    [SerializeField] private GameObject _handThumbTip;
    [SerializeField] private GameObject _handIndex;
    [SerializeField] private GameObject _handIndexTip;
    [SerializeField] private GameObject _handMiddle;
    [SerializeField] private GameObject _handMiddleTip;
    [SerializeField] private Material _guardianMat;
    [SerializeField] private Material _minerMat;
    [SerializeField] private PhotonView _view;

    private GameObject[] _armParts;
    private Dictionary<Constants.Team, Material> _materials;
    

    void Awake()
    {
        _armParts = new GameObject[]
        {
            _playerArm,
            _handThumb,
            _handThumbTip,
            _handIndex,
            _handIndexTip,
            _handMiddle,
            _handMiddleTip
        };
        
        _materials = new Dictionary<Constants.Team, Material>();
        _materials.Add(Constants.Team.Guardian, _guardianMat);
        _materials.Add(Constants.Team.Miner, _minerMat);
    }

    void Start()
    {   
        if (!_view.IsMine) _playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        else _playerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        _minerMat.SetFloat("_CutoffHeight", 50.0f);
		_guardianMat.SetFloat("_CutoffHeight", 50.0f);
    }

    void Update()
    {
        if (!_view.IsMine)
        {
            SetPlayerActive(gameObject.layer == Constants.LayerPlayer);
        }
    }

    public void SetMaterial(Constants.Team team)
    {
        _playerBody.GetComponent<Renderer>().material = _materials[team];
        foreach (GameObject armPart in _armParts)
        {
            armPart.GetComponent<Renderer>().material = _materials[team];
        }
    }

    public void SetArmActive(bool on)
    {
        foreach(GameObject armPart in _armParts)
        {
            armPart.SetActive(on);
        }
    }

    public void SetPlayerActive(bool on)
    {
        _playerBody.SetActive(on);
        foreach(GameObject armPart in _armParts)
        {
            armPart.SetActive(on);
        }
    }

    public void SetVisible(bool on)
    {
        float value = (on) ? 4.0f : -4.0f;

        _playerBody.GetComponent<Renderer>().material.SetFloat("_CutoffHeight", value);
        foreach (GameObject armPart in _armParts)
        {
            armPart.GetComponent<Renderer>().material.SetFloat("_CutoffHeight", value);
        }
    }
}
