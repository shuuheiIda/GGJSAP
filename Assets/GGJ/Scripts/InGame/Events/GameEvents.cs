using System;
using UnityEngine;

namespace GGJ.InGame.Events
{
    /// <summary>
    /// ゲーム全体で使用するイベントを管理する静的クラス
    /// </summary>
    public static class GameEvents
    {
        public static event Action<GameObject> OnNPCInteractionStarted;
        public static event Action OnNPCInteractionEnded;
    }
}
