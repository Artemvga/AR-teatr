// Assets/Scripts/RotationTrainerWithUI.cs
using UnityEngine;
using TMPro;

public class RotationTrainerWithUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text instructionText; // ← сюда перетащи TMP Text из Canvas

    [Header("Настройки")]
    public float rotationSensitivity = 0.5f;
    public string startMessage = "Зажмите кубик и поверните его на 360°.";
    public string completedMessage = "Отлично! Обучение завершено!";

    private float totalAbsoluteRotation = 0f;
    private bool isDragging = false;
    private Vector2 lastTouchPosition;
    private Camera mainCamera;
    private bool trainingCompleted = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main camera not found! Tag it as 'MainCamera'.");

        if (GetComponent<Collider>() == null)
            Debug.LogError("Объект должен иметь Collider!");

        // Показываем стартовое сообщение
        UpdateInstructionText();
    }

    void Update()
    {
        if (trainingCompleted) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f) && hit.collider.gameObject == gameObject)
            {
                HandleTouch(touch);
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
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    float delta = touch.position.x - lastTouchPosition.x;
                    float rotationDelta = delta * rotationSensitivity;

                    transform.Rotate(Vector3.up, rotationDelta, Space.World);
                    totalAbsoluteRotation += Mathf.Abs(rotationDelta);
                    lastTouchPosition = touch.position;

                    UpdateInstructionText(); // обновляем прогресс

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

    void UpdateInstructionText()
    {
        if (instructionText == null) return;

        if (trainingCompleted)
        {
            instructionText.text = completedMessage;
        }
        else
        {
            int degrees = Mathf.Clamp((int)totalAbsoluteRotation, 0, 360);
            instructionText.text = $"Зажмите кубик и поверните его на 360°.\n{degrees}° / 360°";
        }
    }

    void CompleteTraining()
    {
        if (trainingCompleted) return;

        trainingCompleted = true;
        UpdateInstructionText();
    }
}