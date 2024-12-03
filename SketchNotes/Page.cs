using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Ink;
using System.IO;

namespace SketchNotes
{
    internal class Page : InkCanvas
    {
        public Page()
        {
            Background = new SolidColorBrush(Colors.Gray);
            Height = 2970;
            Width = 2100;
            ClipToBounds = true;
            Focusable = false;
            ResizeEnabled = true;
            Margin = new Thickness(0, 0, 0, 10);
        }

        public void LoadPage(FileStream fileStream)
        {
            var transform = this.RenderTransform;
            this.RenderTransform = null;
            this.Strokes = new StrokeCollection(fileStream);
            this.RenderTransform = transform;
        }

        public void SavePage(FileStream fileStream)
        {
            var transform = this.RenderTransform;
            this.RenderTransform = null;
            Strokes.Save(fileStream);
            this.RenderTransform = transform;
        }

        public bool SavePage(string path)
        {
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    Strokes.Save(fs);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
