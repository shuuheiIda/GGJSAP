using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.InGame.Player
{
    public class PlayerCollider : MonoBehaviour
    {
        public List<Collider2D> ObjectsInRange { get; private set; } = new List<Collider2D>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!ObjectsInRange.Contains(other))
            {
                ObjectsInRange.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (ObjectsInRange.Contains(other))
            {
                ObjectsInRange.Remove(other);
            }
        }

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
