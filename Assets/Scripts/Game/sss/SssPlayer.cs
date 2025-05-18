using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.sss.ImgPress.Main;
using Assets.Scripts.Game.sss.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Tool;



namespace Assets.Scripts.Game.sss
{
    public class SssPlayer : YxBaseGamePlayer
    {

        [HideInInspector]
        public List<GameObject> UserCardList = new List<GameObject>();

        /// <summary>
        /// 理牌中字样
        /// </summary>
        [SerializeField]
        private UILabel _sortLabel;

        private float _spaceTime = 0.05f;

        public Transform PokerParent;

        /// <summary>
        /// 手牌为三行时,
        /// </summary>
        public Transform ThreeLinesParent;

        public HandCardType HandCardsType;


        public Shoot ShootAnim;

        public GameObject[] Holes;

        /// <summary>
        /// 显示总结算分数的数值
        /// </summary>
        public UILabel ResultGoldLabel;

        public Transform BetParent;

        [HideInInspector]
        public int Seat;


        /// <summary>
        /// 准备状态
        /// </summary>
        [HideInInspector]
        public bool IsReady;

        public GameObject ReadyMark;

        public GameObject BankerMark;

        /// <summary>
        /// 确立中心位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTargetPos()
        {
            if (UserCardList.Count == 0)
                return Vector3.zero;

            return Vector3.right * 16 * (UserCardList.Count - 1 - 7);

        }


        /// <summary>
        /// 进入等待状态
        /// </summary>
        public void Waitting()
        {
            if (UserCardList.Count == 0)
                return;

            if (!_sorting)
            {
                _sorting = true;
                StartCoroutine(SortLabel());
                _sortLabel.gameObject.SetActive(true);
            }
            InvokeRepeating("DoWaitting", 0, UserCardList.Count * _spaceTime + 1);
        }


