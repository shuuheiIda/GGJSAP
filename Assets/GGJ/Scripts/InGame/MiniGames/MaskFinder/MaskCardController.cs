using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GGJ.InGame.MiniGames.MaskFinder
{
    /// <summary>
    /// マスクカードの制御クラス
    /// カードの表裏切り替え、クリック検出、ハイライト表示を管理
    /// </summary>
    public class MaskCardController : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI要素")]
        [SerializeField] private Image cardImage; // カード画像
        
        private Sprite frontSprite; // 表面のスプライト
        private Sprite backSprite; // 裏面のスプライト
        private bool isCorrectMask; // 正解のマスクかどうか
        private bool isRevealed = false; // 現在表向きかどうか
        private int cardIndex; // カードのインデックス
        private Button button;
        
        /// <summary>カードがクリックされたときのイベント</summary>
        public event Action<MaskCardController> OnCardClicked;
        
        /// <summary>正解のマスクかどうか</summary>
        public bool IsCorrectMask => isCorrectMask;
        
        /// <summary>現在表向きかどうか</summary>
        public bool IsRevealed => isRevealed;
        
        /// <summary>カードのインデックス</summary>
        public int CardIndex => cardIndex;
        
        private void Awake()
        {
            if (cardImage == null)
                cardImage = GetComponent<Image>();
            
            button = GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(OnClick);
        }
        
        /// <summary>
        /// カードを初期化
        /// </summary>
        /// <param name="front">表面のスプライト</param>
        /// <param name="back">裏面のスプライト</param>
        /// <param name="isCorrect">正解のマスクかどうか</param>
        /// <param name="index">カードのインデックス</param>
        public void Initialize(Sprite front, Sprite back, bool isCorrect, int index)
        {
            frontSprite = front;
            backSprite = back;
            isCorrectMask = isCorrect;
            cardIndex = index;
            isRevealed = false;
            
            if (cardImage != null)
            {
                cardImage.sprite = backSprite;
            }
        }
        
        /// <summary>
        /// カードを表向きにする
        /// </summary>
        public void Reveal()
        {
            isRevealed = true;
            if (cardImage != null)
                cardImage.sprite = frontSprite;
        }
        
        /// <summary>
        /// カードを裏返す
        /// </summary>
        public void Hide()
        {
            isRevealed = false;
            if (cardImage != null)
                cardImage.sprite = backSprite;
        }
        
        /// <summary>
        /// カードがクリックされたとき
        /// </summary>
        public void OnClick()
        {
            OnCardClicked?.Invoke(this);
        }
        
        /// <summary>
        /// カードがクリックされたとき（IPointerClickHandler）
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
        
        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);
        }
    }
}
