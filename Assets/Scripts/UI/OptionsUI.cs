using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button defaultButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button changeMoveUpButton;
    [SerializeField] private Button changeMoveDownButton;
    [SerializeField] private Button changeMoveLeftButton;
    [SerializeField] private Button changeMoveRightButton;
    [SerializeField] private Button changeInteractButton;
    [SerializeField] private Button changeInteractAltButton;
    [SerializeField] private Button changeGamepadInteractButton;
    [SerializeField] private Button changeGamepadInteractAltButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI changeMoveUpText;
    [SerializeField] private TextMeshProUGUI changeMoveDownText;
    [SerializeField] private TextMeshProUGUI changeMoveLeftText;
    [SerializeField] private TextMeshProUGUI changeMoveRightText;
    [SerializeField] private TextMeshProUGUI changeInteractText;
    [SerializeField] private TextMeshProUGUI changeInteractAltText;
    [SerializeField] private TextMeshProUGUI changeGamepadInteractText;
    [SerializeField] private TextMeshProUGUI changeGamepadInteractAltText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction;

    private void Awake()
    {
        Instance = this;

        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => {
            Hide();
            onCloseButtonAction();
        });
        changeMoveUpButton.onClick.AddListener(() => {  RebindBinding(GameInput.Binding.Move_Up);   });
        changeMoveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Down); });
        changeMoveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        changeMoveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        changeInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        changeInteractAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact_Alternate); });
        changeGamepadInteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact); });
        changeGamepadInteractAltButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Gamepad_Interact_Alternate); });

        defaultButton.onClick.AddListener(() =>
        {
            GameInput.Instance.DefaultBindings();
            UpdateVisual();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        UpdateVisual();

        HidePressToRebindKey();
        Hide();
    }

    private void GameManager_OnGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects Volume: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music Volume: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        changeMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        changeMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        changeMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        changeMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        changeInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        changeInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        changeGamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        changeGamepadInteractAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact_Alternate);
    }

    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;

        gameObject.SetActive(true);

        closeButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
