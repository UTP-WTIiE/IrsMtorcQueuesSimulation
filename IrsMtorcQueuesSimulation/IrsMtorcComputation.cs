using mTORC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mTORC.Functions
{
    public class IrsMtorcComputation
    {
        IrsMtorcConstants original_consts;
        IrsMtorcConstants consts;

        public IrsMtorcComputation(IrsMtorcConstants consts, IrsMtorcCellStateWrapper startState)
        {
            this.original_consts = consts;
            this.consts = this.original_consts.ApplyNoise(0);
        }

        public void ComputeOneTimestep(IrsMtorcCellStateWrapper state, bool keep_insulin_level = true, AdditionalTimeStepComputationSettings settings = null)
        {
            var start_insulin = state.IrsMtorcCellState.I;
            double c = 1.72 * 6.022e4;

            Random r = new Random();
            double RV(int w = 0) => r.NextDouble();
            double rand(int w = 0) => r.NextDouble();

            // GlycolysisQueues.queue for IR - I
            double v45 = consts.C[45 - 1, 1 - 1] * state.IrsMtorcCellState[2 - 1] * state.IrsMtorcCellState[1 - 1] - consts.C[45 - 1, 2 - 1] * state.IrsMtorcCellState[3 - 1];
            if (RV(45) <= v45 && state.IrsMtorcCellState[2 - 1] > 0 && state.IrsMtorcCellState[1 - 1] > 0)
            {
                state.IrsMtorcCellState[3 - 1] = state.IrsMtorcCellState[3 - 1] + 1;
                state.IrsMtorcCellState[2 - 1] = state.IrsMtorcCellState[2 - 1] - 1;
                state.IrsMtorcCellState[1 - 1] = state.IrsMtorcCellState[1 - 1] - 1;
            }

            // GlycolysisQueues.queue for pIR - I

            double v1 = consts.C[1 - 1, 1 - 1] * state.IrsMtorcCellState[3 - 1];
            if (RV(1) <= v1 && state.IrsMtorcCellState[3 - 1] > 0)
            {
                state.IrsMtorcCellState[4 - 1] = state.IrsMtorcCellState[4 - 1] + 1;
                state.IrsMtorcCellState[3 - 1] = state.IrsMtorcCellState[3 - 1] - 1;
            }

            // First loop - insuline reception.
            double v2 = consts.C[2 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1];
            double v3 = consts.C[3 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1] - consts.C[3 - 1, 2 - 1] * state.IrsMtorcCellState[5 - 1];
            double v4 = consts.C[4 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1] * state.IrsMtorcCellState[1 - 1] - consts.C[4 - 1, 2 - 1] * state.IrsMtorcCellState[6 - 1];
            double V234 = v2 + v3 + v4;

            if (RV(2) <= V234 && state.IrsMtorcCellState[4 - 1] > 0)
            {
                if (RV(3) <= v3 / V234)
                {
                    state.IrsMtorcCellState[5 - 1] = state.IrsMtorcCellState[5 - 1] + 1;
                    state.IrsMtorcCellState[4 - 1] = state.IrsMtorcCellState[4 - 1] - 1;
                }
            }
            else
            {
                
                if (RV(3) <= v2 / V234 && state.IrsMtorcCellState[4 - 1] > 0) //dopisałem warunek && state.IrsMtorcCellState[4 - 1] > 0
                {
                    state.IrsMtorcCellState[2 - 1] = state.IrsMtorcCellState[2 - 1] + 1;
                    state.IrsMtorcCellState[4 - 1] = state.IrsMtorcCellState[4 - 1] - 1;
                }
                else
                {

                    if (state.IrsMtorcCellState[1 - 1] > 0 && state.IrsMtorcCellState[4 - 1] > 0) //dopisałem warunek && state.IrsMtorcCellState[4 - 1] > 0
                    {
                        state.IrsMtorcCellState[6 - 1] = state.IrsMtorcCellState[6 - 1] + 1;
                        state.IrsMtorcCellState[4 - 1] = state.IrsMtorcCellState[4 - 1] - 1;
                        state.IrsMtorcCellState[1 - 1] = state.IrsMtorcCellState[1 - 1] - 1;
                    }

                }
            }

            // GlycolysisQueues.queue for | pIR - II |.
            double v5 = consts.C[5 - 1, 1 - 1] * state.IrsMtorcCellState[6 - 1] * 5 - consts.C[3 - 1, 2 - 1] * state.IrsMtorcCellState[5 - 1];
            if (RV(5) <= v5 && state.IrsMtorcCellState[6 - 1] >= 1)
            {
                state.IrsMtorcCellState[7 - 1] = state.IrsMtorcCellState[7 - 1] + 1;
                state.IrsMtorcCellState[6 - 1] = state.IrsMtorcCellState[6 - 1] - 1;
            }

            // GlycolysisQueues.queues for IRS1 / 3 & PI3K *.
            double v9 = consts.C[9 - 1, 1 - 1] * state.IrsMtorcCellState[12 - 1] - consts.C[9 - 1, 2 - 1] * state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[13 - 1];
            v9 *= settings.v9Coefficient;
            if (RV(9) <= v9 && state.IrsMtorcCellState[12 - 1] >= 1)
            {
                state.IrsMtorcCellState[8 - 1] = state.IrsMtorcCellState[8 - 1] + 1;
                state.IrsMtorcCellState[13 - 1] = state.IrsMtorcCellState[13 - 1] + 1;
                state.IrsMtorcCellState[12 - 1] = state.IrsMtorcCellState[12 - 1] - 1;
            }

            // GlycolysisQueues.queue for pIRS1 / 3.
            double v6 = consts.C[6 - 1, 1 - 1] * state.IrsMtorcCellState[8 - 1] * (state.IrsMtorcCellState[6 - 1] + state.IrsMtorcCellState[4 - 1]) / (state.IrsMtorcCellState[2 - 1] + 0.01) - consts.C[6 - 1, 2 - 1] * state.IrsMtorcCellState[9 - 1];

            if (settings != null)
                v6 *= settings.v6Coefficient;

            if (RV(6) <= v6 && state.IrsMtorcCellState[6 - 1] > 0 && state.IrsMtorcCellState[8 - 1] >= 1)
            {
                state.IrsMtorcCellState[9 - 1] = state.IrsMtorcCellState[9 - 1] + 1;
                state.IrsMtorcCellState[8 - 1] = state.IrsMtorcCellState[8 - 1] - 1;
            }

            // GlycolysisQueues.queue for pIRS1 / 3 - PI3K.
            double v7 = consts.C[7 - 1, 1 - 1] * (state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[10 - 1] - consts.C[7 - 1, 2 - 1] * state.IrsMtorcCellState[11 - 1]);
            v7 *= settings.v7Coefficient;

            if (RV(7) <= v7 && state.IrsMtorcCellState[9 - 1] >= 1 && state.IrsMtorcCellState[10 - 1] >= 1)
            {
                state.IrsMtorcCellState[11 - 1] = state.IrsMtorcCellState[11 - 1] + 1;
                state.IrsMtorcCellState[9 - 1] = state.IrsMtorcCellState[9 - 1] - 1;
                state.IrsMtorcCellState[10 - 1] = state.IrsMtorcCellState[10 - 1] - 1;
            }

            // GlycolysisQueues.queue for pIRS1 / 3 - PI3K *.
            double v8 = consts.C[8 - 1, 1 - 1] * state.IrsMtorcCellState[11 - 1] - consts.C[8 - 1, 2 - 1] * state.IrsMtorcCellState[12 - 1];
            v8 *= settings.v8Coefficient;
            if (RV(8) <= v8 && state.IrsMtorcCellState[11 - 1] >= 1)
            {
                state.IrsMtorcCellState[12 - 1] = state.IrsMtorcCellState[12 - 1] + 1;
                state.IrsMtorcCellState[11 - 1] = state.IrsMtorcCellState[11 - 1] - 1;
            }


            // GlycolysisQueues.queue for PI3K * -PI.
            double v11 = consts.C[11 - 1, 1 - 1] * (state.IrsMtorcCellState[17 - 1] * state.IrsMtorcCellState[13 - 1] - consts.C[11 - 1, 2 - 1] * state.IrsMtorcCellState[14 - 1]);
            if (RV(11) <= v11 && state.IrsMtorcCellState[13 - 1] > 0 && state.IrsMtorcCellState[17 - 1] > 0)
            {
                state.IrsMtorcCellState[14 - 1] = state.IrsMtorcCellState[14 - 1] + 1;
                state.IrsMtorcCellState[13 - 1] = state.IrsMtorcCellState[13 - 1] - 1;
                state.IrsMtorcCellState[17 - 1] = state.IrsMtorcCellState[17 - 1] - 1;
            }

            // GlycolysisQueues.queue for PI3K.
            double v10 = consts.C[10 - 1, 1 - 1] * state.IrsMtorcCellState[13 - 1];
            if (RV(10) <= v10 && state.IrsMtorcCellState[13 - 1] > 0)
            {
                state.IrsMtorcCellState[10 - 1] = state.IrsMtorcCellState[10 - 1] + 1;
                state.IrsMtorcCellState[13 - 1] = state.IrsMtorcCellState[13 - 1] - 1;
            }

            // GlycolysisQueues.queue for PI3K * -PIP3.
            double v12 = consts.C[12 - 1, 1 - 1] * state.IrsMtorcCellState[14 - 1];
            if (RV(12) <= v12 && state.IrsMtorcCellState[14 - 1] > 0)
            {
                state.IrsMtorcCellState[15 - 1] = state.IrsMtorcCellState[15 - 1] + 1;
                state.IrsMtorcCellState[14 - 1] = state.IrsMtorcCellState[14 - 1] - 1;
            }

            // GlycolysisQueues.queues for PI3K * &PIP3.
            double v13 = consts.C[13 - 1, 1 - 1] * state.IrsMtorcCellState[15 - 1];
            if (RV(13) <= v13 && state.IrsMtorcCellState[15 - 1] > 0)
            {
                state.IrsMtorcCellState[13 - 1] = state.IrsMtorcCellState[13 - 1] + 1;
                state.IrsMtorcCellState[16 - 1] = state.IrsMtorcCellState[16 - 1] + 1;
                state.IrsMtorcCellState[15 - 1] = state.IrsMtorcCellState[15 - 1] - 1;
            }

            // GlycolysisQueues.queue for PTEN - PIP3.
            double v14 = consts.C[14 - 1, 1 - 1] * (state.IrsMtorcCellState[16 - 1] * state.IrsMtorcCellState[20 - 1] - consts.C[14 - 1, 2 - 1] * state.IrsMtorcCellState[18 - 1]);
            if (RV(14) <= v14 && state.IrsMtorcCellState[16 - 1] > 0 && state.IrsMtorcCellState[20 - 1] > 0)
            {
                state.IrsMtorcCellState[18 - 1] = state.IrsMtorcCellState[18 - 1] + 1;
                state.IrsMtorcCellState[16 - 1] = state.IrsMtorcCellState[16 - 1] - 1;
                state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] - 1;
            }

            // GlycolysisQueues.queue for PTEN - PI.
            double v15 = consts.C[15 - 1, 1 - 1] * state.IrsMtorcCellState[18 - 1];
            if (RV(15) <= v15 && state.IrsMtorcCellState[18 - 1] > 0)
            {
                state.IrsMtorcCellState[19 - 1] = state.IrsMtorcCellState[19 - 1] + 1;
                state.IrsMtorcCellState[18 - 1] = state.IrsMtorcCellState[18 - 1] - 1;
            }

            // GlycolysisQueues.queues for PTEN & PI.
            double v16 = consts.C[16 - 1, 1 - 1] * state.IrsMtorcCellState[19 - 1];
            if (RV(16) <= v16 && state.IrsMtorcCellState[19 - 1] > 0)
            {
                state.IrsMtorcCellState[17 - 1] = state.IrsMtorcCellState[17 - 1] + 1;
                state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] + 1;
                state.IrsMtorcCellState[19 - 1] = state.IrsMtorcCellState[19 - 1] - 1;
            }

            // GlycolysisQueues.queue for pPTEN.
            double v17 = consts.C[17 - 1, 1 - 1] * state.IrsMtorcCellState[20 - 1] / (consts.C[17 - 1, 2 - 1] + state.IrsMtorcCellState[20 - 1]);
            if (RV(17) <= v17 && state.IrsMtorcCellState[20 - 1] > 0)
            {
                state.IrsMtorcCellState[23 - 1] = state.IrsMtorcCellState[23 - 1] + 1;
                state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] - 1;
            }

            // GlycolysisQueues.queue for pPTEN - PTEN.
            double v18 = consts.C[18 - 1, 1 - 1] * (state.IrsMtorcCellState[20 - 1] * state.IrsMtorcCellState[23 - 1] - consts.C[18 - 1, 2 - 1] * state.IrsMtorcCellState[21 - 1]);
            if (RV(18) <= v18 && state.IrsMtorcCellState[23 - 1] > 0 && state.IrsMtorcCellState[20 - 1] > 0)
            {
                state.IrsMtorcCellState[21 - 1] = state.IrsMtorcCellState[21 - 1] + 1;
                state.IrsMtorcCellState[23 - 1] = state.IrsMtorcCellState[23 - 1] - 1;
                state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] - 1;
            }

            // GlycolysisQueues.queue for PTEN - PTEN.
            double v19 = consts.C[19 - 1, 1 - 1] * state.IrsMtorcCellState[21 - 1];
            if (RV(19) <= v19 && state.IrsMtorcCellState[21 - 1] > 0)
            {
                state.IrsMtorcCellState[22 - 1] = state.IrsMtorcCellState[22 - 1] + 1;
                state.IrsMtorcCellState[21 - 1] = state.IrsMtorcCellState[21 - 1] - 1;
            }

            // GlycolysisQueues.queue for PTEN.
            double v20 = consts.C[20 - 1, 1 - 1] * state.IrsMtorcCellState[22 - 1];
            if (RV(20) <= v20 && state.IrsMtorcCellState[22 - 1] > 0)
            {
                state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] + 1;
                //state.IrsMtorcCellState[20 - 1] = state.IrsMtorcCellState[20 - 1] + 1;
                state.IrsMtorcCellState[22 - 1] = state.IrsMtorcCellState[22 - 1] - 1;
            }

            // GlycolysisQueues.queue for Akt - PIP3.
            double v21 = consts.C[21 - 1, 1 - 1] * (state.IrsMtorcCellState[16 - 1] * state.IrsMtorcCellState[24 - 1] - consts.C[21 - 1, 2 - 1] * state.IrsMtorcCellState[25 - 1]);
            if (RV(21) <= v21 && state.IrsMtorcCellState[16 - 1] > 0 && state.IrsMtorcCellState[24 - 1] > 0)
            {
                state.IrsMtorcCellState[25 - 1] = state.IrsMtorcCellState[25 - 1] + 1;
                state.IrsMtorcCellState[24 - 1] = state.IrsMtorcCellState[24 - 1] - 1;
                state.IrsMtorcCellState[16 - 1] = state.IrsMtorcCellState[16 - 1] - 1;
            }

            // GlycolysisQueues.queue for pAkt - PIP3.
            double v22 = consts.C[22 - 1, 1 - 1] * state.IrsMtorcCellState[25 - 1] / (consts.C[22 - 1, 2 - 1] + state.IrsMtorcCellState[25 - 1]);
            if (RV(22) <= v22 && state.IrsMtorcCellState[25 - 1] > 0)
            {
                state.IrsMtorcCellState[26 - 1] = state.IrsMtorcCellState[26 - 1] + 1;
                state.IrsMtorcCellState[25 - 1] = state.IrsMtorcCellState[25 - 1] - 1;
            }

            double v23 = (consts.C[56 - 1, 1 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[58 - 1] + consts.C[56 - 1, 2 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[60 - 1]);
            if (RV(23) <= v23 && state.IrsMtorcCellState[26 - 1] >= 1)
            {
                state.IrsMtorcCellState[35 - 1] = state.IrsMtorcCellState[35 - 1] + 1;
                state.IrsMtorcCellState[26 - 1] = state.IrsMtorcCellState[26 - 1] - 1;
            }

            // Dephosphorylation of Akt
            double v56 = consts.C[56 - 1, 3 - 1] * state.IrsMtorcCellState[35 - 1];
            if (RV(56) <= v56 && state.IrsMtorcCellState[26 - 1] >= 1)
            {
                state.IrsMtorcCellState[26 - 1] = state.IrsMtorcCellState[26 - 1] + 1;
                state.IrsMtorcCellState[35 - 1] = state.IrsMtorcCellState[35 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - ppAkt - PIP3.
            double v24 = consts.C[24 - 1, 1 - 1] * (state.IrsMtorcCellState[35 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[24 - 1, 2 - 1] * state.IrsMtorcCellState[29 - 1]);
            if (RV(24) <= v24 && state.IrsMtorcCellState[30 - 1] > 0 && state.IrsMtorcCellState[35 - 1] > 0)
            {
                state.IrsMtorcCellState[29 - 1] = state.IrsMtorcCellState[29 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] - 1;
                state.IrsMtorcCellState[35 - 1] = state.IrsMtorcCellState[35 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - pAkt - PIP3.
            double v25 = consts.C[25 - 1, 1 - 1] * state.IrsMtorcCellState[29 - 1];
            if (RV(25) <= v25 && state.IrsMtorcCellState[29 - 1] > 0)
            {
                state.IrsMtorcCellState[28 - 1] = state.IrsMtorcCellState[28 - 1] + 1;
                state.IrsMtorcCellState[29 - 1] = state.IrsMtorcCellState[29 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - pAkt - PIP3.
            double v26 = consts.C[26 - 1, 1 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[30 - 1];
            if (RV(26) <= v26 && state.IrsMtorcCellState[26 - 1] > 0 && state.IrsMtorcCellState[30 - 1] > 0)
            {
                state.IrsMtorcCellState[28 - 1] = state.IrsMtorcCellState[28 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] - 1;
                state.IrsMtorcCellState[26 - 1] = state.IrsMtorcCellState[26 - 1] - 1;
            }

            // GlycolysisQueues.queues for PP2A & pAkt - PIP3.
            double v27 = consts.C[27 - 1, 1 - 1] * state.IrsMtorcCellState[28 - 1];
            if (RV(27) <= v27 && state.IrsMtorcCellState[28 - 1] > 0)
            {
                state.IrsMtorcCellState[26 - 1] = state.IrsMtorcCellState[26 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] + 1;
                state.IrsMtorcCellState[28 - 1] = state.IrsMtorcCellState[28 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - Akt - PIP3.
            double v28 = consts.C[28 - 1, 1 - 1] * state.IrsMtorcCellState[28 - 1];
            if (RV(28) <= v28 && state.IrsMtorcCellState[28 - 1] > 0)
            {
                state.IrsMtorcCellState[27 - 1] = state.IrsMtorcCellState[27 - 1] + 1;
                state.IrsMtorcCellState[28 - 1] = state.IrsMtorcCellState[28 - 1] - 1;
            }

            // GlycolysisQueues.queues for PP2A & Akt - PIP3.
            double v29 = consts.C[29 - 1, 1 - 1] * state.IrsMtorcCellState[27 - 1];
            if (RV(29) <= v29 && state.IrsMtorcCellState[27 - 1] > 0)
            {
                state.IrsMtorcCellState[25 - 1] = state.IrsMtorcCellState[25 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] + 1;
                state.IrsMtorcCellState[27 - 1] = state.IrsMtorcCellState[27 - 1] - 1;
            }

            // GlycolysisQueues.queues for PIP3 & ppAkt.
            double v30 = consts.C[30 - 1, 1 - 1] * state.IrsMtorcCellState[35 - 1];
            if (RV(30) <= v30 && state.IrsMtorcCellState[35 - 1] > 0)
            {
                state.IrsMtorcCellState[16 - 1] = state.IrsMtorcCellState[16 - 1] + 1;
                state.IrsMtorcCellState[34 - 1] = state.IrsMtorcCellState[34 - 1] + 1;
                state.IrsMtorcCellState[35 - 1] = state.IrsMtorcCellState[35 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - ppAkt.
            double v31 = consts.C[31 - 1, 1 - 1] * (state.IrsMtorcCellState[34 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[31 - 1, 2 - 1] * state.IrsMtorcCellState[33 - 1]);
            if (RV(31) <= v31 && state.IrsMtorcCellState[34 - 1] > 0 && state.IrsMtorcCellState[30 - 1] > 0)
            {
                state.IrsMtorcCellState[33 - 1] = state.IrsMtorcCellState[33 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] - 1;
                state.IrsMtorcCellState[34 - 1] = state.IrsMtorcCellState[34 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - pAkt.
            double v32 = consts.C[32 - 1, 1 - 1] * state.IrsMtorcCellState[33 - 1];
            if (RV(32) <= v32 && state.IrsMtorcCellState[33 - 1] > 0)
            {
                state.IrsMtorcCellState[32 - 1] = state.IrsMtorcCellState[32 - 1] + 1;
                state.IrsMtorcCellState[33 - 1] = state.IrsMtorcCellState[33 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - Akt.
            double v33 = consts.C[33 - 1, 1 - 1] * state.IrsMtorcCellState[32 - 1];
            if (RV(33) <= v33 && state.IrsMtorcCellState[32 - 1] > 0)
            {
                state.IrsMtorcCellState[31 - 1] = state.IrsMtorcCellState[31 - 1] + 1;
                state.IrsMtorcCellState[32 - 1] = state.IrsMtorcCellState[32 - 1] - 1;
            }

            // GlycolysisQueues.queues for PP2A & Akt.
            double v34 = consts.C[34 - 1, 1 - 1] * state.IrsMtorcCellState[31 - 1];
            if (RV(34) <= v34 && state.IrsMtorcCellState[31 - 1] > 0)
            {
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] + 1;
                state.IrsMtorcCellState[24 - 1] = state.IrsMtorcCellState[24 - 1] + 1;
                state.IrsMtorcCellState[31 - 1] = state.IrsMtorcCellState[31 - 1] - 1;
            }

            // GlycolysisQueues.queues for pAs160 & ppAkt.
            double v35 = consts.C[35 - 1, 1 - 1] * state.IrsMtorcCellState[37 - 1] / (consts.C[35 - 1, 2 - 1] + state.IrsMtorcCellState[37 - 1]) * 0.125;
            if (RV(35) <= v35 && state.IrsMtorcCellState[34 - 1] > 0 && state.IrsMtorcCellState[37 - 1] > 0)
            {
                state.IrsMtorcCellState[38 - 1] = state.IrsMtorcCellState[38 - 1] + 1;
                state.IrsMtorcCellState[37 - 1] = state.IrsMtorcCellState[37 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - pAs160.
            double v36 = consts.C[36 - 1, 1 - 1] * (state.IrsMtorcCellState[38 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[36 - 1, 2 - 1] * state.IrsMtorcCellState[39 - 1]) * 1;
            if (RV(36) <= v36 && state.IrsMtorcCellState[38 - 1] > 0 && state.IrsMtorcCellState[30 - 1] > 0)
            {
                state.IrsMtorcCellState[39 - 1] = state.IrsMtorcCellState[39 - 1] + 1;
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] - 1;
                state.IrsMtorcCellState[38 - 1] = state.IrsMtorcCellState[38 - 1] - 1;
            }

            // GlycolysisQueues.queue for PP2A - As160.
            double v37 = consts.C[37 - 1, 1 - 1] * state.IrsMtorcCellState[39 - 1];
            if (RV(37) <= v37 && state.IrsMtorcCellState[39 - 1] > 0)
            {
                state.IrsMtorcCellState[40 - 1] = state.IrsMtorcCellState[40 - 1] + 1;
                state.IrsMtorcCellState[39 - 1] = state.IrsMtorcCellState[39 - 1] - 1;
            }

            // GlycolysisQueues.queues for PP2A & As160.
            double v38 = consts.C[38 - 1, 1 - 1] * state.IrsMtorcCellState[40 - 1] * 0.025;
            if (RV(38) <= v38 && state.IrsMtorcCellState[40 - 1] > 0)
            {
                state.IrsMtorcCellState[30 - 1] = state.IrsMtorcCellState[30 - 1] + 1;
                state.IrsMtorcCellState[37 - 1] = state.IrsMtorcCellState[37 - 1] + 1;
                state.IrsMtorcCellState[40 - 1] = state.IrsMtorcCellState[40 - 1] - 1;
            }

            // GlycolysisQueues.queue for RabGTP - As160.
            double v39 = (consts.C[39 - 1, 1 - 1] + consts.C[39 - 1, 2 - 1]) / (consts.C[39 - 1, 3 - 1] / consts.C[39 - 1, 4 - 1]) * state.IrsMtorcCellState[41 - 1] * state.IrsMtorcCellState[37 - 1] * c / 10 * 0.125;
            double v46 = state.IrsMtorcCellState[41 - 1] * 370000 * consts.C[46 - 1, 1 - 1] / 10;
            if (RV(39) <= v39 + v46 && state.IrsMtorcCellState[37 - 1] * c > 10 && state.IrsMtorcCellState[41 - 1] > 10 && state.IrsMtorcCellState[46 - 1] > 20)
            {
                state.IrsMtorcCellState[41 - 1] = state.IrsMtorcCellState[41 - 1] - 10;
                if (rand(1) < v46 / (v46 + v39))
                {
                    state.IrsMtorcCellState[46 - 1] = state.IrsMtorcCellState[46 - 1] - 20;
                    state.IrsMtorcCellState[47 - 1] = state.IrsMtorcCellState[47 - 1] + 20;
                }
                else
                {
                    state.IrsMtorcCellState[43 - 1] = state.IrsMtorcCellState[43 - 1] + 10;
                }
            }

            // GlycolysisQueues.queues for RabGTP & RabGDP.
            double v40 = consts.C[40 - 1, 1 - 1] * state.IrsMtorcCellState[43 - 1] / 10;
            if (RV(40) <= v40 && state.IrsMtorcCellState[43 - 1] > 10)
            {
                state.IrsMtorcCellState[43 - 1] = state.IrsMtorcCellState[43 - 1] - 10;
                if (rand(1) > 0.5)
                {
                    state.IrsMtorcCellState[41 - 1] = state.IrsMtorcCellState[41 - 1] + 10;
                }
                else
                {
                    state.IrsMtorcCellState[42 - 1] = state.IrsMtorcCellState[42 - 1] + 10;
                }
            }


            // GlycolysisQueues.queue for RabGDP - GEF.
            double v42 = (consts.C[42 - 1, 1 - 1] + consts.C[42 - 1, 2 - 1]) / (consts.C[42 - 1, 3 - 1] / consts.C[42 - 1, 4 - 1]) * state.IrsMtorcCellState[42 - 1] * state.IrsMtorcCellState[44 - 1] / 10 * 1;
            if (RV(42) <= v42 && state.IrsMtorcCellState[42 - 1] > 10 && state.IrsMtorcCellState[44 - 1] > 10)
            {
                state.IrsMtorcCellState[45 - 1] = state.IrsMtorcCellState[45 - 1] + 10;
                state.IrsMtorcCellState[42 - 1] = state.IrsMtorcCellState[42 - 1] - 10;
                state.IrsMtorcCellState[44 - 1] = state.IrsMtorcCellState[44 - 1] - 10;
            }

            // GlycolysisQueues.queues for GEF & RabGDP & RabGTP.
            double v43 = consts.C[43 - 1, 1 - 1] * state.IrsMtorcCellState[45 - 1] / 10;
            double v44 = consts.C[44 - 1, 1 - 1] * state.IrsMtorcCellState[45 - 1] / 10;
            if (RV(43) <= v43 + v44 && state.IrsMtorcCellState[45 - 1] > 10)
            {
                state.IrsMtorcCellState[45 - 1] = state.IrsMtorcCellState[45 - 1] - 10;
                state.IrsMtorcCellState[44 - 1] = state.IrsMtorcCellState[44 - 1] + 10;
                if (rand(1) < consts.C[44 - 1, 1 - 1] / (consts.C[44 - 1, 1 - 1] + consts.C[43 - 1, 1 - 1]))
                {
                    state.IrsMtorcCellState[41 - 1] = state.IrsMtorcCellState[41 - 1] + 10;
                }
                else
                {
                    state.IrsMtorcCellState[42 - 1] = state.IrsMtorcCellState[42 - 1] + 10;
                }
            }


            double v47 = state.IrsMtorcCellState[47 - 1] * consts.C[47 - 1, 1 - 1] / 10 * state.IrsMtorcCellState[37 - 1] * c / 112e5;
            if (RV(47) <= v47 && state.IrsMtorcCellState[47 - 1] > 20)
            {
                state.IrsMtorcCellState[47 - 1] = state.IrsMtorcCellState[47 - 1] - 20;
                state.IrsMtorcCellState[46 - 1] = state.IrsMtorcCellState[46 - 1] + 20;
                state.IrsMtorcCellState[41 - 1] = state.IrsMtorcCellState[41 - 1] + 10;
            }

            // GlycolysisQueues.queue for PDK2 - p
            double v60 = consts.C[60 - 1, 1 - 1] * state.IrsMtorcCellState[6 - 1] * 100 * state.IrsMtorcCellState[59 - 1];
            if (RV(60) <= v60 && state.IrsMtorcCellState[59 - 1] >= 10)
            {
                state.IrsMtorcCellState[59 - 1] = state.IrsMtorcCellState[59 - 1] - 10;
                state.IrsMtorcCellState[60 - 1] = state.IrsMtorcCellState[60 - 1] + 10;
            }

            // GlycolysisQueues.queue for PDK2

            double v59 = consts.C[59 - 1, 1 - 1] * state.IrsMtorcCellState[60 - 1];
            if (RV(59) <= v59 && state.IrsMtorcCellState[60 - 1] >= 1)
            {
                state.IrsMtorcCellState[60 - 1] = state.IrsMtorcCellState[60 - 1] - 1;
                state.IrsMtorcCellState[59 - 1] = state.IrsMtorcCellState[59 - 1] + 1;
            }

            // GlycolysisQueues.queue for mTORC2a

            double v58 = consts.C[58 - 1, 1 - 1] * state.IrsMtorcCellState[13 - 1] * state.IrsMtorcCellState[57 - 1] * 100;
            if (RV(58) <= v58 && state.IrsMtorcCellState[57 - 1] >= 10)
            {
                state.IrsMtorcCellState[57 - 1] = state.IrsMtorcCellState[57 - 1] - 10;
                state.IrsMtorcCellState[58 - 1] = state.IrsMtorcCellState[58 - 1] + 10;
            }

            // GlycolysisQueues.queue for mTORC2

            double v57 = consts.C[57 - 1, 1 - 1] * state.IrsMtorcCellState[58 - 1];
            if (RV(57) <= v57 && state.IrsMtorcCellState[58 - 1] >= 1)
            {
                state.IrsMtorcCellState[58 - 1] = state.IrsMtorcCellState[58 - 1] - 1;
                state.IrsMtorcCellState[57 - 1] = state.IrsMtorcCellState[57 - 1] + 1;
            }

            // GlycolysisQueues.queue for AMPK_p

            double v53 = consts.C[53 - 1, 1 - 1] * state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[53 - 1];
            if (RV(53) <= v53 && state.IrsMtorcCellState[53 - 1] >= 1)
            {
                state.IrsMtorcCellState[53 - 1] = state.IrsMtorcCellState[53 - 1] - 1;
                state.IrsMtorcCellState[54 - 1] = state.IrsMtorcCellState[54 - 1] + 1;
            }

            // GlycolysisQueues.queue for AMPK

            double v54 = consts.C[54 - 1, 1 - 1] * state.IrsMtorcCellState[54 - 1];
            if (RV(54) <= v54 && state.IrsMtorcCellState[54 - 1] >= 1)
            {
                state.IrsMtorcCellState[54 - 1] = state.IrsMtorcCellState[54 - 1] - 1;
                state.IrsMtorcCellState[53 - 1] = state.IrsMtorcCellState[53 - 1] + 1;
            }

            // GlycolysisQueues.queue for TSC1_TSC2_S1387

            double v55 = consts.C[55 - 1, 1 - 1] * state.IrsMtorcCellState[55 - 1] * state.IrsMtorcCellState[54 - 1];
            if (RV(55) <= v55 && state.IrsMtorcCellState[55 - 1] >= 1)
            {
                state.IrsMtorcCellState[55 - 1] = state.IrsMtorcCellState[55 - 1] - 1;
                state.IrsMtorcCellState[56 - 1] = state.IrsMtorcCellState[56 - 1] + 1;
            }

            // GlycolysisQueues.queue for TSC1_TSC2_T1462

            double v52 = consts.C[55 - 1, 2 - 1] * state.IrsMtorcCellState[56 - 1];
            if (RV(52) <= v52 && state.IrsMtorcCellState[56 - 1] >= 1)
            {
                state.IrsMtorcCellState[56 - 1] = state.IrsMtorcCellState[56 - 1] - 1;
                state.IrsMtorcCellState[55 - 1] = state.IrsMtorcCellState[55 - 1] + 1;
            }

            // GlycolysisQueues.queue for mTORC1 - a

            double v48 = consts.C[48 - 1, 1 - 1] * state.IrsMtorcCellState[48 - 1] * state.IrsMtorcCellState[49 - 1];
            if (RV(48) <= v48 && state.IrsMtorcCellState[49 - 1] >= 1)
            {
                state.IrsMtorcCellState[49 - 1] = state.IrsMtorcCellState[49 - 1] - 1;
                state.IrsMtorcCellState[50 - 1] = state.IrsMtorcCellState[50 - 1] + 1;
            }

            //Queue for GAPDH_mTORC1 complexes
            var q6 = 1.94e-3;

            double gapdh_active = consts.GAPDH_mTORC1_Complex_Consts[3] * state.GapdhActiveCoefficient;
            double V62k1 = consts.GAPDH_mTORC1_Complex_Consts[0] / (q6 + consts.GAPDH_mTORC1_Complex_Consts[1]);
            double V62k2 = consts.GAPDH_mTORC1_Complex_Consts[2];
            double v62 = ((V62k1 * gapdh_active * state.IrsMtorcCellState[50 - 1]) - (V62k2 * state.IrsMtorcCellState[62 - 1])) / 10;
            double v63 = ((V62k2 * state.IrsMtorcCellState[62 - 1]) - V62k1 * (gapdh_active * state.IrsMtorcCellState[50 - 1])) / 1000;

            v62 *= settings.v62Coefficient;
            v63 *= settings.v63Coefficient;

            if (RV(62) <= v62 && state.IrsMtorcCellState[50-1] >= 1) {
                state.IrsMtorcCellState[50-1] = state.IrsMtorcCellState[50-1] - 1;
                state.IrsMtorcCellState[62-1] = state.IrsMtorcCellState[62-1] + 1;
            }
            if (RV(63) <= v63 && state.IrsMtorcCellState[62-1] >= 1)
            {
                state.IrsMtorcCellState[62-1] = state.IrsMtorcCellState[62-1] - 1;
                state.IrsMtorcCellState[50-1] = state.IrsMtorcCellState[50-1] + 1;
            }
            

            // GlycolysisQueues.queue for mTORC1

            double v49 = consts.C[48 - 1, 2 - 1] * state.IrsMtorcCellState[50 - 1] * state.IrsMtorcCellState[55 - 1];
            if (RV(49) <= v49 && state.IrsMtorcCellState[50 - 1] >= 1)
            {
                state.IrsMtorcCellState[50 - 1] = state.IrsMtorcCellState[50 - 1] - 1;
                state.IrsMtorcCellState[49 - 1] = state.IrsMtorcCellState[49 - 1] + 1;
            }

            // GlycolysisQueues.queue for S6Kp

            double v50 = consts.C[52 - 1, 1 - 1] * state.IrsMtorcCellState[51 - 1] * state.IrsMtorcCellState[50 - 1]; // * 0; //POKAZAĆ!
            v50 *= settings.v50Coefficient;

            if (RV(50) <= v50 && state.IrsMtorcCellState[51 - 1] >= 1)
            {
                state.IrsMtorcCellState[51 - 1] = state.IrsMtorcCellState[51 - 1] - 1;
                state.IrsMtorcCellState[52 - 1] = state.IrsMtorcCellState[52 - 1] + 1;
            }

            // GlycolysisQueues.queue for S6K

            double v51 = consts.C[52 - 1, 2 - 1] * state.IrsMtorcCellState[52 - 1];
            v51 *= settings.v51Coefficient;

            if (RV(51) <= v51 && state.IrsMtorcCellState[52 - 1] >= 1)
            {
                state.IrsMtorcCellState[52 - 1] = state.IrsMtorcCellState[52 - 1] - 1;
                state.IrsMtorcCellState[51 - 1] = state.IrsMtorcCellState[51 - 1] + 1;
            }

            //Queue for IRS1_pS636_PI3K
            double v61 = consts.C[51-1, 1-1] * state.IrsMtorcCellState[52-1] * state.IrsMtorcCellState[8-1] / 1000;
            v61 *= settings.v61Coefficient;

            double v64 = consts.C[51-1, 2-1] * state.IrsMtorcCellState[52-1] * state.IrsMtorcCellState[8-1] * 14;
            v64 *= settings.v64Coefficient;

            if (RV(61) <= v61 && state.IrsMtorcCellState[61-1] >= 1) {
                state.IrsMtorcCellState[8-1] = state.IrsMtorcCellState[8-1] + 1;
                state.IrsMtorcCellState[52 - 1] = state.IrsMtorcCellState[52 - 1] + 1;
                state.IrsMtorcCellState[61-1] = state.IrsMtorcCellState[61-1] - 1;
            }
            if (RV(64) <= v64 && state.IrsMtorcCellState[8-1] >= 1 && state.IrsMtorcCellState[52 - 1] >= 1) {

                state.IrsMtorcCellState[8 - 1] = state.IrsMtorcCellState[8 - 1] - 1;
                state.IrsMtorcCellState[52 - 1] = state.IrsMtorcCellState[52 - 1] - 1;
                state.IrsMtorcCellState[61 - 1] = state.IrsMtorcCellState[61 - 1] + 1;
            }

            if (keep_insulin_level)
            {
                state.IrsMtorcCellState[1 - 1] = start_insulin;
            }
            if (settings != null)
            {
                if(settings.forcedPTENState.HasValue)
                {
                    state.IrsMtorcCellState.PTEN = settings.forcedPTENState.Value;
                }
                if (settings.forcedPIR_IIState.HasValue)
                    state.IrsMtorcCellState.pIR_II = settings.forcedPIR_IIState.Value;
                if (settings.forcedIRS_1_3_State.HasValue)
                    state.IrsMtorcCellState.IRS1_3 = settings.forcedIRS_1_3_State.Value;

                if (settings.IsGapdhZero)
                {
                    if (settings.MaximumPPAktValueIfGAPDHisZero.HasValue)
                    {
                        state.IrsMtorcCellState.ppAkt = Math.Min(state.IrsMtorcCellState.ppAkt, settings.MaximumPPAktValueIfGAPDHisZero.Value);
                    }

                    if (settings.MinimumAs160ValueIfGAPDHisZero.HasValue)
                    {
                        state.IrsMtorcCellState.As160 = Math.Max(state.IrsMtorcCellState.As160, settings.MinimumAs160ValueIfGAPDHisZero.Value);
                    }
                    if (settings.forceI11WhenGAPDHIsZero.HasValue)
                    {
                        state.IrsMtorcCellState[11 - 1] = settings.forceI11WhenGAPDHIsZero.Value;
                    }
                }
            }
        }
    
        public double[] ComputeRates(IrsMtorcCellStateWrapper state, AdditionalTimeStepComputationSettings settings)
        {
            double c = 1.72 * 6.022e4;
            double Akt_pinh = 0;

            double v1 = consts.C[1 - 1, 1 - 1] * state.IrsMtorcCellState[3 - 1];
            double v2 = consts.C[2 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1];
            double v3 = consts.C[3 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1] - consts.C[3 - 1, 2 - 1] * state.IrsMtorcCellState[5 - 1];
            double v4 = consts.C[4 - 1, 1 - 1] * state.IrsMtorcCellState[4 - 1] * state.IrsMtorcCellState[1 - 1] - consts.C[4 - 1, 2 - 1] * state.IrsMtorcCellState[6 - 1];
            double v5 = consts.C[5 - 1, 1 - 1] * state.IrsMtorcCellState[6 - 1] * 5 - consts.C[3 - 1, 2 - 1] * state.IrsMtorcCellState[5 - 1];
            double v6 = consts.C[6 - 1, 1 - 1] * state.IrsMtorcCellState[8 - 1] * (state.IrsMtorcCellState[6 - 1] + state.IrsMtorcCellState[4 - 1]) / (state.IrsMtorcCellState[2 - 1] + 0.01) - consts.C[6 - 1, 2 - 1] * state.IrsMtorcCellState[9 - 1];
            v6 *= settings.v6Coefficient;
            
            double v7 = consts.C[7 - 1, 1 - 1] * (state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[10 - 1] - consts.C[7 - 1, 2 - 1] * state.IrsMtorcCellState[11 - 1]);
            v7 *= settings.v7Coefficient;

            double v8 = consts.C[8 - 1, 1 - 1] * state.IrsMtorcCellState[11 - 1] - consts.C[8 - 1, 2 - 1] * state.IrsMtorcCellState[12 - 1];
            v8 *= settings.v8Coefficient;
            double v9 = consts.C[9 - 1, 1 - 1] * state.IrsMtorcCellState[12 - 1] - consts.C[9 - 1, 2 - 1] * state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[13 - 1];
            v9 *= settings.v9Coefficient;
            double v10 = consts.C[10 - 1, 1 - 1] * state.IrsMtorcCellState[13 - 1];
            double v11 = consts.C[11 - 1, 1 - 1] * (state.IrsMtorcCellState[17 - 1] * state.IrsMtorcCellState[13 - 1] - consts.C[11 - 1, 2 - 1] * state.IrsMtorcCellState[14 - 1]);
            double v12 = consts.C[12 - 1, 1 - 1] * state.IrsMtorcCellState[14 - 1];
            double v13 = consts.C[13 - 1, 1 - 1] * state.IrsMtorcCellState[15 - 1];
            double v14 = consts.C[14 - 1, 1 - 1] * (state.IrsMtorcCellState[16 - 1] * state.IrsMtorcCellState[20 - 1] - consts.C[14 - 1, 2 - 1] * state.IrsMtorcCellState[18 - 1]);
            double v15 = consts.C[15 - 1, 1 - 1] * state.IrsMtorcCellState[18 - 1];
            double v16 = consts.C[16 - 1, 1 - 1] * state.IrsMtorcCellState[19 - 1];
            double v17 = consts.C[17 - 1, 1 - 1] * state.IrsMtorcCellState[20 - 1] / (consts.C[17 - 1, 2 - 1] + state.IrsMtorcCellState[20 - 1]);
            double v18 = consts.C[18 - 1, 1 - 1] * (state.IrsMtorcCellState[20 - 1] * state.IrsMtorcCellState[23 - 1] - consts.C[18 - 1, 2 - 1] * state.IrsMtorcCellState[21 - 1]);
            double v19 = consts.C[19 - 1, 1 - 1] * state.IrsMtorcCellState[21 - 1];
            double v20 = consts.C[20 - 1, 1 - 1] * state.IrsMtorcCellState[22 - 1];
            double v21 = consts.C[21 - 1, 1 - 1] * (state.IrsMtorcCellState[16 - 1] * state.IrsMtorcCellState[24 - 1] - consts.C[21 - 1, 2 - 1] * state.IrsMtorcCellState[25 - 1]);
            double v22 = consts.C[22 - 1, 1 - 1] * state.IrsMtorcCellState[25 - 1] / (consts.C[22 - 1, 2 - 1] + state.IrsMtorcCellState[25 - 1]);
            double v23 = (consts.C[56 - 1, 1 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[58 - 1] + consts.C[56 - 1, 2 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[60 - 1]);
            double v24 = consts.C[24 - 1, 1 - 1] * (state.IrsMtorcCellState[35 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[24 - 1, 2 - 1] * state.IrsMtorcCellState[29 - 1]);
            double v25 = consts.C[25 - 1, 1 - 1] * state.IrsMtorcCellState[29 - 1];
            double v26 = consts.C[26 - 1, 1 - 1] * state.IrsMtorcCellState[26 - 1] * state.IrsMtorcCellState[30 - 1];
            double v27 = consts.C[27 - 1, 1 - 1] * state.IrsMtorcCellState[28 - 1];
            double v28 = consts.C[28 - 1, 1 - 1] * state.IrsMtorcCellState[28 - 1];
            double v29 = consts.C[29 - 1, 1 - 1] * state.IrsMtorcCellState[27 - 1];
            double v30 = consts.C[30 - 1, 1 - 1] * state.IrsMtorcCellState[35 - 1];
            double v31 = consts.C[31 - 1, 1 - 1] * (state.IrsMtorcCellState[34 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[31 - 1, 2 - 1] * state.IrsMtorcCellState[33 - 1]);
            double v32 = consts.C[32 - 1, 1 - 1] * state.IrsMtorcCellState[33 - 1];
            double v33 = consts.C[33 - 1, 1 - 1] * state.IrsMtorcCellState[32 - 1];
            double v34 = consts.C[34 - 1, 1 - 1] * state.IrsMtorcCellState[31 - 1];
            double v35 = consts.C[35 - 1, 1 - 1] * state.IrsMtorcCellState[37 - 1] / (consts.C[35 - 1, 2 - 1] + state.IrsMtorcCellState[37 - 1]) * 0.125;
            double v36 = consts.C[36 - 1, 1 - 1] * (state.IrsMtorcCellState[38 - 1] * state.IrsMtorcCellState[30 - 1] - consts.C[36 - 1, 2 - 1] * state.IrsMtorcCellState[39 - 1]) * 1;
            double v37 = consts.C[37 - 1, 1 - 1] * state.IrsMtorcCellState[39 - 1];
            double v38 = consts.C[38 - 1, 1 - 1] * state.IrsMtorcCellState[40 - 1] * 0.025;
            double v39 = (consts.C[39 - 1, 1 - 1] + consts.C[39 - 1, 2 - 1]) / (consts.C[39 - 1, 3 - 1] / consts.C[39 - 1, 4 - 1]) * state.IrsMtorcCellState[41 - 1] * state.IrsMtorcCellState[37 - 1] * c / 10 * 0.125;
            double v40 = consts.C[40 - 1, 1 - 1] * state.IrsMtorcCellState[43 - 1] / 10;
            double v42 = (consts.C[42 - 1, 1 - 1] + consts.C[42 - 1, 2 - 1]) / (consts.C[42 - 1, 3 - 1] / consts.C[42 - 1, 4 - 1]) * state.IrsMtorcCellState[42 - 1] * state.IrsMtorcCellState[44 - 1] / 10 * 1;
            double v43 = consts.C[43 - 1, 1 - 1] * state.IrsMtorcCellState[45 - 1] / 10;
            double v44 = consts.C[44 - 1, 1 - 1] * state.IrsMtorcCellState[45 - 1] / 10;
            double v45 = consts.C[45 - 1, 1 - 1] * state.IrsMtorcCellState[2 - 1] * state.IrsMtorcCellState[1 - 1] - consts.C[45 - 1, 2 - 1] * state.IrsMtorcCellState[3 - 1];
            double v46 = state.IrsMtorcCellState[41 - 1] * 370000 * consts.C[46 - 1, 1 - 1] / 10;
            double v47 = state.IrsMtorcCellState[47 - 1] * consts.C[47 - 1, 1 - 1] / 10 * state.IrsMtorcCellState[37 - 1] * c / 112e5;
            double v48 = consts.C[48 - 1, 1 - 1] * state.IrsMtorcCellState[48 - 1] * state.IrsMtorcCellState[49 - 1];
            double v49 = consts.C[48 - 1, 2 - 1] * state.IrsMtorcCellState[50 - 1] * state.IrsMtorcCellState[55 - 1];
            double v50 = consts.C[52 - 1, 1 - 1] * state.IrsMtorcCellState[51 - 1] * state.IrsMtorcCellState[50 - 1];// * 0; //POKAZAĆ!
            v50 *= settings.v50Coefficient;

            double v51 = consts.C[52 - 1, 2 - 1] * state.IrsMtorcCellState[52 - 1];
            v51 *= settings.v51Coefficient;

            double v52 = consts.C[55 - 1, 2 - 1] * state.IrsMtorcCellState[56 - 1];
            double v53 = consts.C[53 - 1, 1 - 1] * state.IrsMtorcCellState[9 - 1] * state.IrsMtorcCellState[53 - 1];
            double v54 = consts.C[54 - 1, 1 - 1] * state.IrsMtorcCellState[54 - 1];
            double v55 = consts.C[55 - 1, 1 - 1] * state.IrsMtorcCellState[55 - 1] * state.IrsMtorcCellState[54 - 1];
            double v56 = consts.C[56 - 1, 3 - 1] * state.IrsMtorcCellState[35 - 1];
            double v57 = consts.C[57 - 1, 1 - 1] * state.IrsMtorcCellState[58 - 1];
            double v58 = consts.C[58 - 1, 1 - 1] * state.IrsMtorcCellState[13 - 1] * state.IrsMtorcCellState[57 - 1] * 100;
            double v59 = consts.C[59 - 1, 1 - 1] * state.IrsMtorcCellState[60 - 1];
            
            

            double v60 = consts.C[60 - 1, 1 - 1] * state.IrsMtorcCellState[6 - 1] * 100 * state.IrsMtorcCellState[59 - 1];
            double v61 = consts.C[51 - 1, 1 - 1] * state.IrsMtorcCellState[52 - 1] * state.IrsMtorcCellState[8 - 1] / 1000;
            v61 *= settings.v61Coefficient;

            double v64 = consts.C[51 - 1, 2 - 1] * state.IrsMtorcCellState[52 - 1] * state.IrsMtorcCellState[8 - 1] * 14;
            v64 *= settings.v64Coefficient;

            var q6 = 1.94e-3;
            double gapdh_active = consts.GAPDH_mTORC1_Complex_Consts[3] * state.GapdhActiveCoefficient;
            double V62k1 = consts.GAPDH_mTORC1_Complex_Consts[0] / (q6 + consts.GAPDH_mTORC1_Complex_Consts[1]);
            double V62k2 = consts.GAPDH_mTORC1_Complex_Consts[2];
            double v62 = ((V62k1 * gapdh_active * state.IrsMtorcCellState[50 - 1]) - (V62k2 * state.IrsMtorcCellState[62 - 1])) / 10;
            double v63 = ((V62k2 * state.IrsMtorcCellState[62 - 1]) - V62k1 * (gapdh_active * state.IrsMtorcCellState[50 - 1])) / 1000;

            v62 *= settings.v62Coefficient;
            v63 *= settings.v63Coefficient;

            
            return new double[]
            {
                v1 ,
                v2 ,
                v3 ,
                v4 ,
                v5 ,
                v6 ,
                v7 ,
                v8 ,
                v9 ,
                v10,
                v11,
                v12,
                v13,
                v14,
                v15,
                v16,
                v17,
                v18,
                v19,
                v20,
                v21,
                v22,
                v23,
                v24,
                v25,
                v26,
                v27,
                v28,
                v29,
                v30,
                v31,
                v32,
                v33,
                v34,
                v35,
                v36,
                v37,
                v38,
                v39,
                v40,
                v42,
                v43,
                v44,
                v45,
                v46,
                v47,
                v48,
                v49,
                v50,
                v51,
                v52,
                v53,
                v54,
                v55,
                v56,
                v57,
                v58,
                v59,
                v60,
                v61,
                v62,
                v63,
                v64
            };
        }

        public List<IrsMtorcCellStateWrapper> SimulateIrsMtorcGlycolysis(
            IrsMtorcCellStateWrapper start,
            IrsMtorcConstants consts,
            int timeSteps,
            double noiseAmplitude = 0,
            int savePerTimeSteps = 1,
            int reapplyNoiseAfterTimeSamples = 1,
            int timeStepsToTurnOnInsulin = 300000,
            bool keep_insulin_level = true,
            bool randomize_insulin_level = true,
            int? timeToTurnOffInsulin = null,
            AdditionalTimeStepComputationSettings settings = null)
        {
            int saveSycles = timeSteps / savePerTimeSteps;
            var s = start.Copy();
            this.original_consts = consts;
            this.consts = this.original_consts.ApplyNoise(noiseAmplitude);

            List<IrsMtorcCellStateWrapper> states = new List<IrsMtorcCellStateWrapper>();
            var r = new RandomGenerator();

            for (int i = 0; i < saveSycles; i++)
            {
                for (int j = 0; j < savePerTimeSteps; j++)
                {
                    if (i * savePerTimeSteps + j == timeStepsToTurnOnInsulin)
                        s.TurnOnInsulin(randomize_insulin_level);

                    if (timeToTurnOffInsulin.HasValue && i * savePerTimeSteps + j == timeToTurnOffInsulin)
                        s.TurnOffInsulin();

                    if (i * j % reapplyNoiseAfterTimeSamples == 0)
                        this.consts = original_consts.ApplyNoise(noiseAmplitude);

                    ComputeOneTimestep(s, keep_insulin_level, settings: settings);
                }
                states.Add(s);
                s = s.Copy();
            }

            return states;
        }
    }
}
