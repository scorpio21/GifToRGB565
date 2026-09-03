using SkiaSharp;
using SkiaSharp.Svg;
using System.IO;

string svgDir = args.Length > 0 ? args[0] : @"..\..\..\paratuinfo";
string outDir = args.Length > 1 ? args[1] : @"..\..\..\GifRGB565GUI\Img";

Directory.CreateDirectory(outDir);

string[] svgFiles = { "crop.svg", "resize.svg", "save.svg" };

foreach (var svg in svgFiles)
{
    string svgPath = Path.Combine(svgDir, svg);
    if (!File.Exists(svgPath))
    {
        Console.WriteLine($"No encontrado: {svgPath}");
        continue;
    }

    string pngName = Path.ChangeExtension(svg, ".png");
    string pngPath = Path.Combine(outDir, pngName);

    var svgDoc = new SKSvg();
    svgDoc.Load(svgPath);

    var info = new SKImageInfo(24, 24);
    using var surface = SKSurface.Create(info);
    var canvas = surface.Canvas;
    canvas.Clear(SKColors.Transparent);

    float scale = Math.Min(24f / svgDoc.Picture.CullRect.Width, 24f / svgDoc.Picture.CullRect.Height);
    float offsetX = (24 - svgDoc.Picture.CullRect.Width * scale) / 2;
    float offsetY = (24 - svgDoc.Picture.CullRect.Height * scale) / 2;

    canvas.Translate(offsetX, offsetY);
    canvas.Scale(scale);
    canvas.DrawPicture(svgDoc.Picture);

    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = File.OpenWrite(pngPath);
    data.SaveTo(stream);

    Console.WriteLine($"Convertido: {pngName} (24x24)");
}
Console.WriteLine("Listo!");
