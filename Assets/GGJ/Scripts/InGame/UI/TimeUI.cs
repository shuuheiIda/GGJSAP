using UnityEngine;
using TMPro;
using GGJ.InGame.Events;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// 残り時間を表示するUIコンポーネント
    /// </summary>
    public class TimeUI : MonoBehaviour
    {
        private const float SECONDS_PER_MINUTE = 60f;

        [SerializeField] private TextMeshProUGUI timeText;

        private void OnEnable() => GameEvents.OnTimeUpdate += UpdateTimeDisplay;

        private void OnDisable() => GameEvents.OnTimeUpdate -= UpdateTimeDisplay;

        private void UpdateTimeDisplay(float remainingTime)
        {
            if (timeText == null) return;

            int minutes = Mathf.FloorToInt(remainingTime / SECONDS_PER_MINUTE);
            int seconds = Mathf.FloorToInt(remainingTime % SECONDS_PER_MINUTE);
            timeText.text = $"時間 {minutes:00}:{seconds:00}";
        }
    }
}
