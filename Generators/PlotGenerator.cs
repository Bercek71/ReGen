using ReGen.Model;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace ReGen.Generators;

public static class PlotGenerator
{
    /// <summary>
    ///     Generates a plot as a PNG byte array for insertion into PDFs.
    /// </summary>
    public static void GeneratePlot(List<CsvRecord> csvRecords,
        string filePath = "C:\\Users\\marek.beran\\Desktop\\Chartts\\chart.png")
    {
        if (csvRecords.Count == 0)
            throw new InvalidOperationException("CSV file contains no data.");

        // Parse StepTime to seconds
        var xs = csvRecords
            .Select(r => r.StepTime.TotalSeconds)
            .ToArray();

        // Přeškálování xs tak, aby začínalo od 0
        var minXs = xs.Min();
        var xsRescaled = xs.Select(x => x - minXs).ToArray();

        // Parse voltage (remove '+' sign if present)
        var ys = csvRecords
            .Select(r => r.V)
            .ToArray();

        // Create ScottPlot 5 plot
        var plt = new Plot();
        var scatter = plt.Add.ScatterLine(xsRescaled, ys, Colors.Black);
        scatter.LineWidth = 5;
        plt.XLabel("Čas");

// Vytvoření ticků s rozumným intervalem
        var maxTime = xsRescaled.Max();
        var tickPositions = new List<double>();
        var tickLabels = new List<string>();

// Automatické určení intervalu podle délky dat
        var interval = maxTime switch
        {
            <= 60 => 10,      // Do 1 minuty: každých 10 sekund
            <= 300 => 30,     // Do 5 minut: každých 30 sekund
            <= 600 => 60,     // Do 10 minut: každou minutu
            <= 1800 => 180,   // Do 30 minut: každé 3 minuty
            _ => 300          // Více než 30 minut: každých 5 minut
        };

        for (double t = 0; t <= maxTime; t += interval)
        {
            tickPositions.Add(t);
            var ts = TimeSpan.FromSeconds((int)t);
            tickLabels.Add($"{ts.Minutes}:{ts.Seconds:D2}");
        }

        plt.Axes.Bottom.TickGenerator = new NumericManual(tickPositions.ToArray(), tickLabels.ToArray());

        plt.YLabel("Napětí (V)");
        plt.SavePng(filePath, 800, 800);
    }
}