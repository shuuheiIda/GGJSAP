using UnityEngine;

namespace GGJ.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T I => instance;

        private static T instance;
        private static readonly object lockObject = new object();

        protected virtual bool UseDontDestroyOnLoad => true;
        protected virtual bool DestroyTargetGameObject => false;

        /// <summary>
        /// シングルトンの初期化処理
        /// </summary>
        protected virtual void Awake()
        {
            // 初回のインスタンス登録
            if (instance == null)
            {
                instance = this as T;
                
                if (UseDontDestroyOnLoad)
                {
                    if (transform.parent == null)
                    {
                        DontDestroyOnLoad(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning($"[{typeof(T).Name}] DontDestroyOnLoad requires root GameObject. Parent: {transform.parent.name}");
                    }
                }
                
                Init();
            }
            // 重複インスタンスの処理
            else if (instance != this)
            {
                Debug.LogWarning($"[{typeof(T).Name}] Duplicate instance detected. Destroying {(DestroyTargetGameObject ? "GameObject" : "component")}.");
                
                if (DestroyTargetGameObject)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(this);
                }
            }
        }

        /// <summary>
        /// インスタンス破棄時の処理
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// 派生クラスで初期化処理を追加したい場合にオーバーライド
        /// </summary>
        protected virtual void Init() { }
    }
}