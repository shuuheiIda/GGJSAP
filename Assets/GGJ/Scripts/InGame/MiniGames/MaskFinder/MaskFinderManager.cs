using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace GGJ.InGame.MiniGames.MaskFinder
{
    /// <summary>
    /// マスク探しミニゲームの管理クラス
    /// お題のマスクを30枚の中から3枚見つけ出すゲーム
    /// </summary>
    public class MaskFinderManager : MiniGameBase
    {
        [Header("ゲーム設定")]
        [SerializeField] private int totalMaskCount = 30; // 表示するマスクの総数
        [SerializeField] private int correctMaskCount = 3; // 本物のマスクの数
        [SerializeField] private float revealDuration = 1f; // マスクを表向きにする時間(秒)
        
        [Header("マスク画像")]
        [SerializeField] private List<Sprite> maskSprites = new List<Sprite>(); // すべてのマスク画像リスト(この中からランダムに正解が選ばれる)
        [SerializeField] private Sprite cardBackSprite; // カード裏面の画像
        
        [Header("UI要素")]
        [SerializeField] private Image questionMaskImage; // お題表示用のImage
        [SerializeField] private Transform cardGridParent; // カードを配置する親Transform
        [SerializeField] private GameObject maskCardPrefab; // マスクカードのプレハブ
        [SerializeField] private TextMeshProUGUI instructionText; // 説明テキスト
        
        [Header("グリッド設定")]
        [SerializeField] private int gridColumns = 6; // グリッドの列数
        [SerializeField] private int gridRows = 5; // グリッドの行数
        [SerializeField] private float cardSpacing = 10f; // カード間のスペース
        
        [Header("入力設定")]
        [SerializeField] private float cursorSpeed = 500f; // カーソル移動速度
        
        [Header("テキスト設定")]
        [SerializeField] private string textRememberMasks = "マスクを覚えてください!";
        [SerializeField] private string textFindMasks = "お題のマスクを{0}枚見つけてください";
        [SerializeField] private string textClear = "正解!ゲームクリア!";
        [SerializeField] private string textWrong = "不正解...もう一度挑戦!";
        
        private List<MaskCardController> allCards = new List<MaskCardController>();
        private List<MaskCardController> correctCards = new List<MaskCardController>();
        private List<MaskCardController> selectedCards = new List<MaskCardController>();
        private int currentCursorIndex = 0;
        private bool isRevealing = false;
        private bool canSelect = false;
        private Sprite currentCorrectMask; // 今回のゲームで選ばれた正解のマスク
        
        private void Awake()
        {
            inputActions = new PlayerInput();
        }
        
        private void Start()
        {
            StartMiniGame();
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
            yield return new WaitForSeconds(2f);
            
            UpdateInstructionText(textRememberMasks);
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
            UpdateInstructionText(string.Format(textFindMasks, correctMaskCount));
            canSelect = true;
            
            // 最初のカードをハイライト
            if (allCards.Count > 0)
                allCards[0].SetHighlight(true);
        }
        
        /// <summary>
        /// カードが選択されたときの処理
        /// </summary>
        private void OnCardSelected(MaskCardController card)
        {
            if (!canSelect || isRevealing || card.IsRevealed || selectedCards.Contains(card))
                return;
            
            selectedCards.Add(card);
            card.Reveal();
            
            // すべての正解カードが選ばれたかチェック
            CheckGameClear();
        }
        
        /// <summary>
        /// ゲームクリア判定
        /// </summary>
        private void CheckGameClear()
        {
            // 正解カードがすべて選ばれたかチェック
            bool allCorrectFound = correctCards.All(c => selectedCards.Contains(c));
            
            if (allCorrectFound)
            {
                canSelect = false;
                StartCoroutine(ClearGame());
            }
            else if (selectedCards.Count >= correctMaskCount)
            {
                // 不正解の場合
                bool hasWrongCard = selectedCards.Any(c => !c.IsCorrectMask);
                if (hasWrongCard)
                {
                    StartCoroutine(ShowWrongAnswer());
                }
            }
        }
        
        /// <summary>
        /// クリア処理
        /// </summary>
        private IEnumerator ClearGame()
        {
            UpdateInstructionText(textClear);
            yield return new WaitForSeconds(2f);
            
            onClearCallback?.Invoke();
        }
        
        /// <summary>
        /// 不正解時の処理
        /// </summary>
        private IEnumerator ShowWrongAnswer()
        {
            canSelect = false;
            UpdateInstructionText("不正解...もう一度挑戦!");
            
            yield return new WaitForSeconds(1.5f);
            
            // 選択したカードを裏返す
            foreach (var card in selectedCards)
            {
                card.Hide();
            }
            selectedCards.Clear();
            
            UpdateInstructionText($"お題のマスクを{correctMaskCount}枚見つけてください");
            canSelect = true;
        }
        
        /// <summary>
        /// 説明テキストを更新
        /// </summary>
        private void UpdateInstructionText(string text)
        {
            if (instructionText != null)
                instructionText.text = text;
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
            Vector2 leftStick = Gamepad.current.leftStick.ReadValue();
            
            // 上下左右でカーソル移動
            if (leftStick.magnitude > 0.5f)
            {
                int oldIndex = currentCursorIndex;
                
                if (leftStick.x > 0.5f) // 右
                    currentCursorIndex = Mathf.Min(currentCursorIndex + 1, allCards.Count - 1);
                else if (leftStick.x < -0.5f) // 左
                    currentCursorIndex = Mathf.Max(currentCursorIndex - 1, 0);
                else if (leftStick.y > 0.5f) // 上
                    currentCursorIndex = Mathf.Max(currentCursorIndex - gridColumns, 0);
                else if (leftStick.y < -0.5f) // 下
                    currentCursorIndex = Mathf.Min(currentCursorIndex + gridColumns, allCards.Count - 1);
                
                // ハイライト更新
                if (oldIndex != currentCursorIndex)
                {
                    allCards[oldIndex].SetHighlight(false);
                    allCards[currentCursorIndex].SetHighlight(true);
                }
            }
            
            // Aボタンで選択
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                allCards[currentCursorIndex].OnClick();
            }
        }
    }
}