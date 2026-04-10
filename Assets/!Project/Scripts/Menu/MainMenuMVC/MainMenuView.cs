using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace RPGProject
{
    public class MainMenuView : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button peacefulGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button backFromSettingsButton;

        [Header("Panels")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Other")]
        [SerializeField] private AudioSource buttonClickSound;

        private MainMenuController controller;

        private const int initialFontSize = 60;
        private const int highlightFontSize = 65;

        public void Initialize(MainMenuController controller)
        {
            this.controller = controller;

            AddEventListeners();
        }

        public void SetActiveSettingsPanel(bool isActive)
        {
            settingsPanel.SetActive(isActive);
        }

        public void PlayButtonClickSound() => buttonClickSound.Play();

        private void AddEventListeners()
        {
            newGameButton.onClick.AddListener(() => controller.OnNewGame());
            AddPointerEvents(newGameButton.gameObject, newGameButton.transform.GetComponentInChildren<TextMeshProUGUI>());

            loadGameButton.onClick.AddListener(() => controller.OnLoadGame());
            AddPointerEvents(loadGameButton.gameObject, loadGameButton.transform.GetComponentInChildren<TextMeshProUGUI>());

            peacefulGameButton.onClick.AddListener(() => controller.OnPeacefulGame());
            AddPointerEvents(peacefulGameButton.gameObject, peacefulGameButton.transform.GetComponentInChildren<TextMeshProUGUI>());

            settingsButton.onClick.AddListener(() => controller.OnOpenSettings());
            AddPointerEvents(settingsButton.gameObject, settingsButton.transform.GetComponentInChildren<TextMeshProUGUI>());

            quitButton.onClick.AddListener(() => controller.OnGameQuit());
            AddPointerEvents(quitButton.gameObject, quitButton.transform.GetComponentInChildren<TextMeshProUGUI>());

            backFromSettingsButton.onClick.AddListener(() => controller.OnCloseSettings());
            AddPointerEvents(backFromSettingsButton.gameObject, backFromSettingsButton.transform.GetComponentInChildren<TextMeshProUGUI>());
        }

        private void AddPointerEvents(GameObject button, TextMeshProUGUI textComponent)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = button.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => HighlightText(textComponent));
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => UnhighlightText(textComponent));
            trigger.triggers.Add(exitEntry);
        }

        private void HighlightText(TextMeshProUGUI textComponent)
        {
            textComponent.fontSize = highlightFontSize;
        }

        private void UnhighlightText(TextMeshProUGUI textComponent)
        {
            textComponent.fontSize = initialFontSize;
        }
    }
}