using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum IconType
{
    Character,
    Building
}

public class IconController : MonoBehaviour
{
    [SerializeField] private IconType iconType;
    [SerializeField] private SpriteRenderer icon;

    public Transform targetPosition;
    public float height;


    private void Start()
    {
        if (iconType == IconType.Character)
        {
            StartCoroutine(InitTarget());
        }
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(targetPosition.position.x, height, targetPosition.position.z);
        Quaternion roration = transform.rotation;
        switch(iconType)
        {
            case IconType.Character:
                transform.rotation = Quaternion.Euler(90, targetPosition.eulerAngles.y + 180f, 0); 
                break;

            case IconType.Building:
                transform.rotation = Quaternion.Euler(-90, targetPosition.eulerAngles.y + 180f, 0);
                break;
        }
      
    }

    public void Initialize(Transform target, Sprite iconSprite)
    {
        targetPosition = target;

        if (icon != null)
        {
            icon.sprite = iconSprite;
        }
    }

    public void SetTarget(Transform target)
    {
        targetPosition = target;
    }

    private IEnumerator InitTarget()
    {
        while (targetPosition == null)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.characterController != null)
            {
                targetPosition = GameManager.Instance.characterController.transform;
                yield break;
            }

            yield return null;
        }
    }
}
