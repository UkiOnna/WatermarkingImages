using ImageMagick;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WatermarkApp
{
    public class ImageWatermarkViewModel : WatermarkViewModelBase
    {
        public string Path { get; private set; }

        [JsonIgnore]
        public ImageSource Image { get; }

        public ImageWatermarkViewModel(string path, WindowViewModel parent)
            : base(parent)
        {
            this.Path = path;

            if (path.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
                || path.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
            {
                var img = new MagickImage(path);
                img.AutoOrient();
                img.Strip();

                var memoryStream = new MemoryStream();
                img.Write(memoryStream);
                img.Dispose();

                memoryStream.Position = 0;

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();

                this.Image = image;
            }
            else
            {
                this.Image = new BitmapImage(new Uri(path));
            }

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.Zoom = 1.0;
            this.Margin = 20.0;
            this.Opacity = 1.0;
        }

        private double margin;
        private double opacity;
        private double zoom;

        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom == value)
                    return;
                zoom = value;
                OnPropertyChange(nameof(Zoom));
                OnPropertyChange(nameof(Width));
                OnPropertyChange(nameof(Height));
            }
        }

        public double Margin
        {
            get { return margin; }
            set
            {
                if (margin == value)
                    return;
                margin = value;
                OnPropertyChange(nameof(Margin));
                OnPropertyChange(nameof(ImageMargin));
            }
        }

        [JsonIgnore]
        public Thickness ImageMargin
        {
            get
            {
                return new Thickness(this.Margin);
            }
        }

        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity == value)
                    return;
                opacity = value;
                OnPropertyChange(nameof(Opacity));
            }
        }

        [JsonIgnore]
        public Double? Width => this.Zoom * this.Image?.Width;

        [JsonIgnore]
        public Double? Height => this.Zoom * this.Image?.Height;
    }
}
