using EditableShapes.Commands;
using EditableShapes.Models;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using Microsoft.Win32;

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace EditableShapes.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ImageSource _map;
        private ObservableCollection<MyShape> _myShapes;

        private MyShape _selectedShape;

        private ICommand browseMapCommand;

        private Image<Bgr, byte> mapImage;

        private ICommand onMapPressedCommand;

        private ICommand recognizeShapesCommand;

        public MainViewModel()
        {
            MyShapes = new ObservableCollection<MyShape>();

            GrayIntensity = 230;
            KernelGauss = 5;

            MagicNumber = 0.0009;
        }

        public ImageSource Map
        {
            get => _map;
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _grayMap;

        public ImageSource GrayMap
        {
            get { return _grayMap; }
            set
            {
                _grayMap = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MyShape> MyShapes
        {
            get => _myShapes;
            set
            {
                _myShapes = value;
                OnPropertyChanged();
            }
        }

        public MyShape SelectedShape
        {
            get => _selectedShape;
            set
            {
                _selectedShape = value;
                OnPropertyChanged();
            }
        }

        public ICommand OnMapPressedCommand => onMapPressedCommand ??= new RelayCommand(OnMapPressed);
        public ICommand BrowseMapCommand => browseMapCommand ??= new RelayCommand(BrowseMap);

        public ICommand RecognizeShapesCommand => recognizeShapesCommand
            ??= new RelayCommand(RecognizeShapes,
                obj =>
                    Map is not null &&
                    mapImage is not null);

        private void OnMapPressed(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs)commandParameter;

            if (e.Source is Canvas source)
            {
                Point point = e.GetPosition(source);

                if (SelectedShape is null)
                {
                    MyShapes.Add(new MyShape
                    {
                        Fill = Brushes.Bisque
                    });

                    SelectedShape = MyShapes.FirstOrDefault();
                }

                SelectedShape?.ShapePoints.Add(new ShapePoint
                {
                    Fill = Brushes.DarkGreen,
                    Position = point
                });
            }
        }

        private void BrowseMap(object commandParameter)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Картинки (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (!(openFileDialog.ShowDialog().HasValue || openFileDialog.ShowDialog().Value))
            {
                return;
            }

            string path = openFileDialog.FileName;

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Картинка не была выбрана.");

                return;
            }

            mapImage = new Image<Bgr, byte>(path);

            Map = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));

            PreviewGrayImage();
        }

        private double _grayIntensity;

        public double GrayIntensity
        {
            get { return _grayIntensity; }
            set
            {
                _grayIntensity = value;
                OnPropertyChanged();

                PreviewGrayImage();
            }
        } 

        private int _kernelGauss;

        public int KernelGauss
        {
            get { return _kernelGauss; }
            set
            {
                _kernelGauss = value;
                OnPropertyChanged();

                PreviewGrayImage();
            }
        }

        private void PreviewGrayImage()
        {
            if (mapImage is not null)
            {
                Image<Gray, byte> grayImage = mapImage.SmoothGaussian(KernelGauss).Convert<Gray, byte>()
                    .ThresholdBinaryInv(new Gray(GrayIntensity), new Gray(256));

                GrayMap = ToBitmapImage(grayImage.ToBitmap());
            }
        }

        private double _magicNumber;

        public double MagicNumber
        {
            get { return _magicNumber; }
            set
            {
                _magicNumber = value;
                OnPropertyChanged();
            }
        }

        private void RecognizeShapes(object commandParameter)
        {
            MyShapes.Clear();

            Image<Gray, byte> grayImage = mapImage.SmoothGaussian(KernelGauss).Convert<Gray, byte>()
                .ThresholdBinaryInv(new Gray(GrayIntensity), new Gray(256));

            VectorOfVectorOfPoint contours = new();

            UMat hierarchy = grayImage.ToUMat();

            CvInvoke.FindContours(grayImage, contours, hierarchy, RetrType.Ccomp,
                ChainApproxMethod.ChainApproxSimple);

            int count = contours.Size;

            for (int i = 0; i < count; i++)
            {
                MyShape s = new()
                {
                    Fill = Brushes.Gray,
                    IsClosed = true
                };

                using (VectorOfPoint contour = contours[i])
                {
                    using (VectorOfPoint approxContour = new())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * MagicNumber, true);
                        if (CvInvoke.ContourArea(approxContour) > 10000)
                        {
                            for (int j = 0; j < approxContour.Size; j++)
                            {
                                Point vertex = new(approxContour[j].X, approxContour[j].Y);

                                s.ShapePoints.Add(new ShapePoint
                                {
                                    Fill = Brushes.Green,
                                    Position = vertex
                                });
                            }
                        }
                    }
                }

                GrayMap = ToBitmapImage(grayImage.ToBitmap());

                MyShapes.Add(s);
            }
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using MemoryStream memory = new();

            bitmap.Save(memory, ImageFormat.Png);

            memory.Position = 0;

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();

            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
    }
}