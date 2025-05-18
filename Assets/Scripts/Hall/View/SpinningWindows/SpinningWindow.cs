using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Hall.View.SpinningWindows
{
    /// <summary>
    /// 转盘窗口
    /// </summary>
    public class SpinningWindow : YxNguiWindow
    {
        [Tooltip("初速度")]
        public float InitialVelocity = 0;
        /// <summary>
        /// 角速度
        /// </summary>
        [Tooltip("角加速度")]
        public float AcceleratedAngularSpeed = -80;
        [Tooltip("最大角加速度")]
        public float MaxAngularSpeed = -300;
        [Tooltip("等待时旋转的最少圈数")]
        public int WaitForCylinderCount = 2;
        [Tooltip("需要旋转的对象")]
        public Transform Rolling;
        [Tooltip("高亮标记")]
        public GameObject HeightMark;
        [Tooltip("剩余次数Label")]
        public UILabel ResidueDegreeLabel;
        [Tooltip("剩余次数的格式")]
        public string ResidueDegreeFormat="剩余次数：{0}";
        [Tooltip("已抽到的奖品列表")]
        public RewardListView RewardListsView;
        [Tooltip("中奖名单")]
        public RewardListView AllListsView;
        [Tooltip("转盘上的奖励SpinningItemView数组")]
        public SpinningItemView[] Items;
        [Tooltip("消耗父级")]
        public GameObject CostContainer;
        [Tooltip("消耗类型")]
        public UISprite CostTypeSprite;
        [Tooltip("消耗文本提示")]
        public UILabel CostLabel;
        [Tooltip("消耗文本提示adapter")]
        public NguiLabelAdapter CostLabelAdapter;
        [Tooltip("当前免费次数没有之后是否可以花费资源进行抽奖")]
        public bool RotateBySome = false;
        [Tooltip("抽奖消耗文本格式")]
        public string CostLabelFormat = "消耗     x{0}可以抽奖一次";
        [Tooltip("消耗图片类型")]
        public string CostSpriteFormat = "icon_{0}";
        [Tooltip("奖励请求")]
        public string AwardAction = "awardList_yr";
        [Tooltip("获得的奖励请求")]
        public string GetAwardAction = "getAward_yr";
        [Tooltip("消耗类型的key")]
        public string KeyConsumeType = "consumeType";
        [Tooltip("消耗数量的key")]
        public string KeyConsumeNum = "consumeNum";
        private SpinningState _state = SpinningState.Normal;
        private int _rewardIndex = -1;
        private const string InitAnglesName = "SpinningEulerAngles";
        private float _itemAngle = 360;
        private int _residueDegree;
        private string _msgShow;
        protected override void OnStart()
        {
            if (HeightMark != null) HeightMark.SetActive(false);
            var angles = Rolling.localEulerAngles;
            angles.z = PlayerPrefs.GetFloat(InitAnglesName, Random.Range(0, 360));
            Rolling.localEulerAngles = angles;
            if (Items.Length > 0) _itemAngle = 360f / Items.Length;
            Facade.Instance<TwManager>().SendAction(AwardAction, new Dictionary<string, object>(), UpdateView);
        }

        private readonly Dictionary<int, int> _spinningDatas = new Dictionary<int, int>();
        protected override void OnFreshView()
        {
            if (Data == null) return;
            var dict = Data as Dictionary<string, object>;
            if (dict == null) return;
            DealCost(dict);
            if (dict.ContainsKey("list"))
            {
                var objList = dict["list"];
                if (objList == null) return;
                var list = objList as List<object>;
                if (list == null) return;
                _spinningDatas.Clear();
                var minCount = Mathf.Min(list.Count, Items.Length);
                for (var i = 0; i < minCount; i++)
                {
                    var obj = list[i];
                    if (obj == null) continue;
                    var dictData = obj as Dictionary<string, object>;
                    var item = Items[i];
                    var sdata = new SpinningItemData(dictData, i);
                    _spinningDatas[sdata.Id] = i;
                    item.UpdateView(sdata);
                }
            }
            
        }

        private void DealCost(Dictionary<string,object> dict)
        {
            int cType = 0;
            int cNum = 0;
            if (dict.ContainsKey(KeyConsumeType))
            {
                int.TryParse(dict[KeyConsumeType].ToString(), out cType);
            }
            if (dict.ContainsKey(KeyConsumeNum))
            {
                int.TryParse(dict[KeyConsumeNum].ToString(), out cNum);
            }
            if (dict.ContainsKey("count"))
            {
                int rcount;
                int.TryParse(dict["count"].ToString(), out rcount);
                SetResidueDegree(rcount, cType, cNum);
            }
        }

        private void SetResidueDegree(int count,int costType=0,int costNum=0)
        {
            _residueDegree = count;
            if (ResidueDegreeLabel == null) return;
            if (count==0&& RotateBySome)
            {
                CostContainer.SetActive(true);
                ResidueDegreeLabel.gameObject.SetActive(false);
                if (CostContainer)
                {
                    CostTypeSprite.TrySetComponentValue(string.Format(CostLabelFormat, costNum));
                    CostLabelAdapter.TrySetComponentValue(costNum, costType.ToString(), CostLabelFormat, YxBaseLabelAdapter.YxELabelType.NumberWithUnit);
                    CostTypeSprite.TrySetComponentValue(string.Format(CostSpriteFormat, costType));
                        
                }
                return;
            }
            if (CostContainer)
            {
                CostContainer.SetActive(false);
            }
            ResidueDegreeLabel.text = string.Format(ResidueDegreeFormat, count);
        }

        public void OnStartClick()
        {
            if (Data == null) return;
            if(RotateBySome)
            {
                
            }
            else
            {
                if (_residueDegree < 1) return;
            }
            if (Rolling == null) return;
            if (_state != SpinningState.Normal) return;
            _state = SpinningState.Launch;
        }

        private void FixedUpdate()
        {
            //启动状态
            //加速状态
            //等待状态
            //减速状态
            //停止状态
            switch (_state)
            {
                case SpinningState.Wait:
                    WaitFor();
                    break;
                case SpinningState.Launch:
                    Launch();
                    break;
                case SpinningState.SpeedUp:
                    SpeedUp();
                    break;
                case SpinningState.SpeedDwon:
                    SpeedDown();
                    break;
                case SpinningState.Stop:
                    Stop();
                    break;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void ReSet()
        {
            _rewardIndex = -1;
            _readyStopAngle = -1;
            _waitAllAngles = 0;
            _curVel = InitialVelocity;
            if (HeightMark != null) HeightMark.SetActive(false);
        }

        protected virtual void Launch()
        {
            ReSet();
            _state = SpinningState.SpeedUp;
            Facade.Instance<TwManager>().SendAction(GetAwardAction, new Dictionary<string, object>(),   SetReward,true,
                                                   msg =>
                                                       {
                                                           var msgDict = msg as Dictionary<string, object>;
                                                           if (msgDict != null && msgDict.ContainsKey("errorMessage"))
                                                           {
                                                               YxMessageBox.Show(msgDict["errorMessage"].ToString());
                                                           }
                                                           _state = SpinningState.Normal;
                                                       });
//            SetReward(Random.Range(0,11));//测试用
        }

        /// <summary>
        /// 当前速度
        /// </summary>
        private float _curVel;
        //S=初速度V*时间t+1/2at^2
        protected virtual void SpeedUp()
        {
            var newVel = ChangeAngular(AcceleratedAngularSpeed, _curVel, Rolling);
            if (Mathf.Abs(newVel) > Mathf.Abs(MaxAngularSpeed))
            {
                _state = SpinningState.Wait;
                _waitAllAngles = 0;
                return;
            }
            _curVel = newVel;
        }
        //S=V*T-1/2at^2
        protected virtual void SpeedDown()
        {
            var newVel = ChangeAngular(-AcceleratedAngularSpeed, _curVel, Rolling);
            if (newVel * _curVel <= 0)
            {
                _state = SpinningState.Stop;
                _waitAllAngles = 0;
                _curVel = 0;
                return;
            }
            _curVel = newVel;
        }

        private float _waitAllAngles;
        protected virtual void WaitFor()
        {
            var ds = GetDistance(_curVel, Rolling);
            _waitAllAngles += ds;

            var waitCount = Mathf.Abs((int)(_waitAllAngles / 360));
            if (waitCount < WaitForCylinderCount) return;//已经达到指定圈数

            if (_rewardIndex < 0) return;//已经有奖励
            ReckonReadyStopAngle();
            var curAngles = Rolling.localEulerAngles;//当前的角度
            if (Math.Abs(_readyStopAngle - curAngles.z) > Mathf.Abs(ds)) return;
            curAngles.z = _readyStopAngle;
            Rolling.localEulerAngles = curAngles;
            _state = SpinningState.SpeedDwon;
        }

        
        protected void SetReward(object obj)
        {
            //{"id":"7","name":"\u8868\u60c5\u798f\u888b","img":null,"grade":"2","odds":"50","is_effect":"1"}
//            _rewardIndex = index; 
//            {"getAward_yr":{"count":99997,"info":{"id":"11","name":"\u8c22\u8c22\u53c2\u4e0e","img":null,"num":"1","grade":"1","odds":"200","is_effect":"1","tot_q":null,"remain_q":null}},"success":true
            if (obj == null) return;
            var dict = obj as Dictionary<string, object>;
            if (dict == null) return;
            DealCost(dict);
            if (!dict.ContainsKey("info")) return;
            var dictObj = dict["info"];
            if (dictObj == null) return;
            var spDict = dictObj as Dictionary<string, object>;
            if (spDict == null) return;
            var sdata = new SpinningItemData(spDict);
            _msgShow = sdata.Msg;
            if (_spinningDatas.ContainsKey(sdata.Id))
            {
                _rewardIndex = _spinningDatas[sdata.Id];
            }
            else
            {
                YxMessageBox.Show("数据异常！！！", null, (box, btnName) =>
                    {
                        _state = SpinningState.Normal;
                        Close();
                    });
            }
        }

        /// <summary>
        /// 可以减速的角度
        /// </summary>
        private float _readyStopAngle = -1;
        private void ReckonReadyStopAngle()
        {
            if (_readyStopAngle>0) return;
            var l = Mathf.Abs((_curVel * _curVel / (-AcceleratedAngularSpeed * 2)));
            var off = _itemAngle / 10;
            var range = Random.Range(off, _itemAngle - off);
            var target = _rewardIndex * _itemAngle + range; //目标角度
            //            _readyStopAngle = (l - target) % 360; // 准备停止得角度
            //            var temp = AcceleratedAngularSpeed < 0 ? l - target : 360 - l - target;

            var langle = AcceleratedAngularSpeed < 0 ? l : 360 - l;//减速总距离转换为角度
            var temp = langle < target ? langle - target + 360 : langle - target;//求出指定奖励对应的角度
            temp = temp < 0 ? 360 + temp : temp;
            _readyStopAngle = temp % 360; // 准备停止得角度
            Debug.Log(string.Format("_readyStopAngle:   {0}", _readyStopAngle));
        }

        protected virtual void Stop()
        {
            _state = SpinningState.Normal;
            if (RewardListsView != null&& RewardListsView.gameObject.activeInHierarchy)
            {
                    RewardListsView.UpdateData();
            }
            if (AllListsView != null&& AllListsView.gameObject.activeInHierarchy)
            {
                    AllListsView.UpdateData();
            }
            var rz = Rolling.localEulerAngles.z;
            Debug.Log("停止：" + rz);
            if (!_msgShow.Equals(""))
            {
                YxMessageBox.Show(_msgShow);
            }
            if (HeightMark == null) return;
            if (_rewardIndex == -1) return;
            var item = Items[_rewardIndex];
            HeightMark.SetActive(true);
            var marktAngle = HeightMark.transform.eulerAngles;
            marktAngle.z = item.transform.eulerAngles.z;
            HeightMark.transform.localEulerAngles = marktAngle;
        }

        public override void Close()
        {
            if (_state != SpinningState.Normal)
            {
                YxMessageBox.Show("亲，转盘还没有结果，请稍后！\n谁知道会不会中大奖呢？");
                return;
            }
            PlayerPrefs.SetFloat(InitAnglesName, Rolling.localEulerAngles.z);
            base.Close();
        }

        /// <summary>
        /// 更改角度
        /// </summary>
        /// <param name="accelerated">加速度（角度）</param>
        /// <param name="v0">初速度（角度）</param>
        /// <param name="ts">旋转对象</param>
        /// <returns></returns>
        private static float ChangeAngular(float accelerated, float v0, Transform ts)
        {
            var t = Time.deltaTime;
            var v1 = v0 + t * accelerated;
            var z = (v0 + v1) * t / 2;
            ts.Rotate(0, 0, z, Space.Self);
            return v1;
        }

        private static float GetDistance(float v, Transform ts)
        {
            var t = Time.deltaTime;
            var z = v * t;
            ts.Rotate(0, 0, z, Space.Self);
            return z;
        }

        /// <summary>
        /// 转盘状态
        /// </summary>
        private enum SpinningState
        {
            Normal,
            /// <summary>
            /// 等待状态
            /// </summary>
            Wait,
            /// <summary>
            /// 启动状态
            /// </summary>
            Launch,
            /// <summary>
            /// 加速状态
            /// </summary>
            SpeedUp,
            /// <summary>
            /// 减速状态
            /// </summary>
            SpeedDwon,
            /// <summary>
            /// 停止状态
            /// </summary>
            Stop
        }
    }
}
