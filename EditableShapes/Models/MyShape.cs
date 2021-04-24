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

                bool isBetweenAndOnLine = IsPointBetween(p1.Position, p2.Position, currentPoint) && IsPointOnLine(p1.Position, p2.Position, currentPoint);

                if (isBetweenAndOnLine)
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

        public bool IsPointBetween(Point p1, Point p2, Point p3)
        {
            double zero = (p3.X - p1.X) * (p3.X - p2.X) + (p3.Y - p1.Y) * (p3.Y - p2.Y);

            return (zero < 0);
        }

        public bool IsPointOnLine(Point p1, Point p2, Point p3, float t = 1E+3f)
        {
            double zero = (p3.X - p2.X) * (p1.Y - p2.Y) - (p1.X - p2.X) * (p3.Y - p2.Y);

            if (zero > t || zero < -t) 
                return false;

            if (p2.X - p3.X > t || p3.X - p2.X > t)
                return p2.X > p3.X
                    ? p1.X + t > p3.X && p1.X - t < p2.X
                    : p1.X + t > p2.X && p1.X - t < p3.X;

            return p2.Y > p3.Y
                ? p1.Y + t > p3.Y && p1.Y - t < p2.Y
                : p1.Y + t > p2.Y && p1.Y - t < p3.Y;
        }
    }
}     
      