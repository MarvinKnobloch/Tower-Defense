using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineInputProvider inputProvider;

    private Vector2 cammeraDirection;
    [SerializeField] private float cameraMoveSpeed;

    [SerializeField] private float scrollInMax;
    [SerializeField] private float scrollOutMax;
    [SerializeField] private float scrollSpeed;

    private Camera maincam;
    private bool resetXPosi;
    private bool resetYPosi;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        maincam = Camera.main;
    }
    void Update()
    {
        MoveCamera();
        Scroll();
    }
    private void MoveCamera()
    {
        float x = inputProvider.GetAxisValue(0);
        float y = inputProvider.GetAxisValue(1);

        if (x == 0 && y == 0) return;

        SetDircetion(x, y);

        virtualCamera.VirtualCameraGameObject.transform.position = Vector3.Lerp(transform.position, transform.position + (Vector3)cammeraDirection, Time.deltaTime * cameraMoveSpeed);
    }
    private void SetDircetion(float x, float y)
    {
        float minValue = 0.01f;
        float maxValue = 0.99f;
        cammeraDirection = Vector2.zero;
        if (x >= Screen.width * maxValue)
        {
            if (resetXPosi == false)
            {
                transform.position = new Vector3(maincam.transform.position.x, transform.position.y, -1);
                resetXPosi = true;
            }
            cammeraDirection.x = 1;
        }
        else if (x <= Screen.width * minValue)
        {
            if (resetXPosi == true)
            {
                transform.position = new Vector3(maincam.transform.position.x, transform.position.y, -1);
                resetXPosi = false;
            }
            cammeraDirection.x = -1;
        }

        if (y >= Screen.height * maxValue)
        {
            if (resetYPosi == false)
            {
                transform.position = new Vector3(transform.position.x, maincam.transform.position.y, -1);
                resetYPosi = true;
            }
            cammeraDirection.y = 1;
        }
        else if (y <= Screen.height * minValue)
        {
            if (resetYPosi == true)
            {
                transform.position = new Vector3(transform.position.x, maincam.transform.position.y, -1);
                resetYPosi = false;
            }
            cammeraDirection.y = -1;
        }
    }
    private void Scroll()
    {
        float z = inputProvider.GetAxisValue(2);
        if (z == 0) return;

        float camDistance = virtualCamera.m_Lens.OrthographicSize;
        float targetCamDistance = Mathf.Clamp(camDistance + z, scrollOutMax, scrollInMax);
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(camDistance, targetCamDistance, Time.deltaTime * scrollSpeed);
    }
}
