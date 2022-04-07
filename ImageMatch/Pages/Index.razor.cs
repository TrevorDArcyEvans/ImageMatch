namespace ImageMatch.Pages;

using ImagesComparator;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed partial class Index
{
  private Image<Rgba32> _img1;
  private Image<Rgba32> _img2;
  private string _img1FileName { get; set; } = "Upload image 1";
  private string _img2FileName { get; set; } = "Upload image 2";
  private string _img1Url { get; set; } = string.Empty;
  private string _img2Url { get; set; } = string.Empty;
  private string _text { get; set; }
  private bool _canMatch { get => _img1 is not null && _img2 is not null; }

  private async Task LoadFile1(InputFileChangeEventArgs e)
  {
    _img1 = await GetImage(e);
    _img1FileName = e.File.Name;
    _text = _img1 is null ? $"<b>{_img1FileName}</b> --> unknown format" : string.Empty;
    _img1Url = await GetImageString(e);
  }

  private async Task LoadFile2(InputFileChangeEventArgs e)
  {
    _img2 = await GetImage(e);
    _img2FileName = e.File.Name;
    _text = _img2 is null ? $"<b>{_img2FileName}</b> --> unknown format" : string.Empty;
    _img2Url = await GetImageString(e);
  }

  private static async Task<Image<Rgba32>> GetImage(InputFileChangeEventArgs e)
  {
    var data = e.File.OpenReadStream();
    var ms = new MemoryStream();
    await data.CopyToAsync(ms);
    ms.Seek(0, SeekOrigin.Begin);

    var info = await Image.IdentifyAsync(ms);
    if (info is null)
    {
      return null;
    }

    ms.Seek(0, SeekOrigin.Begin);
    return Image.Load<Rgba32>(ms);
  }

  private static async Task<string> GetImageString(InputFileChangeEventArgs e)
  {
    var imgFile = e.File;
    var buffers = new byte[imgFile.Size];
    await imgFile.OpenReadStream().ReadAsync(buffers);
    return $"data:{imgFile.ContentType};base64,{Convert.ToBase64String(buffers)}";    
  }

  private async Task OnMatch()
  {
    _text = $"Matching images ...";
    StateHasChanged();
    await Task.Run(() =>
    {
      var imgHash1 = new ImgHash(_img1);
      var imgHash2 = new ImgHash(_img2);
      var match = imgHash1.CompareWith(imgHash2);
      _text = $"Match = {match}%";
    });
  }
}
