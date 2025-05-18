using System.Collections.Generic;
using YxFramwork.Common;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelTotalResult), UIPanelhierarchy.System)]
    public class PanelTotalResult : UIPanelBase
    {
        public GameObject ContinueGameBtn;
        public TotalResultItem[] Players;

        private bool mIsShow;

        public override void OnContinueGameUpdate()
        {
            ResetPlayersItem();
            Close();
        }

        public void OnContinueGameClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.OnCreateNewGame);
        }

        private void ResetPlayersItem()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].gameObject.SetActive(false);
            }
            mIsShow = false;
        }

        public override void Open()
        {
            //防止再次打开
            if (mIsShow) return;
            mIsShow = true;

            base.Open();
            TotalResult result;
            TotalResultItem item;
            var db = GameCenter.DataCenter;
            ContinueGameBtn.SetActive(db.ConfigData.ContinueNewGame);
            var time = transform.GetComponent<TimeSign>();
            if (time != null) time.UpdataTimeText();
            ResetPlayersItem();
            List<TotalResult> resultDatas = db.Game.TotalResult;
            SortPlayerData(resultDatas, db.Players.OwnerSeat);
            int maxPao = 0; //点炮次数最多
            int maxGold = 0;//最高分
            int minGold = 0;//最低分
            for (int i = 0; i < resultDatas.Count; i++)
            {
                if (resultDatas[i].Glod > maxGold)
                {
                    maxGold = resultDatas[i].Glod;
                }
                if (resultDatas[i].Pao > maxPao)
                {
                    maxPao = resultDatas[i].Pao;
                }
                if (resultDatas[i].Glod < minGold)
                {
                    minGold = resultDatas[i].Glod;
                }
            }
            //设置信息
            for (int i = 0; i < resultDatas.Count; i++)
            {
                item = Players[i];
                if (item == null) { continue; }
                result = resultDatas[i];
                int chair = MahjongUtility.GetChair(result.Seat);
                item.Reset();
                item.gameObject.SetActive(true);
                item.UserId = result.Id.ToString();
                item.UserName = result.Name;
                item.IsFangZhu = db.Players.IsOwer(result.Seat);

                item.SetHeadImg(db.Players.GetPlayerHead(chair));
                //自摸次数                
                item.Container.SetItem(TextType.ZimoNum, result.Zimo.ToString());
                //胡牌次数                
                item.Container.SetItem(TextType.JiepaoNum, result.Hu.ToString());
                //点炮次数                
                item.Container.SetItem(TextType.DianpaoNum, result.Pao.ToString());
                //明杠次数                
                item.Container.SetItem(TextType.MinggangNum, result.Gang.ToString());
                //暗杠次数                
                item.Container.SetItem(TextType.AngangNum, result.AnGang.ToString());
                //摸宝次数                
                item.Container.SetItem(TextType.MoBao, result.MoBao.ToString());
                //冲宝次数                
                item.Container.SetItem(TextType.ChBao, result.ChBao.ToString());
                //杠开次数
                item.Container.SetItem(TextType.GangKaiNum, result.Gangkais.ToString());
                //总分数
                string info = MahjongUtility.GetShowNumberFloat(result.Glod).ToString();
                item.Container.SetItem(TextType.TotalSocre, info);
                item.IsBigWinner = false;
                item.IsBestPao = false;
                if (result.Glod == maxGold && maxGold != 0)
                {
                    item.IsBigWinner = true;
                }
                else if (result.Pao == maxPao && maxPao != 0)
                {
                    item.IsBestPao = result.Glod == minGold && minGold != 0;
                }
            }
        }

        /// <summary>
        /// 排序玩家信息
        /// </summary>
        private void SortPlayerData(List<TotalResult> resultDatas, int ownerSeat)
        {
            resultDatas.Sort((a, b) =>
            {
                if (ownerSeat == a.Seat && ownerSeat != b.Seat) return -1;
                if (ownerSeat != a.Seat && ownerSeat == b.Seat) return 1;
                if (a.Seat < b.Seat)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
        }

        /// <summary>
        /// 点击回退按钮
        /// </summary>
        public void OnReturnToHallClick()
        {
            Close();
            App.QuitGame();
        }

        /// <summary>
        /// 总战绩截图分享
        /// </summary>
        public void OnshareResultScreen()
        {
            GameUtils.DoScreenShot(this, new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    imageUrl = "file://" + imageUrl;
                }
                GameUtils.WeChatShareGameResult(imageUrl);
            });
        }

        /// <summary>
        /// 显示分享界面
        /// </summary>
        public void OnShowPanelShare()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.ShowPlaneShare);       
        }
    }
}