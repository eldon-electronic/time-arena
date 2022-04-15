using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDevice : MonoBehaviour
{
    public GameObject CompassPointer;
    public GameObject ButtonArrow;
    public GameObject OffLine;
    public GameObject DeviceButton;
    public Material ButtonForwardMat;
    public Material ButtonBackwardMat;
    public Material ButtonOffMat;
    public Material ButtonArrowForwardMat;
    public Material ButtonArrowBackwardMat;
    public Material OffLineMat;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _manager;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    (float, float) GetCrystalWindow(GameObject Crystal)
    {
        CrystalBehaviour c = Crystal.GetComponent<CrystalBehaviour>();
        return (c.existanceRange.x, c.existanceRange.y);
    }

    GameObject[] GetCrystals()
    {
        List<CrystalBehaviour> comps = _manager.GetComponent<CrystalManager>().crystals;
        List<GameObject> crystals = new List<GameObject>();
        foreach(CrystalBehaviour c in comps)
        {
            crystals.Add(c.gameObject);
        }
        return crystals.ToArray();
    }

    float GetPlayerTime(GameObject Player)
    {
        if (Player.GetComponent<PlayerController>() != null)
        {
            return Player.GetComponent<PlayerController>().GetTime();
        }
        return -1;
    }

    GameObject ClosestCrystal(Vector3 player, GameObject[] crystals)
    {
        GameObject output = null;
        float lastDistance = float.MaxValue;
        foreach(GameObject c in crystals)
        {
            if(Vector3.Distance(player, c.transform.position) < lastDistance)
            {
                lastDistance = Vector3.Distance(player, c.transform.position);
                output = c;
            }
        }
        return output;
    }

    int GetRelativeTimePosition(GameObject Player, GameObject Crystal)
    {
        //return -1 if behind time frame
        //return 1 if ahead of time frame
        //return 0 if in time frame
        float p = GetPlayerTime(Player);
        (float, float) w = GetCrystalWindow(Crystal);
        if (p < w.Item1) return -1;
        if (p > w.Item2) return 1;
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject Crystal = ClosestCrystal(_player.transform.position, GetCrystals());

        Vector2 flatPlayer = new Vector2(_player.transform.position.x, _player.transform.position.z);
        Vector2 flatCrystal = new Vector2(Crystal.transform.position.x, Crystal.transform.position.z);

        float look = _player.transform.rotation.eulerAngles.y;
        float angle = Vector2.Angle(Vector2.up, flatCrystal - flatPlayer);
        ChangePointerPosition(angle-look);

        if(Vector2.Distance(flatPlayer, flatCrystal) < Constants.Proximity)
        {
            SetButtonMaterial(new Material[] { ButtonForwardMat, ButtonOffMat, ButtonBackwardMat }[1 - GetRelativeTimePosition(_player, Crystal)]);
        }

           
    }
    public void ChangePointerPosition(float position)
    {
         Vector3 eulerRotation = CompassPointer.transform.rotation.eulerAngles;
         CompassPointer.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, position);
        
    }

    public void SetButtonMaterial(Material buttonMat){

        DeviceButton.GetComponent<Renderer>().material = buttonMat;

        if(buttonMat == ButtonForwardMat){

            ButtonArrow.GetComponent<Renderer>().material = ButtonArrowForwardMat;
            Vector3 eulerRotation = ButtonArrow.transform.rotation.eulerAngles;
            ButtonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 180f);
            OffLine.SetActive(false);

        }
        else if(buttonMat == ButtonBackwardMat){

            ButtonArrow.GetComponent<Renderer>().material = ButtonArrowBackwardMat;
            Vector3 eulerRotation = ButtonArrow.transform.rotation.eulerAngles;
            ButtonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0f);
            OffLine.SetActive(false);


        }
        else if(buttonMat == ButtonOffMat){

            ButtonArrow.GetComponent<Renderer>().material = OffLineMat;
            ButtonArrow.SetActive(false);
            OffLine.SetActive(true);
            

        }
        
    }


   
}
