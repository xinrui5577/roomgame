namespace Assets.Scripts.Game.sanpian.item
{
    public class LandCard:PoKerCard
    {
        private int _realColour;
        private int _realValue;

        public LandCard()
            : base() 
        {

        }
        public LandCard(int id)
            : base(id) 
        {

        }
        

        public int RealColour
        {
            get { return _realColour; }
            set { _realColour = value; }
        }

        public int RealValue
        {
            get { return _realValue; }
            set { _realValue = value; }
        }
    }
}