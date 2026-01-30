using UnityEngine;
using TMPro;
using GGJ.InGame.Manager;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// 残り時間を表示するUIコンポーネント
    /// </summary>
    public class TimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        private void OnEnable()
        {
            if (InGameManager.I != null)
                InGameManager.I.OnTimeUpdate += UpdateTimeDisplay;
        }

        private void OnDisable()
        {
            if (InGameManager.I != null)
                InGameManager.I.OnTimeUpdate -= UpdateTimeDisplay;
        }

        private void UpdateTimeDisplay(float remainingTime)
        {
            if (timeText == null) return;

            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
