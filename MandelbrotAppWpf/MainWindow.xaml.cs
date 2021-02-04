﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandelbrotAppWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SKBitmap bitmap;
        int surfaceWidth;
        int surfaceHeight;
        private void skiaCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            surfaceWidth = e.Info.Width;
            surfaceHeight = e.Info.Height;

            var canvas = surface.Canvas;

            var rect = SKRect.Create(10, 10, surfaceWidth - 20, surfaceHeight - 20);

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.AliceBlue
            };

            canvas.ResetMatrix();
            canvas.DrawRect(rect, paint);
            if (bitmap != null)
                canvas.DrawBitmap(bitmap, 0, 0);

            canvas.Flush();
        }

        private void Draw(int[] data, int width, int height, int iterations, SKColor color)
        {
            var pixels = new SKColor[width * height];

            for (int i = 0; i < width * height; i++)
            {
                if (data[i] == iterations)
                    pixels[i] = color;
                else
                    pixels[i] =  new SkiaSharp.SKColor((UInt32)(4000000000 / ((data[i] < 1) ? 1 : data[i])));
            }

            bitmap = new SKBitmap(width, height);
            bitmap.Pixels = pixels;

        }

        private void mandelbrot_Calc(string mode)
        {
            
            int width = surfaceWidth;
            int height = surfaceHeight;
            int iterations = 1000;
            int[] data = new int[width * height];

            switch (mode)
            {
                case "Single thread CPU":
                    Utils.InitWatch();
                    Mandelbrot.CalcCPU(data, width, height, iterations); // Single thread CPU
                    TimeText.Text = Utils.GetElapsedTime("CPU Mandelbrot");
                    Draw(data, width, height, iterations, SKColors.Blue);
                    break;

                case "ILGPU-CPU-Mode":
                    Mandelbrot.CompileKernel(false);
                    Utils.InitWatch();
                    Mandelbrot.CalcGPU(data, width, height, iterations); // ILGPU-CPU-Mode
                    TimeText.Text = Utils.GetElapsedTime("ILGPU-CPU Mandelbrot");
                    Draw(data, width, height, iterations, SKColors.AntiqueWhite);
                    break;

                case "ILGPU-GPU-Mode":
                    Mandelbrot.CompileKernel(true);
                    Utils.InitWatch();
                    Mandelbrot.CalcGPU(data, width, height, iterations); // ILGPU-GPU-Mode
                    TimeText.Text = Utils.GetElapsedTime("ILGPU-CUDA Mandelbrot");
                    Draw(data, width, height, iterations, SKColors.Red);
                    break;
            }
            Mandelbrot.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string mode = "Single thread CPU";
            ComboBoxItem selectedItem = (ComboBoxItem)CalcMode_ComboBox.SelectedItem;
            if (selectedItem != null)
                mode = selectedItem.Content.ToString();

            mandelbrot_Calc(mode);

            skiaCanvas.InvalidateVisual();
        }

        static (float, float) fun1(float pos)
        {
            float a = 0.5f;
            float x = a * (float)Math.Cos(Math.PI * pos);
            float y = a * (float)Math.Sin(Math.PI * pos);
            return (x, y);
        }

        static (float, float) fun2(float pos)
        {
            var a = Math.Sqrt(pos);
            float x = (float) (a * Math.Sin(4*Math.PI * pos));
            float y = (float) (a * Math.Cos(4*Math.PI * pos));
            return (x, y);
        }

        void MandVar_Calc(float pos)
        {
            int width = surfaceWidth;
            int height = surfaceHeight;
            int iterations = 1000;
            int[] data = new int[width * height];
            var (x, y) = fun2(pos);

            MandelbrotVariable.CompileKernel(true);
            Utils.InitWatch();
            MandelbrotVariable.CalcGPU(data, x,y, width, height, iterations); // ILGPU-GPU-Mode
            TimeText.Text = Utils.GetElapsedTime("ILGPU-CUDA Mandelbrot");
            Draw(data, width, height, iterations, SKColors.Red);

            MandelbrotVariable.Dispose();
        }
        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var N = 500;
            for(var i =0; i<N; i++)
            {
                MandVar_Calc(i * 2.0f / N);
                skiaCanvas.InvalidateVisual();
                DoEvents();
            }
        }
    }
}
