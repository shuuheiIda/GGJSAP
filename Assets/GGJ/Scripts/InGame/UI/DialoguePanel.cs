using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using GGJ.Core;
using GGJ.InGame.Events;
using GGJ.InGame.NPC;
using GGJ.InGame.MiniGames;
using GGJ.Scene;
using System.Collections;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// Npc会話時に表示するパネル
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        [Header("会話パートの設定")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button accuseButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [Header("確認ダイアログ")]
        [SerializeField] private GameObject confirmDialogPanel;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;
        
        [Header("タイプライター設定")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float characterDelay = 0.05f;
        
        [Header("シーン遷移設定")]
        [SerializeField] private float delayBeforeSceneTransition = 2.0f;
        
        private INpc currentNpc;
        private Coroutine typewriterCoroutine;
        private bool canProcessClicks = false; // クリック処理可能フラグ
        private float panelOpenTime = 0f; // パネルを開いた時刻

        private void Awake()
        {
            // イベントリスナーを登録（Awakeで登録することで、MainGameUI非表示時も維持される）
            GameEvents.OnNpcInteractionStarted += OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded += OnNpcInteractionEnded;
            GameEvents.OnHintReceived += OnHintReceived;
        }

        private void Start()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            
            if (hintButton != null)
                hintButton.onClick.AddListener(OnHintButtonClicked);
            
            if (accuseButton != null)
                accuseButton.onClick.AddListener(OnAccuseButtonClicked);
            
            if (confirmYesButton != null)
                confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
            
            if (confirmNoButton != null)
                confirmNoButton.onClick.AddListener(OnConfirmNoClicked);
            
            if (panel != null)
                panel.SetActive(false);
            
            if (confirmDialogPanel != null)
                confirmDialogPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.OnNpcInteractionStarted -= OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded -= OnNpcInteractionEnded;
            GameEvents.OnHintReceived -= OnHintReceived;
            
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            
            if (hintButton != null)
                hintButton.onClick.RemoveListener(OnHintButtonClicked);
            
            if (accuseButton != null)
                accuseButton.onClick.RemoveListener(OnAccuseButtonClicked);
            
            if (confirmYesButton != null)
                confirmYesButton.onClick.RemoveListener(OnConfirmYesClicked);
            
            if (confirmNoButton != null)
                confirmNoButton.onClick.RemoveListener(OnConfirmNoClicked);
        }

        private void OnNpcInteractionStarted(GameObject npc)
        {
            if (panel == null) return;

            currentNpc = npc.GetComponent<INpc>();
            panel.SetActive(true);
            
            // パネルを開いた時刻を記録
            panelOpenTime = Time.unscaledTime;
            canProcessClicks = false;
            
            // ボタンを有効化（前回の会話で無効化されている可能性があるため）
            if (closeButton != null) closeButton.interactable = true;
            if (hintButton != null) hintButton.interactable = true;
            if (accuseButton != null) accuseButton.interactable = true;
            
            // 少し遅延してクリックを有効化
            StartCoroutine(EnableClicksAfterDelay());
            
            // 1フレーム遅延させてボタンフォーカスを設定（○ボタンの入力が残っているのを防ぐ）
            StartCoroutine(SetFirstSelectedDelayed());
            
            if (currentNpc != null)
                DisplayNpcInfo(currentNpc);
            
            UpdateHintButton();
        }
        
        /// <summary>
        /// 少し遅延してクリックを有効化（パネル開いた直後の誤クリックを防ぐ）
        /// </summary>
        private IEnumerator EnableClicksAfterDelay()
        {
            // 0.2秒待つ（マウスクリックのリリースを待つ）
            yield return new WaitForSecondsRealtime(0.2f);
            canProcessClicks = true;
        }
        
        /// <summary>
        /// 1フレーム遅延してボタンフォーカスを設定
        /// </summary>
        private IEnumerator SetFirstSelectedDelayed()
        {
            yield return null; // 1フレーム待つ
            UIHelper.SetFirstSelected(closeButton);
        }
        
        /// <summary>
        /// ヒント獲得時の処理（ミニゲームクリア後）
        /// </summary>
        private void OnHintReceived()
        {
            // 現在会話中のNPCのみにヒントを渡す
            if (currentNpc != null && panel != null)
            {
                // 会話中のNPCにヒント受信フラグを設定
                currentNpc.SetHintReceived(true);
                
                // パネルを再表示
                panel.SetActive(true);
                
                // 更新されたNPC情報を表示（ヒント付きの会話）
                DisplayNpcInfo(currentNpc);
                
                // ヒントボタンを更新（無効化される）
                UpdateHintButton();
                
                // ボタンフォーカスを設定
                UIHelper.SetFirstSelected(closeButton);
            }
        }
        
        /// <summary>
        /// Npcの会話テキストをUIに表示
        /// </summary>
        private void DisplayNpcInfo(INpc npc)
        {
            if (dialogueText == null)
            {
                Debug.LogError("[DialoguePanel] dialogueText is null!");
                return;
            }
            
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
                        () => Keyboard.current.anyKey.wasPressedThisFrame || 
                              (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
                    )
                );
            }
            else
            {
                Debug.Log("[DialoguePanel] Setting text directly (no typewriter)");
                dialogueText.text = dialogue;
                Debug.Log($"[DialoguePanel] After setting, dialogueText.text = {dialogueText.text}");
            }
        }

        private void OnNpcInteractionEnded()
        {
            if (panel == null) return;
            
            currentNpc = null;
            
            // タイプライターコルーチンを停止
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            panel.SetActive(false);
            UIHelper.ClearSelected();
        }
        
        /// <summary>
        /// ポーズから復帰した際にフォーカスを再設定する
        /// </summary>
        public void RestoreFocusAfterPause()
        {
            // 会話パネルが表示されていない場合は何もしない
            if (panel == null || !panel.activeSelf) return;
            
            // 確認ダイアログが表示されている場合
            if (confirmDialogPanel != null && confirmDialogPanel.activeSelf)
            {
                // ボタンを再度有効化
                if (confirmYesButton != null) confirmYesButton.interactable = true;
                if (confirmNoButton != null) confirmNoButton.interactable = true;
                
                UIHelper.SetFirstSelected(confirmNoButton);
            }
            // 通常の会話パネルの場合
            else
                UIHelper.SetFirstSelected(closeButton);
        }
        
        /// <summary>
        /// ポーズ時にボタンを無効化する
        /// </summary>
        public void DisableButtonsForPause()
        {
            // 会話パネルが表示されていない場合は何もしない
            if (panel == null || !panel.activeSelf) return;
            
            // 確認ダイアログが表示されている場合、そのボタンを無効化
            if (confirmDialogPanel != null && confirmDialogPanel.activeSelf)
            {
                if (confirmYesButton != null) confirmYesButton.interactable = false;
                if (confirmNoButton != null) confirmNoButton.interactable = false;
            }
        }

        private void OnCloseButtonClicked()
        {
            // パネルを開いた直後のクリックを無視
            if (!canProcessClicks) return;
            
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            GameEvents.RaiseNpcInteractionEnded();
        }
        
        /// <summary>
        /// ヒントボタンがクリックされた
        /// </summary>
        private void OnHintButtonClicked()
        {
            // パネルを開いた直後のクリックを無視
            if (!canProcessClicks) return;
            
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            if (currentNpc == null)
                return;
            
            // すでにヒントを受け取っている場合は何もしない
            if (currentNpc.HasReceivedHint())
                return;
            
            // MiniGameManagerが見つからない場合
            if (MiniGameManager.I == null)
            {
                Debug.LogError("[DialoguePanel] MiniGameManagerが見つかりません！シーンにMiniGameManagerを配置してください");
                // フォールバック: 直接ヒントを渡す（デバッグ用）
                currentNpc.SetHintReceived(true);
                DisplayNpcInfo(currentNpc);
                UpdateHintButton();
                return;
            }
            
            // タイプライター効果が実行中の場合は完了を待つ
            if (typewriterCoroutine != null)
            {
                StartCoroutine(WaitForTypewriterAndStartMiniGame());
            }
            else
            {
                // タイプライターがない場合は即座に起動
                StartMiniGame();
            }
        }
        
        /// <summary>
        /// タイプライター完了後にミニゲームを起動
        /// </summary>
        private IEnumerator WaitForTypewriterAndStartMiniGame()
        {
            if (typewriterCoroutine != null)
                yield return typewriterCoroutine;
            
            yield return new WaitForSeconds(0.3f);
            
            StartMiniGame();
        }
        
        /// <summary>
        /// ミニゲームを起動
        /// </summary>
        private void StartMiniGame() => MiniGameManager.I.StartRandomMiniGame();
        
        /// <summary>
        /// 犯人指定ボタンがクリックされた
        /// </summary>
        private void OnAccuseButtonClicked()
        {
            // パネルを開いた直後のクリックを無視
            if (!canProcessClicks) return;
            
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            if (currentNpc == null) return;
            
            if (confirmDialogPanel != null)
            {
                if (accuseButton != null) accuseButton.interactable = false;
                if (hintButton != null) hintButton.interactable = false;
                if (closeButton != null) closeButton.interactable = false;
                
                confirmDialogPanel.SetActive(true);
                
                UIHelper.SetFirstSelected(confirmNoButton);
            }
            else
            {
                ExecuteAccusation();
            }
        }
        
        /// <summary>
        /// 告発処理を実行
        /// </summary>
        private void ExecuteAccusation()
        {
            if (currentNpc == null) return;
            
            currentNpc.SetAccused(true);
            DisplayNpcInfo(currentNpc);
            
            if (closeButton != null) closeButton.interactable = false;
            if (hintButton != null) hintButton.interactable = false;
            if (accuseButton != null) accuseButton.interactable = false;
            
            StartCoroutine(CheckAccusationAndTransition());
        }
        
        /// <summary>
        /// 告発結果をチェックしてシーン遷移
        /// </summary>
        private IEnumerator CheckAccusationAndTransition()
        {
            yield return new WaitForSeconds(delayBeforeSceneTransition);
            
            if (currentNpc == null)
            {
                Debug.LogError("[DialoguePanel] currentNpcがnullです");
                yield break;
            }
            
            bool isCriminal = currentNpc.IsCriminal();
            
            if (isCriminal)
            {
                if (SceneController.I == null)
                    throw new System.NullReferenceException("[DialoguePanel] SceneControllerがnullです");
                    
                SceneController.I.LoadScene(SceneName.GoodEnd);
            }
            else
            {
                if (SceneController.I == null)
                    throw new System.NullReferenceException("[DialoguePanel] SceneControllerがnullです");
                    
                SceneController.I.LoadScene(SceneName.BadEnd);
            }
        }
        
        /// <summary>
        /// ヒントボタンの表示状態を更新
        /// </summary>
        private void UpdateHintButton()
        {
            if (hintButton == null || currentNpc == null) return;
            
            hintButton.interactable = !currentNpc.HasReceivedHint();
        }
        
        private void OnConfirmYesClicked()
        {
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            if (confirmDialogPanel != null)
                confirmDialogPanel.SetActive(false);
            
            ExecuteAccusation();
        }
        
        private void OnConfirmNoClicked()
        {
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            if (confirmDialogPanel != null)
                confirmDialogPanel.SetActive(false);
            
            if (accuseButton != null) accuseButton.interactable = true;
            if (closeButton != null) closeButton.interactable = true;
            UpdateHintButton();
            
            UIHelper.SetFirstSelected(accuseButton);
        }
    }
}
