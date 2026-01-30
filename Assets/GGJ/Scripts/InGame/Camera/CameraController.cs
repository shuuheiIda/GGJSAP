using UnityEngine;

namespace GGJ.InGame.Camera
{
    /// <summary>
    /// カメラがターゲット（Player）を追従する処理
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
        [SerializeField] private bool smoothFollow = true;
        [SerializeField] private float smoothSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 targetPosition = target.position + offset;

            if (smoothFollow)
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            else
                transform.position = targetPosition;
        }
    }
}
