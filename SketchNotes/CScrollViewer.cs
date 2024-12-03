using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace SketchNotes
{
    public class CScrollViewer : ScrollViewer
    {
        const int a4Height = 2970;
        const int a4Width = 2100;

        StackPanel pages = new StackPanel();

        static float zoom = 10;
        const float zoomStep = 0.02F;

        public CScrollViewer()
        {
            this.Background = new SolidColorBrush(Colors.White);
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            pages.HorizontalAlignment = HorizontalAlignment.Center;
            this.AddChild(pages);

            pages.Children.Add(new Page());

            SetZoom(zoom);
        }

        public bool LoadPages(string path)
        {
            try
            {
                pages.Children.Clear();
                using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    while(fs.Position < fs.Length)
                    {
                        Page page = new Page();
                        page.LoadPage(fs);
                        pages.Children.Add(page);
                    }
                }
                SetZoom(zoom);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool SavePages(string path)
        {
            try
            {

                using (System.IO.FileStream fs = new System.IO.FileStream(path + ".temp", System.IO.FileMode.Create))
                {
                    fs.SetLength(0);
                    foreach (var item in pages.Children)
                    {
                        if (item is Page && item != null)
                        {
                            Page page = item as Page;
                            page.SavePage(fs);
                        }
                    }
                }
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                System.IO.File.Move(path + ".temp", path);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                zoom += (int)(e.Delta * zoomStep);
                zoom = zoom < 10 ? 10 : zoom;
                zoom = zoom > 500 ? 500 : zoom;
                SetZoom(zoom);
                base.UpdateLayout();
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }


        private void SetZoom(float zoom)
        {
            double verOffset = 0;
            if ((int)base.ScrollableHeight != 0)
            {
                verOffset = base.VerticalOffset / base.ScrollableHeight;
            }
            double horOffset = 0;
            if ((int)base.ScrollableWidth != 0)
            { 
                horOffset = (base.HorizontalOffset) / base.ScrollableWidth;
            }

            foreach (var item in pages.Children)
            {
                if (item is InkCanvas && item != null)
                {
                    InkCanvas inkCanvas = item as InkCanvas;
                    inkCanvas.RenderTransform = new ScaleTransform(zoom / 100.0, zoom / 100.0);
                    inkCanvas.Margin = new Thickness(0, 0, inkCanvas.Width * zoom / 100.0 - a4Width, 10 + inkCanvas.Height * zoom / 100.0 - a4Height);

                    pages.Width = inkCanvas.RenderTransform.Transform(new Point(inkCanvas.Height, inkCanvas.Width)).Y;
                    inkCanvas.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
            base.UpdateLayout();
            base.ScrollToVerticalOffset(verOffset * ScrollableHeight);
            base.UpdateLayout();
            base.ScrollToHorizontalOffset((horOffset) * ScrollableWidth);
        }


    }


}
