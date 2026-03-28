using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Docs.Examples.Showcase;

namespace Spectre.Docs.Examples.SpectreConsole.Tutorials;

/// <summary>
/// A tutorial that builds a custom Pill widget implementing IRenderable.
/// Demonstrates the rendering model, measurements, segments, and capability detection.
/// </summary>
public class CreatingCustomRenderablesTutorial : BaseSample
{
    /// <inheritdoc />
    public override void Run(IAnsiConsole console)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Status")
            .AddColumn("Message");

        table.AddRow(new Pill("Success", PillType.Success), new Text("All systems operational"));
        table.AddRow(new Pill("Warning", PillType.Warning), new Text("High memory usage detected"));
        table.AddRow(new Pill("Error", PillType.Error), new Text("Database connection failed"));
        table.AddRow(new Pill("Info", PillType.Info), new Text("Scheduled maintenance at 2:00 AM"));

        AnsiConsole.Write(table);  // minimal error on this line
    }

    /// <summary>Creates a basic Pill class that implements IRenderable.</summary>
    public static void CreatePillClass()
    {
        // The Pill class implements IRenderable, which requires two methods:
        // - Measure(): Reports width constraints
        // - Render(): Produces styled output segments
    }

    /// <summary>Implements Measure to calculate the pill's width.</summary>
    public static void ImplementMeasure()
    {
        // Measure returns a Measurement with minimum and maximum width.
        // Our pill width = text length + 2 padding spaces + 2 cap characters
        var text = "Success";
        var width = text.Length + 4;
        var measurement = new Measurement(width, width);
        AnsiConsole.WriteLine($"Pill width for '{text}': {measurement.Max} cells");
    }

    /// <summary>Implements Render to yield styled segments.</summary>
    public static void ImplementRender()
    {
        // Render yields Segment objects containing text and style.
        // Each segment is an atomic unit of styled output.
        var pill = new Pill("Test", PillType.Info);
        AnsiConsole.Write(pill);
        AnsiConsole.WriteLine();
    }

    /// <summary>Creates pills with different type variants.</summary>
    public static void CreateColoredPills()
    {
        var success = new Pill("Success", PillType.Success);
        var warning = new Pill("Warning", PillType.Warning);
        var error = new Pill("Error", PillType.Error);

        AnsiConsole.Write(success);
        AnsiConsole.Write(" ");
        AnsiConsole.Write(warning);
        AnsiConsole.Write(" ");
        AnsiConsole.Write(error);
        AnsiConsole.WriteLine();
    }
}

/// <summary>
/// Defines the visual style variants for a Pill widget.
/// </summary>
public enum PillType
{
    /// <summary>Green pill for success states.</summary>
    Success,
    /// <summary>Yellow pill for warning states.</summary>
    Warning,
    /// <summary>Red pill for error states.</summary>
    Error,
    /// <summary>Blue pill for informational states.</summary>
    Info,
}

/// <summary>
/// A custom renderable that displays text as a colored pill with rounded end caps.
/// </summary>
public sealed class Pill : IRenderable
{
    private readonly string _text;
    private readonly Style _style;

    /// <summary>
    /// Creates a new pill with the specified text and type.
    /// </summary>
    /// <param name="text">The text to display inside the pill.</param>
    /// <param name="type">The pill type which determines its color scheme.</param>
    public Pill(string text, PillType type)
    {
        _text = text;
        _style = GetStyleForType(type);
    }

    private static Style GetStyleForType(PillType type) => type switch
    {
        PillType.Success => new Style(Color.White, Color.Green),
        PillType.Warning => new Style(Color.Black, Color.Yellow),
        PillType.Error => new Style(Color.White, Color.Red),
        PillType.Info => new Style(Color.White, Color.Blue),
        _ => new Style(Color.White, Color.Grey),
    };

    /// <summary>
    /// Measures the pill's width in console cells.
    /// </summary>
    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        // Width = text + 2 padding spaces + 2 cap characters
        var width = _text.Length + 4;
        return new Measurement(width, width);
    }

    /// <summary>
    /// Renders the pill as a sequence of styled segments.
    /// </summary>
    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        // Use rounded half-circles if Unicode is supported, otherwise spaces
        const string LeftCap = "\uE0B6";
        const string RightCap = "\uE0B4";

        var inverseStyle = new Style(_style.Background);

        if (options.Capabilities.Unicode)
        {
            yield return new Segment(LeftCap, inverseStyle);
            yield return new Segment($" {_text} ", _style);
            yield return new Segment(RightCap, inverseStyle);
        }
        else
        {
            yield return new Segment($"  {_text}  ", _style);
        }

    }
}
