using System;
using System.Collections.Generic;
using System.Text;

namespace mTORC.Models
{
    public class AdditionalTimeStepComputationSettings
    {
        private double gapdh_value;

        public bool IsGapdhZero => gapdh_value == 0;

        public double? forcedPTENState { get; set; } = null;
        public double? MaximumPPAktValueIfGAPDHisZero { get; set; } = null;
        public double? MinimumAs160ValueIfGAPDHisZero { get; set; } = null;
        public double? forcedPIR_IIState { get; set; } = null;
        public double? forcedIRS_1_3_State { get; set; } = null;
        public double? forceI11WhenGAPDHIsZero { get; set; } = null;
        public bool runIRS1_pS636_PI3KCommentedLines { get; set; } = false;
        public double v6Coefficient { get; set; } = 1.0;
        public double v7Coefficient { get; set; } = 1.0;
        public double v8Coefficient { get; set; } = 1.0;
        public double v9Coefficient { get; set; } = 1.0;
        public double v64Coefficient { get; set; } = 1.0;
        public double v50Coefficient { get; set; } = 1.0;
        public double v51Coefficient { get; set; } = 1.0;
        public double v62Coefficient { get; set; } = 1.0;
        public double v63Coefficient { get; set; } = 1.0;
        public double v61Coefficient { get; set; } = 1.0;
        public double StartMtorcI49Coefficient { get; set; } = 1.0;

        public AdditionalTimeStepComputationSettings(double gapdh_value)
        {
            this.gapdh_value = gapdh_value;
        }

        public string Describe()
        {
            string description = "";

            string describe(double value, string name)
            {
                if (value != 1.0)
                    return $"{description}_{name}_{value}";
                return description;
            }

            if(forcedIRS_1_3_State != null)
            {
                description = $"{description}_forced_IRS_1_3_{forcedIRS_1_3_State.Value}";
            }
            if(forcedPTENState != null)
            {
                description = $"{description}_forced_pten_{forcedPTENState}";
            }
            if (forcedPIR_IIState.HasValue)
            {
                description = $"{description}_forced_pIR_II_{forcedPIR_IIState.Value}";
            }

            if (runIRS1_pS636_PI3KCommentedLines)
                description = $"{description}_IRS1_pS636_PI3K_uncommented";

            if (v6Coefficient != 1.0)
                description = $"{description}_v6_coeff_{v6Coefficient}";

            if (v64Coefficient != 1.0)
                description = $"{description}_v64_coeff_{v64Coefficient}";

            if (v50Coefficient != 1.0)
                description = $"{description}_v50_coeff_{v50Coefficient}";

            if (v51Coefficient != 1.0)
                description = $"{description}_v51_coeff_{v51Coefficient}";

            description = describe(v62Coefficient, "v62");
            description = describe(v63Coefficient, "v63");
            description = describe(StartMtorcI49Coefficient, "start_I49");
            description = describe(v7Coefficient, "v7");
            description = describe(v8Coefficient, "v8");
            description = describe(v9Coefficient, "v9");
            description = describe(v61Coefficient, "v61");

            if (IsGapdhZero)
            {
                if(MaximumPPAktValueIfGAPDHisZero != null)
                {
                    description = $"{description}_max_ppakt_if_gapdh_zero_is_{MaximumPPAktValueIfGAPDHisZero}";
                }
                if(MinimumAs160ValueIfGAPDHisZero != null)
                {
                    description = $"{description}_min_as160_if_gapdh_zero_is_{MinimumAs160ValueIfGAPDHisZero}";
                }
                if(forceI11WhenGAPDHIsZero != null)
                {
                    description = $"{description}_forced_I11_{forceI11WhenGAPDHIsZero.Value}";
                }
            }

            return description;
        }

        public IrsMtorcCellStateWrapper ParseStart(IrsMtorcCellStateWrapper start)
        {
            var a = start.Copy();
            a.IrsMtorcCellState[49 - 1] *= StartMtorcI49Coefficient;

            return a;
        }
    }
}
