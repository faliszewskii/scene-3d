using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scene_3d.model
{
    internal class Mesh
    {
        public Mesh(List<Polygon> polygons, PointF position)
        {
            Polygons = polygons;
            PositionOnScreen = position;
        }

        public List<Polygon> Polygons { get; set; }    
        public PointF PositionOnScreen { get; set; }
    }
}
