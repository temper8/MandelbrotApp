using SkiaSharp;
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
                    bitmapCanvas.DrawText(TEXT, w / 2 - bounds.Right / 2, h / 2 - bounds.Top, textPaint);
                }
            }
        }

        int surfaceWidth;
        int surfaceHeight;
        private void skiaCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var surface = e.Surface;
            surfaceWidth = e.Info.Width;
            surfaceHeight = e.Info.Height;


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

            var rect = SKRect.Create(10, 10, surfaceWidth - 20, surfaceHeight - 20);

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


        private void mandelbrot_Calc(string mode)
        {
            bitmap = new SKBitmap(surfaceWidth, surfaceHeight);
            int width = bitmap.Width;
            int height = bitmap.Height;
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
                   // Mandelbrot.Dispose();
                    Mandelbrot.CompileKernel(false);
                    Utils.InitWatch();
                    Mandelbrot.CalcGPU(data, width, height, iterations); // ILGPU-CPU-Mode
                    TimeText.Text = Utils.GetElapsedTime("ILGPU-CPU Mandelbrot");
                    Draw(data, width, height, iterations, SKColors.AntiqueWhite);
                    break;

                case "ILGPU-GPU-Mode":
                   // Mandelbrot.Dispose();
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
    }
}
