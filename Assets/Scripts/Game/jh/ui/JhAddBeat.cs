using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhAddBeat : MonoBehaviour
    {

        public GameObject ChipPrefab;

        public UIGrid ChipCrid;

        public List<JhChip> ChipList;
        public void SetBeatInfo(int[] antes)
        {
            foreach (JhChip jhChip in ChipList)
            {
                Destroy(jhChip.gameObject);
            }
            ChipList.Clear();
            

            UISprite spr = GetComponent<UISprite>();
            for (int i = 0; i < antes.Length; i++)
            {
                GameObject chip = Instantiate(ChipPrefab);
                chip.SetActive(true);
                chip.transform.parent = ChipCrid.transform;
                chip.transform.localScale = Vector3.one;
                JhChip c = chip.GetComponent<JhChip>();
                c.SetChip(i, antes[i]);
                ChipList.Add(c);
            }

            float x = Math.Abs(ChipCrid.transform.localPosition.x);
            x *= 2;
            x += (ChipList.Count-1)* ChipCrid.cellWidth;

            spr.width = (int)x;

            ChipCrid.Reposition();
        }

        public void SetBetShow(int minBet,long maxBet)
        {
            foreach (JhChip chip in ChipList)
            {
                UIButton btn = chip.GetComponent<UIButton>();
                if (chip.Value > minBet&&chip.Value<maxBet)
                {
                    btn.SetState(UIButtonColor.State.Normal, true);
                    btn.isEnabled = true;
                }
                else
                {
                    btn.SetState(UIButtonColor.State.Disabled, true);
                    btn.isEnabled = false;
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            foreach (JhChip chip in ChipList)
            {
                UIButton btn = chip.GetComponent<UIButton>();
                btn.SetState(UIButtonColor.State.Normal, true);
                btn.isEnabled = true;
            }
        }
    }
}
