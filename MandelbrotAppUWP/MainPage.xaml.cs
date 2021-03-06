﻿using SkiaSharp;
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

            InitBitmap(500, 500);
        }

        SKBitmap bitmap;
        const string TEXT = "Hello, Bitmap!";
        void InitBitmap(int w, int h)
        {
            // Create bitmap and draw on it
            using (SKPaint textPaint = new SKPaint { TextSize = 48 })
            {
                SKRect bounds = new SKRect();
                textPaint.MeasureText(TEXT, ref bounds);

                bitmap = new SKBitmap(w, h);

                using (SKCanvas bitmapCanvas = new SKCanvas(bitmap))
                {
                    bitmapCanvas.Clear();
                    bitmapCanvas.DrawText(TEXT, w/2 - bounds.Right / 2 , h / 2 - bounds.Top, textPaint);
                }
            }
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
            canvas.DrawBitmap(bitmap, 20, 20);

            canvas.Flush();
        }

        private void skiaCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        //----

        private void Draw(int[] data, int width, int height, int iterations, SKColor color)
        {
            var bmp = bitmap;
            for (int i = 0; i < width * height; i++)
            {
                int x = i % width;
                int y = i / width;
                if (data[i] == iterations)
                    bmp.SetPixel(x, y, color);
                else
                    bmp.SetPixel(x, y, new SkiaSharp.SKColor((UInt32)(4000000000 / ((data[i] < 1) ? 1 : data[i]))));
            }
          //  pictureBox1.Image = bmp;
        }


        private void mandelbrot_CB(object sender, EventArgs e)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int iterations = 1000;
            int[] data = new int[width * height];

            //Utils.InitWatch();
         //   Mandelbrot.CalcCPU(data, width, height, iterations); // Single thread CPU
            //Utils.PrintElapsedTime("CPU Mandelbrot");
        //    Draw(data, width, height, iterations, SKColors.Blue);
            /*
            Mandelbrot.Dispose();
            Mandelbrot.CompileKernel(false);
            Utils.InitWatch();
            Mandelbrot.CalcGPU(data, width, height, iterations); // ILGPU-CPU-Mode
            Utils.PrintElapsedTime("ILGPU-CPU Mandelbrot");
            Draw(data, width, height, iterations, Color.Black);

            Mandelbrot.Dispose();
            Mandelbrot.CompileKernel(true);
            Utils.InitWatch();
            Mandelbrot.CalcGPU(data, width, height, iterations); // ILGPU-GPU-Mode
            Utils.PrintElapsedTime("ILGPU-CUDA Mandelbrot");
            Draw(data, width, height, iterations, Color.Red);
            */
        }

    }
}
