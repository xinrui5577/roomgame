using System.Collections.Generic;
using YxFramwork.ConstDefine;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class ScoreDoubleTemplete
    {
        //实际值
        public int Value;
        //显示内容
        public string Context;
        //字号     
        public int Fontsize;
    }

    [System.Serializable]
    public class ScoreDoubleTempletes
    {
        public ScoreDoubleTemplete[] TempleteArrary;
    }

    [UIPanelData(typeof(PanelScoreDouble), UIPanelhierarchy.Popup)]
    public class PanelScoreDouble : UIPanelBase, IUIPanelControl<ScoreDoubleArgs>
    {
        public Sprite Sprite;
        public Transform GroupBg;
        public Transform BtnGroupParent;
        public HeadTextItem SdFlagItem;
        public ScoreDoubleBtnItem SdBtnItem;
        public List<ScoreDoubleTempletes> TempletesList;
        public string TitleContent;

        /// <summary>
        /// 加飘时不显示几分,只显示加不加飘.例如长春麻将中钓鱼只显示钓鱼不钓
        /// </summary>
        public bool IsOnlyShowStr;
        /// <summary>
        /// 只适应0和1
        /// </summary>
        public string[] TitleContents;

        private bool mFlagBtnInit;
        private HeadItemsCtrl mHeadCtrl;
        private List<HeadTextItem> mSdFlagItemListObj = new List<HeadTextItem>();

        private List<HeadTextItem> mSdFlagItemList {
            get {
                for (int i=0;i<mSdFlagItemListObj.Count;) {
                    if (mSdFlagItemListObj[i] == null)
                    {
                        mSdFlagItemListObj.RemoveAt(i);
                    }else
                    {
                        i++;
                    }
                }
                return mSdFlagItemListObj;
            }
        }

        private void Awake()
        {
            mHeadCtrl = transform.GetComponent<HeadItemsCtrl>();
        }

        public override void OnContinueGameUpdate()
        {
            Close();
        }

        public override void OnReconnectedUpdate()
        {
            var flag = GameCenter.DataCenter.Players.IsSende();
            if (flag)
            {
                GroupBg.gameObject.SetActive(false);
                for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
                {
                    var userInfo = DataCenter.Players[i];
                    SetHeadFlag(i, userInfo.ScoreDouble);
                }
                mHeadCtrl.Play(mSdFlagItemList);
            }             
        }

        public override void OnReadyUpdate()
        {
            mHeadCtrl.Reset(mSdFlagItemList);
        }

        public override void Open()
        {
            base.Open();
            //生成按钮
            if (!mFlagBtnInit)
            {
                mFlagBtnInit = true;
                GroupBg.gameObject.SetActive(true);
                ScoreDoubleBtnItem item;
                var temp = TempletesList[DataCenter.ConfigData.PuScore].TempleteArrary;
                for (int i = 0; i < temp.Length; i++)
                {
                    item = Instantiate(SdBtnItem).ExCompShow();
                    item.ExSetParent(BtnGroupParent);
                    item.OnInit(temp[i].Context, temp[i].Value, temp[i].Fontsize);
                    item.OnClick = OnBtnClick;
                }
            }
            else
            {
                GroupBg.gameObject.SetActive(true);
            }
        }

        public void Open(ScoreDoubleArgs args)
        {
            base.Open();
            if (null != args)
            {
                var arrray = args.ScoreDoubleArray;
                GroupBg.gameObject.SetActive(false);
                for (int i = 0; i < arrray.Length; i++)
                {
                    SetHeadFlag(i, arrray[i]);
                }
                mHeadCtrl.Play(mSdFlagItemList);
            }
        }

        private HeadTextItem SetHeadFlag(int index, int score)
        {
            HeadTextItem item;
            //生成对象
            if (index >= mSdFlagItemList.Count)
            {
                item = Instantiate(SdFlagItem);
                mSdFlagItemList.Add(item);
            }
            else
            {
                item = mSdFlagItemList[index];
                item.gameObject.SetActive(true);
            }
            if (IsOnlyShowStr && score < TitleContents.Length)
            {
                item.SetContext(TitleContents[score]);
            }
            else
            {
                item.SetContext(TitleContent.ExFormat(score));
            }
            return item;
        }

        public void OnBtnClick(int score)
        {
            GroupBg.gameObject.SetActive(false);
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                sfs.PutInt(RequestKey.KeyType, NetworkProls.ShowPiao);
                sfs.PutInt("piao", score);
                return sfs;
            });
        }
    }
}