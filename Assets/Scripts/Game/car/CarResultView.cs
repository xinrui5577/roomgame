using UnityEngine;

namespace Assets.Scripts.Game.car
{
    public class CarResultView : MonoBehaviour
    {
        public EventObject EventObj;
        public CarResultItemView WinItemView;
        public CarResultItemView LoseItemView;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Result":
                    OnResult((ResultData)data.Data);
                    break;
            }
        }


        private void OnResult(ResultData resultData) 
        {
            if (resultData.Win >= 0)
            {
                WinItemView.Show();
                WinItemView.UpdateView(resultData);
            }
            else
            {
                LoseItemView.Show();
                LoseItemView.UpdateView(resultData);
            }
        }
    }
}
