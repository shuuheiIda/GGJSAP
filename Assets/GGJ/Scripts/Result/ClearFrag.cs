using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GGJ.Result
{
    public class ClearFrag : MonoBehaviour
    {
    [SerializeField] public GameObject Win;//今はテキストを格納だが、のちにイメージファイルに変えてもよい。
    [SerializeField] public GameObject WinStoryText;
    [SerializeField] public GameObject Lose;
    [SerializeField] public GameObject LoseStoryText;
    [SerializeField] public GameObject Time;//経過した時間（ResultTime:）のテキストオブジェクトを格納
    [SerializeField] public GameObject UseHint;//ヒントを聞いた数（UseHint:）のテキストオブジェクトを格納
    [SerializeField] public GameObject ReturnTitle;
    [SerializeField] public bool Result = false;//勝ちでtrue.負けでfalse.結果も別スクリプトから引っ張る。
    [SerializeField] public int ResultTime = 0;//この数字、ゲームクリア時に経過した時間を入れる。別のスクリプトから引っ張ってくる必要あり
    [SerializeField] public int UseHintPoint = 0;//この数字、NPCから情報を聞いた数が反映される予定。
    [SerializeField] public TMP_Text ScoreText;//経過した時間（ResultTime:）のテキストオブジェクトを格納
    [SerializeField] public TMP_Text UseHintText;//ヒントを聞いた数（UseHint:）のテキストオブジェクトを格納
   
        void Awake()
        {
            //
            Win.SetActive(false);
            WinStoryText.SetActive(false);
            Lose.SetActive(false);
            LoseStoryText.SetActive(false);
            Time.SetActive(false);
            UseHint.SetActive(false);
            ReturnTitle.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            if(Result == true)
            {
                Win.SetActive(true); 
                WinStoryText.SetActive(true);             
                Time.SetActive(true);
                UseHint.SetActive(true);
                ReturnTitle.SetActive(true);
            }else
            {
                Lose.SetActive(true);
                LoseStoryText.SetActive(true);
                ReturnTitle.SetActive(true);
            }

            ScoreText.text = "ResultTime:" + ResultTime;
            UseHintText.text = "UseHint:" + UseHintPoint;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
