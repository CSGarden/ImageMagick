using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ImageMagick;
using ImagemagickDemo.Model;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ImagemagickDemo.ViewModel {
    public class MainWindowViewModel : ViewModelBase {
        #region 图片选择相关属性
        public ObservableCollection<MagickFormatItem> ImageFormats { get; set; }

        private string presentFormat;
        public string PresentFormat {
            get {
                return presentFormat;
            }
            set {
                Set(ref presentFormat, value);
            }
        }


        private ImageSource fileImage;
        public ImageSource FileImage {
            get {
                return fileImage;
            }
            set {
                Set(ref fileImage, value);
            }
        }
        private bool isBtnEnale = false;
        public bool IsBtnEnale {
            get {
                return isBtnEnale;
            }
            set {
                Set(ref isBtnEnale, value);
            }
        }

        private Visibility textVisibility = Visibility.Visible;
        public Visibility TextVisibility {
            get {
                return textVisibility;
            }
            set {
                Set(ref textVisibility, value);
            }
        }

        public string FileImageName { get; set; }

        private string selectedImageFormat;
        public string SelectedImageFormat {
            get {
                if (SelectedIndex >= 0 && SelectedIndex < ImageFormats.Count) {
                    return ImageFormats[SelectedIndex].Name;
                }
                return string.Empty;
            }
            set {
                Set(ref selectedImageFormat, value);

            }
        }

        private int selectedIndex;
        public int SelectedIndex {
            get {
                return selectedIndex;
            }
            set {
                Set(ref selectedIndex, value);
                OnPropertyChanged(nameof(SelectedIndex));
                OnPropertyChanged(nameof(SelectedImageFormat));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private double newImageWidth;
        public double NewImageWidth {
            get {
                return newImageWidth;
            }
            set {
                Set(ref newImageWidth, value);
            }
        }
        private double newImageHeight;
        public double NewImageHeight {
            get {
                return newImageHeight;
            }
            set {
                Set(ref newImageHeight, value);
            }
        }

        private int angle;
        public int Angle {
            get {
                return angle;
            }
            set {
                Set(ref angle, value);
            }
        }

        public ObservableCollection<string> ImageFilters { get; set; }

        private string selectedImageFilter;
        public string SelectedImageFilter {
            get {
                if (SelectedFilterIndex >= 0 && SelectedFilterIndex < ImageFilters.Count) {
                    return ImageFilters[SelectedFilterIndex];
                }
                return string.Empty;
            }
            set {
                Set(ref selectedImageFilter, value);

            }
        }

        private int selectedFilterIndex;
        public int SelectedFilterIndex {
            get {
                return selectedFilterIndex;
            }
            set {
                Set(ref selectedFilterIndex, value);
                OnPropertyChanged(nameof(SelectedFilterIndex));
                OnPropertyChanged(nameof(SelectedImageFilter));
            }
        }
        public MainWindowViewModel() {
            #region 添加所需要的图片格式
            ImageFormats = new ObservableCollection<MagickFormatItem>() {
                new MagickFormatItem { Name = "JPEG", Format = MagickFormat.Jpeg },
                new MagickFormatItem { Name = "PNG", Format = MagickFormat.Png },
                new MagickFormatItem { Name = "GIF", Format = MagickFormat.Gif },
                new MagickFormatItem { Name = "SVG", Format = MagickFormat.Svg },
                new MagickFormatItem { Name = "JPG", Format = MagickFormat.Jpg },
                new MagickFormatItem { Name = "BMP", Format = MagickFormat.Bmp },
                new MagickFormatItem { Name = "TGA", Format = MagickFormat.Tga },
                new MagickFormatItem { Name = "EPS", Format = MagickFormat.Eps },
                new MagickFormatItem { Name = "WEBP", Format = MagickFormat.WebP },
                new MagickFormatItem { Name = "TIFF", Format = MagickFormat.Tiff },
                new MagickFormatItem { Name = "AVIF", Format = MagickFormat.Avif },
                new MagickFormatItem { Name = "PSD", Format = MagickFormat.Psd },

            };
            #region 未做转换的格式
            //ImageFormats.Add("Jpg");
            //ImageFormats.Add("Png");
            //ImageFormats.Add("Gif");
            //ImageFormats.Add("Bmp");
            //ImageFormats.Add("Svg");
            //ImageFormats.Add("Tga");
            //ImageFormats.Add("Eps");
            ////ImageFormats.Add("Wbmp");
            //ImageFormats.Add("WebP");
            ////ImageFormats.Add("Heic");
            //ImageFormats.Add("Tiff");
            //ImageFormats.Add("Avif");
            //ImageFormats.Add("Psd"); 
            #endregion
            #endregion

            #region 添加所需要的图片滤镜
            ImageFilters = new ObservableCollection<string>() {
                "Undefined",
                "Point",
                "Box",
                "Triangle",
                "Hermite",
                "Hanning",
                "Hamming",
                "Blackman",
                "Gaussian",
                "Quadratic",
                "Cubic",
                "Catrom",
                "Mitchell",
                "Lanczos",
                "Bessel",
                "Sinc"
            };
            #endregion
        }

        private RelayCommand openFileButtonCommand = null;
        public RelayCommand OpenFileButtonCommand {
            get {
                return openFileButtonCommand ?? new RelayCommand(() => {
                    IsBtnEnale = false;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = false;
                    openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == true) {
                        FileImageName = openFileDialog.FileName;
                        foreach (string fileName in openFileDialog.FileNames) {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(fileName);
                            bitmap.EndInit();
                            FileImage = bitmap;
                        }
                        TextVisibility = Visibility.Collapsed;
                        IsBtnEnale = true;
                        string extension = Path.GetExtension(FileImageName);
                        PresentFormat = extension.Substring(1, extension.Length - 1).ToUpper();
                        NewImageWidth = new BitmapImage(new Uri(FileImageName)).PixelWidth;
                        NewImageHeight = new BitmapImage(new Uri(FileImageName)).PixelHeight;
                    }
                });
            }
        }


        private RelayCommand saveImageToFormatCommand = null;
        public RelayCommand SaveImageToFormatCommand {
            get {
                return saveImageToFormatCommand ?? new RelayCommand(() => {
                    if (!string.IsNullOrEmpty(SelectedImageFormat)) {
                        ConvertImageFormat(FileImageName, SelectedImageFormat);
                    }
                });
            }
        }

        private RelayCommand imageFilterTypeCommand = null;
        public RelayCommand ImageFilterTypeCommand {
            get {
                return imageFilterTypeCommand ?? new RelayCommand(() => {
                    ImageFilterType(FileImageName, NewImageWidth, NewImageHeight, Angle, SelectedImageFilter, SelectedImageFormat);
                });
            }
        }

        public void ImageFilterType(string sourceFilePath, double width, double heigth, int angle, string selectedImageFilter, string targetFormat) {
            if (Enum.TryParse(targetFormat, out MagickFormat format)) {
                using (MagickImage image = new MagickImage(sourceFilePath)) {
                    image.Format = format;
                    image.Resize((int)width, (int)heigth);
                    //image.Crop(new MagickGeometry(0, 0,(int)width, (int)heigth));
                    image.Rotate(angle);
                    image.FilterType = (FilterType)Enum.Parse(typeof(FilterType), selectedImageFilter);
                    var targetFilePath = Path.ChangeExtension(sourceFilePath, targetFormat.ToLowerInvariant());
                    image.Write(targetFilePath);
                    HandyControl.Controls.MessageBox.Show("图像已保存至：" + System.IO.Path.GetDirectoryName(targetFilePath));
                }
            }

        }


        public void ConvertImageFormat(string sourceFilePath, string targetFormat) {
            #region 未做格式转换
            //if (Enum.TryParse(targetFormat, out MagickFormat format)) {
            //    using (var image = new MagickImage(sourceFilePath)) {
            //        image.Format = format; // 修改图片格式
            //        var targetFilePath = Path.ChangeExtension(sourceFilePath, targetFormat.ToLowerInvariant()); // 修改文件扩展名
            //        image.Write(targetFilePath); // 保存修改后的图片
            //        HandyControl.Controls.MessageBox.Show("图像已保存至：" + System.IO.Path.GetDirectoryName(targetFilePath));
            //    }

            //} 
            #endregion

            using (MagickImage image = new MagickImage(sourceFilePath)) {
                image.Format = GetMagickFormat(targetFormat);
                image.Write(Path.ChangeExtension(sourceFilePath, targetFormat.ToLowerInvariant()));
                HandyControl.Controls.MessageBox.Show("图像已保存至：" + System.IO.Path.GetDirectoryName(Path.ChangeExtension(sourceFilePath, targetFormat.ToLowerInvariant())));
            }
        }

        private MagickFormat GetMagickFormat(string name) {
            return ImageFormats.FirstOrDefault(f => f.Name == name)?.Format ?? MagickFormat.Unknown;
        }

    }
}
