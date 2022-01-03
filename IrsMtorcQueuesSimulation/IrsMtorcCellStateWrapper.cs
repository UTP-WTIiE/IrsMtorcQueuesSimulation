using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace mTORC.Models
{
    public class IrsMtorcCellStateWrapper
    {
        public double noise_amplitude;

        public IrsMtorcCellState IrsMtorcCellState { get; set; }
        public double GapdhActiveCoefficient { get; set; } = 1;

        public IrsMtorcCellStateWrapper(double noise_amplitude)
        {
            this.noise_amplitude = noise_amplitude;
            IrsMtorcCellState = new IrsMtorcCellState();

            double a = 0.2;
            var r = new RandomGenerator();
            double RL(int whatever) => r.NextGaussian() * noise_amplitude / 3;
            double floor(double x) => Math.Floor(x);

            IrsMtorcCellState[2 - 1] = floor(9 * (2 + a / 2 * RL(2)));
            IrsMtorcCellState[8 - 1] = floor(60 * (1 + 1 / 3 * RL(8))) * 1;
            IrsMtorcCellState[10 - 1] = floor(200 * (1 + 7 / 40 * RL(10)));
            IrsMtorcCellState[17 - 1] = floor(300 * (1 + 1 / 20 * RL(17)));
            IrsMtorcCellState[20 - 1] = floor(30 * (1 + 1 / 20 * RL(20)));
            IrsMtorcCellState[24 - 1] = floor(100 * (1 + 3 / 1200 * RL(24)) * 1);
            IrsMtorcCellState[30 - 1] = floor(10 * (1 + 1 / 8 * RL(30)));
            IrsMtorcCellState[37 - 1] = floor(108 * (1 + a / 2 * RL(37))) * 1;
            IrsMtorcCellState[38 - 1] = floor(12 * (1 + a / 2 * RL(38)));
            IrsMtorcCellState[41 - 1] = 230;
            IrsMtorcCellState[42 - 1] = 90000;
            IrsMtorcCellState[44 - 1] = 52800;
            IrsMtorcCellState[46 - 1] = 370000;
            IrsMtorcCellState[47 - 1] = 20000;
            IrsMtorcCellState[48 - 1] = 10199.42; 
            IrsMtorcCellState[49 - 1] = 102;      
            IrsMtorcCellState[51 - 1] = 12961;   
            IrsMtorcCellState[53 - 1] = 6709;    
            IrsMtorcCellState[55 - 1] = 4880.49;  
            IrsMtorcCellState[57 - 1] = 634.15;   
            IrsMtorcCellState[59 - 1] = 1235.91; 
            IrsMtorcCellState[61 - 1] = 0.30241; 

        }

        public IrsMtorcCellStateWrapper(double[] array, double noise_amplitude) : this(noise_amplitude)
        {
            var irsMtorc = array.Take(IrsMtorcCellState.data.Length).ToArray();
            this.IrsMtorcCellState = new IrsMtorcCellState();
            this.IrsMtorcCellState.data = irsMtorc;
        }
    
        public IrsMtorcCellStateWrapper Copy()
        {
            return new IrsMtorcCellStateWrapper(this.noise_amplitude)
            {
                IrsMtorcCellState = IrsMtorcCellState.Copy(),
                GapdhActiveCoefficient = GapdhActiveCoefficient
            };
        }

        public void TurnOnInsulin(bool randomize_insulin_level)
        {
            double a = 0.2;             
            double c = 1.72 * 6.022e4;  
            var r = new RandomGenerator();
            double RL(int whatever) => r.NextGaussian();

            var rl = randomize_insulin_level ? RL(1) : 1.0;

            IrsMtorcCellState[1 - 1] = Math.Floor(0.6 * c * (1 + a / 2 * rl)) * 1;
        }
    
        public double[] GetState()
        {
            return IrsMtorcCellState.State;
        }


        public string[] SubstrateNames => new string[] {
            "I[1] I",
            "I[2] IR",
            "I[3] IR_I",
            "I[4] pIR_I",
            "I[5] abs_pIR_I",
            "I[6] pIR_II",
            "I[7] abs_pIR_II",
            "I[8] IRS1_3",
            "I[9] pIRS1_3",
            "I[10] PI3K",
            "I[11] pIRS1_3_PI3K",
            "I[12] pIRS1_3_PI3K_activated",
            "I[13] PI3K_activated",
            "I[14] PI3K_activated_PI",
            "I[15] PI3K_activated_PIP3",
            "I[16] PIP3",
            "I[17] PI",
            "I[18] PTEN_PIP3",
            "I[19] PTEN_PI",
            "I[20] PTEN",
            "I[21] pPTEN_PTEN",
            "I[22] PTEN_PTEN",
            "I[23] pPTEN",
            "I[24] Akt",
            "I[25] Akt_PIP3",
            "I[26] pAkt_PIP3",
            "I[27] PP2A_Akt_PIP3",
            "I[28] PP2A_pAkt_PIP3",
            "I[29] PP2A_ppAkt_PIP3",
            "I[30] PP2A",
            "I[31] PP2A_Akt",
            "I[32] PP2A_pAkt",
            "I[33] PP2A_ppAkt",
            "I[34] ppAkt",
            "I[35] ppAkt_PIP3",
            "I[36] PDK",
            "I[37] As160",
            "I[38] pAs160",
            "I[39] PP2A_pAs160",
            "I[40] PP2A_As160",
            "I[41] RabGTP",
            "I[42] RabGDP",
            "I[43] RabGTP_As160",
            "I[44] GEF",
            "I[45] RabGDP_GEF",
            "I[46] Glut4",
            "I[47] Glut4InVesicles",
            "I[48] Amino Acids",
            "I[49] mTORC1",
            "I[50] phosphorylated mTORC1",
            "I[51] S6K",
            "I[52] phosphorylated S6K",
            "I[53] AMPK",
            "I[54] phosphorylated AMPK",
            "I[55] TSC1-TSC2 complex",
            "I[56] phosphorylated TSC1-TSC complex",
            "I[57] mTORC2",
            "I[58] phosphorylated mTORC2",
            "I[59] PDK2",
            "I[60] phosphorylated PDK2",
            "I[61] phosphorylated S6K-IRS1/3 complex",
            "I[62] GAPDH-Rheb (phosphorylated mTORC1) complex",
        };

        public void TurnOffInsulin()
        {
            IrsMtorcCellState[1 - 1] = 0;
        }
    }
}
