using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace GGJ.InGame.MiniGames.MaskFinder
{
    /// <summary>
    /// マスク探しミニゲームの管理クラス
    /// お題のマスクを30枚の中から3枚見つけ出すゲーム
    /// </summary>
    public class MaskFinderManager : MiniGameBase
    {
        // 定数定義
        private const float GAME_START_DELAY = 2f; // ゲーム開始前の待機時間(秒)
        private const float CLEAR_DELAY = 2f; // クリア時の待機時間(秒)
        private const float WRONG_ANSWER_FLIP_DELAY = 0.5f; // 不正解カードをひっくり返すまでの時間(秒)
        private const float GAME_OVER_DELAY = 1f; // ゲームオーバー時の待機時間(秒)
        private const float STICK_INPUT_THRESHOLD = 0.5f; // スティック入力の閾値
        private const float CURSOR_HIGHLIGHT_SCALE = 1.1f; // カーソル強調表示のスケール
        private const float CURSOR_SELECTED_SCALE = 1.3f; // カーソル選択時のスケール
        private const string REMAINING_TRIES_TEXT_FORMAT = "残り回数: {0}"; // 残り試行回数テキストのフォーマット
        
        [Header("ゲーム設定")]
        [SerializeField] private int totalMaskCount = 30; // 表示するマスクの総数
        [SerializeField] private int correctMaskCount = 3; // 本物のマスクの数
        [SerializeField] private float revealDuration = 1f; // マスクを表向きにする時間(秒)
        [SerializeField] private int maxWrongAttempts = 5; // 間違えられる最大回数
        
        [Header("マスク画像")]
        [SerializeField] private List<Sprite> maskSprites = new List<Sprite>(); // すべてのマスク画像リスト(この中からランダムに正解が選ばれる)
        [SerializeField] private Sprite cardBackSprite; // カード裏面の画像
        
        [Header("UI要素")]
        [SerializeField] private Image questionMaskImage; // お題表示用のImage
        [SerializeField] private Transform cardGridParent; // カードを配置する親Transform
        [SerializeField] private GameObject maskCardPrefab; // マスクカードのプレハブ
        [SerializeField] private TMPro.TextMeshProUGUI remainingTriesText; // 残り試行回数表示用テキスト
        
        [Header("グリッド設定")]
        [SerializeField] private int gridColumns = 6; // グリッドの列数
        
        private List<MaskCardController> allCards = new List<MaskCardController>();
        private List<MaskCardController> correctCards = new List<MaskCardController>();
        private List<MaskCardController> selectedCards = new List<MaskCardController>();
        private int currentCursorIndex = 0;
        private bool isRevealing = false;
        private bool canSelect = false;
        private Sprite currentCorrectMask; // 今回のゲームで選ばれた正解のマスク
        private int remainingTries; // 残りの試行回数
        private float stickInputDelay = 0.2f; // スティック入力の遅延時間
        private float lastStickInputTime = 0f; // 最後のスティック入力時刻
        
        private void Awake()
        {
            inputActions = new PlayerInput();
            inputActions.UI.Enable();
        }
        
        private void OnEnable()
        {
            inputActions?.Enable();
        }
        
        private void OnDisable()
        {
            inputActions?.Disable();
        }
        
        protected override void OnMiniGameStart()
        {
            remainingTries = maxWrongAttempts;
            UpdateRemainingTriesText();
            SetupGame();
            StartCoroutine(GameSequence());
        }
        
        protected override void OnMiniGameStop()
        {
            CleanupGame();
        }
        
        protected override void OnMiniGameReset()
        {
            CleanupGame();
            selectedCards.Clear();
            currentCursorIndex = 0;
            canSelect = false;
            remainingTries = maxWrongAttempts;
            UpdateRemainingTriesText();
            
            // すべてのカードのスケールをリセット
            foreach (var card in allCards)
            {
                if (card != null)
                    card.transform.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// ゲームのセットアップ
        /// </summary>
        private void SetupGame()
        {
            if (maskSprites.Count == 0)
            {
                return;
            }
            
            currentCorrectMask = maskSprites[Random.Range(0, maskSprites.Count)];
            
            if (questionMaskImage != null)
            {
                questionMaskImage.sprite = currentCorrectMask;
            }
            
            GenerateCards();
        }
        
        /// <summary>
        /// カードを生成
        /// </summary>
        private void GenerateCards()
        {
            if (cardGridParent == null || maskCardPrefab == null)
            {
                return;
            }
            
            foreach (Transform child in cardGridParent)
            {
                Destroy(child.gameObject);
            }
            allCards.Clear();
            correctCards.Clear();
            
            List<int> correctIndices = new List<int>();
            while (correctIndices.Count < correctMaskCount)
            {
                int randomIndex = Random.Range(0, totalMaskCount);
                if (!correctIndices.Contains(randomIndex))
                    correctIndices.Add(randomIndex);
            }
            
            for (int i = 0; i < totalMaskCount; i++)
            {
                GameObject cardObj = Instantiate(maskCardPrefab, cardGridParent);
                
                if (!cardObj.activeSelf)
                {
                    cardObj.SetActive(true);
                }
                
                MaskCardController card = cardObj.GetComponent<MaskCardController>();
                
                if (card != null)
                {
                    bool isCorrect = correctIndices.Contains(i);
                    Sprite frontSprite = isCorrect ? currentCorrectMask : GetRandomDecoySprite();
                    
                    card.Initialize(frontSprite, cardBackSprite, isCorrect, i);
                    card.OnCardClicked += OnCardSelected;
                    
                    allCards.Add(card);
                    
                    if (isCorrect)
                        correctCards.Add(card);
                }
            }
        }
        
        /// <summary>
        /// ランダムなおとりマスク画像を取得(正解以外)
        /// </summary>
        private Sprite GetRandomDecoySprite()
        {
            if (maskSprites.Count <= 1)
            {
                return null;
            }
            
            Sprite decoy;
            do
            {
                decoy = maskSprites[Random.Range(0, maskSprites.Count)];
            } while (decoy == currentCorrectMask);
            
            return decoy;
        }
        
        /// <summary>
        /// ゲームのシーケンス
        /// </summary>
        private IEnumerator GameSequence()
        {
            yield return new WaitForSeconds(GAME_START_DELAY);
            
            isRevealing = true;
            foreach (var card in allCards)
            {
                card.Reveal();
            }
            
            yield return new WaitForSeconds(revealDuration);
            
            // 3. すべてのカードを裏返す
            foreach (var card in allCards)
            {
                card.Hide();
            }
            isRevealing = false;
            
            // 4. 選択開始
            canSelect = true;
            
            // コントローラー使用時、最初のカードを強調表示
            if (isConnectiongController && allCards.Count > 0)
            {
                currentCursorIndex = 0;
                allCards[0].transform.localScale = Vector3.one * CURSOR_HIGHLIGHT_SCALE;
            }
        }
        
        /// <summary>
        /// カードが選択されたときの処理
        /// </summary>
        private void OnCardSelected(MaskCardController card)
        {
            if (!canSelect || isRevealing || card.IsRevealed || selectedCards.Contains(card))
                return;
            
            card.Reveal();
            
            // 正解かどうかを即座に判定
            if (card.IsCorrectMask)
            {
                // 正解の場合、選択リストに追加して表示を維持
                selectedCards.Add(card);
                
                // すべての正解カードが選ばれたかチェック
                if (selectedCards.Count >= correctMaskCount)
                {
                    canSelect = false;
                    StartCoroutine(ClearGame());
                }
            }
            else
            {
                // 不正解の場合、すぐにひっくり返す
                StartCoroutine(ShowWrongAnswer(card));
            }
        }
        
        /// <summary>
        /// クリア処理
        /// </summary>
        private IEnumerator ClearGame()
        {
            yield return new WaitForSeconds(CLEAR_DELAY);
            
            OnMiniGameCleared();
        }
        
        /// <summary>
        /// 不正解時の処理
        /// </summary>
        private IEnumerator ShowWrongAnswer(MaskCardController wrongCard)
        {
            canSelect = false;
            
            // 残り回数を減らす
            remainingTries--;
            UpdateRemainingTriesText();
            
            yield return new WaitForSeconds(WRONG_ANSWER_FLIP_DELAY);
            
            // 不正解のカードをひっくり返す
            wrongCard.Hide();
            
            // 残り回数が0になったらゲームオーバー
            if (remainingTries <= 0)
            {
                yield return new WaitForSeconds(GAME_OVER_DELAY);
                // ゲームオーバー処理（リセットまたは終了）
                ResetMiniGame();
                yield break;
            }
            
            canSelect = true;
        }
        
        /// <summary>
        /// 残り試行回数テキストを更新
        /// </summary>
        private void UpdateRemainingTriesText()
        {
            if (remainingTriesText != null)
            {
                remainingTriesText.text = string.Format(REMAINING_TRIES_TEXT_FORMAT, remainingTries);
            }
        }
        
        /// <summary>
        /// ゲームのクリーンアップ
        /// </summary>
        private void CleanupGame()
        {
            foreach (var card in allCards)
            {
                if (card != null)
                    card.OnCardClicked -= OnCardSelected;
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (!isActive || !canSelect || isRevealing)
                return;
            
            HandleInput();
        }
        
        /// <summary>
        /// 入力処理
        /// </summary>
        private void HandleInput()
        {
            // コントローラーでの選択
            if (isConnectiongController && Gamepad.current != null)
            {
                HandleGamepadInput();
            }
            // マウスでの選択は各カードのOnClickで処理
        }
        
        /// <summary>
        /// ゲームパッド入力処理
        /// </summary>
        private void HandleGamepadInput()
        {
            Vector2 navigateInput = inputActions.UI.Navigate.ReadValue<Vector2>();
            
            // 上下左右でカーソル移動（遅延制御付き）
            if (navigateInput.magnitude > STICK_INPUT_THRESHOLD && Time.time - lastStickInputTime > stickInputDelay)
            {
                int oldIndex = currentCursorIndex;
                
                if (navigateInput.x > STICK_INPUT_THRESHOLD) // 右
                    currentCursorIndex = Mathf.Min(currentCursorIndex + 1, allCards.Count - 1);
                else if (navigateInput.x < -STICK_INPUT_THRESHOLD) // 左
                    currentCursorIndex = Mathf.Max(currentCursorIndex - 1, 0);
                else if (navigateInput.y > STICK_INPUT_THRESHOLD) // 上
                    currentCursorIndex = Mathf.Max(currentCursorIndex - gridColumns, 0);
                else if (navigateInput.y < -STICK_INPUT_THRESHOLD) // 下
                    currentCursorIndex = Mathf.Min(currentCursorIndex + gridColumns, allCards.Count - 1);
                
                // カーソルが移動した場合、視覚的フィードバックを更新
                if (oldIndex != currentCursorIndex)
                {
                    UpdateCursorVisuals(oldIndex, currentCursorIndex);
                    lastStickInputTime = Time.time;
                }
            }
            
            // Submitアクションで選択（Space, Enter, Gamepad Bボタン）
            if (inputActions.UI.Submit.triggered)
            {
                if (currentCursorIndex >= 0 && currentCursorIndex < allCards.Count)
                {
                    allCards[currentCursorIndex].OnClick();
                }
            }
        }
        
        /// <summary>
        /// カーソル位置の視覚的フィードバックを更新
        /// </summary>
        private void UpdateCursorVisuals(int oldIndex, int newIndex)
        {
            // 前のカードを通常サイズに戻す
            if (oldIndex >= 0 && oldIndex < allCards.Count)
            {
                allCards[oldIndex].transform.localScale = Vector3.one;
            }
            
            // 新しいカードを少し大きくする
            if (newIndex >= 0 && newIndex < allCards.Count)
            {
                allCards[newIndex].transform.localScale = Vector3.one * CURSOR_SELECTED_SCALE;
            }
        }
    }
}