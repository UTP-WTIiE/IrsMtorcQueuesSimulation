using mTORC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace mTORC.Functions
{
    public static class Save
    {
        
        public static void SaveIrsMtorcGlycolysisResults(List<IrsMtorcCellStateWrapper> states, string destination_folder_path, string file_name, IrsMtorcConstants weights_used, AdditionalTimeStepComputationSettings settings)
        {
            var rate_names = new List<string>() { 
                "v1 ",
                "v2 ",
                "v3 ",
                "v4 ",
                "v5 ",
                "v6 ",
                "v7 ",
                "v8 ",
                "v9 ",
                "v10",
                "v11",
                "v12",
                "v13",
                "v14",
                "v15",
                "v16",
                "v17",
                "v18",
                "v19",
                "v20",
                "v21",
                "v22",
                "v23",
                "v24",
                "v25",
                "v26",
                "v27",
                "v28",
                "v29",
                "v30",
                "v31",
                "v32",
                "v33",
                "v34",
                "v35",
                "v36",
                "v37",
                "v38",
                "v39",
                "v40",
                "v42",
                "v43",
                "v44",
                "v45",
                "v46",
                "v47",
                "v48",
                "v49",
                "v50",
                "v51",
                "v52",
                "v53",
                "v54",
                "v55",
                "v56",
                "v57",
                "v58",
                "v59",
                "v60",
                "v61",
                "v62",
                "v63",
                "v64",
        };

            var simulation = new IrsMtorcComputation(weights_used, states[0]);
            string header = states[0].SubstrateNames.Concat(rate_names).Aggregate((a, b) => $"{a};{b}");

            var data = states
                .Select(x => {
                    string s = x.GetState().Take(62).Concat(simulation.ComputeRates(x, settings)).Select(a => a.ToString()).Aggregate((a, b) => $"{a};{b}");
                    s = s.Replace(',', '.');
                    return s;
                })
                .ToList();

            var to_save = new List<string>();
            to_save.Add(header);
            to_save.AddRange(data);

            string path = Path.Combine(destination_folder_path, $"{file_name}.csv");

            File.WriteAllLines(path, to_save);
        }
    }
}
