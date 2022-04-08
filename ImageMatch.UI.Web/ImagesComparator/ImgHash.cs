namespace ImagesComparator;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public sealed class ImgHash
{
  private bool[] _hashData;

  public ImgHash(Image<Rgba32> img, int hashSideSize = 16)
  {
    _hashData = GenerateFromImage(img, hashSideSize);
  }

  /// <summary>
  /// Method to compare 2 image hashes
  /// </summary>
  /// <returns>% of similarity</returns>
  public double CompareWith(ImgHash compareWith)
  {
    if (_hashData.Length != compareWith._hashData.Length)
    {
      throw new ArgumentException("Cannot compare hashes with different sizes");
    }

    var numDiffs = _hashData.Where((t, i) => t != compareWith._hashData[i]).Count();

    return 100.0 - numDiffs / 100.0 * _hashData.Length / 2.0;
  }

  private static bool[] GenerateFromImage(Image<Rgba32> img, int hashSideSize = 16)
  {
    // resize img to 16x16px (by default) or with configured size 
    img.Mutate(ctx => ctx
      .Resize(new Size(hashSideSize, hashSideSize))
      .BlackWhite());

    var bwLevels = new List<bool>();
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          var isWhite = (px.R + px.G + px.B) > 45;
          bwLevels.Add(isWhite);
        }
      }
    });

    return bwLevels.ToArray();
  }
}
