using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

#pragma warning disable 649



namespace Assets.Scripts.Game.sssjp
{
    public class MatchMgr : MonoBehaviour
    {

        /// <summary>
        /// 是否正在展示播放动画
        /// </summary>
        protected bool IsMatching;

        /// <summary>
        /// 特殊牌型座位列表
        /// </summary>
        protected readonly List<UserMatchInfo> SpecialList = new List<UserMatchInfo>();

        /// <summary>
        /// 打枪列表
        /// </summary>
        protected readonly List<ShootInfo> ShootList = new List<ShootInfo>();

        /// <summary>
        /// 玩家手牌列表
        /// </summary>
        protected readonly List<UserMatchInfo> MatchInfoList = new List<UserMatchInfo>();

        /// <summary>
        /// 本玩家每道得分列表
        /// </summary>
        protected readonly List<DunScore> DunScoreList = new List<DunScore>();

        /// <summary>
        /// 特殊牌型整体特效
        /// </summary>
        [SerializeField]
        protected ParticleSystem SpecialParticle;

        /// <summary>
        /// 特殊牌型的文字特效
        /// </summary>
        [SerializeField]
        protected ParticleSystem SpecialLabelParticle;

        /// <summary>
        /// 开始比牌特效
        /// </summary>
        [SerializeField]
        private ParticleSystem _beginMatchParticle;

        /// <summary>
        /// 开始打枪特效
        /// </summary>
        [SerializeField]
        private ParticleSystem _beginShootParticle;


        [SerializeField]
        protected List<Texture> SpecialTextureList;


        /// <summary>
        /// 自己对每个玩家输赢分数,由服务器发送并本地记录
        /// </summary>
        protected int[] ShootScoreArray;

        public virtual void MatchCards(List<UserMatchInfo> usersHandCardList)
        {
            var gdata = App.GetGameData<SssGameData>();

            foreach (UserMatchInfo user in usersHandCardList)
            {
                if (user.Special > (int)CardType.none)
                {
                    SpecialList.Add(user);
                    if (gdata.SelfSeat != user.Seat)
                        continue;
                    DunScore dunScore = new DunScore
                    {
                        NormalScore = new List<int>(),
                        AddScore = new List<int>()
                    };

                    ShootScoreArray = user.ShootScore;
                    for (int i = 0; i < user.AddScore.Count; i++)
                    {
                        dunScore.NormalScore.Add(user.NormalScores[i]);
                        dunScore.AddScore.Add(user.AddScore[i]);
                    }
                    continue;
                }

                MatchInfoList.Add(user);
                if (user.Shoot != null && user.Shoot.ShootCount > 0)
                {
                    ShootList.Add(user.Shoot);
                }

                if (gdata.GetLocalSeat(user.Seat) != 0)
                    continue;


                ShootScoreArray = user.ShootScore;
                for (int i = 0; i < user.AddScore.Count; i++)
                {
                    DunScoreList.Add(new DunScore()
                    {
                        Seat = user.Seat,
                        NormalScore = user.NormalScores,
                        AddScore = user.AddScore
                    });
                }
            }

            MatchBegin();   //开始比牌
        }


        /// <summary>
        /// 开始比牌
        /// </summary>
        public virtual void MatchBegin()
        {
            //开始比牌
            if (!IsMatching)
            {
                YxDebug.Log(" === Match begin === ");
                IsMatching = true;
                StartCoroutine(MatchPlayerCards());
            }
        }

