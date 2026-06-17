using Mdazor;
using Pennington.ApiMetadata.Reflection;
using Pennington.Infrastructure;
using Pennington.MonorailCss;
using Pennington.TreeSitter;
using Pennington.UI.Components;
using Spectre.Console;
using Spectre.Docs.Components;
using Spectre.Docs.Components.Reference;
using Spectre.Docs.Components.Shared;
using Spectre.Docs.Services;
using ColorName = Pennington.MonorailCss.ColorName;
using IContentService = Pennington.Content.IContentService;
using IContentRenderer = Pennington.Pipeline.IContentRenderer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

// Typed views over the Pennington content pipeline that preserve the ergonomics the Razor
// components were already built against. Each is scoped by base URL so it only surfaces pages
// from its own markdown source; the pipeline does the parsing (each source already binds its
// own front-matter type), so these just filter, render, and shape the result.
builder.Services.AddScoped<IMarkdownContentService<SpectreConsoleFrontMatter>>(sp =>
    new MarkdownContentService<SpectreConsoleFrontMatter>(
        sp.GetRequiredService<IEnumerable<IContentService>>(),
        sp.GetRequiredService<IContentRenderer>(),
        "/console"));
builder.Services.AddScoped<IMarkdownContentService<SpectreConsoleCliFrontMatter>>(sp =>
    new MarkdownContentService<SpectreConsoleCliFrontMatter>(
        sp.GetRequiredService<IEnumerable<IContentService>>(),
        sp.GetRequiredService<IContentRenderer>(),
        "/cli"));
builder.Services.AddScoped<IMarkdownContentService<BlogFrontMatter>>(sp =>
    new MarkdownContentService<BlogFrontMatter>(
        sp.GetRequiredService<IEnumerable<IContentService>>(),
        sp.GetRequiredService<IContentRenderer>(),
        "/blog"));
builder.Services.AddScoped<TableOfContentsService>();

// Pennington content engine: one markdown source per content area.
builder.Services.AddPennington(penn =>
{
    penn.SiteTitle = "Spectre.Console Documentation";
    penn.SiteDescription = "Beautiful console applications with Spectre.Console";
    penn.ContentRootPath = "Content";
    penn.SiteProjection.ContentSelector = "article";

    penn.AddMarkdownContent<SpectreConsoleFrontMatter>(md =>
    {
        md.ContentPath = "Content/console";
        md.BasePageUrl = "/console";
        md.SectionLabel = "console";
    });

    penn.AddMarkdownContent<SpectreConsoleCliFrontMatter>(md =>
    {
        md.ContentPath = "Content/cli";
        md.BasePageUrl = "/cli";
        md.SectionLabel = "cli";
    });

    penn.AddMarkdownContent<BlogFrontMatter>(md =>
    {
        md.ContentPath = "Content/blog";
        md.BasePageUrl = "/blog";
    });

    penn.AddLlmsTxt();
});

// Reflection-backed API metadata providers, one keyed registration per reference area.
// These ship in Pennington.ApiMetadata(.Reflection); the higher-level AddApiReference
// content service is not in this package version, so the /console/api and /cli/api pages
// render the metadata themselves (see ApiReferenceService + ConsoleApiPage/CliApiPage).
// Registering a provider also registers the shared IXmlDocParser / IXmlDocHtmlRenderer.
builder.Services.AddApiMetadataFromCompiledAssembly("console", opts =>
{
    opts.FromPackageReference("Spectre.Console");
    opts.FromPackageReference("Spectre.Console.Json");
    opts.FromPackageReference("Spectre.Console.ImageSharp");
});
builder.Services.AddApiMetadataFromCompiledAssembly("cli", opts =>
    opts.FromPackageReference("Spectre.Console.Cli"));

builder.Services.AddScoped<ApiReferenceService>();

// Supplies API route discovery (so the static build emits the pages) and the sidebar
// "API Reference" entry, standing in for the unreleased AddApiReference content service.
builder.Services.AddSingleton<IContentService, ApiReferenceContentService>();

// Tree-sitter-backed code-fragment fences (`:symbol`). Reads source files directly —
// no MSBuild workspace. ContentRoot is the repo root so fence bodies resolve against
// the sibling Spectre.Docs.Examples / Spectre.Docs.Cli.Examples source projects.
builder.Services.AddTreeSitter(treeSitter =>
{
    treeSitter.ContentRoot = "..";
});

// Mdazor component registry for markdown-embedded Razor components.
builder.Services
    .AddMdazorComponent<Step>()
    .AddMdazorComponent<Steps>()
    .AddMdazorComponent<Screenshot>()
    .AddMdazorComponent<BoxBorderList>()
    .AddMdazorComponent<ColorList>()
    .AddMdazorComponent<EmojiList>()
    .AddMdazorComponent<SpinnerList>()
    .AddMdazorComponent<TableBorderList>()
    .AddMdazorComponent<TreeGuideList>()
    .AddMdazorComponent<WidgetApiReference>();

builder.Services.AddMonorailCss(_ => new MonorailCssOptions
{
    ColorScheme = new NamedColorScheme
    {
        PrimaryColorName = ColorName.Sky,
        AccentColorName = ColorName.Amber,
        BaseColorName = ColorName.Neutral,
        AdditionalMappings =
        {
            ["tertiary-one"] = ColorName.Emerald,
            ["tertiary-two"] = ColorName.Violet,
        },
    },
});

var app = builder.Build();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>();

app.UsePennington();
app.UseMonorailCss();

await app.RunOrBuildAsync(args);
