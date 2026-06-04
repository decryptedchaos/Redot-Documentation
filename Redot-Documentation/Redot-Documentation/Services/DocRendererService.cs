namespace Redot_Documentation.Services;

using System.IO;
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
        return Markdown.ToHtml(markdown, MarkdownPipeline);
    }
}