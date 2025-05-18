using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class ShowBetResult : MonoBehaviour
    {

        public void ShowResult(int[] points)
        {
            List<string> NameList = new List<string>();
            int Sum = 0;
            bool baozi = true;
            for (int i = 0; i < points.Length; i++)
            {
                Sum += points[i];
                NameList.Add(points[i] + "");
                for (int j = 0; j < points.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    if (points[i] <= points[j])
                    {
                        NameList.Add(points[i] + "-" + points[j]);
                    }
                }
                if (i != points.Length - 1 && points[i] != points[i + 1])
                {
                    baozi = false;
                }
            }
            NameList.Add("point-" + Sum);
            if (Sum % 2 == 0)
            {
                NameList.Add(BetArea.Double);
            }
            else
            {
                NameList.Add(BetArea.Single);
            }
            if (Sum >= 4 && Sum <= 10)
            {
                NameList.Add(BetArea.Small);
            }
            else if (Sum >= 11 && Sum <= 17)
            {
                NameList.Add(BetArea.Big);
            }
            if (baozi)
            {
                NameList.Add(BetArea.BaoZi);
                NameList.Add(points[0] + "-" + points[0] + "-" + points[0]);
            }

            for (int i = 0; i < NameList.Count; i++)
            {
                Transform tran = transform.FindChild("Parents/"+NameList[i]);
                if (tran)
                {
                    tran.GetComponent<BetAreaItem>().PlayTurn();
                }

            }
        }
    }

    class BetArea
    {
        public const string Single = "single";
        public const string Double = "double";
        public const string Big = "big";
        public const string Small = "small";
        public const string BaoZi = "baozi";
    }
}