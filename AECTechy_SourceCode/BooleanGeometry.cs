using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    class BooleanGeometry : IExternalCommand
    {
        public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Solid box1 = CreateBox1();
            Solid box2 = CreateBox2();

            Solid newSolid = BooleanOperationsUtils.ExecuteBooleanOperation(box1, box2, BooleanOperationsType.Union);

            GeometryObject[] geomObj = new GeometryObject[] { newSolid };

            using (Transaction t = new Transaction(doc, "Transaction"))
            {
                t.Start();
                DirectShape ds = DirectShape.CreateElement( doc , new ElementId( BuiltInCategory.OST_GenericModel ) );
                ds.SetShape(geomObj);

                t.Commit();
            }

            return Result.Succeeded;
        }

        public static Solid CreateBox1( )
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

            Solid box = GeometryCreationUtilities.CreateExtrusionGeometry(cl, XYZ.BasisZ, 4.5 );

            return box;

        }
        public static Solid CreateBox2()
        {
            XYZ btmLeft = new XYZ(2, -1, 0);
            XYZ topRight = new XYZ(7.5, 6.5, 0);
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

            Solid box = GeometryCreationUtilities.CreateExtrusionGeometry(cl, XYZ.BasisZ, 7);

            return box;

        }
    }
}
