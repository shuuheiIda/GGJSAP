using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.InGame.MiniGames
{
    /// <summary>
    /// ミニゲームの基底クラス
    /// 各ミニゲームはこのクラスを継承してIMiniGameを実装します
    /// </summary>
    public abstract class MiniGameBase : MonoBehaviour, IMiniGame
    {
        /// <summary>
        /// プレイヤーのインプットアクション
        /// </summary>
        protected PlayerInput inputActions;

        /// <summary>
        /// 画面の最大サイズ
        /// </summary>
        protected const float MaxWindowSizeX = 9f, MaxWindowSizeY = 5f;

        /// <summary>
        /// コントローラーが接続されているか
        /// </summary>
        protected bool isConnectiongController = false;

        /// <summary>
        /// ミニゲームがアクティブかどうか
        /// </summary>
        protected bool isActive = false;

        /// <summary>
        /// クリア時のコールバック
        /// </summary>
        protected Action onClearCallback;

        protected virtual void Update()
        {
            // コントローラーが接続されているか
            isConnectiongController = Gamepad.all.Count > 0;
        }

        // ====== IMiniGame インターフェースの実装 ======

        /// <summary>
        /// ミニゲームを開始する
        /// </summary>
        public virtual void StartMiniGame()
        {
            gameObject.SetActive(true);
            isActive = true;
            OnMiniGameStart();
        }

        /// <summary>
        /// ミニゲームを停止する
        /// </summary>
        public virtual void StopMiniGame()
        {
            isActive = false;
            OnMiniGameStop();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ミニゲームをリセットする
        /// </summary>
        public virtual void ResetMiniGame()
        {
            OnMiniGameReset();
        }

        /// <summary>
        /// ミニゲームがアクティブかどうか
        /// </summary>
        public bool IsActive() => isActive;

        /// <summary>
        /// クリア時のコールバックを登録
        /// </summary>
        public void RegisterOnClearCallback(Action onClear)
        {
            onClearCallback = onClear;
        }

        // ====== 子クラスでオーバーライドするメソッド ======

        /// <summary>
        /// ミニゲーム開始時の処理（子クラスで実装）
        /// </summary>
        protected virtual void OnMiniGameStart()
        {
            Debug.Log($"[{GetType().Name}] ミニゲーム開始");
        }

        /// <summary>
        /// ミニゲーム停止時の処理（子クラスで実装）
        /// </summary>
        protected virtual void OnMiniGameStop()
        {
            Debug.Log($"[{GetType().Name}] ミニゲーム停止");
        }

        /// <summary>
        /// ミニゲームリセット時の処理（子クラスで実装）
        /// </summary>
        protected virtual void OnMiniGameReset()
        {
            Debug.Log($"[{GetType().Name}] ミニゲームリセット");
        }

        /// <summary>
        /// ミニゲームクリア時に呼ぶメソッド
        /// 子クラスからクリア時に呼び出す
        /// </summary>
        protected void OnMiniGameCleared()
        {
            Debug.Log($"[{GetType().Name}] クリア！");
            isActive = false;
            onClearCallback?.Invoke();
        }
    }
}
