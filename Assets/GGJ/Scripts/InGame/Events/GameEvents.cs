using System;
using UnityEngine;
using GGJ.InGame.NPC;

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
        public static event Action<GameObject> OnNpcInteractionStarted;
        /// <summary>NPC会話終了時に発火</summary>
        public static event Action OnNpcInteractionEnded;
        
        /// <summary>ミニゲームクリア時、ヒント取得時に発火</summary>
        public static event Action OnHintReceived;

        /// <summary>ゲーム開始イベントを発火</summary>
        public static void RaiseGameStart() => OnGameStart?.Invoke();
        /// <summary>ゲーム終了イベントを発火</summary>
        public static void RaiseGameEnd() => OnGameEnd?.Invoke();
        /// <summary>時間更新イベントを発火</summary>
        public static void RaiseTimeUpdate(float time) => OnTimeUpdate?.Invoke(time);
        /// <summary>NPC会話開始イベントを発火</summary>
        public static void RaiseNpcInteractionStarted(GameObject npc) => OnNpcInteractionStarted?.Invoke(npc);
        /// <summary>NPC会話終了イベントを発火</summary>
        public static void RaiseNpcInteractionEnded() => OnNpcInteractionEnded?.Invoke();
        /// <summary>ヒント取得イベントを発火</summary>
        public static void RaiseHintReceived() => OnHintReceived?.Invoke();
    }
}
