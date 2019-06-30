using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    class DirShape : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Solid box = CreateBox( );
            GeometryObject[] boxObj = new GeometryObject[] { box };

            using (Transaction t = new Transaction(doc, "Transaction"))
            {
                t.Start();
                
                DirectShape ds = DirectShape.CreateElement( doc , new ElementId( BuiltInCategory.OST_GenericModel ) );
                ds.SetShape( boxObj );

                t.Commit();
            }

            return Result.Succeeded;
        }

        public static Solid CreateBox( )
        {
            XYZ btmLeft = new XYZ(-3 , -3 , 0 );
            XYZ topRight = new XYZ( 3 , 3 , 0 );
            XYZ btmRight = new XYZ(topRight.X, btmLeft.Y, 0);
            XYZ topLeft = new XYZ(btmLeft.X, topRight.Y, 0);

            Curve btm = Line.CreateBound(btmLeft, btmRight) as Curve;
            Curve right = Line.CreateBound(btmRight, topRight) as Curve;
            Curve top = Line.CreateBound(topRight, topLeft) as Curve;
            Curve left = Line.CreateBound(topLeft, btmLeft) as Curve;

            CurveLoop crvLoop = new CurveLoop();

            crvLoop.Append(btm);
            crvLoop.Append(right);
            crvLoop.Append(top);
            crvLoop.Append(left);

            IList<CurveLoop> cl = new List<CurveLoop>();
            cl.Add(crvLoop);

            Solid box = GeometryCreationUtilities.CreateExtrusionGeometry(cl, XYZ.BasisZ, 5 );

            return box;

        }
    }
}
