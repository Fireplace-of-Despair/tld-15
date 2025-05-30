﻿namespace ApplePie.Pages;

public sealed record MetaPage
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string LocalUrl { get; set; }
}