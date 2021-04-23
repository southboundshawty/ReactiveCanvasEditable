using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using EditableShapes.Commands;

namespace EditableShapes.Models
{
    public class ShapePoint : ObservableModel
    {
        private Brush _fill;
        private Point _position;

        private ICommand onDragCommand;

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

        private void OnDrag(object commandParameter)
        {
            DragDeltaEventArgs e = (DragDeltaEventArgs) commandParameter;

            ShapePoint n = (ShapePoint) ((FrameworkElement) e.Source).DataContext;

            double x = n.Position.X + e.HorizontalChange;
            double y = n.Position.Y + e.VerticalChange;

            n.Position = new Point(x, y);
        }
    }
}