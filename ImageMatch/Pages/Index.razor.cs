namespace ImageMatch.Pages;

using ImagesComparator;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed partial class Index
{
  private Image<Rgba32> _img1;
  private Image<Rgba32> _img2;
  private string _text { get; set; }

  private async Task LoadFile1(InputFileChangeEventArgs e)
  {
    _img1 = await GetImage(e);
  }

  private async Task LoadFile2(InputFileChangeEventArgs e)
  {
    _img2 = await GetImage(e);
  }

  private async Task<Image<Rgba32>> GetImage(InputFileChangeEventArgs e)
  {
    var data = e.File.OpenReadStream();
    var ms = new MemoryStream();
    await data.CopyToAsync(ms);
    ms.Seek(0, SeekOrigin.Begin);

    var info = await Image.IdentifyAsync(ms);
    if (info is null)
    {
      _text = $"<b>{e.File.Name}</b> --> unknown format";
      return null;
    }

    ms.Seek(0, SeekOrigin.Begin);
    return Image.Load<Rgba32>(ms);
  }

  private async Task OnCheck()
  {
    _text = $"Comparing images ...";
    StateHasChanged();
    await Task.Run(() =>
    {
      var imgHash1 = new ImgHash(_img1);
      var imgHash2 = new ImgHash(_img2);
      var similarity = imgHash1.CompareWith(imgHash2);
      _text = $"Similarity = {similarity}%";
    });
  }
}
