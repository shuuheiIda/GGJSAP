using System;
using UnityEngine;
using GGJ.InGame.NPC;

namespace GGJ.InGame.Events
{
    /// <summary>
    /// 繧ｲ繝ｼ繝蜈ｨ菴薙〒菴ｿ逕ｨ縺吶ｋ繧､繝吶Φ繝医ｒ邂｡逅・☆繧矩撕逧・け繝ｩ繧ｹ
    /// </summary>
    public static class GameEvents
    {
        /// <summary>繧ｲ繝ｼ繝髢句ｧ区凾縺ｫ逋ｺ轣ｫ</summary>
        public static event Action OnGameStart;
        /// <summary>繧ｲ繝ｼ繝邨ゆｺ・凾縺ｫ逋ｺ轣ｫ</summary>
        public static event Action OnGameEnd;
        /// <summary>譎る俣譖ｴ譁ｰ譎ゅ↓逋ｺ轣ｫ</summary>
        public static event Action<float> OnTimeUpdate;
        
        /// <summary>NPC莨夊ｩｱ髢句ｧ区凾縺ｫ逋ｺ轣ｫ</summary>
        public static event Action<GameObject> OnNpcInteractionStarted;
        /// <summary>NPC莨夊ｩｱ邨ゆｺ・凾縺ｫ逋ｺ轣ｫ</summary>
        public static event Action OnNpcInteractionEnded;
        
        /// <summary>繝溘ル繧ｲ繝ｼ繝繧ｯ繝ｪ繧｢譎ゅ√ヲ繝ｳ繝亥叙蠕玲凾縺ｫ逋ｺ轣ｫ</summary>
        public static event Action OnHintReceived;

        /// <summary>繧ｲ繝ｼ繝髢句ｧ九う繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseGameStart() => OnGameStart?.Invoke();
        /// <summary>繧ｲ繝ｼ繝邨ゆｺ・う繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseGameEnd() => OnGameEnd?.Invoke();
        /// <summary>譎る俣譖ｴ譁ｰ繧､繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseTimeUpdate(float time) => OnTimeUpdate?.Invoke(time);
        /// <summary>NPC莨夊ｩｱ髢句ｧ九う繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseNpcInteractionStarted(GameObject npc) => OnNpcInteractionStarted?.Invoke(npc);
        /// <summary>NPC莨夊ｩｱ邨ゆｺ・う繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseNpcInteractionEnded() => OnNpcInteractionEnded?.Invoke();
        /// <summary>繝偵Φ繝亥叙蠕励う繝吶Φ繝医ｒ逋ｺ轣ｫ</summary>
        public static void RaiseHintReceived() => OnHintReceived?.Invoke();
    }
}
