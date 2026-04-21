using UnityEngine;
using UnityEngine.Rendering;

public class MinimapCameraController : MonoBehaviour
{
    public Transform characterPosition;
    public float height = 100f;

    public float heightMax;
    public float heightMin;
    public float wheelSpeed;

    public Quaternion rotationDefault = Quaternion.Euler(90f, 0, 0f);

    private bool _initialFogState;
    private Camera _minimapCamera;

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        RenderPipelineManager.endCameraRendering += OnEndCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
        RenderPipelineManager.endCameraRendering -= OnEndCamera;
    }

    private void Start()
    {
        _minimapCamera = GetComponent<Camera>();

        if (TryResolveCharacter(out Transform characterPos))
        {
            characterPosition = characterPos;
        }
        else
        {
            Debug.Log("Character Controller를 찾을 수가 없음");
            return;
        }
    }

    private void LateUpdate()
    {
        if (characterPosition == null)
        {
            if (!TryResolveCharacter(out characterPosition)) return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && Input.GetKey(KeyCode.LeftControl))
        {
            height -= scroll * wheelSpeed;
            height = Mathf.Clamp(height, heightMin, heightMax);
        }

        if (characterPosition != null)
        {
            transform.position = new Vector3(characterPosition.position.x, height, characterPosition.position.z);
            transform.rotation = rotationDefault;
        }
    }

    private bool TryResolveCharacter(out Transform target)
    {
        target = null;

        if (GameManager.Instance == null) return false;
        if (GameManager.Instance.characterController == null) return false;

        target = GameManager.Instance.characterController.transform;
        return target != null;
    }

    private void OnBeginCamera(ScriptableRenderContext context, Camera camera)
    {
        if (camera == _minimapCamera)
        {
            _initialFogState = RenderSettings.fog;
            RenderSettings.fog = false;
        }
    }

    private void OnEndCamera(ScriptableRenderContext context, Camera camera)
    {
        if (camera == _minimapCamera)
        {
            RenderSettings.fog = _initialFogState;
        }
    }
}
