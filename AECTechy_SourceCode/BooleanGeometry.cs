using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System;

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    class BooleanGeometry : IExternalCommand
    {
        public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Solid box = Box();
            Solid sphere = Sphere();

            Solid unionSolid = BooleanOperationsUtils.ExecuteBooleanOperation(box, sphere, BooleanOperationsType.Union);
            Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(box, sphere, BooleanOperationsType.Intersect);
            Solid diffSolid = BooleanOperationsUtils.ExecuteBooleanOperation(box, sphere, BooleanOperationsType.Difference);

            //Use DirectShape to visualise geometries
            using (Transaction t = new Transaction(doc, "Transaction"))
            {
                t.Start();
                GeometryObject[] unionGeomObj = new GeometryObject[] { unionSolid };
                GeometryObject[] interGeomObj = new GeometryObject[] { intersectSolid };
                GeometryObject[] diffGeomObj = new GeometryObject[] { diffSolid };
                DirectShape ds1 = DirectShape.CreateElement( doc , new ElementId( BuiltInCategory.OST_GenericModel ) );
                ds1.SetShape(unionGeomObj);
                DirectShape ds2 = DirectShape.CreateElement( doc , new ElementId( BuiltInCategory.OST_GenericModel ) );
                ds2.SetShape(interGeomObj);
                DirectShape ds3 = DirectShape.CreateElement( doc , new ElementId( BuiltInCategory.OST_GenericModel ) );
                ds3.SetShape(diffGeomObj);
                t.Commit();
            }

            return Result.Succeeded;
        }

        public static Solid Box( )
        {
            XYZ btmLeft = new XYZ(-0.5 , -0.5 , 0 );
            XYZ topRight = new XYZ( 0.5 , 0.5 , 0 );
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

            Solid box = GeometryCreationUtilities.CreateExtrusionGeometry(cl, XYZ.BasisZ, 1 );

            return box;
        }

        public static Solid Sphere()
        {
            XYZ center = new XYZ(0, 0, 0.5);
            double radius = 0.75;
            // Use the standard global coordinate system 
            // as a frame, translated to the sphere bottom.
            Frame frame = new Frame(center, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);

            // Create a vertical half-circle loop;
            // this must be in the frame location.
            XYZ start = center - radius * XYZ.BasisZ;
            XYZ end = center + radius * XYZ.BasisZ;
            XYZ XyzOnArc = center + radius * XYZ.BasisX;

            Arc arc = Arc.Create(start, end, XyzOnArc);

            Line line = Line.CreateBound(arc.GetEndPoint(1), arc.GetEndPoint(0));

            CurveLoop halfCircle = new CurveLoop();
            halfCircle.Append(arc);
            halfCircle.Append(line);

            List<CurveLoop> loops = new List<CurveLoop>(1);
            loops.Add(halfCircle);

            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, loops, 0, 2 * Math.PI);
        }
    }
}
