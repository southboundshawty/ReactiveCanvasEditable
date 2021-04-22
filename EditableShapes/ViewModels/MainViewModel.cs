using EditableShapes.Commands;
using EditableShapes.Models;

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EditableShapes.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            MyShapes = new ObservableCollection<MyShape>();
        }

        private ObservableCollection<MyShape> _myShapes;

        public ObservableCollection<MyShape> MyShapes
        {
            get => _myShapes;
            set
            {
                _myShapes = value;
                OnPropertyChanged();
            }
        }

        private MyShape _selectedShape;

        public MyShape SelectedShape
        {
            get => _selectedShape;
            set
            {
                _selectedShape = value;
                OnPropertyChanged();
            }
        }

        private ICommand onMapPressedCommand;
        public ICommand OnMapPressedCommand => onMapPressedCommand ??= new RelayCommand(OnMapPressed);

        private void OnMapPressed(object commandParameter)
        {
            MouseButtonEventArgs e = ((MouseButtonEventArgs)commandParameter);

            if (e.Source is Canvas source)
            {
                Point point = e.GetPosition(source);

                if (SelectedShape is null)
                {
                    MyShapes.Add(new MyShape()
                    {
                        Fill = Brushes.Bisque
                    });

                    SelectedShape = MyShapes.FirstOrDefault();
                }

                SelectedShape?.ShapePoints.Add(new ShapePoint()
                {
                    Fill = Brushes.DarkGreen,
                    Position = point
                });
            }
        }
    }
}
