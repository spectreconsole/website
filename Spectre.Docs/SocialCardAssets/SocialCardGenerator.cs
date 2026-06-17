using Ashcroft;
using Pennington.SocialCards;

namespace Spectre.Docs.SocialCardAssets;

public static class SocialCardGenerator
{
    // Social-card assets live in the project's SocialCardAssets folder. Resolved against the
    // content root, so the paths are correct under both `dotnet run` and `dotnet run -- build`
    // (the static-build crawler fetches /social-cards/*.png from the in-memory host).
    public static Task<byte[]?> Build(SocialCardRequest socialCardRequest, IWebHostEnvironment environment)
    {
        var socialCardAssetsPath = Path.Combine(environment.ContentRootPath, "SocialCardAssets");
        var cardBackgroundPath = Path.Combine(socialCardAssetsPath, "spectre-og-card.png");
        var cardFontPath = Path.Combine(socialCardAssetsPath, "JetBrainsMono[wght].ttf");

        var card = SocialCard.Create(socialCardRequest.Width, socialCardRequest.Height)
            .Background(cardBackgroundPath)
            .Theme(new Theme { FontPath = cardFontPath })
            .At(Anchor.BottomLeft, stack =>
            {
                stack.MaxWidth(960);

                stack.Text(socialCardRequest.Title, new TextStyle { Size = 72, Weight = 800, MaxLines = 2, ShrinkToFit = true });
                if (!string.IsNullOrWhiteSpace(socialCardRequest.Description))
                {
                    stack.Text(socialCardRequest.Description,
                        new TextStyle
                        {
                            MaxLines = 2,
                            ShrinkToFit = true,
                            Size = 30,
                            Weight = 200,
                            LetterSpacing = -0.5f
                        });
                }

                stack.Spacer(8);
                stack.Meta(socialCardRequest.SiteTitle, color: "#38bdf8");
            });

        return Task.FromResult<byte[]?>(card.ToBytes());
    }
}