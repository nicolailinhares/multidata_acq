using System;

namespace MultiData_Acq.Util
{
    public class BoardConfiguration
    {
        private int rate;
        public int Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        private int qChanns;
        public int QChanns
        {
            get { return qChanns; }
            set { qChanns = value; }
        }
        private int lowChannel;
        public int LowChannel
        {
            get { return lowChannel; }
            set { lowChannel = value; }
        }
        private int pointsRead;
        public int PointsRead
        {
            get { return pointsRead; }
            set { pointsRead = value; }
        }
        public BoardConfiguration(int lC, int qC, int r, int pR)
        {
            Rate = r;
            lowChannel = lC;
            PointsRead = pR;
            QChanns = qC;
        }
    }
}
