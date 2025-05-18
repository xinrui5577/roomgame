using UnityEngine;

namespace Assets.Scripts.Game.pdk.PokerRule
{
    public class Card
    {
        /**
         * 
         */
        private static readonly long serialVersionUID = -806794057929222718L;
        private static readonly Card[] CARDS = new Card[55];
        public static readonly Card MAGIC = new Card(54); //魔法牌
        /**
         * 1: 黑桃
         * 2: 梅花
         * 3: 红桃
         * 4: 方块
         */
        private readonly byte type;
        /**1,2,3,4...**/
        private readonly byte value;
        private readonly string name;

        protected int weight;



        static Card()
        {
            for (int i = 0; i < 54; ++i)
            {
                Card card = new Card(i);
                CARDS[i] = card;
            }

            CARDS[54] = MAGIC; //魔法牌
        }

        public static Card newCard(int number)
        {
            return CARDS[number];
        }

        public static Card newCard(int type, int value)
        {
            if (type == 7 && value == 18)
            {
//魔法牌
                return newCard(54);
            }
            else
            {
                int number = (byte) ((type - 1)*13 + value - 1);
                return newCard(number);
            }
        }

        //
        protected Card(int number)
        {
            if (number == 54)
            {
//魔法牌
                this.type = (byte) 7;
                this.value = (byte) 18;
            }
            else
            {
                byte type = (byte) (number/13);
                this.type = (byte) (type + 1);
                this.value = (byte) (number - type*13 + 1);
            }
            this.name = this.type + "_" + value;
            this.weight = calculateWeight();
        }

        protected Card(int type, int value)
        {
            if (type == 7 && value == 18)
            {
//魔法牌
                this.type = (byte) type;
                this.value = (byte) value;
            }
            else
            {
                this.type = (byte) type;
                this.value = (byte) value;
            }
            this.name = type + "_" + value;
            this.weight = calculateWeight();
        }

        public int getType()
        {
            return type;
        }

        public int getValue()
        {
            return value;
        }

        public int getNumber()
        {
            return (type - 1)*13 + value - 1;
            //return this.number;
        }

        public string getName()
        {
            return this.name;
        }


        public string toString()
        {
            return name;
            //return String.valueOf(this.AiToDesk());
        }

        public int getWeight()
        {
            return weight;
        }

        protected int calculateWeight()
        {
            if (this.type == 5)
            {
                return value + 15; //小王16，大王17
            }
            if (this.type == 7)
            {
                return -1; //魔法牌3,最小值
            }
            if (value <= 2)
                return value + 13;
            return value;
        }

        public int compareTo(Card o)
        {
            return this.getWeight() - o.getWeight() == 0 ? 0 : (this.getWeight() - o.getWeight() > 0) ? -1 : 1;
        }

        //AI中的扑克数据转化为Desk中的扑克数据
        public int AiToDesk()
        {
            int num = this.getNumber();
            int ty = this.getType();
            int va = this.getValue();

            if (ty < 5 && va >= 3)
                return ty*16 + va; //3-K

            if (ty < 5 && va <= 2)
                return ty*16 + (va + 13); //A,2

            if (ty == 5 && va == 1)
                return 5*16 + 1; //小王81，大王97

            if (ty == 5 && va == 2)
                return 6*16 + 1; //小王81，大王97

            if (ty == 7 && va == 18)
                return 113; //魔法牌113

            return num;
        }

        public static Card DeskToAi(int i)
        {
            int type = i/16;
            if (type == 0) type = 1;

            int value = i%16;

            if (type < 5 && value < 14)
                return Card.newCard(type, value); //3-K

            if (type < 5 && value >= 14)
                return Card.newCard(type, value - 13); //A,2

            if (type == 5)
                return Card.newCard(5, 1); //小王5_1，大王5_2

            if (type == 6)
                return Card.newCard(5, 2); //小王5_1，大王5_2

            if (i == 113)
                return Card.newCard(7, 18); //魔法牌5_3

            return Card.newCard(i);
        }

    }
}
