#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    public class UsingTransaction : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("New Transaction Name");
                //Do something to Revit model
                tx.Commit();
            }
            return Result.Succeeded;
        }
    }
}
