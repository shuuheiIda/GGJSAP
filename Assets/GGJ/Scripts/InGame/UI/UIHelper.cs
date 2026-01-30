using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// UI操作に関する共通ヘルパー機能を提供する静的クラス
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// 指定したボタンにコントローラーフォーカスを設定する
        /// </summary>
        public static void SetFirstSelected(Button button)
        {
            if (button == null)
            {
                Debug.LogWarning("[UIHelper] Button is null");
                return;
            }
            if (EventSystem.current == null)
            {
                Debug.LogError("[UIHelper] EventSystem.current is null!");
                return;
            }

            EventSystem.current.SetSelectedGameObject(button.gameObject);
            Debug.Log($"[UIHelper] Selected button: {button.name}, CurrentSelected: {EventSystem.current.currentSelectedGameObject?.name}");
        }

        /// <summary>
        /// 指定したGameObjectにコントローラーフォーカスを設定する
        /// </summary>
        public static void SetFirstSelected(GameObject obj)
        {
            if (obj == null) return;
            if (EventSystem.current == null) return;

            EventSystem.current.SetSelectedGameObject(obj);
        }

        /// <summary>
        /// 現在のフォーカスをクリアする
        /// </summary>
        public static void ClearSelected()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
