using UnityEngine;

namespace GGJ.Core
{
    /// <summary>
    /// 背景の位置を強制的に固定するスクリプト
    /// </summary>
    public class ForceBackgroundPosition : MonoBehaviour
    {
        private void Start()
        {
            // 位置を強制的に 0 にする
            transform.position = new Vector3(0f, 0f, 0f);
        }
    }
}
