﻿namespace Scaffolding.Extensions.Docs;

public class DocsSettings
{
    public bool Enabled { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string AuthorEmail { get; set; }
    public string PathToReadme { get; set; }
    public string RedocUrl { get; set; }
    public string SwaggerJsonUrl { get; set; }
    public string SwaggerJsonTemplateUrl { get; set; }
    public List<string> IgnoredEnums { get; set; }

    public IEnumerable<string> GetDocsFinalRoutes()
    {
        return new List<string>
        {
            SwaggerJsonUrl,
            RedocUrl
        };
    }
}
