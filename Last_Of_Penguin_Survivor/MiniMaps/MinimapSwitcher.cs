using GlobalAudio;
using UnityEngine;

public class MinimapSwitcher : Popup<MinimapSwitcher>
{
    public override PopupType PopupType => PopupType.MiniMapSwitcher;
    public static MinimapSwitcher Instance;

    public GameObject bigMap;
    public GameObject minimap;
    public Camera minimapCamera;

    [SerializeField] private GameObject bigIcon;
    [SerializeField] private GameObject miniIcon;

    [SerializeField] private AudioClip clickClip;

    [SerializeField] private float bigMapSize = 100f;
    [SerializeField] private float miniMapSize = 60f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bigMap.SetActive(false);
        minimap.SetActive(true);
        
        bigIcon.SetActive(false);
        miniIcon.SetActive(true);
    }

    public void ToggleMinimap(bool miniMapSwitch)
    {
        bigMap.SetActive(miniMapSwitch);
        minimap.SetActive(!miniMapSwitch);

        bigIcon.SetActive(miniMapSwitch);
        miniIcon.SetActive(miniMapSwitch);
    }

    protected override void OnShow(PopupArgument popupArguments)
    {
        AudioManager.Instance.PlaySFX(clickClip);
        bigMap.SetActive(true);
        minimap.SetActive(false);

        bigIcon.SetActive(true);
        miniIcon.SetActive(false);
        minimapCamera.orthographicSize = bigMapSize;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void Show()
    {
        OnShow(new PopupArgument());
    }

    public void HideMap()
    {
        OnHide();
    }

    protected override void OnHide()
    {
        AudioManager.Instance.PlaySFX(clickClip);
        bigMap.SetActive(false);
        minimap.SetActive(true);

        bigIcon.SetActive(false);
        miniIcon.SetActive(true);

        minimapCamera.orthographicSize = miniMapSize;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

}