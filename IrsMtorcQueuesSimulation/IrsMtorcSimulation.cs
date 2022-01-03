using mTORC.Functions;
using mTORC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IrsMtorcQueuesSimulation
{
    public static class IrsMtorcSimulation
    {
        public static List<List<IrsMtorcCellStateWrapper>> SimulateLiteratureIrsMtorcGlycolysis(string outputFolderPath, IrsMtorcCellStateWrapper start_state, IrsMtorcConstants consts, int timeSteps, double noiseAmplitude, int cells = 1, int savePerTimeSteps = 1, int timeStepsToTurnOnInsulin = 300000,
            double gapdh_active_coeff = 1,
            bool randomize_insulin_level = true, int? timeToTurnOffInsulin = null, AdditionalTimeStepComputationSettings settings = null, string custom_name = null)
        {
            string settings_description = settings != null ? settings.Describe() : "";
            string destinationPath = Path.Combine(outputFolderPath, "results", $"{custom_name}_{settings_description}_GAPDH_{gapdh_active_coeff}_{DateTime.Now.Ticks}");

            if (Directory.Exists(destinationPath) == false)
                Directory.CreateDirectory(destinationPath);

            List<List<IrsMtorcCellStateWrapper>> results = new List<List<IrsMtorcCellStateWrapper>>();
            Enumerable.Range(0, cells)
                .AsParallel()
                .ForAll(x => {
                    var start = start_state == null ? new IrsMtorcCellStateWrapper(noiseAmplitude) : start_state.Copy();
                    start.GapdhActiveCoefficient = gapdh_active_coeff;

                    start = settings.ParseStart(start);

                    var computation = new IrsMtorcComputation(consts, start);

                    var result = computation.SimulateIrsMtorcGlycolysis(start,
                        consts,
                        timeSteps,
                        noiseAmplitude: noiseAmplitude,
                        savePerTimeSteps: savePerTimeSteps,
                        timeStepsToTurnOnInsulin: timeStepsToTurnOnInsulin,
                        timeToTurnOffInsulin: timeToTurnOffInsulin,
                        randomize_insulin_level: randomize_insulin_level,
                        settings: settings);

                    results.Add(result);
                    Save.SaveIrsMtorcGlycolysisResults(result, destinationPath, x.ToString(), consts, settings);
                });

            return results;
        }
    }
}
