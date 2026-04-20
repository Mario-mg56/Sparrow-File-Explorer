using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace DynamicFileExplorer.UI.Helpers;

public class BackdropBlurControl : Control
{
    public static readonly StyledProperty<ExperimentalAcrylicMaterial> MaterialProperty =
        AvaloniaProperty.Register<BackdropBlurControl, ExperimentalAcrylicMaterial>("Material");

    public ExperimentalAcrylicMaterial Material
    {
        get => GetValue(MaterialProperty);
        set => SetValue(MaterialProperty, value);
    }

    static ImmutableExperimentalAcrylicMaterial DefaultAcrylicMaterial =
        (ImmutableExperimentalAcrylicMaterial)new ExperimentalAcrylicMaterial()
        {
            MaterialOpacity = 0.1,
            TintColor = Colors.White,
            TintOpacity = 0.1,
            PlatformTransparencyCompensationLevel = 0
        }.ToImmutable();

    static BackdropBlurControl()
    {
        AffectsRender<BackdropBlurControl>(MaterialProperty);
    }

    private static SKShader? s_acrylicNoiseShader;

    // CACHE DEL BLUR
    private SKImage? _cachedImage;
    private Size _lastSize;

    /// <summary>
    /// Llama a esto cuando quieras refrescar manualmente el blur
    /// (por ejemplo al abrir menú).
    /// </summary>
    public void RefreshBlur()
    {
        _cachedImage?.Dispose();
        _cachedImage = null;
        InvalidateVisual();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cachedImage?.Dispose();
        _cachedImage = null;
    }

    class BlurBehindRenderOperation : ICustomDrawOperation
    {
        private readonly BackdropBlurControl _owner;
        private readonly ImmutableExperimentalAcrylicMaterial _material;
        private readonly Rect _bounds;

        public BlurBehindRenderOperation(
            BackdropBlurControl owner,
            ImmutableExperimentalAcrylicMaterial material,
            Rect bounds)
        {
            _owner = owner;
            _material = material;
            _bounds = bounds;
        }

        public void Dispose() { }

        public bool HitTest(Point p) => _bounds.Contains(p);

        static SKColorFilter CreateAlphaColorFilter(double opacity)
        {
            opacity = Math.Clamp(opacity, 0, 1);

            var c = new byte[256];
            var a = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                c[i] = (byte)i;
                a[i] = (byte)(i * opacity);
            }

            return SKColorFilter.CreateTable(a, c, c, c);
        }

        private void EnsureNoiseShader()
        {
            if (s_acrylicNoiseShader != null)
                return;

            using var stream =
                typeof(SkiaPlatform).Assembly.GetManifestResourceStream(
                    "Avalonia.Skia.Assets.NoiseAsset_256X256_PNG.png");

            using var bitmap = SKBitmap.Decode(stream);

            s_acrylicNoiseShader =
                SKShader.CreateBitmap(
                    bitmap,
                    SKShaderTileMode.Repeat,
                    SKShaderTileMode.Repeat)
                .WithColorFilter(CreateAlphaColorFilter(0.0225));
        }

        private void BuildCache(SKCanvas canvas, GRContext? grContext)
        {
            if (!canvas.TotalMatrix.TryInvert(out var currentInvertedTransform))
                return;

            using var backgroundSnapshot = canvas.Surface!.Snapshot();

            using var backdropShader = SKShader.CreateImage(
                backgroundSnapshot,
                SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp,
                currentInvertedTransform);

            using var blurredSurface = SKSurface.Create(
                grContext,
                false,
                new SKImageInfo(
                    (int)Math.Ceiling(_bounds.Width),
                    (int)Math.Ceiling(_bounds.Height),
                    SKImageInfo.PlatformColorType,
                    SKAlphaType.Premul));

            using (var filter = SKImageFilter.CreateBlur(10, 10))
            using (var blurPaint = new SKPaint
            {
                Shader = backdropShader,
                ImageFilter = filter
            })
            {
                blurredSurface.Canvas.DrawRect(
                    0,
                    0,
                    (float)_bounds.Width,
                    (float)_bounds.Height,
                    blurPaint);
            }

            _owner._cachedImage?.Dispose();
            _owner._cachedImage = blurredSurface.Snapshot();
            _owner._lastSize = _bounds.Size;
        }

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
                return;

            using var lease = leaseFeature.Lease();

            var skia = lease;
            if (skia == null)
                return;

            // Solo regenerar si no existe cache o cambió tamaño
            if (_owner._cachedImage == null || _owner._lastSize != _bounds.Size)
            {
                BuildCache(skia.SkCanvas, skia.GrContext);
            }

            if (_owner._cachedImage == null)
                return;

            using var imageShader = SKShader.CreateImage(_owner._cachedImage);

            using (var paint = new SKPaint
            {
                Shader = imageShader,
                IsAntialias = true
            })
            {
                skia.SkCanvas.DrawRect(
                    0,
                    0,
                    (float)_bounds.Width,
                    (float)_bounds.Height,
                    paint);
            }

            EnsureNoiseShader();

            using var acrylicPaint = new SKPaint();
            acrylicPaint.IsAntialias = true;

            var tintColor = _material.TintColor;
            var tint = new SKColor(
                tintColor.R,
                tintColor.G,
                tintColor.B,
                tintColor.A);

            using var backdrop =
                SKShader.CreateColor(new SKColor(
                    _material.MaterialColor.R,
                    _material.MaterialColor.G,
                    _material.MaterialColor.B,
                    _material.MaterialColor.A));

            using var tintShader = SKShader.CreateColor(tint);
            using var effectiveTint = SKShader.CreateCompose(backdrop, tintShader);
            using var compose = SKShader.CreateCompose(effectiveTint, s_acrylicNoiseShader);

            acrylicPaint.Shader = compose;

            skia.SkCanvas.DrawRect(
                0,
                0,
                (float)_bounds.Width,
                (float)_bounds.Height,
                acrylicPaint);
        }

        public Rect Bounds => _bounds.Inflate(4);

        public bool Equals(ICustomDrawOperation? other)
        {
            return false;
        }
    }

    public override void Render(DrawingContext context)
    {
        var mat = Material != null
            ? (ImmutableExperimentalAcrylicMaterial)Material.ToImmutable()
            : DefaultAcrylicMaterial;

        context.Custom(
            new BlurBehindRenderOperation(
                this,
                mat,
                new Rect(default, Bounds.Size)));
    }
}
