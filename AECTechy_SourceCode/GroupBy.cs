using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;


namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    public class GroupBy : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Duct> allDucts = new FilteredElementCollector(doc)
                .OfClass(typeof(Duct))
                .WhereElementIsNotElementType()
                .Cast<Duct>()
                .ToList();

            var ductGroups =
                from duct in allDucts
                group duct by duct.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM).AsString();
            
            foreach (var sysGrp in ductGroups)
            {
                foreach(Duct ductEle in sysGrp)
                {
                    //ductEle : Access indivdual duct elements
                    //sysGrp.Key : System Classification, i.e. Supply Air, Return Air, Exhaust Air, etc
                }
            }

             return Result.Succeeded;
        }
    }
}
