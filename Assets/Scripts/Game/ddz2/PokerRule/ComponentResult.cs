using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerRule
{
    public class ComponentResult{

        public ComponentResult()
        {
            Init();
        }

        private List<List<int>> _bombList;//副条件，后判断
        private List<List<int>> _groupList;//主条件，先判断
        private int _count;

        public List<List<int>> GroupList
        {
            get { return _groupList; }
            set { _groupList = value; }
        }

        public List<List<int>> BombList
        {
            get { return _bombList; }
            set { _bombList = value; }
        }

        public int Count
        {
            get { return _groupList.Count; }
        }

        protected void Init()
        {
            _bombList = new List<List<int>>();
            _groupList = new List<List<int>>();
        }

        /// <summary>
        /// 判定只有相同的牌
        /// </summary>
        /// <returns></returns>
        public bool IsSingle()
        {
            return _bombList.Count > 0;
        }
        /// <summary>
        /// 判定顺子和飞机不带翅膀
        /// </summary>
        /// <returns></returns>
        public bool IsGroup()
        {
            return _groupList.Count > 0;
        }
        /// <summary>
        /// 判定飞机带翅膀
        /// </summary>
        /// <returns></returns>
        public bool IsAll()
        {
            return IsGroup() && IsSingle();
        }

        public void Reset()
        {
            _bombList.Clear();
            _groupList.Clear();
        }
    }
}
