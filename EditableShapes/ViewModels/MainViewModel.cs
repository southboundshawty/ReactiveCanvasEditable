

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EditableShapes.Commands;
using EditableShapes.Models;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Win32;
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

        private ICommand onMapPressedCommand;

        private ICommand recognizeShapesCommand;

        private Image<Bgr, byte> mapImage;

        public MainViewModel()
        {
            MyShapes = new ObservableCollection<MyShape>();
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
        }

        private void RecognizeShapes(object commandParameter)
        {
            Image<Gray, byte> grayImage = mapImage.SmoothGaussian(5).Convert<Gray, byte>()
                .ThresholdBinaryInv(new Gray(230), new Gray(255));

            VectorOfVectorOfPoint contours = new();

            Mat hierarchy = new Mat();

            CvInvoke.FindContours(grayImage, contours, hierarchy, RetrType.External,
                ChainApproxMethod.ChainApproxSimple);

            for (int i = 0; i < contours.Size; i++)
            {
                MyShape s = new MyShape
                {
                    Fill = Brushes.Gray
                };

                for (int j = 0; j < contours[i].Size; j++)
                {
                    var vertex = new Point(contours[i][j].X, contours[i][j].Y);

                    s.ShapePoints.Add(new ShapePoint()
                    {
                        Fill = Brushes.Green,
                        Position = vertex
                    });
                }

                MyShapes.Add(s);
            }
        }
    }
}