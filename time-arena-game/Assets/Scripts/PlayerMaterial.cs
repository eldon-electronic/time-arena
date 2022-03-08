using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterial : MonoBehaviour
{
    public GameObject PlayerBody;
	public GameObject PlayerArm;
	public GameObject HandThumb;
	public GameObject HandThumbTip;
	public GameObject HandIndex;
	public GameObject HandIndexTip;
	public GameObject HandMiddle;
	public GameObject HandMiddleTip;
    public Material SeekerMat;
    public Material HiderMat;
    public PhotonView View;

    private GameObject[] _bodyParts;
    private Dictionary<string, Material> _materials;
    
    void Start()
    {
        _bodyParts = new GameObject[]{PlayerBody, PlayerArm, HandThumb, 
            HandThumbTip, HandIndex, HandIndexTip, HandMiddle, HandMiddleTip};
        
        _materials = new Dictionary<string, Material>();
        _materials.Add("seeker", SeekerMat);
        _materials.Add("hider", HiderMat);
        
        if (!View.IsMine)
        {
            PlayerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        else
        {
            PlayerBody.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        HiderMat.SetFloat("_CutoffHeight", 50.0f);
		SeekerMat.SetFloat("_CutoffHeight", 50.0f);
    }

    public void SetMaterial(string material)
    {
        foreach (GameObject bodyPart in _bodyParts)
        {
            bodyPart.GetComponent<Renderer>().material = _materials[material];
        }
    }
}
