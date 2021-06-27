using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using EditableShapes.Api;
using EditableShapes.Commands;
using EditableShapes.Models.Dto;

namespace EditableShapes.Models
{
    public class ShapePoint : ObservableModel
    {
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private int _areaId;

        public int AreaId
        {
            get => _areaId;
            set
            {
                _areaId = value;
                OnPropertyChanged();
            }
        }

        private Brush _fill;
        private Point _position;

        private ICommand onDragCommand;
        private ICommand onDragCompletedCommand;

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        public Brush Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                OnPropertyChanged();
            }
        }

        public ICommand OnDragCommand => onDragCommand ??= new RelayCommand(OnDrag);

        public ICommand OnDragCompletedCommand => onDragCompletedCommand ??= new RelayCommand(OnDragCompleted);

        private async void OnDragCompleted(object obj)
        {
          
        }

        private async void OnDrag(object commandParameter)
        {
            DragDeltaEventArgs e = (DragDeltaEventArgs) commandParameter;

            ShapePoint n = (ShapePoint) ((FrameworkElement) e.Source).DataContext;

            double x = n.Position.X + e.HorizontalChange;
            double y = n.Position.Y + e.VerticalChange;

            n.Position = new Point(x, y);

            using AreaPointApi areaPointApi = new();

            await areaPointApi.UpdateAsync(new AreaPointDto()
            {
                Id = Id,
                AreaId = AreaId,
                X = (int)Position.X,
                Y = (int)Position.Y
            });
        }
    }
}