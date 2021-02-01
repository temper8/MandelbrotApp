using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace MandelbrotAppUWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void skiaCanvas_PaintSurface(object sender, SkiaSharp.Views.UWP.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            var surfaceWidth = e.Info.Width;
            var surfaceHeight = e.Info.Height;


            var canvas = surface.Canvas;
            /*
            if (snapshot == null)
            {
                canvas.Clear();
                planeView.Render(canvas);
                snapshot = surface.Snapshot();
                SaveSnapshot();
            }
            else
            {
                canvas.Clear();
                canvas.DrawImage(snapshot, new SKPoint(0, 0));
            }
            */

            var rect = SKRect.Create(10, 10, surfaceWidth - 20 , surfaceHeight - 20);

            // the brush (fill with blue)
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.AliceBlue
            };


            canvas.ResetMatrix();
          //  canvas.Translate(dx, dy);
            canvas.DrawRect(rect, paint);


            canvas.Flush();
        }

        private void skiaCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
