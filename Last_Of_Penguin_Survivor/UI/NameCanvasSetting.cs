using UnityEngine;

public class NameCanvasSetting : MonoBehaviour
{
    public float rotationSpeed = 5f;

    [Header("Scale Settings")]
    public float minScale = 0.8f;    
    public float maxScale = 2.0f;    
    public float scaleFactor = 0.1f;

    private void LateUpdate()
    {
        if (CharacterController.Instance == null || CharacterController.Instance.MainCam == null) return;
        Camera currentCam = CharacterController.Instance.MainCam;

        Vector3 lookDir = currentCam.transform.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        float distance = Vector3.Distance(currentCam.transform.position, transform.position);

        float currentScale = distance * scaleFactor;

        currentScale = Mathf.Clamp(currentScale, minScale, maxScale);

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }
}