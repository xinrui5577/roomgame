using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.ChessCommon;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.UI;
using JetBrains.Annotations;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Users
{
    /// <summary>
    /// 炮台
    /// </summary>
    public class PlayerBatterys : UIView, IEnumerable<Player>, IEnumerator<Player>
    {
        /// <summary>
        /// 炮台
        /// </summary>
        [SerializeField, UsedImplicitly] 
        private Battery[] _batterys; 
        /// <summary>
        /// 最小样式
        /// </summary>
        public int MinGunStyle;
        /// <summary>
        /// 最大样式
        /// </summary>
        public int MaxGunStyle;
        /// <summary>
        /// 大炮种类个数
        /// </summary>
        public int GunStyleCount = 3;
        public Player UserSelf;
         
        private readonly List<Player> _playerList = new List<Player>();
        private int[] _gunStyles = {1,2,5,10,50,100};
        private bool _isInit;
        private ISFSObject _data;
         
        /// <summary>
        /// 索引对应的玩家(注:索引超出范围,说明调用的位置是不允许的)
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        public Player this[int seat]
        {
            get { return _playerList[seat]; }
        }

        void Awake()
        {
            InitPlayer();
            _isInit = true;
        }

        public void Start()
        { 
            Fresh();
        }
         
        /// <summary>
        /// 初始化玩家
        /// </summary>
        private void InitPlayer()
        {
            _playerList.Clear();
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                var battery = _batterys[i];
                var player = battery.CreatePlayer(); 
                _playerList.Add(player);
            }
        }

        public void Init(ISFSObject data)
        {
            _data = data;
            Fresh();
        }

        private void Fresh()
        {
            if (_data == null) return;
            if (!_isInit) return;
            InitGunStyls(_data);
            AddPlayers(_data);
            AddNpcs(_data);
        }

        /// <summary>
        /// 初始化枪的类型
        /// </summary>
        /// <param name="gameInfo"></param>
        private void InitGunStyls(ISFSObject gameInfo)
        {
            var maxGunScore = gameInfo.ContainsKey("bmax") ? gameInfo.GetInt("bmax") : 1000;
            var basicGunScore = gameInfo.ContainsKey("ante") ? gameInfo.GetInt("ante") : 10;
            var blts = gameInfo.ContainsKey("blts") ? gameInfo.GetIntArray("blts") : _gunStyles;
            Defines.FireSpeed = gameInfo.ContainsKey("blsp") ? gameInfo.GetFloat("blsp") : 0.32f;
            //YxDebug.Log("枪炮数据:" + maxGunScore + "   " + basicGunScore);
            var count = blts.Length;
            _gunStyles = new int[count];
            var i = 0;
            for (; i < count; i++)
            {
                var score = blts[i]*basicGunScore;
                if (score > maxGunScore) break;
                _gunStyles[i] = score;
            }
            Array.Resize(ref _gunStyles, i);
            if (i < 1) return;
            MinGunStyle = _gunStyles[0];
            MaxGunStyle = _gunStyles[_gunStyles.Length - 1];
        }

        public void AddPlayers(ISFSObject gameInfo)
        {
            if (gameInfo.ContainsKey(RequestKey.KeyUser))
            {
                //设置玩家自己
                var user = gameInfo.GetSFSObject(RequestKey.KeyUser);
                UserSelf = AddPlayer(user);
                UserSelf.ShowCtrBtn();
//                App.GameData.SelfSeat = UserSelf.Idx;
            }
            if (!gameInfo.ContainsKey(RequestKey.KeyUserList)) return;
            var users = gameInfo.GetSFSArray(RequestKey.KeyUserList);
            var count = users.Count;
            for(var i=0;i<Count;i++)
            {
                var user = users.GetSFSObject(i);
                AddPlayer(user);
            }
        }

        public Player AddPlayer(ISFSObject userInfo, bool isRobot = false)
        {
            var seat = userInfo.GetInt(RequestKey.KeySeat);
            seat %= Count;
            var player = _playerList[seat];
            player.Init(userInfo, isRobot);
            player.ChangeNeedScore(_gunStyles[0]);
            return player;
        }

        private string[] _npcs;
        public void AddNpcs(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey("npc"))return;
            _npcs = gameInfo.GetUtfStringArray("npc");
            StartCoroutine(CheckNpcs(gameInfo)); 
        }

        public void AddNps(ISFSObject info)
        {

        }

        private IEnumerator CheckNpcs(ISFSObject gameInfo)
        {
            var npslist = gameInfo.GetUtfStringArray("npc");
            YxDebug.LogArray(npslist);
            var maxNpcIndex = npslist.Length-1;
            var isInit = true;
            var gdata = App.GetGameData<FishGameData>();
            var self = App.GameData.GetPlayerInfo();
            var batterys = GameMain.Singleton.PlayersBatterys;
            var coin = gdata.NeedUpperScore ? batterys.MaxGunStyle * 30 : self.CoinA;
            while (true)
            {
                //查找所有空缺位置
                var vacants = new List<Player>();
                foreach (var player in _playerList)
                {
                    if (!player.IsHide())continue;
                    if (Random.Range(0, 100) < 40) continue;
                    vacants.Add(player);
                }
                YxDebug.Log("---准备加入rb【{0}】-----","PlayerBatterys",null,vacants.Count);
                
                foreach (var player in vacants)
                { 
                    var index = Random.Range(0, maxNpcIndex);//随机获取机器人
                    var npcName = _npcs[index];
                    if(string.IsNullOrEmpty(npcName))continue;//忽略
                    var obj = new SFSObject();
                    var x1 = Random.Range(coin*0.8f,coin*1.5f);//随机
                    //max*Random.Range(20, 100) + Random.Range(35342, 105480)
                    var ncoin = isInit ? x1 : 0;
                    obj.PutInt(RequestKey.KeyCoin, (int)ncoin);
                    obj.PutInt(RequestKey.KeySeat, player.Idx);
                    obj.PutInt("nid",index);
                    obj.PutUtfString(RequestKey.KeyName, npcName);
                    _npcs[index] = null;
                    AddPlayer(obj, true);
                }
                isInit = false;
                YxDebug.LogArray(_npcs);
                yield return new WaitForSeconds(Random.Range(300f, 600f));
            } 
        }
         
        /// <summary>
        /// 从分数转换到枪的等级类型
        /// </summary>
        /// <returns></returns>
        public int GunNeedScoreToLevelType(int curScore)
        {
            var count = _gunStyles.Length;
            var startIdx = count > GunStyleCount ? count - GunStyleCount : 0;
            var tempIdx = startIdx;
            for (; tempIdx < count; tempIdx++)
            {
                if (curScore <= _gunStyles[tempIdx]) break;
            }
            tempIdx = tempIdx < count ? tempIdx : count - 1;
            return tempIdx - startIdx;
        }

        public int GetGunStyle(ref int curIndex, int fluctuate = 0)
        {
            if (fluctuate == 0) return _gunStyles[curIndex];
            var len = _gunStyles.Length;
            curIndex = (len + curIndex + fluctuate) % len;
            return _gunStyles[curIndex];
        }
          
        /// <summary>
        /// 玩家个数
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return _batterys.Length; }
        }

        [ContextMenu("刷新位置")]
        public void UpdatePosition()
        {
            foreach (var battery in _batterys)
            {
                var pos = GetNewPostion(battery.transform.localPosition, Bound, battery.Side);
                battery.transform.localPosition = pos;
            }
        }

        public void UpdatePosition(Rect worldRect)
        { 
            SetBound(worldRect); 
            UpdatePosition();
        }

        private static Vector3 GetNewPostion(Vector3 pos, Rect rect,UIAnchor.Side side)
        {
            switch (side)
            {
                case UIAnchor.Side.BottomLeft://左下
                    pos.x = rect.x;
                    pos.y = rect.y;
                    break;
                case UIAnchor.Side.Left://左
                    pos.x = rect.x;
                    break;
                case UIAnchor.Side.TopLeft://左上 
                    pos.x = rect.x;
                    pos.y = rect.y+rect.height;
                    break;
                case UIAnchor.Side.Top://上
                    pos.y = rect.y + rect.height;
                    break;
                case UIAnchor.Side.TopRight://右上
                    pos.x = rect.x + rect.width;
                    pos.y = rect.y + rect.height;
                    break;
                case UIAnchor.Side.Right://右
                    pos.x = rect.x + rect.width;
                    break;
                case UIAnchor.Side.BottomRight://右下
                    pos.x = rect.x + rect.width;
                    pos.y = rect.y;
                    break;
                case UIAnchor.Side.Bottom://下
                    pos.y = rect.y;
                    break; 
            }
            return pos; 
        }

        #region 迭代器
        public IEnumerator<Player> GetEnumerator()
        {
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                yield return _playerList[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } 
         
        private int _position = -1;
        public bool MoveNext()
        {
            _position++;
            return (_position < Count);
        }

        public void Reset()
        {
            _position = -1;
        }

        public Player Current
        {
            get { return _playerList[_position]; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
        }
        #endregion 

        /// <summary>
        /// 玩家退出
        /// </summary>
        /// <param name="seat"></param>
        public void RemovePlayer(int seat)
        {  
            seat %= Count;
            var player = _playerList[seat];
            player.Display(false);
        }

        public void AddNpc(string npcName, int nid)
        {
            if (nid < 0) return;
            if (_npcs == null) return;
            if (nid >= _npcs.Length) return;
            _npcs[nid] = npcName;
            YxDebug.LogArray(_npcs);
        }
    } 
}
