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

        var directory = directoryService.FileSystem.FileInfo.New(filePath).Directory;
        var directoryName = directory?.Name ?? string.Empty;
        var parentDirectoryName = directory != null
            ? (directoryService.FileSystem.DirectoryInfo.New(directory.FullName).Parent?.Name ?? string.Empty)
            : string.Empty;
        var fileName = directoryService.FileSystem.Path.GetFileNameWithoutExtension(filePath);

        var seriesName = Parser.IsImage(filePath) ? directoryName : fileName;
        var volumes = "1";
        if (seriesName.Length <= 4)
        {
            // If the series name is less than 4 characters, we assume it's a volume number
            if (int.TryParse(seriesName, out _))
            {
                volumes = seriesName;
            }

            seriesName = parentDirectoryName;
        }


        var ret = new ParserInfo
        {
            Series = seriesName,
            Volumes = volumes,
            Chapters = Parser.DefaultChapter,
            ComicInfo = comicInfo,
            Format = Parser.ParseFormat(filePath),
            Filename = Path.GetFileName(filePath),
            FullFilePath = Parser.NormalizePath(filePath),
            Title = fileName
        };

        // Patch in other information from ComicInfo
        UpdateFromComicInfo(ret);

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
