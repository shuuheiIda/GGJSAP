using UnityEngine;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPC本体のクラス
    /// シーン上に配置され、INpcインターフェースを実装
    /// </summary>
    public class NpcController : MonoBehaviour, INpc
    {
        [Header("NPCデータ")]
        [SerializeField] private NPCData NpcData;
        
        [Header("状態")]
        [SerializeField] private bool hasReceivedHint = false;
        [SerializeField] private bool isAccused = false;
        
        private bool isCriminalRuntime = false;
        private SpriteRenderer spriteRenderer;
        private NpcOutlineController outlineController;
        
        // InGameManaagerより早く初期化を行いNpcを登録したいためAwakeにしている
        private void Awake()
        {
            InitializeNpc();
            
            if (NpcManager.I != null)
                NpcManager.I.RegisterNpc(this);
        }
        
        private void InitializeNpc()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // アウトラインコントローラーを取得または追加
            outlineController = GetComponent<NpcOutlineController>();
            if (outlineController == null)
            {
                outlineController = gameObject.AddComponent<NpcOutlineController>();
            }
            
            // AppearanceはゲームオブジェクトにSpriteRendererが設定されているので不要
        }
        
        #region INpc実装
        
        public NPCData GetNpcData() => NpcData;
        
        public string GetCurrentDialogue()
        {
            if (NpcManager.I != null)
                return NpcManager.I.GetDialogueForNpc(this);
            
            return "...";
        }
        
        public void SetHintReceived(bool received) => hasReceivedHint = received;
        
        public bool HasReceivedHint() => hasReceivedHint;
        
        public Vector3 GetPosition() => transform.position;
        
        public GameObject GetGameObject() => gameObject;
        
        public void SetCriminal(bool isCriminal) => isCriminalRuntime = isCriminal;
        
        public bool IsCriminal() => isCriminalRuntime;
        
        public void SetAccused(bool accused) => isAccused = accused;
        
        public bool IsAccused() => isAccused;
        
        #endregion
    }
}