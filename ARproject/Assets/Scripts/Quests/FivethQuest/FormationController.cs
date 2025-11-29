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

    private void Start()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
        else
            Debug.LogWarning("ResultPanel не назначена в инспекторе.");
    }

    public void OnDollClicked()
    {
        if (isFormationComplete)
        {
            // Игнорировать клики после завершения построения
            return;
        }

        ArrangeDollsInFormation();
    }

    private void ArrangeDollsInFormation()
    {
        if (dolls.Count != targetPoints.Count)
        {
            Debug.LogError("Количество кукол не совпадает с количеством точек назначения.");
            return;
        }

        isFormationComplete = true;

        // Отключаем обработчики кликов у всех кукол
        foreach (var doll in dolls)
        {
            var handler = doll.GetComponent<DollClickHandler>();
            if (handler != null)
                handler.enabled = false;
        }

        // Анимируем каждую куклу
        for (int i = 0; i < dolls.Count; i++)
        {
            Transform doll = dolls[i];
            Transform target = targetPoints[i];

            Vector3 targetPos = target.position;
            Quaternion targetRot = Quaternion.LookRotation(target.forward, Vector3.up);

            doll.DOKill();

            doll.DOMove(targetPos, moveDuration).SetEase(moveEase);
            doll.DORotateQuaternion(targetRot, rotationDuration).SetEase(rotationEase);
        }

        // Отложенный вызов показа панели после завершения анимации
        Invoke(nameof(ShowResultPanel), moveDuration + 0.2f);
    }

    private void ShowResultPanel()
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        // Анимация появления через прозрачность
        CanvasGroup canvasGroup = resultPanel.GetComponent<CanvasGroup>()
            ?? resultPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutElastic);

        // Анимация масштаба
        RectTransform rect = resultPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = Vector3.zero;
            rect.DOScale(1f, 0.7f).SetEase(Ease.OutBack);
        }
    }
}