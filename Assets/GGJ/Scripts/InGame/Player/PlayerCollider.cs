using System;
using UnityEngine;

namespace GGJ.InGame.Player
{
    public class PlayerCollider : MonoBehaviour
    {
        public event Action<Collision2D> OnCollisionEntered;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEntered?.Invoke(collision);
        }
    }
}
