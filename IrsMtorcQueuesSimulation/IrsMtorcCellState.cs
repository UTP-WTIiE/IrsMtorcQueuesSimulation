using System;
using System.Collections.Generic;
using System.Text;

namespace mTORC.Models
{
    public class IrsMtorcCellState
    {
        public double[] data;

        public double this[int a]
        {
            get => data[a];
            set => data[a] = value;
        }

        public IrsMtorcCellState()
        {
            data = new double[63];

            double a = 0.2;
            var r = new RandomGenerator();
            double RL(int whatever) => r.NextGaussian();
            double floor(double x) => Math.Floor(x);

            data[1 - 1] = floor(1200 * (1 + a / 2 * RL(1))) * 0;
            data[2 - 1] = floor(22000 * (1 + a / 2 * RL(2)));
            data[8 - 1] = floor(60 * (1 + 1 / 3 * RL(8))) * 1;
            data[10 - 1] = floor(200 * (1 + 7 / 40 * RL(10)));
            data[17 - 1] = floor(300 * (1 + 1 / 20 * RL(17)));
            data[20 - 1] = floor(50 * (1 + 1 / 20 * RL(20)));
            data[24 - 1] = floor(100 * (1 + 3 / 200 * RL(24))) * 1;
            data[30 - 1] = floor(10 * (1 + 1 / 8 * RL(30)));
            data[37 - 1] = floor(108 * (1 + a / 2 * RL(37))) * 1;
            data[38 - 1] = floor(12 * (1 + a / 2 * RL(38)));
            data[41 - 1] = 230;
            data[42 - 1] = 90000;
            data[44 - 1] = 52800;
            data[46 - 1] = 370000;
            data[47 - 1] = 20000;
            data[48 - 1] = 10199.42; 
            data[49 - 1] = 0.9 * 4400.87;     
            data[51 - 1] = 60000;
            data[53 - 1] = 6709;
            data[55 - 1] = 4880.49;
            data[57 - 1] = 634.15; 
            data[59 - 1] = 1235.91; 
            data[61 - 1] = 0.30241;
        }

        public void TurnOnInsulin()
        {
            double a = 0.2;            
            double c = 1.72 * 6.022e4; 
            var r = new RandomGenerator();
            double RL(int whatever) => r.NextGaussian();

            data[1 - 1] = Math.Floor(0.6 * c * (1 + a / 2 * RL(1))) * 1;
        }

        public IrsMtorcCellState Copy()
        {
            var a = new IrsMtorcCellState();
            for (int i = 0; i < data.Length; i++)
                a[i] = this[i];

            return a;
        }

        public double[] State => data;

        public double I { get => data[1 - 1]; set => data[1 - 1] = value; }
        public double IR { get => data[2 - 1]; set => data[2 - 1] = value; }
        public double IR_I { get => data[3 - 1]; set => data[3 - 1] = value; }
        public double pIR_I { get => data[4 - 1]; set => data[4 - 1] = value; }
        public double abs_pIR_I { get => data[5 - 1]; set => data[5 - 1] = value; }
        public double pIR_II { get => data[6 - 1]; set => data[6 - 1] = value; }
        public double abs_pIR_II { get => data[7 - 1]; set => data[7 - 1] = value; }
        public double IRS1_3 { get => data[8 - 1]; set => data[8 - 1] = value; }
        public double pIRS1_3 { get => data[9 - 1]; set => data[9 - 1] = value; }
        public double PI3K { get => data[10 - 1]; set => data[10 - 1] = value; }
        public double pIRS1_3_PI3K { get => data[11 - 1]; set => data[11 - 1] = value; }
        public double pIRS1_3_PI3K_activated { get => data[12 - 1]; set => data[12 - 1] = value; }
        public double PI3K_activated { get => data[13 - 1]; set => data[13 - 1] = value; }
        public double PI3K_activated_PI { get => data[14 - 1]; set => data[14 - 1] = value; }
        public double PI3K_activated_PIP3 { get => data[15 - 1]; set => data[15 - 1] = value; }
        public double PIP3 { get => data[16 - 1]; set => data[16 - 1] = value; }
        public double PI { get => data[17 - 1]; set => data[17 - 1] = value; }
        public double PTEN_PIP3 { get => data[18 - 1]; set => data[18 - 1] = value; }
        public double PTEN_PI { get => data[19 - 1]; set => data[19 - 1] = value; }
        public double PTEN { get => data[20 - 1]; set => data[20 - 1] = value; }
        public double pPTEN_PTEN { get => data[21 - 1]; set => data[21 - 1] = value; }
        public double PTEN_PTEN { get => data[22 - 1]; set => data[22 - 1] = value; }
        public double pPTEN { get => data[23 - 1]; set => data[23 - 1] = value; }
        public double Akt { get => data[24 - 1]; set => data[24 - 1] = value; }
        public double Akt_PIP3 { get => data[25 - 1]; set => data[25 - 1] = value; }
        public double pAkt_PIP3 { get => data[26 - 1]; set => data[26 - 1] = value; }
        public double PP2A_Akt_PIP3 { get => data[27 - 1]; set => data[27 - 1] = value; }
        public double PP2A_pAkt_PIP3 { get => data[28 - 1]; set => data[28 - 1] = value; }
        public double PP2A_ppAkt_PIP3 { get => data[29 - 1]; set => data[29 - 1] = value; }
        public double PP2A { get => data[30 - 1]; set => data[30 - 1] = value; }
        public double PP2A_Akt { get => data[31 - 1]; set => data[31 - 1] = value; }
        public double PP2A_pAkt { get => data[32 - 1]; set => data[32 - 1] = value; }
        public double PP2A_ppAkt { get => data[33 - 1]; set => data[33 - 1] = value; }
        public double ppAkt { get => data[34 - 1]; set => data[34 - 1] = value; }
        public double ppAkt_PIP3 { get => data[35 - 1]; set => data[35 - 1] = value; }
        public double PDK { get => data[36 - 1]; set => data[36 - 1] = value; }
        public double As160 { get => data[37 - 1]; set => data[37 - 1] = value; }
        public double pAs160 { get => data[38 - 1]; set => data[38 - 1] = value; }
        public double PP2A_pAs160 { get => data[39 - 1]; set => data[39 - 1] = value; }
        public double PP2A_As160 { get => data[40 - 1]; set => data[40 - 1] = value; }
        public double RabGTP { get => data[41 - 1]; set => data[41 - 1] = value; }
        public double RabGDP { get => data[42 - 1]; set => data[42 - 1] = value; }
        public double RabGTP_As160 { get => data[43 - 1]; set => data[43 - 1] = value; }
        public double GEF { get => data[44 - 1]; set => data[44 - 1] = value; }
        public double RabGDP_GEF { get => data[45 - 1]; set => data[45 - 1] = value; }
        public double Glut4 { get => data[46 - 1]; set => data[46 - 1] = value; }
        public double Glut4InVesicles { get => data[47 - 1]; set => data[47 - 1] = value; }
    }
}
