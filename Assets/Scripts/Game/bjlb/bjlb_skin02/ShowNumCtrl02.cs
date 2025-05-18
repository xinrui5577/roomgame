namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class ShowNumCtrl02 : ShowNumCtrl
    {

        public override void ReSet()
        {
            base.ReSet();
            int len = ZLabels.Length;
            for (int i = 0; i < len; i++)
            {
                ZLabels[i].gameObject.SetActive(false);
                WLabels[i].gameObject.SetActive(false);
            }
        }
        
    }
}