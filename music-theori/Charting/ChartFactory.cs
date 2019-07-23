namespace theori.Charting
{
    public class ChartFactory
    {
        public static readonly ChartFactory Default = new ChartFactory();

        public ChartFactory()
        {
        }

        public virtual Chart CreateNew()
        {
            return new Chart(null);
        }
    }
}
