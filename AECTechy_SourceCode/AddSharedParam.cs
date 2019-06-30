#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
#endregion

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    public class AddSharedParamCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            FamilyManager famMgr = doc.FamilyManager;

            DefinitionFile defFile = app.OpenSharedParameterFile( );
            DefinitionGroups defGrps = defFile.Groups;

            foreach (DefinitionGroup defGrp in defGrps )
            {
                var externalDef = from ExternalDefinition extDef in defGrp.Definitions select extDef;

                foreach( ExternalDefinition extDef in externalDef )
                {
                    using( Transaction tx = new Transaction(doc) )
                    {
                        tx.Start("New Param");

                    
                        FamilyParameter famParam = famMgr.AddParameter(extDef , BuiltInParameterGroup.PG_GENERAL , false);

                        tx.Commit( );
                    }
                }
            }


            //Main
            //Reference pickedObj = uidoc.Selection.PickObject(ObjectType.Element);
            //ElementId eleid = pickedObj.ElementId;
            //Element ele = doc.GetElement(eleid);

            





            return Result.Succeeded;
        }

        #region Add param
        //public void AddSharedParameters( )
        //{

        //    UIDocument uiDoc = this.ActiveUIDocument;   //dont need this
        //    Document doc = this.ActiveUIDocument.Document;

        //    insert the shared parameters from the file into the family document
        //    Category cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_LightingFixtures);  //change the category here
        //    if you are using something besides light fixtures
        //    DefinitionFile spFile = this.Application.OpenSharedParameterFile( );
        //    FamilyManager famMan = doc.FamilyManager;
        //    foreach( DefinitionGroup dG in spFile.Groups )  //get each group in the shared parameter file 
        //    {
        //        var v = ( from ExternalDefinition d in dG.Definitions select d );

        //        using( Transaction t = new Transaction(doc) )
        //        {
        //            t.Start("Add Shared Parameters");
        //            foreach( ExternalDefinition eD in v )  //get each parameter in the current group
        //            {
        //                FamilyParameter fp = famMan.AddParameter(eD , BuiltInParameterGroup.PG_GENERAL , false);  //change the
        //                group here if you want the parameters in a different group

        //            }//end foreach
        //            t.Commit( );
        //        }//end using transaction

        //    }// end foreach

        //    add the parameters to an Ilist
        //    IList<FamilyParameter> familyPar = famMan.GetParameters( );

        //    list all the parameters in order to show the orignal order
        //    String strNames = null;
        //    foreach( FamilyParameter fp in familyPar )
        //    {
        //        strNames = strNames + fp.Definition.Name.ToString( ) + "\n";
        //    }
        //    TaskDialog.Show("Original Order" , strNames);



        //    start creating a list of sorted parameters here------------------------------------------------------------

        //    IList < FamilyParameter > sortedfamilyPar = new List<FamilyParameter>( );  // create an ILIST for the sorted parameters

        //    foreach( FamilyParameter fp in familyPar )  //go through each entry and find "TYPE"
        //        and add it to the empty ILIST
        //    {
        //        if( fp.Definition.Name.ToString( ) == "TYPE" )
        //        {
        //            sortedfamilyPar.Insert(sortedfamilyPar.Count , fp);
        //            break;
        //        }
        //    }

        //    foreach( FamilyParameter fp in familyPar )
        //    {
        //        if( fp.Definition.Name.ToString( ) == "STYLE" )  //find STYLE and add it second
        //        {
        //            sortedfamilyPar.Insert(sortedfamilyPar.Count , fp);
        //            break;
        //        }
        //    }

        //    foreach( FamilyParameter fp in familyPar )  //...
        //    {
        //        if( fp.Definition.Name.ToString( ) == "MOUNTING" )
        //        {
        //            sortedfamilyPar.Insert(sortedfamilyPar.Count , fp);
        //            break;
        //        }
        //    }

        //    foreach( FamilyParameter fp in familyPar )
        //    {
        //        if( fp.Definition.Name.ToString( ) == "LENS" )
        //        {
        //            sortedfamilyPar.Insert(sortedfamilyPar.Count , fp);
        //            break;
        //        }
        //    }
        //    foreach( FamilyParameter fp in familyPar ) //the list must contain all parameters,
        //        even if they are not visible to the user
        //                                               add all other parameters in familyPar to sortedfamilyPar
        //    {
        //        if( !sortedfamilyPar.Contains(fp) )
        //        {
        //            sortedfamilyPar.Insert(sortedfamilyPar.Count , fp);
        //        }
        //    }

        //    show a list of all the parameters in order to show they are sorted
        //    String sortedstrNames = null;
        //    foreach( FamilyParameter fp in sortedfamilyPar )
        //    {
        //        sortedstrNames = sortedstrNames + fp.Definition.Name.ToString( ) + "\n";
        //    }
        //    TaskDialog.Show("Sorted Order" , sortedstrNames);

        //    Finally, apply the sorted parameter list to the family document

        //    using( Transaction t = new Transaction(doc) )
        //    {
        //        t.Start("Sort Parameters");
        //        famMan.ReorderParameters(sortedfamilyPar);
        //        t.Commit( );
        //    }

        //}//end addshared parameters
        #endregion
    }
}
