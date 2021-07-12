using EditableShapes.Api;
using EditableShapes.Commands;
using EditableShapes.Hubs;
using EditableShapes.Models;
using EditableShapes.Models.Dto;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
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
using AutoMapper;
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

            _mapEntitiesHub = MapEntitiesHub.GetInstance();

            _mapEntitiesHub.OnAreaReceived += _mapEntitiesHub_OnAreaReceived;

            _mapEntitiesHub.OnAreaDeleted += _mapEntitiesHub_OnAreaDeleted;

            Initialize();
        }

        private void _mapEntitiesHub_OnAreaDeleted(AreaDto areaDto)
        {
            MyShape shape = MyShapes.FirstOrDefault(s => s.Id == areaDto.Id);

            MyShapes.Remove(shape);
        }

        private void _mapEntitiesHub_OnAreaReceived(AreaDto areaDto)
        {
            MyShape myShape = new()
            {
                Id = areaDto.Id,
                Name = areaDto.Name,
                IsClosed = areaDto.IsClosed
            };

            MyShape firstShape = MyShapes.FirstOrDefault(s => s.Id == myShape.Id);

            if (firstShape is null)
            {
                MyShapes.Add(myShape);
            }
            else
            {
                firstShape.IsClosed = myShape.IsClosed;
            }
        }

        private async void Initialize()
        {
            using AreaApi areaApi = new();

            IEnumerable<AreaDto> areas = await areaApi.ReadAsync();

            MyShapes.Clear();

            foreach (AreaDto area in areas)
            {
                MyShape myShape = new()
                {
                    Id = area.Id,
                    Name = area.Name,
                    IsClosed = area.IsClosed
                };

                using AreaPointApi areaPointApi = new();

                IEnumerable<AreaPointDto> areaPoints = await areaPointApi.ReadAsync(area.Id);

                if (areaPoints is null)
                    continue;

                foreach (AreaPointDto areaPoint in areaPoints)
                {
                    myShape.ShapePoints.Add(new ShapePoint()
                    {
                        Id = areaPoint.Id,
                        AreaId = areaPoint.AreaId,
                        Position = new Point(areaPoint.X, areaPoint.Y)
                    });
                }

                MyShapes.Add(myShape);
            }
        }

        private readonly MapEntitiesHub _mapEntitiesHub;

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
            get => _grayMap;
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

        private async void OnMapPressed(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs)commandParameter;

            if (e.Source is Canvas source)
            {
                Point point = e.GetPosition(source);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (SelectedShape is null)
                    {
                        using AreaApi areaApi = new();

                        await areaApi.CreateAsync(new AreaDto()
                        {
                            Name = Guid.NewGuid().ToString()
                        });

                        SelectedShape = MyShapes.LastOrDefault();
                    }

                    if (SelectedShape is null)
                        return;

                    using AreaPointApi areaPointApi = new();

                    await areaPointApi.CreateAsync(new AreaPointDto()
                    {
                        X = (int)point.X,
                        Y = (int)point.Y,
                        AreaId = SelectedShape.Id
                    });
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (SelectedShape is not null)
                    {
                        using AreaApi areaApi = new();

                        await areaApi.UpdateAsync(new AreaDto()
                        {
                            Id = SelectedShape.Id,
                            Name = SelectedShape.Name,
                            IsClosed = true
                        });

                        SelectedShape = null;
                    }
                }
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
            get => _grayIntensity;
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
            get => _kernelGauss;
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
            get => _magicNumber;
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

                using VectorOfPoint contour = contours[i];

                using VectorOfPoint approxContour = new();

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