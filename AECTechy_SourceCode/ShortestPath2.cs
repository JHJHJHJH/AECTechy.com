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
    class ShortestPath2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            IList<Reference> selection1 = uidoc.Selection.PickObjects(ObjectType.Element);      //Select 1st list
            IList<XYZ> roomsXyz1 = selection1                                                   //Room 1,2 & 3
                .Select(r => doc.GetElement(r.ElementId).Location)
                .Cast<LocationPoint>()
                .Select(locPoint => locPoint.Point)
                .ToList();

            IList<Reference> selection2 = uidoc.Selection.PickObjects( ObjectType.Element );    //Select 2nd list
            IList<XYZ> roomsXyz2 = selection2                                                   //Room 4,6 & 8
                .Select(r => doc.GetElement(r.ElementId).Location)                              
                .Cast<LocationPoint>()
                .Select(locPoint => locPoint.Point)
                .ToList();

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Find shortest Path !");

                //IList<PathOfTravel> routeMap = PathOfTravel.CreateMapped(doc.ActiveView, roomsXyz1, roomsXyz2);
                IList<PathOfTravel> routeMultiple = PathOfTravel.CreateMultiple(doc.ActiveView, roomsXyz1, roomsXyz2);

                tx.Commit();
            }
            
            return Result.Succeeded;
        }
    }
}
