using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.pdk.PokerRule
{

    /** 牌型和主牌大小的组合类 */

    public class CardTypeResult
    {

        private CardType type;

        private int value; //最小或者最大，depends on 牌型

        private CardCount ci; //当前只有飞机牌型此值有效

        public CardTypeResult(CardType type, int value, CardCount ci = null)
        {
            this.type = type;
            this.value = value;
            this.ci = ci;
        }

        public CardType getType()
        {
            return type;
        }

        public void setType(CardType type)
        {
            this.type = type;
        }

        public int getValue()
        {
            return value;
        }

        public void setValue(int value)
        {
            this.value = value;
        }

        public String toString()
        {
            return type.ToString() + ", value = " + value;
        }

        public CardCount getCi()
        {
            return ci;
        }

        public void setCi(CardCount ci)
        {
            this.ci = ci;
        }

    }

    /** a[0],a[1],a[2],a[3] 分别表示 单张、对子、三张、四张 的数量 */
	public sealed  class CardCount {
	
		public  List<int>[] a = new List<int>[4];
		public CardCount()
		{
			for (int i =0; i < 4;++i)
                a[i] = new List<int>();
		}
	}
}