        /// <summary>
        /// 比牌全过程控制协程
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator MatchPlayerCards()
        {
            var main = App.GetGameManager<SssjpGameManager>();
            var gdata = App.GetGameData<SssGameData>();
            int selfSeat = gdata.SelfSeat;

            //每行开始展示
            if (SpecialList.Count > 0) //特殊牌型不参与比牌,给出特殊牌型提示
            {
                for (int i = 0; i < SpecialList.Count; i++)
                {
                    gdata.GetPlayer<SssPlayer>(SpecialList[i].Seat, true).HandCardsType.SetSpecialMarkActive(true);
                }
            }

            //播放开始比牌特效
            StartParticle(_beginMatchParticle);
            Facade.Instance<MusicManager>().Play("start");
            yield return new WaitForSeconds(0.6f);
            App.GetGameManager<SssjpGameManager>().PlayOnesSound("start_compare", App.GameData.GetPlayerInfo().SexI); //开始比牌音效
            yield return new WaitForSeconds(2);
            StopParticle(_beginMatchParticle);

            for (int i = 0; i < 3; i++) //展示手牌的行数
            {
                SortList(MatchInfoList, i); //对列表进行排序,这样可以依次展示手牌

                for (int j = 0; j < MatchInfoList.Count; j++)
                {
                    var tempInfo = MatchInfoList[j];
                    //展示玩家手牌
                    var player = gdata.GetPlayer<SssPlayer>(tempInfo.Seat, true);
                    player.ShowHandPoker(i, tempInfo);
                    player.ShowCardType(i, tempInfo);
                    yield return new WaitForSeconds(0.8f);
                }
                main.TurnRes.ShowResultItem(i, DunScoreList[0].NormalScore[i], DunScoreList[0].AddScore[i]); //显示每行的得分,只有自己的
            }


            ShootInfo swatItem = ShootList.Find(info => info.ShootTargs.Length > 2);
            if (swatItem != null && gdata.SwatModel)
                ShootList.Remove(swatItem);

            //播放打枪
            if (ShootList.Count > 0)
            {
                //播放开始打枪动画
                StartParticle(_beginShootParticle);
                main.PlayOnesSound("daqiang", App.GameData.GetPlayerInfo().SexI);
                yield return new WaitForSeconds(3f);
                StopParticle(_beginShootParticle);

                foreach (ShootInfo item in ShootList)
                {
                    if (item.ShootCount > 2) //如果是全垒打,打枪个数为3人
                        continue;

                    if (item.ShootTargs == null || item.ShootTargs.Length <= 0)
                    {
                        //ShootList.Remove(item);
                        continue;
                    }

                    for (int i = 0; i < item.ShootTargs.Length; i++)
                    {
                        int serverTargSeat = item.ShootTargs[i];
                        int serverShootSeat = item.Seat;

                        //gdata.GetPlayer<SssPlayer>(serverShootSeat, true).ShootSomeone(gdata.GetPlayer<SssPlayer>(serverTargSeat, true),
                        //    ((3 - (localTargSeat + 4 - localShootSeat) % 4) % 3));
                        gdata.GetPlayer<SssPlayer>(serverShootSeat, true)
                            .ShootSomeone(gdata.GetPlayer<SssPlayer>(serverTargSeat, true));

                        //打枪需要修改总分
                        if (serverShootSeat == selfSeat) //说明是自己打枪,获取额外得分
                        {
                            //设置总分数
                            int shootScore = gdata.ShootScore == 0 ? ShootScoreArray[serverTargSeat] : gdata.ShootScore;
                            main.TurnRes.ResultTotal.SetValue(shootScore);
                        }
                        if (serverTargSeat == selfSeat) //说明自己被打枪,扣除额外分数
                        {
                            int shootScore = gdata.ShootScore == 0
                                ? ShootScoreArray[serverShootSeat]
                                : -gdata.ShootScore;
                            main.TurnRes.ResultTotal.SetValue(shootScore);
                        }

                        yield return new WaitForSeconds(2.2f);
                    }
                }
            }

            //播放特殊牌型
            if (SpecialList.Count > 0)
            {
                foreach (UserMatchInfo item in SpecialList)
                {
                    Renderer component = SpecialLabelParticle.GetComponent<Renderer>();
                    CardType cardType = (CardType)item.Special;
                    string s = cardType.ToString();
                    component.material.mainTexture = SpecialTextureList.Find(tex => tex.name == s);


                    if (gdata.GetLocalSeat(item.Seat) == 0)
                    {
                        if (gdata.SwatModel || App.GameData.SelfSeat == 0)
                        {
                            int playerCount = 0;
                            foreach (var user in gdata.PlayerList)
                            {
                                var player = (SssPlayer)user;
                                if (player.Info != null && player.IsReady)
                                {
                                    ++playerCount;
                                }
                            }
                            main.TurnRes.ResultTotal.SetValue(GetSpecialScore(cardType) * (playerCount - 1));
                        }
                        else
                        {
                            main.TurnRes.ResultTotal.SetValue(GetSpecialScore(cardType));
                        }
                    }
                    else
                    {
                        int serSeat = (item.Seat + App.GameData.SelfSeat) % gdata.SeatTotalCount;
                        if (gdata.SwatModel || serSeat == 0 || App.GameData.SelfSeat == 0)
                        {
                            main.TurnRes.ResultTotal.SetValue(-GetSpecialScore(cardType));
                        }
                    }

                    //播放特殊牌型效果(特效)  item.AddScore
                    StartParticle(SpecialParticle);
                    StartParticle(SpecialLabelParticle);
                    Facade.Instance<MusicManager>().Play("teshupai");
                    yield return new WaitForSeconds(3f);
                    StopParticle(SpecialParticle);
                    gdata.GetPlayer<SssPlayer>(item.Seat, true).ShowAllHandPoker(item);
                }
            }

            //播放全垒打,有特殊牌型必然不会全垒打
            //全垒打必须是4个人玩
            else if (swatItem != null && (swatItem.ShootTargs != null && gdata.SwatModel))
            {
                main.ShowSwatAnim();
                Facade.Instance<MusicManager>().Play("swat");
                int swatSeat = swatItem.Seat;

                int shootScore = gdata.ShootScore;
                if (swatSeat == App.GameData.SelfSeat)
                {
                    int sum = 0;
                    foreach (int score in ShootScoreArray)
                    {
                        sum += score;
                    }
                    shootScore = shootScore > 0 ? shootScore * 2 * 3 : sum * 2;
                    main.TurnRes.ResultTotal.SetValue(sum + shootScore);
                }
                else
                {
                    int sum = ShootScoreArray[swatSeat];
                    shootScore = shootScore > 0 ? shootScore : -sum;
                    main.TurnRes.ResultTotal.SetValue(sum - shootScore * 2);
                }
            }

            IsMatching = false;
        }


