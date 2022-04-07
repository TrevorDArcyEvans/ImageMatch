namespace ImagesComparator;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

[Serializable]
public class ImgHash
{
  private readonly int _hashSide;

  private bool[] _hashData;
  public bool[] HashData => _hashData;

  public ImgHash(Image<Rgba32> img, int hashSideSize = 16)
  {
    _hashSide = hashSideSize;

    _hashData = GenerateFromImage(img, hashSideSize);
  }

  /// <summary>
  /// Method to compare 2 image hashes
  /// </summary>
  /// <returns>% of similarity</returns>
  public double CompareWith(ImgHash compareWith)
  {
    if (HashData.Length != compareWith.HashData.Length)
    {
      throw new Exception("Cannot compare hashes with different sizes");
    }

    var differenceCounter = HashData.Where((t, i) => t != compareWith.HashData[i]).Count();

    return 100 - differenceCounter / 100.0 * HashData.Length / 2.0;
  }

  private static bool[] GenerateFromImage(Image<Rgba32> img, int hashSideSize = 16)
  {
    // resize img to 16x16px (by default) or with configured size 
    img.Mutate(ctx => ctx.Resize(new Size(hashSideSize, hashSideSize)).Grayscale(GrayscaleMode.Bt709));

    var grayLevels = new List<int>();
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          var module = (30 * px.R + 59 * px.G + 11 * px.B) / 100;
          grayLevels.Add(module);
        }
      }
    });

    var sortedGrayLevels = grayLevels.ToArray();
    Array.Sort(sortedGrayLevels);
    var median = grayLevels[sortedGrayLevels.Length / 2];

    return grayLevels.Select(level => level < median).ToArray();
  }
}
