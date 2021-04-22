using EditableShapes.Commands;

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace EditableShapes.Models
{
    public class ShapePoint : ObservableModel
    {
        private Point _position;

        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }

        private Brush _fill;

        public Brush Fill
        {
            get => _fill;
            set
            {
                _fill = value;
                OnPropertyChanged();
            }
        }

        private ICommand onDragCommand;
        public ICommand OnDragCommand => onDragCommand ??= new RelayCommand(OnDrag);

        private void OnDrag(object commandParameter)
        {
            DragDeltaEventArgs e = ((DragDeltaEventArgs)commandParameter);

            ShapePoint n = (ShapePoint)((FrameworkElement)e.Source).DataContext;

            double x = n.Position.X + e.HorizontalChange;
            double y = n.Position.Y + e.VerticalChange;

            n.Position = new Point(x, y);
        }
    }
}
