using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using ReGen.Model;
using ScottPlot;

namespace ReGen.Generators
{
    public static class PlotGenerator
    {

        /// <summary>
        /// Generates a plot as a PNG byte array for insertion into PDFs.
        /// </summary>
        public static void GeneratePlot(List<CsvRecord> csvRecords, string filePath = "C:\\Users\\marek.beran\\Desktop\\Chartts\\chart.png")
        {

            if (csvRecords.Count == 0)
                throw new InvalidOperationException("CSV file contains no data.");

            // Parse StepTime to seconds
            var xs = csvRecords
                .Select(r => r.StepTime.TotalSeconds)
                .ToArray();

            // Parse voltage (remove '+' sign if present)
            var ys = csvRecords
                .Select(r => r.V)
                .ToArray();

            // Create ScottPlot 5 plot
            var plt = new Plot();
            plt.Add.ScatterLine(xs, ys, color: Colors.Blue);
            plt.XLabel("Time (seconds)");
            plt.YLabel("Voltage (V)");

            // Save to byte[]
            // using var ms = new MemoryStream();
            plt.SavePng(filePath, 800, 800);
            // plt.SavePng(ms, 800, 600);
            // return ms.ToArray();
        }
    }
}