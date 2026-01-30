using UnityEngine;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤーの移動処理を担当するクラス
    /// </summary>
    [System.Serializable]
    public class PlayerMovement
    {
        [SerializeField] private float moveSpeed = 5f;
        /// <summary>
        /// 入力に基づいてプレイヤーを移動させる
        /// </summary>
        public void Move(Vector2 input, Rigidbody2D rb)
        {
            Vector2 velocity = input * moveSpeed;
            rb.velocity = velocity;
        }
    }
}
