using System;
using UnityEngine;

namespace GGJ.InGame.MiniGames
{
    /// <summary>
    /// ミニゲームの共通インターフェース
    /// すべてのミニゲームはこのインターフェースを実装する必要があります
    /// </summary>
    public interface IMiniGame
    {
        /// <summary>
        /// ミニゲームを開始する
        /// </summary>
        void StartMiniGame();

        /// <summary>
        /// ミニゲームを停止する（途中終了時）
        /// </summary>
        void StopMiniGame();

        /// <summary>
        /// ミニゲームをリセットする
        /// </summary>
        void ResetMiniGame();

        /// <summary>
        /// ミニゲームがアクティブかどうか
        /// </summary>
        bool IsActive();

        /// <summary>
        /// ミニゲームクリア時のコールバックを登録
        /// </summary>
        /// <param name="onClear">クリア時に呼ばれるコールバック</param>
        void RegisterOnClearCallback(Action onClear);
    }
}
