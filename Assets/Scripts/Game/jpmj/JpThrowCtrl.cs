using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    public class JpThrowCtrl : MonoBehaviour
    {

        [SerializeField]
        public JpTableData JpTbData;
        [SerializeField]
        protected int Chair;
        [SerializeField]
        protected MahjongThrow MjongThrow;


        // Use this for initialization
        void Start()
        {
            JpTbData.OnGetPlayerNum += OnGetplayerNum;
        }

        private void OnGetplayerNum(int playerNum)
        {
            if (playerNum == 2)
            {
                if (Chair == 0)
                {
                    gameObject.transform.localPosition = new Vector3(2.4f, 0.175f,0.81f);
                    MjongThrow.RowCnt = 16;
                    MjongThrow.ColCnt = 3;
                }
                else if (Chair == 3)
                {
                    gameObject.transform.localPosition = new Vector3(-2.4f, 0.175f, -0.5f);
                    MjongThrow.RowCnt = 16;
                    MjongThrow.ColCnt = 3;
                }
            }
            else if (playerNum >= 3)
            {
                switch (Chair)
                {
                    case 0:
                        {
                            gameObject.transform.localPosition = new Vector3(0.57f, 0.175f,0.75f);
                            MjongThrow.RowCnt = 9;
                            MjongThrow.ColCnt = 3;
                            break;
                        }
                    case 1:
                        {
                            gameObject.transform.localPosition = new Vector3(-0.71f, 0.175f,0.62f);
                            MjongThrow.RowCnt = 7;
                            MjongThrow.ColCnt = 4;
                            break;
                        }
                    case 2:
                        {
                            gameObject.transform.localPosition = new Vector3(0.63f, 0.175f, -0.5f);
                            MjongThrow.RowCnt = 7;
                            MjongThrow.ColCnt = 4;
                            break;
                        }
                    case 3:
                        {
                            gameObject.transform.localPosition = new Vector3(-0.67f, 0.175f, -0.5f);
                            MjongThrow.RowCnt = 9;
                            MjongThrow.ColCnt = 3;
                            break;
                        }
                }
            }


        }

    }
}
