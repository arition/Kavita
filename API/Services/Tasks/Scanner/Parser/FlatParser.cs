using System.IO;
using API.Data.Metadata;
using API.Entities.Enums;

namespace API.Services.Tasks.Scanner.Parser;
#nullable enable

public class FlatParser(IDirectoryService directoryService) : DefaultParser(directoryService)
{
    public override ParserInfo? Parse(string filePath, string rootPath, string libraryRoot, LibraryType type, ComicInfo? comicInfo = null)
    {
        if (!IsApplicable(filePath, type)) return null;

        var directoryName = directoryService.FileSystem.FileInfo.New(filePath).Directory?.Name ?? string.Empty;
        var fileName = directoryService.FileSystem.Path.GetFileNameWithoutExtension(filePath);
        var ret = new ParserInfo
        {
            Series = Parser.IsImage(filePath) ? directoryName : fileName,
            Volumes = "1",
            Chapters = Parser.DefaultChapter,
            ComicInfo = comicInfo,
            Format = Parser.ParseFormat(filePath),
            Filename = Path.GetFileName(filePath),
            FullFilePath = Parser.NormalizePath(filePath),
            Title = fileName,
            IsSpecial = true
        };

        return string.IsNullOrEmpty(ret.Series) ? null : ret;
    }

    /// <summary>
    /// Only applicable for Image files and Image library type
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public override bool IsApplicable(string filePath, LibraryType type)
    {
        return type == LibraryType.Flat;
    }
}
