using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class PlayerRecord : MonoBehaviour
    {
        #region Instance
        private static PlayerRecord m_instance;
        public static PlayerRecord GetInstance()
        {
            return m_instance;
        }
        #endregion

        public UILabel ownRecord;
        private long markBitCoin;

        void Awake()
        {
            m_instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (markBitCoin == 0)
            {
                markBitCoin = GetComponent<CustomInfo>().Coin;
                //Debug.LogError("MarkBitCoin: " + markBitCoin);
            }
        }

        public void ShowRealResultRecord()
        {
            ownRecord.text = "￥" + YxFramwork.Tool.YxUtiles.GetShowNumberForm((GetComponent<CustomInfo>().Coin - markBitCoin));
        }
    }
}