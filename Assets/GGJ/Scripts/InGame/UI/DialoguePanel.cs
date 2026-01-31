using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GGJ.Core;
using GGJ.InGame.Events;
using GGJ.InGame.NPC;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// NPC会話時に表示するパネル
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button accuseButton;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [Header("タイプライター設定")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float characterDelay = 0.05f;
        
        private INpc currentNPC;
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
            
            if (hintButton != null)
                hintButton.onClick.RemoveListener(OnHintButtonClicked);
            
            if (accuseButton != null)
                accuseButton.onClick.RemoveListener(OnAccuseButtonClicked);
        }

        private void OnNPCInteractionStarted(GameObject npc)
        {
            if (panel == null) return;

            // NPCデータを取得
            currentNPC = npc.GetComponent<INpc>();
            
            // 先にパネルをアクティブにする
            panel.SetActive(true);
            UIHelper.SetFirstSelected(closeButton);
            
            // その後でテキスト表示（コルーチン開始）
            if (currentNPC != null)
            {
                DisplayNPCInfo(currentNPC);
            }
            
            UpdateHintButton();
        }
        
        /// <summary>
        /// NPCの会話テキストをUIに表示
        /// </summary>
        private void DisplayNPCInfo(INpc npc)
        {
            if (dialogueText == null) return;
            
            string dialogue = npc.GetCurrentDialogue();
            
            Debug.Log($"[DialoguePanel] DisplayNPCInfo: useTypewriterEffect={useTypewriterEffect}, dialogue length={dialogue.Length}");
            
            // 既存のタイプライターコルーチンを停止
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            // タイプライター効果を使用する場合
            if (useTypewriterEffect)
            {
                Debug.Log("[DialoguePanel] タイプライター効果を開始します");
                typewriterCoroutine = StartCoroutine(
                    TextTypewriterEffect.TypeTextSkippable(
                        this,
                        dialogueText,
                        dialogue,
                        characterDelay,
                        () => Input.anyKeyDown // 任意のキーでスキップ
                    )
                );
            }
            else
            {
                Debug.Log("[DialoguePanel] タイプライター効果なしで即座に表示します");
                dialogueText.text = dialogue;
            }
        }

        private void OnNPCInteractionEnded()
        {
            if (panel == null) return;
            
            currentNPC = null;
            
            // タイプライターコルーチンを停止
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            panel.SetActive(false);
            UIHelper.ClearSelected();
        }

        private void OnCloseButtonClicked() => GameEvents.RaiseNPCInteractionEnded();
        
        /// <summary>
        /// ヒントボタンがクリックされた
        /// </summary>
        private void OnHintButtonClicked()
        {
            if (currentNPC == null) return;
            
            // ヒント受信フラグを立てる
            currentNPC.SetHintReceived(true);
            
            // セリフを更新（ヒント用のセリフに変わる）
            DisplayNPCInfo(currentNPC);
            
            // ボタン状態を更新
            UpdateHintButton();
        }
        
        /// <summary>
        /// 犯人指定ボタンがクリックされた
        /// </summary>
        private void OnAccuseButtonClicked()
        {
            if (currentNPC == null) return;
            
            // ヒント受信フラグを立てて犯人指定状態にする
            currentNPC.SetHintReceived(true);
            
            // セリフを更新（犯人指定用のセリフに変わる）
            DisplayNPCInfo(currentNPC);
            
            // ボタン状態を更新
            UpdateHintButton();
        }
        
        /// <summary>
        /// ヒントボタンの表示状態を更新
        /// </summary>
        private void UpdateHintButton()
        {
            if (hintButton == null || currentNPC == null) return;
            
            // ヒント未取得なら有効、取得済みなら無効
            hintButton.interactable = !currentNPC.HasReceivedHint();
        }
    }
}
