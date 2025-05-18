using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class TurnResult : MonoBehaviour
    {

        public TurnResultItem[] ResultItems;

        public TurnResultTotal ResultTotal;

        private readonly List<DunScore> _scoreList = new List<DunScore>();


        public void InitTurnResultInfo(ISFSObject data)
        {
            ISFSArray scroeArray = data.GetSFSArray("dunscore");

            DunScore dunScore = new DunScore
            {
                NormalScore = new List<int>(),
                AddScore = new List<int>()
            };
            foreach (ISFSObject score in scroeArray)
            {
                dunScore.NormalScore.Add(score.GetInt("normal"));
                dunScore.AddScore.Add(score.GetInt("add"));
                _scoreList.Add(dunScore);
            }
        }


        public void ShowResultItem(int line, int normal, int special)
        {
            if (line >= ResultItems.Length) return;

            ResultItems[line].SetValue(normal, special);
            ResultItems[line].MoveItem();
            ResultTotal.SetValue(normal + special);
            ResultTotal.MoveItem();
        }

        public void ShowAllResultItem()
        {
            if (_scoreList.Count < 3)
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("回合结算数据个数不对,请查验");
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                int normal = _scoreList[0].NormalScore[i];
                int special = _scoreList[0].AddScore[i];
                ResultItems[i].SetValue(normal, special);
                ResultItems[i].MoveItem();
                ResultTotal.SetValue(normal + special);
                ResultTotal.MoveItem();
            }
        }

        public void Reset()
        {
            foreach (TurnResultItem item in ResultItems)
            {
                item.Reset();
            }
            ResultTotal.Reset();
        }
    }

    public struct DunScore
    {
        public int Seat;
        public List<int> NormalScore;
        public List<int> AddScore;
    }
}