using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;

namespace EditableShapes.Models
{
    public class MyShape : ObservableModel
    {
        public MyShape()
        {
            ShapePoints = new TrulyObservableCollection<ShapePoint>();

            ShapePoints.CollectionChanged += ShapePoints_CollectionChanged;
            
        }

        private void ShapePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        { 
            OnPropertyChanged(nameof(Points));


        }

        private double _x;

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                OnPropertyChanged();
            }
        }

        private double _y;

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
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

        private TrulyObservableCollection<ShapePoint> _shapePoints;

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
            new PointCollection(ShapePoints.Select(p => p.Position));
    }
}
