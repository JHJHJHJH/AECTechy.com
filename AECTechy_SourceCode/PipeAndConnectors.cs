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
using Autodesk.Revit.UI.Selection;
using System.Linq;
#endregion

namespace AECTechy
{
    [Transaction(TransactionMode.Manual)]
    public class PipeAndConnectors : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            //Main
            Reference pickedObj = uidoc.Selection.PickObject(ObjectType.Element);
            ElementId eleid = pickedObj.ElementId;
            Element ele = doc.GetElement(eleid);


            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("New Transaction");

                Pipe newPipe = CreatePipe(doc);
                List<Connector> connectors = GetConnectors(ele);
                List<XYZ> connectorlocations = GetConnectorsXYZ(newPipe);

                tx.Commit();
            }
            return Result.Succeeded;
        }

        #region Get connectors MEPcurve
        //Input MEPCurve object, e.g. Pipe, Duct, CableTray, Wire
        //Function returns list of connectors from MEPCurve object
        public List<Connector> GetConnectors(MEPCurve mepCurve)
        {
            //1. Get connector set of MEPCurve
            ConnectorSet connectorSet = mepCurve.ConnectorManager.Connectors;
            //2. Initialise empty list of connectors
            List<Connector> connectorList = new List<Connector>();
            //3. Loop through connector set and add to list
            foreach (Connector connector in connectorSet)
            {
                connectorList.Add(connector);
            }
            return connectorList;
        }
        #endregion
        #region Get connectors Ele
        //Input MEP Element object 
        //Function returns list of connectors from MEP element
        public List<Connector> GetConnectors(Element element)
        {
            //1. Cast Element to FamilyInstance
            FamilyInstance inst = element as FamilyInstance;
            //2. Get MEPModel Property
            MEPModel mepModel = inst.MEPModel;
            //3. Get connector set of MEPModel
            ConnectorSet connectorSet = mepModel.ConnectorManager.Connectors;
            //4. Initialise empty list of connectors
            List<Connector> connectorList = new List<Connector>();
            //5. Loop through connector set and add to list
            foreach (Connector connector in connectorSet)
            {
                connectorList.Add(connector);
            }
            return connectorList;
        }
        #endregion
        #region CreatePipe

        //Input Document
        //Function returns pipe created
        public Pipe CreatePipe(Document doc)
        {
            //System Type (DomesticHotWater, DomesticColdWater, Sanitary, etc)
            MEPSystemType mepSystemType = new FilteredElementCollector(doc)
                .OfClass(typeof(MEPSystemType))
                .Cast<MEPSystemType>()
                .FirstOrDefault(sysType => sysType.SystemClassification == MEPSystemClassification.DomesticColdWater);

            //Pipe Type (Standard, ChilledWater)
            PipeType pipeType = new FilteredElementCollector(doc)
                .OfClass(typeof(PipeType))
                .Cast<PipeType>()
                .FirstOrDefault();

            //Level
            Level level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault();

            Pipe newPipe = Pipe.Create(doc, mepSystemType.Id, pipeType.Id, level.Id, XYZ.Zero, new XYZ(0, 0, 50));

            return newPipe;
        }

        #endregion
        #region GetConnectorLocation
        public List<XYZ> GetConnectorsXYZ(MEPCurve mepCurve)
        {
            ConnectorSet connectorSet = mepCurve.ConnectorManager.Connectors;
            List<XYZ> connectorPointList = new List<XYZ>();
            foreach (Connector connector in connectorSet) 
            {
                XYZ connectorPoint = connector.Origin;
                connectorPointList.Add(connectorPoint);
            }

            return connectorPointList;
        }
        #endregion

        
    }
}
