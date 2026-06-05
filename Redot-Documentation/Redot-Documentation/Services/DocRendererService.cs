namespace Redot_Documentation.Services;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Markdig;

public class DocRendererService
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private readonly string docsRootPath;

    public DocRendererService(IWebHostEnvironment webHostEnvironment)
    {
        docsRootPath = Path.Combine(webHostEnvironment.ContentRootPath, "docs");
    }

    public async Task<string> RenderToHtmlAsync(string documentPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
        {
            throw new ArgumentException("Document path cannot be null or empty.", nameof(documentPath));
        }

        var normalizedPath = documentPath.Replace('\\', '/').Trim('/');
        var fullPath = Path.GetFullPath(Path.Combine(docsRootPath, normalizedPath));

        if (!fullPath.StartsWith(docsRootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Document path points outside the docs directory.");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Markdown document not found: {documentPath}", fullPath);
        }

        var markdown = await File.ReadAllTextAsync(fullPath, cancellationToken);
        var transformedMarkdown = TransformMarkdown(markdown);
        return Markdown.ToHtml(transformedMarkdown, MarkdownPipeline);
    }

    private static string TransformMarkdown(string markdown)
    {
        var transformedMarkdown = markdown;

        transformedMarkdown = Regex.Replace(
            transformedMarkdown,
            @"^import\s+Tabs\s+from\s+[\""'][^\""']+[\""'];\s*$",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        transformedMarkdown = Regex.Replace(
            transformedMarkdown,
            @"^import\s+TabItem\s+from\s+[\""'][^\""']+[\""'];\s*$",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        // Parse callout blocks
        // tip
        transformedMarkdown = Regex.Replace(
            transformedMarkdown,
            @":::tip\s*\r?\n(.*?)\r?\n:::",
            match => TransformCallout(match.Value, "tip", IconConstants.TipsIcon),
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // note
        transformedMarkdown = Regex.Replace(
            transformedMarkdown,
            @":::note\s*\r?\n(.*?)\r?\n:::",
            match => TransformCallout(match.Value, "note", IconConstants.TipsIcon),
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        // info
        transformedMarkdown = Regex.Replace(
            transformedMarkdown,
            @":::info\s*\r?\n(.*?)\r?\n:::",
            match => TransformCallout(match.Value, "info", IconConstants.TipsIcon),
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        transformedMarkdown = TransformTabsBlocks(transformedMarkdown);

        return transformedMarkdown;
    }

    private static string TransformTabsBlocks(string markdown)
    {
        var tabsIndex = 0;
        return Regex.Replace(
            markdown,
            @"<Tabs>(.*?)</Tabs>",
            match => TransformSingleTabsBlock(match, tabsIndex++),
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }

    private static string TransformSingleTabsBlock(Match tabsBlockMatch, int tabsIndex)
    {
        var tabsContent = tabsBlockMatch.Groups[1].Value;
        var tabItemMatches = Regex.Matches(
            tabsContent,
            @"<TabItem\b([^>]*)>(.*?)</TabItem>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (tabItemMatches.Count == 0)
        {
            return tabsBlockMatch.Value;
        }

        var tabButtonsMarkup = new List<string>(tabItemMatches.Count);
        var tabPanelsMarkup = new List<string>(tabItemMatches.Count);

        for (var i = 0; i < tabItemMatches.Count; i++)
        {
            var tabItemMatch = tabItemMatches[i];
            var attributes = tabItemMatch.Groups[1].Value;
            var body = tabItemMatch.Groups[2].Value.Trim();

            var label = GetAttributeValue(attributes, "label");
            var value = GetAttributeValue(attributes, "value");

            if (string.IsNullOrWhiteSpace(label))
            {
                label = !string.IsNullOrWhiteSpace(value) ? value : $"Tab {i + 1}";
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                value = $"tab-{i + 1}";
            }

            var panelId = $"doc-tab-{tabsIndex}-{SanitizeIdentifier(value)}";
            var isActive = i == 0;
            var activeClass = isActive ? " active" : string.Empty;

            tabButtonsMarkup.Add(
                $"<button class=\"doc-tab-button{activeClass}\" type=\"button\" role=\"tab\" data-tab-target=\"#{panelId}\" aria-selected=\"{isActive.ToString().ToLowerInvariant()}\">{WebUtility.HtmlEncode(label)}</button>");

            var panelHtml = Markdown.ToHtml(body, MarkdownPipeline).Trim();

            tabPanelsMarkup.Add(
                $"<div id=\"{panelId}\" class=\"doc-tab-panel{activeClass}\" role=\"tabpanel\" data-value=\"{WebUtility.HtmlEncode(value)}\" data-label=\"{WebUtility.HtmlEncode(label)}\">{panelHtml}</div>");
        }

        return
            "<div class=\"doc-tabs\">" +
            $"<div class=\"doc-tab-buttons\" role=\"tablist\">{string.Join(string.Empty, tabButtonsMarkup)}</div>" +
            $"<div class=\"doc-tab-panels\">{string.Join(string.Empty, tabPanelsMarkup)}</div>" +
            "</div>";
    }

    private static string GetAttributeValue(string attributes, string attributeName)
    {
        var match = Regex.Match(
            attributes,
            $@"\b{Regex.Escape(attributeName)}\s*=\s*[\""']([^\""']*)[\""']",
            RegexOptions.IgnoreCase);

        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static string SanitizeIdentifier(string value)
    {
        var sanitized = Regex.Replace(value.ToLowerInvariant(), @"[^a-z0-9_-]+", "-").Trim('-');

        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return "tab";
        }

        return sanitized;
    }

    public static string TransformCallout(string markdownSection, string calloutType, string? iconPath = null)
    {
        string calloutLower = calloutType.ToLowerInvariant();
        string calloutUpper = calloutType.ToUpperInvariant();
        string transformedSection = markdownSection.Replace($":::{calloutLower}", string.Empty)
            .Replace(":::", string.Empty)
            .Trim();
        string transformed = $"""
                              <div class="callout-{calloutLower}">
                                  <div class="callout-icon-{calloutLower}"></div>
                                  <div class="callout-content">
                                      <div class="callout-title">{calloutUpper}</div>
                                      <div class="callout-message">
                                          <p>
                                              {transformedSection}
                                          </p>
                                      </div>
                                  </div>
                              </div>
                              """;
        return transformed;
    }
}