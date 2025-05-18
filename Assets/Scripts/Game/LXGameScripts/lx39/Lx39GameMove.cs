using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    public class Lx39GameMove : GameMoveManager
    {

        [HideInInspector]
        public int DanLine = -1;
        [HideInInspector]
        public int DanRow = -1;
        [HideInInspector]
        public bool IsHaveDan;
        [HideInInspector]
        public bool NeedPlayDan;

        protected string[] IconNames = { "0", "50", "51", "52", "1", "2", "3", "4", "1000", "dan" };

        private string NameInDan = "";

        protected override void Start()
        {
            base.Start();
            GetAllPos();
        }

        #region 移动图片并自动停止

        //每列最后阶段倒数几轮的滚动
        protected override void MoveLineIconInEnd(int num, int line)
        {
            if (EndRollNums[num] > 4)
            {
                for (int j = 0; j < AllChild[num].Count; j++)
                {
                    if (AllChild[num][j].GetComponent<MoveJettonIcon>().Move(Speeds[line] * Time.deltaTime * _speedTimes))
                    {
                        EndRollNums[num]--;
                        ResetJettonPos(num);
                        ChangeJettonOrder(AllChild[num]);
                    }
                }
            }
            else if (EndRollNums[num] >= 1 && EndRollNums[num] <= 4)
            {
                for (int j = 0; j < AllChild[num].Count; j++)
                {
                    if (AllChild[num][j].GetComponent<MoveJettonIcon>().Move(Speeds[line] * Time.deltaTime * _speedTimes))
                    {
                        EndRollNums[num]--;
                        ResetJettonPos(num);
                        if (EndRollNums[num] != 0)
                        {
                            if (IsHaveDan && line == DanLine && EndRollNums[num] == DanRow)
                            {
                                var _response = App.GetGameData<OverallData>().Response;
                                NameInDan = _response.GetJettonName(SpriteName);
                                AllChild[num][0].GetComponent<UISprite>().spriteName = SpriteName + "dan";//这个图片的顺序是从下到上从左到右一列一列显示的
                            }
                            else
                            {
                                var _response = App.GetGameData<OverallData>().Response;
                                AllChild[num][0].GetComponent<UISprite>().spriteName = _response.GetJettonName(SpriteName);
                            }
                            Facade.Instance<MusicManager>().Play("stopcard");
                        }
                        else
                        {
                            ChangeJettonOrder(AllChild[num]);
                            if (num == HoldTimes.Length - 1)
                            {
                                EventDispatch.Dispatch((int)EventID.GameEventId.WhenIconStop);
                                if (NeedPlayDan) EventDispatch.Dispatch((int)EventID.GameEventId.PlayZaDanEffect,
                                        new EventData(NameInDan));
                                InitData();
                            }
                        }
                    }
                }
            }
        }

        //随机图片名
        protected override string RandonSpriteName()
        {
            string temp;
            int num = Random.Range(0, IconNames.Length);
            temp = SpriteName + IconNames[num];
            return temp;
        }
        #endregion


        ///<summary>
        /// 从场景中通过物体的名字来获取所有的图片,并为其添加脚本
        /// </summary>
        protected virtual void GetAllPos()
        {
            if (AllChild == null || AllChildPos == null)
            {
                AllChild = new List<GameObject>[LineNum];
                AllChildPos = new List<Vector3>[LineNum];
            }
            for (int i = 0; i < LineNum; i++)
            {
                AllChild[i] = new List<GameObject>();
                AllChildPos[i] = new List<Vector3>();
                for (int j = 0; j < PosNum; j++)
                {
                    GameObject go = transform.FindChild(LineName + i + "/" + PosName + j).gameObject;
                    if (i == 0 && j == 0)
                    {
                        GameObject temp = transform.FindChild(LineName + i + "/" + PosName + (PosNum - 1)).gameObject;
                        dis = Mathf.Abs(temp.transform.localPosition.y + (temp.transform.localPosition.y - go.transform.localPosition.y) / (PosNum - 1));
                    }
                    if (go != null)
                    {
                        go.AddComponent<MoveJettonIcon>();
                        go.GetComponent<MoveJettonIcon>().DisY = dis;
                        AllChild[i].Add(go);
                        AllChildPos[i].Add(go.transform.localPosition);
                    }
                    else
                    {
                        Debug.Log("------> Get Child error in ShowGameJetton when i=" + i + "j=" + j);
                    }
                }
            }
        }
        #region 点击停止按钮时调用

        public override void StopRoll()
        {
            StopRollJettonNow();
            EventDispatch.Dispatch((int)EventID.GameEventId.WhenIconStop);
            //如果需要砸蛋,则播放砸蛋特效
            if (NeedPlayDan)
            {
                EventDispatch.Dispatch((int)EventID.GameEventId.PlayZaDanEffect, new EventData(NameInDan));
            }
            InitData();
        }

        //由于三九连线需要砸蛋,所以要在结束的时候随机一个位置有个蛋
        protected override void StopRollJettonNow()
        {
            for (int i = 0; i < AllChild.Length; i++)
            {
                for (int j = 0; j < AllChild[i].Count; j++)
                {
                    AllChild[i][j].transform.localPosition = AllChildPos[i][j];
                }
            }
            List<int> _jettons = App.GetGameData<OverallData>().Response.JettonList;
            int num = 0;
            //获得要显示的图片名
            for (int j = 0; j < AllChild.Length; j++)
            {
                for (int i = 0; i < AllChild[j].Count; i++)
                {
                    if (IsHaveDan && j == DanLine && i == DanRow)
                    {
                        NameInDan = SpriteName + _jettons[num++];
                        AllChild[j][i].GetComponent<UISprite>().spriteName = SpriteName + "dan";
                    }
                    else
                    {
                        if (i == 0)
                        {
                            string newName = RandonSpriteName();
                            AllChild[j][i].GetComponent<UISprite>().spriteName = newName;
                        }
                        else
                        {
                            AllChild[j][i].GetComponent<UISprite>().spriteName = SpriteName + _jettons[num++];
                        }
                    }
                }
            }
        }
        #endregion

        public override void GetJettonOrder()
        {
            App.GetGameData<OverallData>().Response.RegroupIconList();
            RandomShowDanAndSure();//如果不是三九连线,不需要砸蛋,则不需要执行
        }



        protected override void InitData()
        {
            base.InitData();
            NeedPlayDan = false;
            IsHaveDan = false;
            DanRow = -1;
            DanLine = -1;
        }

        #region 以下是三九连线出现蛋的概率

        private void RandomShowDanAndSure()
        {
            int dan = Random.Range(0, 10);
            if (dan == 0)
            {
                IsHaveDan = true;
                DanLine = Random.Range(0, 3);
                DanRow = Random.Range(1, 4);
                NeedPlayDan = App.GetGameData<OverallData>().Response.NeedPlayZaDan(DanLine, DanRow);
            }
            else
            {
                DanLine = -1;
                DanRow = -1;
                NeedPlayDan = false;
                IsHaveDan = false;
            }
        }

        #endregion

    }
}

