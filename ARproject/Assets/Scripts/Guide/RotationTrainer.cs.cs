// Assets/Scripts/RotationTrainer.cs
using UnityEngine;

public class RotationTrainer : MonoBehaviour
{
    [Header("Настройки")]
    public float rotationSensitivity = 0.5f; // чем выше — тем чувствительнее вращение
    public bool debugLog = true;

    private float totalAbsoluteRotation = 0f;
    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private Camera mainCamera;
    private bool trainingCompleted = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found! Assign a camera with tag 'MainCamera'.");
        }

        // Убедимся, что у объекта есть Collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError("RotationTrainer: объект должен иметь Collider!");
        }
    }

    void Update()
    {
        if (trainingCompleted) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Преобразуем позицию касания в луч
            Ray ray = mainCamera.ScreenPointToRay(touch.position);

            // Проверяем, попадает ли луч в этот объект
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    HandleTouch(touch);
                }
            }
        }
    }

    void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                isDragging = true;
                lastTouchPosition = touch.position;
                if (debugLog) Debug.Log("Начато вращение кубика");
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    Vector2 delta = touch.position - lastTouchPosition;
                    float rotationDelta = delta.x * rotationSensitivity;

                    // Вращаем объект вокруг вертикальной оси
                    transform.Rotate(Vector3.up, rotationDelta, Space.World);

                    // Считаем АБСОЛЮТНУЮ сумму поворота (всегда положительная)
                    totalAbsoluteRotation += Mathf.Abs(rotationDelta);

                    lastTouchPosition = touch.position;

                    if (debugLog) Debug.Log($"Вращение: {totalAbsoluteRotation:F1}°");

                    // Проверяем завершение: 360° или больше
                    if (totalAbsoluteRotation >= 360f)
                    {
                        CompleteTraining();
                    }
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }

    void CompleteTraining()
    {
        if (trainingCompleted) return;

        trainingCompleted = true;
        if (debugLog) Debug.Log("✅ Обучение вращению завершено! (360° достигнуто)");

        // Опционально: вызов события, уведомление менеджера и т.д.
        OnTrainingCompleted();
    }

    // Виртуальный метод — можно переопределить в дочернем классе
    protected virtual void OnTrainingCompleted()
    {
        // Например, активировать UI, проиграть звук и т.д.
    }
}