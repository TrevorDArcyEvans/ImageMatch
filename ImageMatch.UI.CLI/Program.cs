namespace ImageMatch.UI.CLI;

using CommandLine;
using ImagesComparator;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    var result = await Parser.Default.ParseArguments<Options>(args)
      .WithParsedAsync(Run);
    await result.WithNotParsedAsync(HandleParseError);
  }

  private static async Task Run(Options opt)
  {
    var img1 = await Image.LoadAsync<Rgba32>(opt.ImageFile1Path);
    var img2 = await Image.LoadAsync<Rgba32>(opt.ImageFile2Path);
    var imgHash1 = new ImgHash(img1);
    var imgHash2 = new ImgHash(img2);
    var match = imgHash1.CompareWith(imgHash2);
    Console.WriteLine($"ImageFile1 = {opt.ImageFile1Path}");
    Console.WriteLine($"ImageFile2 = {opt.ImageFile2Path}");
    Console.WriteLine($"  Match = {match}%");
  }

  private static Task HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine("Version Request");
      return Task.CompletedTask;
    }

    if (errs.IsHelp())
    {
      Console.WriteLine("Help Request");
      return Task.CompletedTask;
      ;
    }

    Console.WriteLine("Parser Fail");
    return Task.CompletedTask;
  }
}
