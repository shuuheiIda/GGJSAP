using System;
using UnityEngine;

namespace GGJ.InGame.Events
{
    /// <summary>
    /// ゲーム全体で使用するイベントを管理する静的クラス
    /// </summary>
    public static class GameEvents
    {
        /// <summary>ゲーム開始時に発火</summary>
        public static event Action OnGameStart;
        /// <summary>ゲーム終了時に発火</summary>
        public static event Action OnGameEnd;
        /// <summary>時間更新時に発火</summary>
        public static event Action<float> OnTimeUpdate;
        
        /// <summary>NPC会話開始時に発火</summary>
        public static event Action<GameObject> OnNPCInteractionStarted;
        /// <summary>NPC会話終了時に発火</summary>
        public static event Action OnNPCInteractionEnded;
        
        /// <summary>ミニゲームクリア時、ヒント取得時に発火</summary>
        public static event Action OnHintReceived;


        /// <summary>ゲーム開始イベントを発火</summary>
        public static void RaiseGameStart() => OnGameStart?.Invoke();
        /// <summary>ゲーム終了イベントを発火</summary>
        public static void RaiseGameEnd() => OnGameEnd?.Invoke();
        /// <summary>時間更新イベントを発火</summary>
        public static void RaiseTimeUpdate(float time) => OnTimeUpdate?.Invoke(time);
        /// <summary>NPC会話開始イベントを発火</summary>
        public static void RaiseNPCInteractionStarted(GameObject npc) => OnNPCInteractionStarted?.Invoke(npc);
        /// <summary>NPC会話終了イベントを発火</summary>
        public static void RaiseNPCInteractionEnded() => OnNPCInteractionEnded?.Invoke();
        /// <summary>ヒント取得イベントを発火</summary>
        public static void RaiseHintReceived() => OnHintReceived?.Invoke();
    }
}
