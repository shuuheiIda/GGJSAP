using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private GameObject panel;
        [SerializeField] private Button accuseButton;
        [SerializeField] private Button hintButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [Header("タイプライター設定")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float characterDelay = 0.05f;
        
        [Header("シーン遷移設定")]
        [SerializeField] private float delayBeforeSceneTransition = 2.0f;
        
        private INpc currentNpc;
        private Coroutine typewriterCoroutine;

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
            
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnDestroy()
        {
            // イベントリスナーを解除
            GameEvents.OnNpcInteractionStarted -= OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded -= OnNpcInteractionEnded;
            GameEvents.OnHintReceived -= OnHintReceived;
            
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
            
            // ボタンを有効化（前回の会話で無効化されている可能性があるため）
            if (closeButton != null) closeButton.interactable = true;
            if (hintButton != null) hintButton.interactable = true;
            if (accuseButton != null) accuseButton.interactable = true;
            
            // 1フレーム遅延させてボタンフォーカスを設定（○ボタンの入力が残っているのを防ぐ）
            StartCoroutine(SetFirstSelectedDelayed());
            
            if (currentNpc != null)
                DisplayNpcInfo(currentNpc);
            
            UpdateHintButton();
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
            
            // タイプライターコルーチンを停止
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            panel.SetActive(false);
            UIHelper.ClearSelected();
        }

        private void OnCloseButtonClicked()
        {
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            GameEvents.RaiseNpcInteractionEnded();
        }
        
        /// <summary>
        /// ヒントボタンがクリックされた
        /// </summary>
        private void OnHintButtonClicked()
        {
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
            // タイプライターの完了を待つ
            if (typewriterCoroutine != null)
                yield return typewriterCoroutine;
            
            // 0.3秒待ってからミニゲーム起動（読み終わる時間を与える）
            yield return new WaitForSeconds(0.3f);
            
            StartMiniGame();
        }
        
        /// <summary>
        /// ミニゲームを起動
        /// </summary>
        private void StartMiniGame()
        {
            // ミニゲームを起動（MainGameUIの切り替えで会話パネルも自動的に非表示になる）
            MiniGameManager.I.StartRandomMiniGame();
        }
        
        /// <summary>
        /// 犯人指定ボタンがクリックされた
        /// </summary>
        private void OnAccuseButtonClicked()
        {
            if (GGJ.InGame.Audio.AudioManager.I != null)
                GGJ.InGame.Audio.AudioManager.I.PlaySE(GGJ.InGame.Audio.SEType.ButtonClick);
            
            if (currentNpc == null) return;
            
            // 告発フラグを立てる
            currentNpc.SetAccused(true);
            DisplayNpcInfo(currentNpc);
            
            // 入力を即座に無効化（連打防止）
            // UpdateHintButton()の後に実行することで、再有効化を防ぐ
            if (closeButton != null) closeButton.interactable = false;
            if (hintButton != null) hintButton.interactable = false;
            if (accuseButton != null) accuseButton.interactable = false;
            
            // 犯人かどうかをチェックしてシーン遷移
            StartCoroutine(CheckAccusationAndTransition());
        }
        
        /// <summary>
        /// 告発結果をチェックしてシーン遷移
        /// </summary>
        private IEnumerator CheckAccusationAndTransition()
        {
            // セリフ表示を待つ
            yield return new WaitForSeconds(delayBeforeSceneTransition);
            
            if (currentNpc == null)
            {
                Debug.LogError("[DialoguePanel] currentNpcがnullです");
                yield break;
            }
            
            // 犯人を正しく告発したか確認
            bool isCriminal = currentNpc.IsCriminal();
            
            if (isCriminal)
            {
                // 犯人を見つけた！ → GoodEnd
                Debug.Log("[DialoguePanel] 犯人を見つけました！GoodEndへ遷移します");
                if (SceneController.I != null)
                {
                    SceneController.I.LoadScene(SceneName.GoodEnd);
                }
                else
                {
                    Debug.LogError("[DialoguePanel] SceneControllerが見つかりません");
                }
            }
            else
            {
                // 間違った人を告発した → BadEnd
                Debug.Log("[DialoguePanel] 間違った人を告発しました！BadEndへ遷移します");
                if (SceneController.I != null)
                {
                    SceneController.I.LoadScene(SceneName.BadEnd);
                }
                else
                {
                    Debug.LogError("[DialoguePanel] SceneControllerが見つかりません");
                }
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
    }
}
