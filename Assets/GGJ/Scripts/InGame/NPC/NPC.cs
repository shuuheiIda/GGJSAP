using UnityEngine;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// NPC本体のクラス
    /// シーン上に配置され、INpcインターフェースを実装
    /// </summary>
    public class NPC : MonoBehaviour, INpc
    {
        [Header("NPCデータ")]
        [SerializeField] private NPCData npcData;
        
        [Header("状態")]
        [SerializeField] private bool hasReceivedHint = false;
        
        /// <summary>実行時の犯人フラグ（動的に変更可能）</summary>
        private bool isCriminalRuntime = false;
        
        private SpriteRenderer spriteRenderer;
        
        private void Start()
        {
            InitializeNPC();
        }
        
        private void InitializeNPC()
        {
            // SpriteRendererを取得
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // NPCManagerに自身を登録
            if (NPCManager.I != null)
                NPCManager.I.RegisterNPC(this);
            
            // 外見を適用
            ApplyAppearance();
        }
        
        private void ApplyAppearance()
        {
            if (npcData == null) return;
            
            // スプライトを設定
            if (spriteRenderer != null && npcData.npcSprite != null)
                spriteRenderer.sprite = npcData.npcSprite;
            
            // 将来的に：色情報を各パーツに適用する処理
            // 例: 服の色、マスクの色などを子オブジェクトに適用
        }
        
        #region INpc実装
        
        public NPCData GetNPCData() => npcData;
        
        public string GetCurrentDialogue()
        {
            // NPCManagerからセリフを取得
            if (NPCManager.I != null)
                return NPCManager.I.GetDialogueForNPC(this);
            
            return "...";
        }
        
        public void SetHintReceived(bool received)
        {
            hasReceivedHint = received;
        }
        
        public bool HasReceivedHint() => hasReceivedHint;
        
        public Vector3 GetPosition() => transform.position;
        
        public GameObject GetGameObject() => gameObject;
        
        public void SetCriminal(bool isCriminal)
        {
            isCriminalRuntime = isCriminal;
        }
        
        public bool IsCriminal() => isCriminalRuntime;
        
        #endregion
    }
}
