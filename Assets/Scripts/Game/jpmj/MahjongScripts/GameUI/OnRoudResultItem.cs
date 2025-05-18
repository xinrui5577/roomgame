using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Tool;
using AsyncImage = Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool.AsyncImage;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class OnRoudResultItem : MonoBehaviour
    {
        public Text UserName;
        public Text HuInfo;
        public Text HuType;
        public Text HuScore;
        public Text GangScore;
        public Text Score;
        //郑谭 鸟的分数
        public Text NiaoScore;
        public Image HuIcon;
        public Image BankerTag;
        public Text PiaoScore;
        public GameObject FangPao;

        /// <summary>
        /// 头像
        /// </summary>
        public RawImage HeadImg;

        public HdCdsZone CardGroup;

        //玩家位置
        [HideInInspector]public int Chair = -1;
        public Text UserId;

        public string Name
        {
            set { UserName.text = value; }
        }

        public string UserID
        {
            set
            {
                if (UserId != null)
                {
                    UserId.text = value;
                }
            }
        }

        public string StrHuInfo
        {
            set { HuInfo.text = value; }
        }
        //郑谭  鸟的分数
        public virtual int SetNiaoScore
        {
            set
            {
                if (NiaoScore == null) return;
                NiaoScore.text = YxUtiles.GetShowNumber(value).ToString(CultureInfo.InvariantCulture);
            }
        }
        public virtual int IntHuScore
        {
            set { HuScore.text = YxUtiles.GetShowNumber(value).ToString(CultureInfo.InvariantCulture);}
        }

        public virtual int IntGangScore
        {
            set { GangScore.text = YxUtiles.GetShowNumber(value).ToString(CultureInfo.InvariantCulture); }
        }

        public virtual int IntScore
        {
            set { Score.text = YxUtiles.GetShowNumber(value).ToString(CultureInfo.InvariantCulture); }
        }

        public bool IsHu
        {
            set { if (HuIcon != null) HuIcon.gameObject.SetActive(value); }
        }

        public bool IsBanker
        {
            set { BankerTag.gameObject.SetActive(value); }
        }

        public bool SetVisible
        {
            set { gameObject.SetActive(value); }
        }

        public int SetPiaoScore
        {
            set
            {
                if (PiaoScore != null)
                {
                    PiaoScore.text = YxUtiles.GetShowNumber(value).ToString(CultureInfo.InvariantCulture);
                }
            }
        }


        public int HuTypeValue
        {
            set
            {
                if (HuType != null)
                {
                    if (value == 0)
                    {
                        HuType.text = "";
                    }
                    if (value == 1)
                    {
                        HuType.text = "胡";
                    }
                    if (value == 2)
                    {
                        HuType.text = "自摸";
                    }
                    if (value == -1)
                    {
                        HuType.text = "点炮";
                        if (FangPao != null)
                        {
                            FangPao.SetActive(true);
                        }
                    }
                }
            }

        }

        public virtual void SetCpgCard(List<CpgData> value)
        {
            foreach (CpgData cpgData in value)
            {
                UiCardGroup uiGroup = D2MahjongMng.Instance.GetGroup(cpgData.AllCards().ToArray(), EnD2MjType.Up, false, UtilDef.NullMj, cpgData.Type);

                CardGroup.AddUiCdGroup(uiGroup);
            }
        }

        public virtual void SetCpgCardJp(List<CpgData> value, int laizi = UtilDef.NullMj)
        {
            foreach (CpgData cpgData in value)
            {
                UiCardGroup uiGroup = D2MahjongMng.Instance.GetGroup(cpgData.AllCards().ToArray(), EnD2MjType.Up, false, laizi, cpgData.Type);

                CardGroup.AddUiCdGroup(uiGroup);
            }
        }

        public virtual void SetHardCard(int[] value, int laizi)
        {
            UiCardGroup uiGroupCard = D2MahjongMng.Instance.GetGroup(value, EnD2MjType.Up, false, laizi);
            CardGroup.AddUiCdGroup(uiGroupCard);
        }

        public virtual void SetHuCard(int value)
        {
            UiCardGroup uiGroupCard = D2MahjongMng.Instance.GetGroup(value, EnD2MjType.Up);
            CardGroup.AddUiCdGroup(uiGroupCard);
        }

        public void SortCardGroup()
        {
            CardGroup.Sort(0);
        }

        public void Cleare()
        {
            Chair = -1;
            CardGroup.Clear();
            IsHu = false;
            gameObject.SetActive(false);
            if (FangPao != null)
            {
                FangPao.SetActive(false);
            }
        }

        public void SetHeadImg(string url, Texture define)
        {
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url, HeadImg, define);
        }
    }
}