        protected int GetSpecialScore(CardType specialType)
        {
            switch (specialType)
            {
                case CardType.santonghua:
                    return 3;
                case CardType.sanshunzi:
                    return 4;
                case CardType.liuduiban:
                    return 4;
                case CardType.wuduisan:
                    return 5;
                case CardType.sitiaosan:
                    return 6;
                case CardType.couyise:
                case CardType.quanxiao:
                case CardType.quanda:
                    return 10;
                case CardType.sanzhadan:
                case CardType.santonghuashun:
                    return 20;
                case CardType.shierhuang:
                    return 24;
                case CardType.shisanshui:
                    return 36;
                case CardType.tonghuashisanshui:
                    return 108;
                default:
                    return 0;
            }
        }



        /// <summary>
        /// 对列表进行排序(注:对传入的list进行排序)
        /// </summary>
        /// <param name="infoList">要排序的列表</param>
        /// <param name="line">根据第几行的信息进行排序</param>
        protected void SortList(List<UserMatchInfo> infoList, int line)
        {
            infoList.Sort((info1, info2) => info1.NormalScores[line] - info2.NormalScores[line]);
            //将列表按当前行手牌进行从小到大排列
            //infoList.Sort((info1, info2) =>
            //{
            //if (info1.DunTypeList[line] != info2.DunTypeList[line])
            //{
            //    return info1.DunTypeList[line] - info2.DunTypeList[line];
            //}
            //else
            //{
            //    int beginIndex = 0;
            //    int count = 0;
            //    switch (line)
            //    {
            //        case 0:
            //            beginIndex = 0;
            //            count = 3;
            //            break;
            //        case 1:
            //            beginIndex = 3;
            //            count = 5;
            //            break;
            //        case 2:
            //            beginIndex = 8;
            //            count = 5;
            //            break;
            //    }
            //    List<int> list1 = info1.Cards.GetRange(beginIndex, count);
            //    List<int> list2 = info2.Cards.GetRange(beginIndex, count);
            //    SortList(list1);
            //    SortList(list2);

            //    HelpLz.VnList vn1 = new HelpLz.VnList(list1);
            //    HelpLz.VnList vn2 = new HelpLz.VnList(list2);
            //    return vn1.CompareLine(vn2);
            //}
            //});
        }

        /// <summary>
        /// 将列表从小到大排列
        /// </summary>
        /// <param name="list"></param>
        void SortList(List<int> list)
        {
            list.Sort((x, y) =>
            {

                int x1 = x % 16;
                int y1 = y % 16;
                x1 = x1 == 1 ? 14 : x1;
                y1 = y1 == 1 ? 14 : y1;
                return x1 - y1;
            });
        }


        /// <summary>
        /// 开始特效
        /// </summary>
        /// <param name="particle">要修改的特效</param>
        protected void StartParticle(ParticleSystem particle)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }

        /// <summary>
        /// 停止特效
        /// </summary>
        /// <param name="particle">要修改的特效</param>
        protected void StopParticle(ParticleSystem particle)
        {
            particle.Stop();
            particle.gameObject.SetActive(false);
        }


        public virtual void Reset()
        {
            ShootList.Clear();
            SpecialList.Clear();
            MatchInfoList.Clear();
            DunScoreList.Clear();

            StopParticle(_beginShootParticle);
            StopParticle(_beginMatchParticle);
            StopParticle(SpecialParticle);

            IsMatching = false;
            StopAllCoroutines();
        }

        protected void OnDestroy()
        {
            Reset();
        }
    }

    public struct UserMatchInfo
    {
        /// <summary>
        /// 服务器座位号
        /// </summary>
        public int Seat;
        /// <summary>
        /// 特殊牌型 51是没有
        /// </summary>
        public int Special;
        /// <summary>
        /// 总得分
        /// </summary>
        public int TtScore;
        /// <summary>
        /// 全垒打
        /// </summary>
        public bool Swat;
        /// <summary>
        /// 牌信息
        /// </summary>
        public List<int> Cards;
        /// <summary>
        /// 每道牌的牌型
        /// </summary>
        public List<int> DunTypeList;
        /// <summary>
        /// 普通得分
        /// </summary>
        public List<int> NormalScores;
        /// <summary>
        /// 额外牌型加分
        /// </summary>
        public List<int> AddScore;
        /// <summary>
        /// 打枪信息
        /// </summary>
        public ShootInfo Shoot;
        /// <summary>
        /// 打枪得分
        /// </summary>
        public int[] ShootScore;
    }

    public class ShootInfo
    {
        public int Seat;
        /// <summary>
        /// 打枪次数
        /// </summary>
        public int ShootCount;
        /// <summary>
        /// 被打枪次数
        /// </summary>
        public int BeShootCount;
        public int[] ShootTargs;
    }

}
