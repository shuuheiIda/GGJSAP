using System.Collections.Generic;
using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺ｮ蠖薙◆繧雁愛螳壹→蜻ｨ蝗ｲ縺ｮ繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ邂｡逅・☆繧・
    /// </summary>
    public class PlayerCollider : MonoBehaviour
    {
        public List<Collider2D> ObjectsInRange { get; private set; } = new List<Collider2D>();

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Tags.Npc) && !ObjectsInRange.Contains(collision.collider))
                ObjectsInRange.Add(collision.collider);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.collider.CompareTag(Tags.Npc) && ObjectsInRange.Contains(collision.collider))
                ObjectsInRange.Remove(collision.collider);
        }

        /// <summary>
        /// 遽・峇蜀・〒譛繧りｿ代＞繧ｪ繝悶ず繧ｧ繧ｯ繝医ｒ蜿門ｾ励☆繧・
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
