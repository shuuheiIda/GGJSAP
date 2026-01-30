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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.NPC) && !ObjectsInRange.Contains(other))
                ObjectsInRange.Add(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Tags.NPC) && ObjectsInRange.Contains(other))
                ObjectsInRange.Remove(other);
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
