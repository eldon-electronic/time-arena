using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDevice : MonoBehaviour
{
    [SerializeField] private GameObject _compassPointer;
    [SerializeField] private GameObject _buttonArrow;
    [SerializeField] private GameObject _buttonOffLine;
    [SerializeField] private Renderer _buttonRenderer;
    [SerializeField] private Renderer _arrowRenderer;
    [SerializeField] private Material _buttonForwardMat;
    [SerializeField] private Material _buttonBackwardMat;
    [SerializeField] private Material _buttonOffMat;
    [SerializeField] private Material _buttonArrowForwardMat;
    [SerializeField] private Material _buttonArrowBackwardMat;
    [SerializeField] private Material _offLineMat;
    private CrystalManager _manager;
    private TimeLord _timeLord;
    private Dictionary<Constants.RelTime, Material> _buttonColours;
    private Dictionary<Constants.RelTime, Material> _arrowColours;
    private bool _active;


    // ------------ UNITY METHODS ------------

    void Awake()
    {
        _buttonColours = new Dictionary<Constants.RelTime, Material> {
            {Constants.RelTime.Behind, _buttonBackwardMat},
            {Constants.RelTime.Same, _buttonOffMat},
            {Constants.RelTime.Ahead, _buttonForwardMat}
        };

        _arrowColours = new Dictionary<Constants.RelTime, Material> {
            {Constants.RelTime.Behind, _buttonArrowBackwardMat},
            {Constants.RelTime.Same, _offLineMat},
            {Constants.RelTime.Ahead, _buttonArrowForwardMat},
        };

        _active = false;
    }

    void OnEnable()
    {
        GameController.gameActive += OnGameActive;
        GameController.newTimeLord += OnNewTimeLord;
    }

    void OnDisable()
    {
        GameController.gameActive += OnGameActive;
        GameController.newTimeLord -= OnNewTimeLord;
    }

    void Update()
    {
        if (!_active) return;

        CrystalBehaviour crystal = GetClosestCrystal();

        if (crystal == null)
        {
            ChangePointerPosition(0);
            SetButtonMaterial(Constants.RelTime.Same);
        }
        else
        {
            Vector2 flatPlayer = new Vector2(transform.position.x, transform.position.z);
            Vector2 flatCrystal = new Vector2(crystal.transform.position.x, crystal.transform.position.z);

            float look = transform.rotation.eulerAngles.y;
            Vector2 difference = flatCrystal - flatPlayer;
            float angle = Mathf.Atan(difference.x / difference.y) * Mathf.Rad2Deg;
            if (flatPlayer.y < flatCrystal.y)
            {
                angle = (angle + 180) % 360;
            }
            ChangePointerPosition(angle - look);

            if (Vector2.Distance(flatPlayer, flatCrystal) < Constants.Proximity)
            {
                Constants.RelTime relativeTime = GetRelativeTime(crystal);
                SetButtonMaterial(relativeTime);
            }
            else SetButtonMaterial(Constants.RelTime.Same);
        }
    }


    // ------------ EVENT FUNCIONS ------------

    private void OnGameActive(GameController game) { _manager = game.gameObject.GetComponent<CrystalManager>(); }

    private void OnNewTimeLord(TimeLord timeLord)
    {
        _timeLord = timeLord;
        _active = true;
    }


    // ------------ PRIVATE METHODS FOR COMPASS ------------

    private CrystalBehaviour GetClosestCrystal()
    {
        List<CrystalBehaviour> crystals = _manager.crystals;
        CrystalBehaviour closestCrystal = null;
        float shortestDistance = float.MaxValue;
        float distance;

        foreach (var crystal in crystals)
        {
            distance = Vector3.Distance(transform.position, crystal.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestCrystal = crystal;
            }
        }
        Debug.Log(closestCrystal.ID);
        return closestCrystal;
    }

    private void ChangePointerPosition(float position)
    {
        Vector3 eulerRotation = _compassPointer.transform.rotation.eulerAngles;
        _compassPointer.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, position);
    }


    // ------------ PRIVATE METHODS FOR LIGHT ------------

    private Constants.RelTime GetRelativeTime(CrystalBehaviour crystal)
    {
        float yourFrame = _timeLord.GetYourFrame();
        float curentFrame = _timeLord.GetCurrentFrame();
        float crystalStart = crystal.existanceRange.x;
        float crystalEnd = crystal.existanceRange.y;
        if (crystalStart < curentFrame)
        {
            if (crystalEnd < yourFrame) return Constants.RelTime.Behind;
            if (yourFrame < crystalStart) return Constants.RelTime.Ahead;
        }
        return Constants.RelTime.Same;
    }

    private void SetButtonMaterial(Constants.RelTime relativeTime)
    {
        _buttonRenderer.material = _buttonColours[relativeTime];
        _arrowRenderer.material = _arrowColours[relativeTime];

        float angle = 0f;
        if (relativeTime == Constants.RelTime.Ahead) angle = 180f;
        Vector3 eulerRotation = _buttonArrow.transform.rotation.eulerAngles;
        _buttonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, angle);

        _buttonOffLine.SetActive(relativeTime == Constants.RelTime.Same);
        _buttonArrow.SetActive(relativeTime != Constants.RelTime.Same);
    }
}