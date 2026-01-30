using UnityEngine;

namespace GGJ.Core
{
    /// <summary>
    /// シングルトンパターンの基底クラス
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T I => instance;

        private static T instance;

        protected virtual bool UseDontDestroyOnLoad => true;
        protected virtual bool DestroyTargetGameObject => false;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                
                if (UseDontDestroyOnLoad)
                {
                    if (transform.parent == null)
                        DontDestroyOnLoad(gameObject);
                    else
                        Debug.LogWarning($"[{typeof(T).Name}] DontDestroyOnLoad requires root GameObject. Parent: {transform.parent.name}");
                }
                
                Init();
            }
            else if (instance != this)
            {
                Debug.LogWarning($"[{typeof(T).Name}] Duplicate instance detected. Destroying {(DestroyTargetGameObject ? "GameObject" : "component")}.");
                
                if (DestroyTargetGameObject)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        /// <summary>
        /// 派生クラスで初期化処理を追加したい場合にオーバーライド
        /// </summary>
        protected virtual void Init() { }
    }
}