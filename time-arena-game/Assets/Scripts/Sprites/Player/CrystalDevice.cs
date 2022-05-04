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
    private Dictionary<int, Material> _buttonColours;
    private Dictionary<int, Material> _arrowColours;
    private bool _active;


    // ------------ UNITY METHODS ------------

    void Awake()
    {
        _buttonColours = new Dictionary<int, Material> {
            {0, _buttonBackwardMat},
            {1, _buttonOffMat},
            {2, _buttonForwardMat}
        };

        _arrowColours = new Dictionary<int, Material> {
            {0, _buttonArrowBackwardMat},
            {1, _offLineMat},
            {2, _buttonArrowForwardMat},
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
            SetButtonMaterial(1);
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
                int relativeTime = GetRelativeTime(crystal);
                SetButtonMaterial(relativeTime);
            }
            else SetButtonMaterial(1);
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

    private int GetRelativeTime(CrystalBehaviour crystal)
    {
        float yourFrame = _timeLord.GetYourPerceivedFrame();
        float crystalStart = crystal.ExistanceRange.x * Constants.FrameRate;
        float crystalEnd = crystal.ExistanceRange.y * Constants.FrameRate;

        if (crystalEnd < yourFrame) return 0;
        if (yourFrame < crystalStart) return 2;
        return 1;
    }

    private void SetButtonMaterial(int relativeTime)
    {
        _buttonRenderer.material = _buttonColours[relativeTime];
        _arrowRenderer.material = _arrowColours[relativeTime];

        float angle = 0f;
        if (relativeTime == 2) angle = 180f;
        Vector3 eulerRotation = _buttonArrow.transform.rotation.eulerAngles;
        _buttonArrow.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, angle);

        _buttonOffLine.SetActive(relativeTime == 1);
        _buttonArrow.SetActive(relativeTime != 1);
    }
}