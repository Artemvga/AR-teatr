using UnityEngine;

public class RotateOnSwipe : MonoBehaviour
{
    [Header("Настройки вращения")]
    public float rotationSpeed = 2.0f; // Чувствительность (подбери опытным путём)

    [Header("Сброс")]
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;

    private Vector2 lastMousePosition;
    private Vector2 lastTouchPosition;
    private bool isDragging = false;

    void Update()
    {
        if (Application.isEditor || (!Application.isMobilePlatform && !SystemInfo.deviceType.ToString().Contains("Handheld")))
        {
            HandleMouseInput();
        }
        else
        {
            HandleTouchInput();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 current = Input.mousePosition;
            Vector2 delta = current - lastMousePosition;

            // Вращаем только если есть движение
            if (delta.sqrMagnitude > 0.01f)
            {
                RotateObject(delta);
            }

            lastMousePosition = current;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPosition;

                if (delta.sqrMagnitude > 0.1f)
                {
                    RotateObject(delta);
                }

                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    void RotateObject(Vector2 delta)
    {
        // Важно: используем World-оси для стабильного поведения
        float rotX = -delta.y * rotationSpeed * 0.1f; // вертикаль → поворот по X (наклон)
        float rotY = delta.x * rotationSpeed * 0.1f; // горизонталь → поворот по Y (поворот)

        transform.Rotate(Vector3.right, rotX, Space.World);
        transform.Rotate(Vector3.up, rotY, Space.World);
    }

    // === МЕТОД СБРОСА ===
    public void ResetRotation()
    {
        transform.eulerAngles = defaultRotation;
    }

    // Удобно вызывать из инспектора в Play Mode
    [ContextMenu("Сбросить вращение")]
    private void ResetRotationFromInspector()
    {
        ResetRotation();
    }
}