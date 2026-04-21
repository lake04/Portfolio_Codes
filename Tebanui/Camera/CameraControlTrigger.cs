using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;
using Unity.VisualScripting;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;

    private Collider2D coll;

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            Vector2 exitDir = (collision.transform.position - coll.bounds.center).normalized;

            if(customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDir);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }

}



[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTigger;

    private void OnEnable()
    {
        cameraControlTigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(cameraControlTigger.customInspectorObjects.swapCameras)
        {
            cameraControlTigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left",cameraControlTigger.customInspectorObjects.cameraOnLeft,
                typeof(CinemachineVirtualCamera),true) as CinemachineVirtualCamera;

            cameraControlTigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTigger.customInspectorObjects.cameraOnRight,
                typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if(cameraControlTigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Distance", 
                cameraControlTigger.customInspectorObjects.panDirection);


            cameraControlTigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTigger.customInspectorObjects.panDistance);
            cameraControlTigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTigger.customInspectorObjects.panTime);
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTigger);
        }
    }
}
#endif