using System.Collections.Generic;
using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤーの当たり判定と周囲のオブジェクトを管理する
    /// </summary>
    public class PlayerCollider : MonoBehaviour
    {
        public List<Collider2D> ObjectsInRange { get; private set; } = new List<Collider2D>();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Tags.NPC) && !ObjectsInRange.Contains(collision.collider))
                ObjectsInRange.Add(collision.collider);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Tags.NPC) && ObjectsInRange.Contains(collision.collider))
                ObjectsInRange.Remove(collision.collider);
        }

        /// <summary>
        /// 範囲内で最も近いオブジェクトを取得する
        /// </summary>
        public Collider2D GetNearestObject()
        {
            if (ObjectsInRange.Count == 0) return null;

            Collider2D nearest = null;
            float minDistance = float.MaxValue;

            foreach (var obj in ObjectsInRange)
            {
                if (obj == null) continue;
                
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = obj;
                }
            }

            return nearest;
        }
    }
}
