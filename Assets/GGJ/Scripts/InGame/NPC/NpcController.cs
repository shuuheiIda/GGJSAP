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
        [SerializeField] private NpcData NpcData;
        
        [Header("状態")]
        [SerializeField] private bool hasReceivedHint = false;
        [SerializeField] private bool isAccused = false;
        
        /// <summary>実行時の犯人フラグ（動的に変更可能）</summary>
        private bool isCriminalRuntime = false;
        
        private SpriteRenderer spriteRenderer;
        
        private void Start()
        {
            InitializeNpc();
        }
        
        private void InitializeNpc()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            ApplyAppearance();
        }
        
        private void ApplyAppearance()
        {
            if (NpcData == null) return;
            
            if (spriteRenderer != null && NpcData.npcSprite != null)
                spriteRenderer.sprite = NpcData.npcSprite;
        }
        
        #region INpc実装
        
        public NpcData GetNpcData() => NpcData;
        
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