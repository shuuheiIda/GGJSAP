using UnityEngine;
using UnityEngine.UI;
using GGJ.InGame.Events;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// NPC会話時に表示するパネル
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            Debug.Log("[DialoguePanel] Start called");
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
                Debug.Log("[DialoguePanel] Close button listener registered");
            }
            else
            {
                Debug.LogError("[DialoguePanel] Close button is null!");
            }
            
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnNPCInteractionStarted += OnNPCInteractionStarted;
            GameEvents.OnNPCInteractionEnded += OnNPCInteractionEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnNPCInteractionStarted -= OnNPCInteractionStarted;
            GameEvents.OnNPCInteractionEnded -= OnNPCInteractionEnded;
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        private void OnNPCInteractionStarted(GameObject npc)
        {
            if (panel == null) return;

            panel.SetActive(true);
            UIHelper.SetFirstSelected(closeButton);
        }

        private void OnNPCInteractionEnded()
        {
            if (panel == null) return;

            panel.SetActive(false);
            UIHelper.ClearSelected();
        }

        private void OnCloseButtonClicked()
        {
            Debug.Log("[DialoguePanel] Close button clicked!");
            GameEvents.RaiseNPCInteractionEnded();
        }
    }
}
