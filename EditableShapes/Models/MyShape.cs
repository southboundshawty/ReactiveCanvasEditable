using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using EditableShapes.Commands;

namespace EditableShapes.Models
{
    public class MyShape : ObservableModel
    {
        private Brush _fill;

        private bool _isClosed;

        private TrulyObservableCollection<ShapePoint> _shapePoints;

        private double _X;

        private double _y;

        private ICommand closeShapeCommand;

        private ICommand insertPointCommand;

        public MyShape()
        {
            ShapePoints = new TrulyObservableCollection<ShapePoint>();

            ShapePoints.CollectionChanged += ShapePoints_CollectionChanged;
        }

        public bool IsClosed
        {
            get => _isClosed;
            set
            {
                _isClosed = value;
                OnPropertyChanged();
            }
        }

        public double X
        {
            get => _X;
            set
            {
                _X = value;
                OnPropertyChanged();
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
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

        public TrulyObservableCollection<ShapePoint> ShapePoints
        {
            get => _shapePoints;
            set
            {
                _shapePoints = value;

                OnPropertyChanged();
            }
        }

        public PointCollection Points =>
            new(ShapePoints.Select(p => p.Position));

        public ICommand CloseShapeCommand => closeShapeCommand ??= new RelayCommand(CloseShape);
        public ICommand InsertPointCommand => insertPointCommand ??= new RelayCommand(InsertPoint);

        private void ShapePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Points));
        }

        private void CloseShape(object commandParameter)
        {
            IsClosed = !IsClosed;
        }

        private void InsertPoint(object commandParameter)
        {
            MouseButtonEventArgs e = (MouseButtonEventArgs) commandParameter;

            UIElement source = (UIElement) e.Source;

            Point currentPoint = e.GetPosition(source);

            for (int i = 0; i < ShapePoints.Count - 1; i++)
            {
                ShapePoint p1 = ShapePoints[i];
                ShapePoint p2 = ShapePoints[i + 1];

                bool isBetween = PointOnLine2D(p1.Position, p2.Position, currentPoint);

                if (isBetween)
                {
                    int index = ShapePoints.IndexOf(p2);

                    ShapePoints.Insert(index, new ShapePoint
                    {
                        Position = currentPoint,
                        Fill = Brushes.Black
                    });

                    return;
                }
            }
        }

        public bool PointOnLine2D(Point p, Point a, Point b, float t = 1E+3f)
        {
            double zero = (b.X - a.X) * (p.Y - a.Y) - (p.X - a.X) * (b.Y - a.Y);

            if (zero > t || zero < -t) return false;

            if (a.X - b.X > t || b.X - a.X > t)
                return a.X > b.X
                    ? p.X + t > b.X && p.X - t < a.X
                    : p.X + t > a.X && p.X - t < b.X;

            return a.Y > b.Y
                ? p.Y + t > b.Y && p.Y - t < a.Y
                : p.Y + t > a.Y && p.Y - t < b.Y;
        }
    }
}