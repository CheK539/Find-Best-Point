using System.Drawing;

namespace Exercise_1
{
    public class EnergyPoint
    {
        public Point Point { get; }
        public int Energy { get; set; }

        public EnergyPoint(Point point)
        {
            Point = point;
        }
        
        public EnergyPoint(Point point, int energy)
        {
            Point = point;
            Energy = energy;
        }
    }
}