        private bool _sorting;
        // ReSharper disable once FunctionRecursiveOnAllPaths
        IEnumerator SortLabel()
        {
            string tempStr = "理牌中";
            _sortLabel.text = tempStr;
            while (_sorting)
            {
                _sortLabel.text += ".";

                if (_sortLabel.text.Length > 6)
                {
                    _sortLabel.text = tempStr;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private bool _jumping;
        /// <summary>
        /// 播放等待动画,Invoke中调用
        /// </summary>
        protected void DoWaitting()
        {
            if (UserCardList.Count == 0)
                return;
            if (!_jumping)
            {
                InvokeRepeating("DoJump", 0, _spaceTime);
                _jumping = true;
            }
        }

        /// <summary>
        /// 卡牌排列到一行
        /// </summary>
        public void RepositionCards()
        {
            if (UserCardList.Count == 0)
                return;
            int mid = (UserCardList.Count + 1) / 2;
            for (int i = 0; i < UserCardList.Count; i++)
            {
                Transform pokerTran = UserCardList[i].transform;
                pokerTran.parent = PokerParent;
                pokerTran.transform.localPosition = Vector3.right * 16 * (i - mid);
                pokerTran.transform.localScale = Vector3.one * _scale;
                pokerTran.transform.localEulerAngles = Vector3.zero;
            }

            //刷新一次扑克的父层级,保证子层级的sprite渲染正确
            PokerParent.gameObject.SetActive(false);
            PokerParent.gameObject.SetActive(true);
        }


        /// <summary>
        /// 跳牌动画的计数
        /// </summary>
        int _jumpCounter;

        /// <summary>
        /// 扑克跳动动画,Invoke中调用
        /// </summary>
        protected void DoJump()
        {
            if (UserCardList.Count == 0)
                return;
            if (_jumpCounter >= UserCardList.Count)
            {
                _jumpCounter -= UserCardList.Count;
                CancelInvoke("DoJump");
                _jumping = false;

                return;
            }
            UserCardList[_jumpCounter++].GetComponent<PokerCard>().PokerJump();
        }


        /// <summary>
        /// 停止等待动作
        /// </summary>
        public void StopWaitting()
        {
            CancelInvoke();
            _jumping = false;
            _sorting = false;
            _jumpCounter = 0;
            _sortLabel.text = "理牌中";
            StopCoroutine("SortLabel");
            _sortLabel.gameObject.SetActive(false);
            foreach (GameObject poker in UserCardList)
            {
                poker.GetComponent<PokerCard>().StopJump();
            }
            RepositionCards();
        }

        private int _handSpaceX = 30;       //同行手牌的x轴间距
        private int _handSPaceY = -4;       //同行手牌的y轴间距
        private float _rotationZ = -10;     //牌的倾斜角度
        private float _moveTime = 0.5f;     //移动牌的时长
        private float _lineSpace = 55;      //行与行之间的间距

        public bool FinishChoise;
        public virtual void FinishChoiseCards()
        {
            FinishChoise = true;
            MoveHandCardWithAnim();
        }

        /// <summary>
        /// 将手牌排列成三排,有过程
        /// </summary>
        public void MoveHandCardWithAnim()
        {
            if (UserCardList.Count < 13 || !FinishChoise)
                return;

            StopWaitting();

            MoveToLineWithAnim(0, 3, 1);    //移动第一行三张牌
            MoveToLineWithAnim(3, 5, 2);    //移动第二行五张牌
            MoveToLineWithAnim(8, 5, 3);    //移动第三行五张牌
        }


        /// <summary>
        /// 移动手牌到某行,有过程
        /// </summary>
        /// <param name="beginIndex">开始的数组索引</param>
        /// <param name="cardCount">要求的手牌个数</param>
        /// <param name="line">第几行</param>
        void MoveToLineWithAnim(int beginIndex, int cardCount, int line)
        {
            int mid = beginIndex + (cardCount - 1) / 2;
            int lineDif = 2 - line;     //行差
            for (int i = beginIndex; i < beginIndex + cardCount; i++)
            {
                GameObject poker = UserCardList[i];
                poker.transform.parent = ThreeLinesParent;
                int temp = i - mid;
                MoveWithAnim(poker, temp, lineDif);
                RotateWithAnim(poker, temp);
            }
        }


        /// <summary>
        /// 移动卡牌到指定位置,有过程
        /// </summary>
        /// <param name="poker">要移动的目标牌</param>
        /// <param name="index">与中心位置牌索引的差值(index-mid),index 从0起算</param>
        /// <param name="lineDif">行差</param>
        void MoveWithAnim(GameObject poker, int index, int lineDif)
        {
            Vector3 tarPos = new Vector3(index * _handSpaceX, Mathf.Abs(index) * _handSPaceY + _lineSpace * lineDif, 0);
            MoveCard(poker, tarPos);
        }

        /// <summary>
        /// 移动牌，有过程
        /// </summary>
        /// <param name="poker">移动扑克</param>
        /// <param name="targetPOs">目标位置</param>
        void MoveCard(GameObject poker, Vector3 targetPOs)
        {
            var tp = poker.GetComponent<TweenPosition>();
            tp.from = poker.transform.localPosition;
            tp.to = targetPOs;
            tp.duration = _moveTime;

            tp.ResetToBeginning();
            tp.PlayForward();
        }

        /// <summary>
        /// 旋转到指定角度,有过程
        /// </summary>
        /// <param name="poker"></param>
        /// <param name="index">与中心位置牌索引的差值(index-mid)</param>
        void RotateWithAnim(GameObject poker, int index)
        {
            Vector3 rotate = GetTargetEulerAngles(index);
            RotateCard(poker, rotate);
        }

        /// <summary>
        /// 旋转牌,有过程
        /// </summary>
        /// <param name="poker">旋转的牌</param>
        /// <param name="targetRot">目标角度</param>
        void RotateCard(GameObject poker, Vector3 targetRot)
        {
            var tr = poker.GetComponent<TweenRotation>();
            tr.from = poker.transform.eulerAngles;
            tr.to = targetRot;
            tr.duration = _moveTime;

            tr.ResetToBeginning();
            tr.PlayForward();
        }


        /// <summary>
        /// 将手牌变为三行,无过程
        /// </summary>
        public void MoveHandCardNoAnim()
        {
            MoveCardNoAnim(0, 3, 1);    //移动第一行牌
            MoveCardNoAnim(3, 5, 2);    //移动第二行牌
            MoveCardNoAnim(8, 5, 3);    //移动第三行牌
        }

        private float _scale = 0.6f;
        protected void CreatCards(int pokerCount, Transform parent)
        {
            if (UserCardList.Count <= 0)
            {
                YxDebug.LogError("玩家无手牌,信息错误");
                return;
            }

            for (int i = 0; i < pokerCount; i++)
            {
                GameObject poker = Instantiate(UserCardList[0]);
                poker.transform.parent = parent;
                poker.transform.localPosition = Vector3.zero;
                poker.transform.localScale = Vector3.one * _scale;

                UserCardList.Add(poker);
            }
        }

        /// <summary>
        /// 移动卡牌到指定位置,无动画
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="cardCount"></param>
        /// <param name="line"></param>
        void MoveCardNoAnim(int beginIndex, int cardCount, int line)
        {
            int mid = beginIndex + (cardCount - 1) / 2;
            int lineDif = 2 - line;
            for (int i = beginIndex; i < beginIndex + cardCount; i++)
            {
                GameObject poker = UserCardList[i];
                poker.GetComponent<PokerCard>().StopAllTween();
                int temp = i - mid;
                poker.transform.parent = ThreeLinesParent;
                poker.transform.localPosition = GetTargetPosition(temp, lineDif);
                poker.transform.localEulerAngles = GetTargetEulerAngles(temp);
            }
        }

        /// <summary>
        /// 获得目标的本地坐标值
        /// </summary>
        /// <param name="index">与中心位置牌索引的差值(index-mid)</param>
        /// <param name="lineDif">与中间行的行差(lineIndex - mid),lineIndex从1开始,3行mid = 2</param>
        /// <returns></returns>
        Vector3 GetTargetPosition(int index, int lineDif)
        {
            return new Vector3(index * _handSpaceX, GetSum(index) * _handSPaceY + _lineSpace * lineDif, 0);
        }

        /// <summary>
        /// 获得一个整数绝对值的n!的值
        /// </summary>
        /// <param name="v">数据</param>
        int GetSum(int v)
        {
            int sum = 0;
            v = Mathf.Abs(v);
            for (int i = v; i > 0; i--)
            {
                sum += i;
            }

            return sum;
        }

        /// <summary>
        /// 获得指定的角度值
        /// </summary>
        /// <param name="index">与中心位置牌索引的差值(index-mid)</param>
        /// <returns></returns>
        Vector3 GetTargetEulerAngles(int index)
        {
            return new Vector3(0, 0, (index * _rotationZ));
        }

        public void ShowAllHandPoker(List<int> cardsValList)
        {
            if (UserCardList.Count < cardsValList.Count)
                return;

            for (int i = 0; i < cardsValList.Count; i++)
            {
                var poker = UserCardList[i].GetComponent<PokerCard>();
                poker.SetCardId(cardsValList[i]);
                poker.SetCardFront();
            }
        }


        /// <summary>
        /// 逐行展示手牌
        /// </summary>
        /// <param name="line">行数</param>
        /// <param name="cardValueList">牌</param>
        /// <param name="type">牌型</param>
        public void ShowHandPoker(int line, List<int> cardValueList, CardType type)
        {
            int beginIndex = 0;
            int count = 0;
            InitRange(line, ref beginIndex, ref count);

            List<PokerCard> curShowCards = new List<PokerCard>();
            //设置显示手牌
            for (int i = 0; i < UserCardList.Count; i++)
            {
                GameObject obj = UserCardList[i];
                if (i >= beginIndex && i < beginIndex + count)
                {
                    obj.transform.localScale = Vector3.one * (_scale + .1f);
                    PokerCard card = obj.GetComponent<PokerCard>();
                    card.SetCardDepth(100 + i * 2);
                    card.SetCardId(cardValueList[i]);
                    card.SetCardFront();
                    curShowCards.Add(card);
                }
                else
                {
                    obj.transform.localScale = Vector3.one * _scale;
                    obj.GetComponent<PokerCard>().SetCardDepth(i * 2);
                }
            }

            string typeName;
            if (line == 0 && type == CardType.santiao)
            {
                typeName = "chongsan";
            }
            else if (line == 1 && type == CardType.hulu)
            {
                typeName = "zhongdunhulu";
            }
            else
            {
                typeName = type.ToString();
            }

            App.GetGameManager<SssGameManager>().PlayOnesSound(typeName, Info.Seat);
            HandCardsType.ShowType(line, typeName); //显示手牌牌型

            StartCoroutine(HideHandPoker(line, beginIndex, count));
        }


        IEnumerator HideHandPoker(int line, int beginIndex, int count)
        {
            yield return new WaitForSeconds(1.5f);
            HandCardsType.HideType(line);

            for (int i = 0; i < count; i++)
            {
                int index = beginIndex + i;
                if (index >= UserCardList.Count || UserCardList[index] == null)
                    break;

                GameObject obj = UserCardList[index];
                PokerCard poker = obj.GetComponent<PokerCard>();
                poker.transform.localScale = Vector3.one * _scale;
                poker.SetCardDepth((beginIndex + i) * 2);
            }

            StopAllCoroutines();
        }


        /// <summary>
        /// 根据行数,确定开始索引和牌的个数
        /// </summary>
        /// <param name="line"></param>
        /// <param name="beginIndex">开始索引</param>
        /// <param name="count">牌的个数</param>
        void InitRange(int line, ref int beginIndex, ref int count)
        {
            if (beginIndex < 0) return;
            switch (line)
            {
                case 0:
                    beginIndex = 0;
                    count = 3;
                    break;
                case 1:
                    beginIndex = 3;
                    count = 5;
                    break;
                case 2:
                    beginIndex = 8;
                    count = 5;
                    break;
                default:
                    beginIndex = 0;
                    count = 13;
                    break;
            }
        }


        /// <summary>
        /// 滚动显示结算筹码值
        /// </summary>
        /// <param name="result"></param>
        public void ShowResultLabel(int result)
        {
            ResultGoldLabel.gameObject.SetActive(true);
            SetLabel(ResultGoldLabel, result);
            ResultGoldLabel.GetComponent<Animator>().Play("ResultAnim", 0, 0);
        }


        /// <summary>
        /// 销毁所有卡牌
        /// </summary>
        protected void DestroyPokers()
        {
            StopWaitting();
            if (UserCardList.Count == 0)
                return;
            ClearCards();
        }


        /// <summary>
        /// 打枪动画的位置
        /// </summary>
        [SerializeField]
        private Vector3[] _gunPositions;

        /// <summary>
        /// 打枪动画的角度
        /// </summary>
        [SerializeField]
        private Vector3[] _gunEulers;

        /// <summary>
        /// 打枪
        /// </summary>
        /// <param name="someone">打枪的目标人物</param>
        /// <param name="index">打枪的枪号索引</param>
        public void ShootSomeone(SssPlayer someone, int index)
        {
            YxDebug.Log(" ==== 射击的是" + index + "号枪 === ");
            ShootAnim.transform.localEulerAngles = _gunEulers[index];
            ShootAnim.transform.localPosition = _gunPositions[index];
            ShootAnim.DoShoot(someone);
        }

        /// <summary>
        /// 设置准备按钮是否显示
        /// </summary>
        /// <param name="active"></param>
        public virtual void SetReadyBtnActive(bool active)
        {

        }

        public virtual void OnGameStart()
        {
            SetReadyBtnActive(false);
            SetReadyMarkActive(false);
        }

        /// <summary>
        /// 设置准备标记是否显示
        /// </summary>
        /// <param name="active"></param>
        public void SetReadyMarkActive(bool active)
        {
            if (ReadyMark == null)
                return;

            ReadyMark.SetActive(active);
        }


        /// <summary>
        /// 清理掉所有卡牌
        /// </summary>
        void ClearCards()
        {
            foreach (GameObject card in UserCardList)
            {
                if (card != null)
                    Destroy(card);
            }
        }


        private void SetLabel(UILabel label, int result)
        {
            label.text = YxUtiles.ReduceNumber(result);
            if (result < 0)
            {
                label.gradientTop = Tools.ChangeToColor(0x8CFFFA);
                label.gradientBottom = Tools.ChangeToColor(0x0060FF);
                label.effectColor = Tools.ChangeToColor(0x03052E);
            }
            else
            {
                label.gradientTop = Tools.ChangeToColor(0xFFFF00);
                label.gradientBottom = Tools.ChangeToColor(0xFF9600);
                label.effectColor = Tools.ChangeToColor(0x2E0303);
            }
        }

        public void PlayerPay(int count)
        {
            InvokeRepeating("PayOne", 0, 0.03f);
        }

        public void OnClickReady()
        {
            App.GetRServer<SssGameServer>().ReadyGame();
        }

        public virtual void OnUserReady()
        {
            SetReadyBtnActive(false);
            SetReadyMarkActive(true);
        }

        public void OnAllowReady()
        {
            IsReady = false;
            SetReadyBtnActive(true);
            SetReadyMarkActive(false);
        }

        /// <summary>
        /// 当可以开始的时候
        /// </summary>
        public virtual void OnCouldStart()
        {

        }


        public void Reset()
        {
            ShootAnim.Reset();
            UserCardList.Clear();
            StopWaitting();
            Vector3 pos = ResultGoldLabel.transform.localPosition;
            ResultGoldLabel.transform.localPosition = new Vector3(pos.x, 70, pos.z);
            ResultGoldLabel.gameObject.SetActive(false);
            SetReadyMarkActive(false);
            HandCardsType.Reset();
            FinishChoise = false;
            ReadyState = false;
            foreach (GameObject hole in Holes)
            {
                hole.SetActive(false);
            }
        }



        public void SetBankerMarkActive()
        {
            if (BankerMark == null || Info == null)
                return;
            BankerMark.SetActive(App.GetGameData<SssGameData>().BankerSeat == Info.Seat);
        }
    }
}
