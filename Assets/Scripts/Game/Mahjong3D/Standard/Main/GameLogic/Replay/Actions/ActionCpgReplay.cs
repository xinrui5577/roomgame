using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionCpgReplay : AbsCommandAction
    {
        public void ReplayChi(ReplayFrameData data)
        {
            var allCards = data.Cards;
            var removeCards = new List<int>();
            for (int i = 0; i < allCards.Count; i++)
            {
                var card = allCards[i];
                //过滤
                if (card != data.LastFrameData.Cards[0]) removeCards.Add(card);
            }

            allCards.Sort();
            SetCpgLayout(data, removeCards, new CpgModel() { Cards = allCards });
            PlayEffect(data.OpChair, PoolObjectType.chi);
        }

        public void ReplayPeng(ReplayFrameData data)
        {
            var card = data.Cards[0];
            var allCards = new List<int>() { card, card, card };
            var removeCards = new List<int>() { card, card };

            var cpg = new CpgModel()
            {
                Cards = allCards,
                Type = CpgProtocol.Peng,
            };

            SetCpgLayout(data, removeCards, cpg);
            PlayEffect(data.OpChair, PoolObjectType.peng);
        }

        private void SetCpgLayout(ReplayFrameData data, List<int> removeCards, CpgModel cpg)
        {
            var lastFrameData = data.LastFrameData;
            var outCard = lastFrameData.Cards[0];

            var group = Game.MahjongGroups;
            group.MahjongThrow[lastFrameData.OpChair].PopMahjong(outCard);
            Game.TableManager.GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
            // 移除当前操作玩家手牌 
            group.MahjongHandWall[data.OpChair].RemoveMahjong(removeCards);
            // 设置吃碰杠      
            group.MahjongCpgs[data.OpChair].SetCpg(cpg);
        }

        public void ReplayZhuaGang(ReplayFrameData data)
        {
            var card = data.Cards[0];
            var group = Game.MahjongGroups;
            group.MahjongHandWall[data.OpChair].RemoveMahjong(card);
            // 设置吃碰杠      
            group.MahjongCpgs[data.OpChair].SetZhuaGang(card);

            PlayEffect(data.OpChair, PoolObjectType.gang);
        }

        public void ReplayMingGang(ReplayFrameData data)
        {
            var card = data.Cards[0];
            var allCards = new List<int>() { card, card, card, card };
            var removeCards = new List<int>() { card, card, card };
            var cpg = new CpgModel() { Cards = allCards };

            SetCpgLayout(data, removeCards, cpg);
            PlayEffect(data.OpChair, PoolObjectType.gang);
        }

        public void ReplayAnGang(ReplayFrameData data)
        {
            var card = data.Cards[0];
            var allCards = new List<int>() { card, card, card, card };

            var group = Game.MahjongGroups;
            group.MahjongHandWall[data.OpChair].RemoveMahjong(allCards);
            var cpg = new CpgModel()
            {
                Cards = allCards,
                Hide = true
            };
            group.MahjongCpgs[data.OpChair].SetCpg(cpg);

            PlayEffect(data.OpChair, PoolObjectType.gang);
        }

        private void PlayEffect(int chair, PoolObjectType type)
        {
            GameCenter.Controller.PlayOperateEffect(chair, type);
        }
    }
}
