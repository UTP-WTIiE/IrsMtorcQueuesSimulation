using mTORC.Models;
using System;

namespace IrsMtorcQueuesSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // amount of time steps performed in the simulation
            int timeSteps = 2100000;

            // how many cells should be simulated (50 means that experiment will be conducted 50 times)
            // code is written in the way to perform experiments in parallel to each other using
            // multiprocessing capabilities of the CPU
            int cells = 50;

            // noise amplitude of 0.1 is equivalent of 10% gaussian noise 
            double noiseAmplitude = 0.1;

            // activeGapdhCoefficient of 0.5 means that simulation will act as only 50%
            // of GAPDH is available in the experiment. Modify this value in order to
            // experiment with different availabilities of the GAPDH in the cells
            double activeGapdhCoefficient = 1.0;

            // path to folder where code saves simulation results
            string outputFolder = @"D:\mTORC\TestyCzystegoRepozytorium";

            // after how many steps should value be saved. This parameter does not affect
            // simulation, but it influences process of saving the results. The value of 100
            // means that the csv files from simulations have results from 0-th time step,
            // 100-th time step, 200-th time step and so on. This value is important for 
            // huge experiments like 2100000 time steps of 50 cells due to RAM shortage and
            // hard drive saving time
            int savePerTimeSteps = 100;

            // after how many time steps should insulin level be increased from 0 to its
            // default value
            int timeStepsToTurnOnInsulin = 300000;

            // after how many time steps should insulin level be reduced to 0.
            // null means that insulin will not be reduced to zero
            int? timeToTurnOffInsulin = null;

            // should maximum insulin level be randomized
            bool randomizeInsulinLevel = false;

            // custom name of the experiment used during results saving on the hard drive
            string customExperimentName = "first_experiment";

            // values of substrates inside the cell(s) at the start of simulation
            var startCell = new IrsMtorcCellStateWrapper(noiseAmplitude);

            // kinetic costnats used for queues computation
            var constants = new IrsMtorcConstants();

            // additional settings for controlling the experiment. Allows for certain tweaks
            // like increasing/decreasing certain reaction rates. If no value is changed, then
            // no additional conditions will be aplied
            var additionalSettings = new AdditionalTimeStepComputationSettings(activeGapdhCoefficient);

            // perform the simulation
            IrsMtorcSimulation.SimulateLiteratureIrsMtorcGlycolysis(
                outputFolder,
                startCell,
                constants,
                timeSteps,
                noiseAmplitude,
                cells,
                savePerTimeSteps,
                timeStepsToTurnOnInsulin,
                activeGapdhCoefficient,
                randomizeInsulinLevel,
                timeToTurnOffInsulin,
                additionalSettings,
                customExperimentName
            );
        }
    }
}
