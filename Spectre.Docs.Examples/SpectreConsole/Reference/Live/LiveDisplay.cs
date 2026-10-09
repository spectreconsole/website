using Spectre.Console;

namespace Spectre.Docs.Examples.SpectreConsole.Reference.Live;

internal static class LiveDisplayExamples
{
    /// <summary>
    /// Demonstrates creating a basic live display that updates a simple table.
    /// </summary>
    public static void BasicLiveDisplayExample()
    {
        var table = new Table();
        table.AddColumn("Status");
        table.AddRow("Starting...");

        AnsiConsole.Live(table)
            .Start(ctx =>
            {
                Thread.Sleep(1000);
                table.AddRow("Processing...");
                ctx.Refresh();

                Thread.Sleep(1000);
                table.AddRow("Complete!");
                ctx.Refresh();

                Thread.Sleep(1000);
            });
    }

    /// <summary>
    /// Demonstrates building a table incrementally with rows added over time.
    /// </summary>
    public static void LiveDisplayWithTableExample()
    {
        var table = new Table()
            .AddColumn("Server")
            .AddColumn("Status")
            .AddColumn("Uptime");

        AnsiConsole.Live(table)
            .Start(ctx =>
            {
                var servers = new[]
                {
                    ("web-01", "[green]Online[/]", "99.9%"),
                    ("web-02", "[green]Online[/]", "99.8%"),
                    ("db-01", "[green]Online[/]", "100%"),
                    ("cache-01", "[yellow]Degraded[/]", "95.2%"),
                    ("api-01", "[green]Online[/]", "99.7%")
                };

                foreach (var (server, status, uptime) in servers)
                {
                    table.AddRow(server, status, uptime);
                    ctx.Refresh();
                    Thread.Sleep(500);
                }
            });
    }

    /// <summary>
    /// Demonstrates using UpdateTarget to completely replace the displayed renderable.
    /// </summary>
    public static void LiveDisplayUpdateTargetExample()
    {
        AnsiConsole.Live(new Text("Initializing..."))
            .Start(ctx =>
            {
                Thread.Sleep(1000);

                // Replace with a panel
                var panel = new Panel("Loading configuration...")
                    .Header("Step 1")
                    .BorderColor(Color.Blue);
                ctx.UpdateTarget(panel);
                Thread.Sleep(1000);

                // Replace with a different panel
                var panel2 = new Panel("Connecting to database...")
                    .Header("Step 2")
                    .BorderColor(Color.Yellow);
                ctx.UpdateTarget(panel2);
                Thread.Sleep(1000);

                // Replace with final panel
                var panel3 = new Panel("[green]Ready![/]")
                    .Header("Complete")
                    .BorderColor(Color.Green);
                ctx.UpdateTarget(panel3);
                Thread.Sleep(1000);
            });
    }

    /// <summary>
    /// Demonstrates displaying live status information inside a panel.
    /// </summary>
    public static void LiveDisplayWithPanelExample()
    {
        var table = new Table()
            .Border(TableBorder.None)
            .AddColumn("Metric")
            .AddColumn("Value");

        var panel = new Panel(table)
            .Header("System Monitor")
            .BorderColor(Color.Cyan)
            .RoundedBorder();

        AnsiConsole.Live(panel)
            .Start(ctx =>
            {
                for (int i = 0; i < 10; i++)
                {
                    table.Rows.Clear();
                    table.AddRow("CPU Usage", $"{Random.Shared.Next(10, 80)}%");
                    table.AddRow("Memory", $"{Random.Shared.Next(2, 8)} GB / 16 GB");
                    table.AddRow("Network", $"{Random.Shared.Next(100, 999)} MB/s");
                    table.AddRow("Uptime", $"{i + 1} seconds");

                    ctx.Refresh();
                    Thread.Sleep(1000);
                }
            });
    }

