using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GGJ.Core;
using GGJ.InGame.Events;
using GGJ.InGame.NPC;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// Npc会話時に表示するパネル
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button accuseButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [Header("タイプライター設定")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float characterDelay = 0.05f;
        
        private INpc currentNpc;
        private Coroutine typewriterCoroutine;

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            
            if (hintButton != null)
                hintButton.onClick.AddListener(OnHintButtonClicked);
            
            if (accuseButton != null)
                accuseButton.onClick.AddListener(OnAccuseButtonClicked);
            
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnNpcInteractionStarted += OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded += OnNpcInteractionEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnNpcInteractionStarted -= OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded -= OnNpcInteractionEnded;
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            
            if (hintButton != null)
                hintButton.onClick.RemoveListener(OnHintButtonClicked);
            
            if (accuseButton != null)
                accuseButton.onClick.RemoveListener(OnAccuseButtonClicked);
        }

        private void OnNpcInteractionStarted(GameObject npc)
        {
            if (panel == null) return;

            currentNpc = npc.GetComponent<INpc>();
            panel.SetActive(true);
            UIHelper.SetFirstSelected(closeButton);
            
            if (currentNpc != null)
                DisplayNpcInfo(currentNpc);
            
            UpdateHintButton();
        }
        
        /// <summary>
        /// Npcの会話テキストをUIに表示
        /// </summary>
        private void DisplayNpcInfo(INpc npc)
        {
            if (dialogueText == null) return;
            
            string dialogue = npc.GetCurrentDialogue();
            
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            if (useTypewriterEffect)
            {
                typewriterCoroutine = StartCoroutine(
                    TextTypewriterEffect.TypeTextSkippable(
                        this,
                        dialogueText,
                        dialogue,
                        characterDelay,
                        () => Input.anyKeyDown
                    )
                );
            }
            else
            {
                dialogueText.text = dialogue;
            }
        }

        private void OnNpcInteractionEnded()
        {
            if (panel == null) return;
            
            currentNpc = null;
            
            // 繧ｿ繧､繝励Λ繧､繧ｿ繝ｼ繧ｳ繝ｫ繝ｼ繝√Φ繧貞●豁｢
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            panel.SetActive(false);
            UIHelper.ClearSelected();
        }

        private void OnCloseButtonClicked() => GameEvents.RaiseNpcInteractionEnded();
        
        /// <summary>
        /// ヒントボタンがクリックされた
        /// </summary>
        private void OnHintButtonClicked()
        {
            if (currentNpc == null) return;
            
            currentNpc.SetHintReceived(true);
            DisplayNpcInfo(currentNpc);
            UpdateHintButton();
        }
        
        /// <summary>
        /// 犯人指定ボタンがクリックされた
        /// </summary>
        private void OnAccuseButtonClicked()
        {
            if (currentNpc == null) return;
            
            // 告発フラグを立てる
            currentNpc.SetAccused(true);
            DisplayNpcInfo(currentNpc);
            UpdateHintButton();
        }
        
        /// <summary>
        /// ヒントボタンの表示状態を更新
        /// </summary>
        private void UpdateHintButton()
        {
            if (hintButton == null || currentNpc == null) return;
            
            hintButton.interactable = !currentNpc.HasReceivedHint();
        }
    }
}
