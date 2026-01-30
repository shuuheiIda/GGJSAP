using UnityEngine;

namespace GGJ.InGame.Player
{
    [System.Serializable]
    public class PlayerMovement
    {
        [SerializeField] private float moveSpeed = 5f;

        public void Move(Vector2 input, Rigidbody2D rb)
        {
            Vector2 velocity = input * moveSpeed;
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }
    }
}
