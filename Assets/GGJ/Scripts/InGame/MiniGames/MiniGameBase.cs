using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ {
    public class MiniGameBase : MonoBehaviour {
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

        protected virtual void Update() {
            // コントローラーが接続されているか
            isConnectiongController = Gamepad.all.Count > 0;
        }
    }
}
