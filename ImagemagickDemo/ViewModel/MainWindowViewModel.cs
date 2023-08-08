using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ImageMagick;
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

namespace ImagemagickDemo.ViewModel {
    public class MainWindowViewModel : ViewModelBase {
        public ObservableCollection<string> ImageFormats { get; set; }
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

        private Visibility textVisibility=Visibility.Visible;
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
                    return ImageFormats[SelectedIndex];
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

        public MainWindowViewModel() {
            #region 添加所需要的图片格式
            ImageFormats = new ObservableCollection<string>();
            ImageFormats.Add("Jpg");
            ImageFormats.Add("Png");
            ImageFormats.Add("Gif");
            ImageFormats.Add("Bmp");
            ImageFormats.Add("Svg");
            ImageFormats.Add("Tga");
            ImageFormats.Add("Eps");
            //ImageFormats.Add("Wbmp");
            ImageFormats.Add("WebP");
            //ImageFormats.Add("Heic");
            ImageFormats.Add("Tiff");
            ImageFormats.Add("Avif");
            ImageFormats.Add("Psd");
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

        public void ConvertImageFormat(string sourceFilePath, string targetFormat) {
            if (Enum.TryParse(targetFormat, out MagickFormat format)) {
                using (var image = new MagickImage(sourceFilePath)) {
                    image.Format = format; // 修改图片格式
                    var targetFilePath = Path.ChangeExtension(sourceFilePath, targetFormat.ToLowerInvariant()); // 修改文件扩展名
                    image.Write(targetFilePath); // 保存修改后的图片
                    HandyControl.Controls.MessageBox.Show("图像已保存至：" + System.IO.Path.GetDirectoryName(targetFilePath));
                }

            }
        }


    }
}
