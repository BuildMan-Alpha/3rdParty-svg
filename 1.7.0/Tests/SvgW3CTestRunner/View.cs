﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Svg;
using System.Diagnostics;

namespace SvgW3CTestRunner
{
    public partial class View : Form
    {
        private const string _svgBasePath = @"..\..\..\W3CTestSuite\svg\";
        private const string _pngBasePath = @"..\..\..\W3CTestSuite\png\";

        public View()
        {
            InitializeComponent();
            // ignore tests pertaining to javascript or xml reading
            var passes = File.ReadAllLines(_svgBasePath + @"..\PassingTests.txt").ToDictionary((f) => f, (f) => true);
            var files = (from f in
                         (from g in Directory.GetFiles(_svgBasePath)
                          select Path.GetFileName(g))
                         where !f.StartsWith("animate-") && !f.StartsWith("conform-viewer") &&
                         !f.Contains("-dom-") && !f.StartsWith("linking-") && !f.StartsWith("interact-") &&
                         !f.StartsWith("script-")
                         orderby f
                         select (object)f);
            files = files.Where((f) => !passes.ContainsKey((string)f)).Union(Enumerable.Repeat((object)"## PASSING ##", 1)).Union(files.Where((f) => passes.ContainsKey((string)f)));

            lstFiles.Items.AddRange(files.ToArray());
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //render svg
            var fileName = lstFiles.SelectedItem.ToString();
            if (fileName.StartsWith("#")) return;
            
            //display png
            var png = Image.FromFile(_pngBasePath + Path.GetFileNameWithoutExtension(fileName) + ".png");
            picPng.Image = png;
            
            var doc = new SvgDocument();
            try
            {
                Debug.Print(fileName);
                doc = SvgDocument.Open(_svgBasePath + fileName);
                if (fileName.StartsWith("__"))
                {
                    picSvg.Image = doc.Draw();
                }
                else
                {
                    var img = new Bitmap(480, 360);
                    doc.Draw(img);
                    picSvg.Image = img;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SVG Rendering");
                picSvg.Image = null;
            }
            
            //save load
            try 
            {
                using(var memStream = new MemoryStream())
                {
                    doc.Write(memStream);
                    memStream.Position = 0;  
                    var reader = new StreamReader(memStream);
                    var tempFilePath = Path.Combine(Path.GetTempPath(), "test.svg");
                    System.IO.File.WriteAllText(tempFilePath, reader.ReadToEnd());
                    memStream.Position = 0;
                    var baseUri = doc.BaseUri;
                    doc = SvgDocument.Open(tempFilePath);
                    doc.BaseUri = baseUri;
                    
                    if (fileName.StartsWith("__"))
                    {
                        picSaveLoad.Image = doc.Draw();
                    }
                    else
                    {
                        var img = new Bitmap(480, 360);
                        doc.Draw(img);
                        picSaveLoad.Image = img;
                    }
                }
            } 
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SVG Serialization");
                picSaveLoad.Image = null;
            }
            
            //compare svg to png
            try
            {
                picSVGPNG.Image = PixelDiff((Bitmap)picSvg.Image, (Bitmap)picPng.Image);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "SVG Comparison");
                picSVGPNG.Image = null;
            }
            
           
        }
        
        unsafe Bitmap PixelDiff(Bitmap a, Bitmap b)
        {
            Bitmap output = new Bitmap(a.Width, a.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(Point.Empty, a.Size);
            using (var aData = a.LockBitsDisposable(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb))
                using (var bData = b.LockBitsDisposable(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb))
                    using (var outputData = output.LockBitsDisposable(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb))
            {
                byte* aPtr = (byte*)aData.Scan0;
                byte* bPtr = (byte*)bData.Scan0;
                byte* outputPtr = (byte*)outputData.Scan0;
                int len = aData.Stride * aData.Height;
                for (int i = 0; i < len; i++)
                {
                    // For alpha use the average of both images (otherwise pixels with the same alpha won't be visible)
                    if ((i + 1) % 4 == 0)
                        *outputPtr = (byte)((*aPtr  + *bPtr) / 2);
                    else
                        *outputPtr = (byte)~(*aPtr ^ *bPtr);

                    outputPtr++;
                    aPtr++;
                    bPtr++;
                }
            }
            return output;
        }


    }
    
    static class BitmapExtensions
    {
        public static DisposableImageData LockBitsDisposable(this Bitmap bitmap, Rectangle rect, ImageLockMode flags, PixelFormat format)
        {
            return new DisposableImageData(bitmap, rect, flags, format);
        }

        public class DisposableImageData : IDisposable
        {
            private readonly Bitmap _bitmap;
            private readonly BitmapData _data;

            internal DisposableImageData(Bitmap bitmap, Rectangle rect, ImageLockMode flags, PixelFormat format)
            {
                _bitmap = bitmap;
                _data = bitmap.LockBits(rect, flags, format);
            }

            public void Dispose()
            {
                _bitmap.UnlockBits(_data);
            }

            public IntPtr Scan0
            {
                get { return _data.Scan0; }
            }

            public int Stride
            {
                get { return _data.Stride;}
            }

            public int Width
            {
                get { return _data.Width;}
            }

            public int Height
            {
                get { return _data.Height;}
            }

            public PixelFormat PixelFormat
            {
                get { return _data.PixelFormat;}
            }

            public int Reserved
            {
                get { return _data.Reserved;}
            }
        }
    }
}
