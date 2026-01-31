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
    [SerializeField] public GameObject Lose;
    [SerializeField] public GameObject Time;
    [SerializeField] public bool Result = false;//勝ちでtrue.負けでfalse.
    [SerializeField] public int ResultTime = 0;
    [SerializeField] public TMP_Text ScoreText;

        void Awake()
        {
            Win.SetActive(false);
            Lose.SetActive(false);
            Time.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            if(Result == true)
            {
                Win.SetActive(true);
                Time.SetActive(true);
            }else
            {
                Lose.SetActive(true);
            }

             ScoreText.text = "ResultTime:" + ResultTime;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
