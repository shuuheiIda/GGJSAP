using UnityEngine;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤーのスプライト表示を担当するクラス
    /// 4方向（上下左右）の移動に応じてスプライトを切り替える
    /// </summary>
    [System.Serializable]
    public class PlayerSpriteController
    {
        private const float MOVEMENT_THRESHOLD = 0.01f;
        
        [Header("4方向スプライト")]
        [SerializeField] private Sprite spriteUp;
        [SerializeField] private Sprite spriteDown;
        [SerializeField] private Sprite spriteLeft;
        [SerializeField] private Sprite spriteRight;
        
        [Header("デフォルト向き")]
        [SerializeField] private Direction defaultDirection = Direction.Down;
        
        private SpriteRenderer spriteRenderer;
        private Direction currentDirection;
        
        /// <summary>
        /// プレイヤーの向き
        /// </summary>
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        
        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(SpriteRenderer renderer)
        {
            spriteRenderer = renderer;
            currentDirection = defaultDirection;
            UpdateSprite(currentDirection);
        }
        
        /// <summary>
        /// 移動入力に基づいてスプライトを更新
        /// </summary>
        public void UpdateSpriteByMovement(Vector2 moveInput)
        {
            if (moveInput.sqrMagnitude < MOVEMENT_THRESHOLD) return;
            
            Direction newDirection = GetDirectionFromInput(moveInput);
            
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                UpdateSprite(currentDirection);
            }
        }
        
        /// <summary>
        /// 入力ベクトルから向きを判定
        /// </summary>
        private Direction GetDirectionFromInput(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                return input.x > 0 ? Direction.Right : Direction.Left;
            else
                return input.y > 0 ? Direction.Up : Direction.Down;
        }
        
        /// <summary>
        /// 指定した向きのスプライトを表示
        /// </summary>
        private void UpdateSprite(Direction direction)
        {
            if (spriteRenderer == null) return;
            
            Sprite targetSprite = direction switch
            {
                Direction.Up => spriteUp,
                Direction.Down => spriteDown,
                Direction.Left => spriteLeft,
                Direction.Right => spriteRight,
                _ => spriteDown
            };
            
            if (targetSprite != null)
                spriteRenderer.sprite = targetSprite;
        }
        
        /// <summary>
        /// 現在の向きを取得
        /// </summary>
        public Direction GetCurrentDirection() => currentDirection;
        
        /// <summary>
        /// 向きを強制的に設定（イベントシーンなどで使用）
        /// </summary>
        public void SetDirection(Direction direction)
        {
            currentDirection = direction;
            UpdateSprite(direction);
        }
    }
}
