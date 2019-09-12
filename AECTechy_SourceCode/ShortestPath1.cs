#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI.Selection;
using System.Linq;
#endregion

namespace AECTechy_SourceCode
{
    [Transaction(TransactionMode.Manual)]
    class ShortestPath1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            
            //Select first room
            Reference selection = uidoc.Selection.PickObject( ObjectType.Element);      //Select Element 1
            ElementId eleId = selection.ElementId;
            LocationPoint eleLocPt = doc.GetElement(eleId).Location as LocationPoint;
            XYZ roomXyzPoint1 = eleLocPt.Point;

            //Select second room
            Reference selection2 = uidoc.Selection.PickObject( ObjectType.Element);     //Select Element 2
            ElementId eleId2 = selection2.ElementId;
            LocationPoint eleLocPt2 = doc.GetElement(eleId2).Location as LocationPoint;
            XYZ roomXyzPoint2 = eleLocPt2.Point;

            //Find shortest Path
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Find shortest Path !");

                PathOfTravel route = PathOfTravel.Create( doc.ActiveView, roomXyzPoint1, roomXyzPoint2);
                
                tx.Commit();
            }
            
            return Result.Succeeded;
        }
    }
}
