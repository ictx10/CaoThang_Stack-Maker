﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    PlayerController _playerController;

    [Header("UI Slider Controller")]
    public Image slider;
    private float fillAmount = 0f;
    public int maxFillAmount = 20;
    [SerializeField] private RectTransform _childSlider;


    [Header("UI Data Display")]
    [SerializeField] private GameObject _loadPanelObject;
    [SerializeField] private Image _sliderLoading;
    private int _timeToLoad = 3;
    [SerializeField] private RectTransform _childSliderLoading;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _coinText;

    public float FillAmount
    {
        get => fillAmount;
        set
        {
            SetFillAmount(value);
        }
    }

    void Start()
    {
        if (slider != null)
        {
            slider.fillAmount = FillAmount;
        }

        UpdateUIData();
        StartCoroutine(LoadSceneAsync());
    }

    // Cập nhật dữ liệu UI từ LevelManager
    private void UpdateUIData()
    {
        if (_playerController == null)
        {
            _playerController = FindObjectOfType<PlayerController>();
        }
        if (LevelManager.Instance != null)
        {
            int currentLevel = LevelManager.Instance.GetCurrentLevel();
            int currentScore = LevelManager.Instance.GetCurrentScore();
            ShowDataUI(currentLevel, currentScore);
        }
        else
        {
            Debug.LogError("Can't find LevelManager");
        }
    }

    // Method to set fillAmount externally (e.g., from another script or UI input)
    public void SetFillAmount(float newAmount)
    {
        if (slider == null || _childSlider == null)
        {
            return;
        }
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;
        fillAmount = Mathf.Clamp01(newAmount / maxFillAmount);
        if (slider != null)
        {
            slider.fillAmount = fillAmount;
            _childSlider.anchoredPosition = new Vector2(sliderWidth * fillAmount, _childSlider.anchoredPosition.y);
        }
    }

    //Show data to UI
    public void ShowDataUI(int level, int score)
    {
        if (_levelText != null)
        {
            _levelText.text = "LEVEL " + level.ToString();
        }
        else
        {
            Debug.LogWarning("LevelText is not assigned.");
        }

        if (_coinText != null)
        {
            _coinText.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("CoinText is not assigned.");
        }
    }

    //Showing Loadscreen
    IEnumerator LoadSceneAsync()
    {
        float elapsedTime = 0f;
        //turn off playercontroller while loading sceen
        _playerController.enabled = false;
        _loadPanelObject.SetActive(true);
        while (elapsedTime < _timeToLoad)
        {
            elapsedTime += Time.deltaTime;
            float fillAmountLoading = Mathf.Clamp01(elapsedTime / _timeToLoad);
            _sliderLoading.fillAmount = fillAmountLoading;
            _childSliderLoading.anchoredPosition = new Vector2(_sliderLoading.rectTransform.rect.width * fillAmountLoading, _childSliderLoading.anchoredPosition.y);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        _loadPanelObject.SetActive(false);
        //turn on playercontroller after loaded screen
        _playerController.enabled = true;
    }
}