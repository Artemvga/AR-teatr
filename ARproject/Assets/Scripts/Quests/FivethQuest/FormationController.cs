using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FormationController : MonoBehaviour
{
    [Header("Куклы")]
    public List<Transform> dolls = new List<Transform>();

    [Header("Точки назначения (по порядку)")]
    public List<Transform> targetPoints = new List<Transform>();

    [Header("Панель, которая появляется после построения")]
    public GameObject resultPanel;

    [Header("Настройки анимации")]
    public float moveDuration = 1f;
    public float rotationDuration = 0.8f;
    public Ease moveEase = Ease.OutBack;
    public Ease rotationEase = Ease.OutSine;

    private bool isFormationComplete = false;
    private bool isAnimating = false; // <<< НОВЫЙ ФЛАГ
    private int nextTargetIndex = 0;

    // Отслеживаем, какие кубики уже заняты
    private Dictionary<Transform, bool> dollOccupied = new Dictionary<Transform, bool>();

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
        else
            Debug.LogWarning("ResultPanel не назначена.");

        foreach (var doll in dolls)
        {
            dollOccupied[doll] = false;
            var handler = doll.GetComponent<DollClickHandler>();
            if (handler == null)
                handler = doll.gameObject.AddComponent<DollClickHandler>();
            handler.formationController = this;
        }
    }

    public void OnDollClicked(Transform clickedDoll)
    {
        if (isFormationComplete) return;

        // <<< БЛОКИРУЕМ КЛИКИ ВО ВРЕМЯ АНИМАЦИИ
        if (isAnimating)
        {
            Debug.Log("Подождите, идет анимация другого кубика.");
            return;
        }

        if (!dolls.Contains(clickedDoll))
        {
            Debug.LogError("Кликнутый кубик не в списке dolls!");
            return;
        }

        if (dollOccupied[clickedDoll])
        {
            Debug.Log("Кубик " + clickedDoll.name + " уже занял позицию.");
            return;
        }

        if (nextTargetIndex >= targetPoints.Count)
        {
            Debug.LogWarning("Все точки уже заняты.");
            return;
        }

        // <<< ВКЛЮЧАЕМ БЛОКИРОВКУ
        isAnimating = true;

        Transform target = targetPoints[nextTargetIndex];
        dollOccupied[clickedDoll] = true;

        clickedDoll.DOKill();

        Vector3 targetPos = target.position;
        Quaternion targetRot = Quaternion.LookRotation(target.forward, Vector3.up);

        // Анимации
        clickedDoll.DOMove(targetPos, moveDuration).SetEase(moveEase);
        clickedDoll.DORotateQuaternion(targetRot, rotationDuration).SetEase(rotationEase);

        // <<< ОТКЛЮЧАЕМ БЛОКИРОВКУ после завершения
        DOVirtual.DelayedCall(moveDuration + 0.1f, () =>
        {
            isAnimating = false;
            nextTargetIndex++;

            if (nextTargetIndex >= targetPoints.Count)
            {
                isFormationComplete = true;
                ShowResultPanel();
            }
        });
    }

    private void ShowResultPanel()
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        CanvasGroup canvasGroup = resultPanel.GetComponent<CanvasGroup>()
            ?? resultPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutElastic);

        RectTransform rect = resultPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero;
            rect.DOScale(1f, 0.7f).SetEase(Ease.OutBack);
        }
    }
}