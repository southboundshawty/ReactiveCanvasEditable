using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EditableShapes.Commands;
using EditableShapes.Models;

namespace EditableShapes.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<MyShape> _myShapes;

        private MyShape _selectedShape;

        private ICommand onMapPressedCommand;

        public MainViewModel()
        {
            MyShapes = new ObservableCollection<MyShape>();
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

        private void OnMapPressed(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs) commandParameter;

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
    }
}