    /// <summary>
    /// Demonstrates using AutoClear to remove the display when complete.
    /// </summary>
    public static void LiveDisplayAutoClearExample()
    {
        var spinner = new Table()
            .AddColumn("Status")
            .AddRow("[blue]Processing...[/]");

        AnsiConsole.Live(spinner)
            .AutoClear(true)
            .Start(ctx =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    spinner.Rows.Clear();
                    spinner.AddRow($"[blue]Processing step {i}/5...[/]");
                    ctx.Refresh();
                    Thread.Sleep(800);
                }
            });

        AnsiConsole.WriteLine("Display cleared - task complete!");
    }

    /// <summary>
    /// Demonstrates handling overflow with ellipsis when content exceeds console height.
    /// </summary>
    public static void LiveDisplayOverflowEllipsisExample()
    {
        var table = new Table()
            .AddColumn("Line");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Ellipsis)
            .Start(ctx =>
            {
                for (int i = 1; i <= 100; i++)
                {
                    table.AddRow($"Line {i}");
                    ctx.Refresh();
                    Thread.Sleep(50);
                }
            });
    }

    /// <summary>
    /// Demonstrates handling overflow with cropping when content exceeds console height.
    /// </summary>
    public static void LiveDisplayOverflowCropExample()
    {
        var table = new Table()
            .AddColumn("Line");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Crop)
            .Start(ctx =>
            {
                for (int i = 1; i <= 100; i++)
                {
                    table.AddRow($"Line {i}");
                    ctx.Refresh();
                    Thread.Sleep(50);
                }
            });
    }

    /// <summary>
    /// Demonstrates handling overflow with visible mode allowing content to scroll.
    /// </summary>
    public static void LiveDisplayOverflowVisibleExample()
    {
        var table = new Table()
            .AddColumn("Line");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Visible)
            .Start(ctx =>
            {
                for (int i = 1; i <= 30; i++)
                {
                    table.AddRow($"Line {i}");
                    ctx.Refresh();
                    Thread.Sleep(100);
                }
            });
    }

    /// <summary>
    /// Demonstrates cropping content from the top when exceeding console height.
    /// </summary>
    public static void LiveDisplayCroppingTopExample()
    {
        var table = new Table()
            .AddColumn("Log Entry");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Crop)
            .Cropping(VerticalOverflowCropping.Top)
            .Start(ctx =>
            {
                for (int i = 1; i <= 50; i++)
                {
                    table.AddRow($"[dim]{DateTime.Now:HH:mm:ss}[/] Log entry {i}");
                    ctx.Refresh();
                    Thread.Sleep(100);
                }
            });
    }

    /// <summary>
    /// Demonstrates cropping content from the bottom when exceeding console height.
    /// </summary>
    public static void LiveDisplayCroppingBottomExample()
    {
        var table = new Table()
            .AddColumn("Task Queue");

        AnsiConsole.Live(table)
            .Overflow(VerticalOverflow.Crop)
            .Cropping(VerticalOverflowCropping.Bottom)
            .Start(ctx =>
            {
                for (int i = 1; i <= 50; i++)
                {
                    table.AddRow($"Task {i}: Processing");
                    ctx.Refresh();
                    Thread.Sleep(100);
                }
            });
    }

    /// <summary>
    /// Demonstrates using the async StartAsync pattern for asynchronous operations.
    /// </summary>
    public static async Task LiveDisplayAsyncExample()
    {
        var status = new Table()
            .AddColumn("Operation")
            .AddColumn("Status");

        await AnsiConsole.Live(status)
            .StartAsync(async ctx =>
            {
                var operations = new[]
                {
                    "Fetching data",
                    "Processing records",
                    "Updating database",
                    "Generating report",
                    "Sending notifications"
                };

                foreach (var operation in operations)
                {
                    status.AddRow(operation, "[yellow]In Progress[/]");
                    ctx.Refresh();

                    await Task.Delay(1000);

                    status.Rows.Update(status.Rows.Count - 1, 1, new Markup("[green]Complete[/]"));
                    ctx.Refresh();
                }
            });
    }

    /// <summary>
    /// Demonstrates returning a value from the live display context.
    /// </summary>
    public static void LiveDisplayReturnValueExample()
    {
        var result = AnsiConsole.Live(new Text("Processing..."))
            .Start(ctx =>
            {
                int total = 0;

                for (int i = 1; i <= 10; i++)
                {
                    total += i;
                    ctx.UpdateTarget(new Text($"Processing: {i}/10 (Total: {total})"));
                    Thread.Sleep(300);
                }

                ctx.UpdateTarget(new Text("[green]Complete![/]"));
                Thread.Sleep(500);

                return total;
            });

        AnsiConsole.WriteLine($"Final result: {result}");
    }

    /// <summary>
    /// Demonstrates a complex dashboard combining multiple widgets in a live display.
    /// </summary>
    public static void LiveDisplayCompositeExample()
    {
        // Create status table
        var statusTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Cyan)
            .AddColumn("Service")
            .AddColumn("Status")
            .AddColumn("Requests/sec");

        // Create metrics table
        var metricsTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Yellow)
            .AddColumn("Metric")
            .AddColumn("Current")
            .AddColumn("Average");

        // Create layout with both tables
        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header"),
                new Layout("Body").SplitColumns(
                    new Layout("Status"),
                    new Layout("Metrics")
                )
            );

        layout["Header"].Update(
            new Panel("[bold cyan]Real-Time Dashboard[/]")
                .BorderColor(Color.Blue)
                .Padding(1, 0)
        );

        layout["Status"].Update(new Panel(statusTable).Header("Services"));
        layout["Metrics"].Update(new Panel(metricsTable).Header("System Metrics"));

        AnsiConsole.Live(layout)
            .Start(ctx =>
            {
                for (int i = 0; i < 15; i++)
                {
                    // Update status table
                    statusTable.Rows.Clear();
                    statusTable.AddRow("API Gateway", "[green]Healthy[/]", $"{Random.Shared.Next(100, 500)}");
                    statusTable.AddRow("Auth Service", "[green]Healthy[/]", $"{Random.Shared.Next(50, 200)}");
                    statusTable.AddRow("Database", i > 10 ? "[yellow]Degraded[/]" : "[green]Healthy[/]", $"{Random.Shared.Next(200, 800)}");
                    statusTable.AddRow("Cache", "[green]Healthy[/]", $"{Random.Shared.Next(1000, 3000)}");

                    // Update metrics table
                    metricsTable.Rows.Clear();
                    metricsTable.AddRow("CPU", $"{Random.Shared.Next(20, 75)}%", "45%");
                    metricsTable.AddRow("Memory", $"{Random.Shared.Next(4, 12)} GB", "8 GB");
                    metricsTable.AddRow("Disk I/O", $"{Random.Shared.Next(10, 90)} MB/s", "45 MB/s");
                    metricsTable.AddRow("Network", $"{Random.Shared.Next(100, 500)} MB/s", "250 MB/s");

                    ctx.Refresh();
                    Thread.Sleep(1000);
                }
            });
    }
}
