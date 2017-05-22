using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace KirkseyAppsRibbon
{
    class ViewportUpdater : IUpdater
    {
        // private data:
        private UpdaterId m_updaterId = null;
        private TimeSpan humanClick = new TimeSpan(10000);

        private ICollection<ElementId> previousIds = null;
        private DateTime previousTimeStamp = DateTime.MinValue;

        // map with key = sheet element id and
        // value = list of viewport element ids:
        Dictionary<ElementId, List<ElementId>> mapSheetToViewport = new Dictionary<ElementId, List<ElementId>>();

        // map with key = viewport element id and
        // value = sheet element id:
        Dictionary<ElementId, ElementId> mapViewportToSheet = new Dictionary<ElementId, ElementId>();


        internal ViewportUpdater(AddInId addinID)
        {
            m_updaterId = new UpdaterId(addinID, new Guid("BCB1FA75-3901-45D7-BC20-6F18EBEE7350"));
        }

        internal void Register(Document doc)
        {
            //Register the viewport updater if the updater is not registered
            if (!UpdaterRegistry.IsUpdaterRegistered(m_updaterId))
                UpdaterRegistry.RegisterUpdater(this, doc);
        }


        internal void AddTriggerForUpdater(Document doc, ElementFilter filter)
        {
            UpdaterRegistry.AddTrigger(m_updaterId, doc, filter, Element.GetChangeTypeAny());
            UpdaterRegistry.AddTrigger(m_updaterId, doc, filter, Element.GetChangeTypeElementAddition());
        }


        #region IUpdater members
        public void Execute(UpdaterData data)
        {
            try
            {

                DateTime currentTimeStamp = DateTime.Now;
                var difference = currentTimeStamp - previousTimeStamp;
                Document doc = data.GetDocument();
                ICollection<ElementId> ids = data.GetModifiedElementIds();

                foreach (ElementId id in ids)
                {
                    if (data.IsChangeTriggered(id, Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.VIEWPORT_DETAIL_NUMBER))))
                    {
                        break;
                    }
                    else
                    {
                        if (difference > humanClick)
                        {
                            upDateDrawingNumber(id, doc);
                        }
                    }
                }

                //if (CompareCollection(ids, previousIds))
                //{
                //Collections are equal, check difference in time
                //var difference = currentTimeStamp - previousTimeStamp;
                //if (difference > humanClick)
                //{
                //upDateDrawingNumber(ids, doc);
                //}
                //}
                //else
                //upDateDrawingNumber(ids, doc);
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Exception", ex.ToString());
            }
            return;
        }

        private void upDateDrawingNumber(ICollection<ElementId> ids, Document doc)
        {
            foreach (ElementId id in ids)
            {
                Viewport viewport = doc.GetElement(id) as Viewport;
                var sheetID = viewport.SheetId;
                View associatedView = doc.GetElement(viewport.ViewId) as View;

                IList<Parameter> ps = associatedView.GetParameters("UseDetailNumber");

                int shouldUpdate = 0;

                if (ps.Count > 0)
                {
                    shouldUpdate = ps[0].AsInteger();
                }
                if (shouldUpdate == 1)
                {
                    renumberViewport(viewport, doc);
                }
            }
            previousIds = ids;
            previousTimeStamp = DateTime.Now;
        }

        private void upDateDrawingNumber(ElementId id, Document doc)
        {
            View activeview = doc.ActiveView;
            if (activeview.ViewType == ViewType.DrawingSheet)
            {
                Viewport viewport = doc.GetElement(id) as Viewport;
                var sheetID = viewport.SheetId;
                if (sheetID.IntegerValue > 0)
                {
                    View associatedView = doc.GetElement(viewport.ViewId) as View;

                    IList<Parameter> ps = associatedView.GetParameters("UseDetailNumber");

                    int shouldUpdate = 0;

                    if (ps.Count > 0)
                    {
                        shouldUpdate = ps[0].AsInteger();
                    }
                    if (shouldUpdate == 1)
                    {
                        renumberViewport(viewport, doc);
                    }
                }
            }



        }

        static bool CompareCollection(ICollection<ElementId> currentIds, ICollection<ElementId> previousIds)
        {
            if (currentIds != null)
            {
                if (previousIds != null)
                {
                    IEnumerable<ElementId> currentIdsList = currentIds.ToList();
                    IEnumerable<ElementId> previousIdsList = previousIds.ToList();

                    var cnt = new Dictionary<ElementId, int>();
                    foreach (ElementId id in currentIdsList)
                    {
                        if (cnt.ContainsKey(id))
                        {
                            cnt[id]++;
                        }
                        else
                        {
                            cnt.Add(id, 1);
                        }
                    }

                    foreach (ElementId id in previousIdsList)
                    {
                        if (cnt.ContainsKey(id))
                        {
                            cnt[id]--;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    return cnt.Values.All(c => c == 0);
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void renumberViewport(Viewport view, Document doc)
        {
            string originalDetailNumber = view.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).AsString();
            TitleBlockProperties tbp = null;
            Outline outline = view.GetBoxOutline();
            XYZ LRCorner = new XYZ(outline.MaximumPoint.X, outline.MinimumPoint.Y, 0);

            ViewSheet vs = doc.GetElement(view.SheetId) as ViewSheet;
            Element titleBlock;

            FilteredElementCollector col = new FilteredElementCollector(doc);
            col.OfClass(typeof(FamilyInstance));
            col.OfCategory(BuiltInCategory.OST_TitleBlocks);
            col.OwnedByView(vs.Id);

            FilteredElementCollector vpCol = new FilteredElementCollector(doc);
            vpCol.OfCategory(BuiltInCategory.OST_Viewports);
            vpCol.OwnedByView(vs.Id);

            IList<Element> viewPorts = vpCol.ToElements();

            IList<Element> title_blocks = col.ToElements();

            if (title_blocks.Count > 0)
            {
                titleBlock = title_blocks[0];
                var p = titleBlock.get_Parameter(BuiltInParameter.SHEET_WIDTH);
                //This is causing issues.  If the sheet isn't the active view, but a view on a sheet is.
                View activeview = doc.ActiveView;
                if (activeview.ViewType != ViewType.DrawingSheet)
                {
                    //Sets up mapping of sheets with views and views on sheets.
                    GetViewMapping(doc);

                    ElementId viewId = mapViewportToSheet[activeview.Id];

                    activeview = doc.GetElement(viewId) as View;

                    //https://forums.autodesk.com/t5/revit-api-forum/how-do-i-determine-if-the-active-view-is-being-accessed-through/m-p/6449863/highlight/true#M17471
                    //http://thebuildingcoder.typepad.com/blog/2009/01/viewports-and-sheets.html
                    //var sheet = activeview.
                }

                BoundingBoxXYZ box = titleBlock.get_BoundingBox(activeview);
                double dwidth = p.AsDouble();
                tbp = setSheetParameters(dwidth, tbp, box);
            }

            string cellId = getCellID(LRCorner, tbp);
            if (originalDetailNumber != cellId)
            {
                if (cellId != null)
                {
                    foreach (Element viewPort in viewPorts)
                    {
                        Viewport v = viewPort as Viewport;
                        string detailNumber = v.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).AsString();
                        if (cellId == detailNumber)
                        {
                            cellId = cellId + ".1";
                        }
                    }
                    view.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).Set(cellId);
                }
            }
        }

        private TitleBlockProperties setSheetParameters(double widthV, TitleBlockProperties tb, BoundingBoxXYZ box)
        {
            //double height = Math.Round(heightV, 1);
            double width = Math.Round(widthV, 1);
            tb = new TitleBlockProperties(width.ToString(), box);
            return tb;
        }

        private string getCellID(XYZ LRCorner, TitleBlockProperties tb)
        {
            string foundCell = null;
            foreach (KeyValuePair<string, Outline> cell in tb.CellIDs)
            {
                if (cell.Value.Contains(LRCorner, 0))
                {
                    foundCell = cell.Key;
                    break;
                }
            }

            return foundCell;
        }

        public string GetAdditionalInformation()
        {
            return "Automatically updates a viewport's drawing number to reflect the cell the lower right hand corner of the viewport occupies";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        public UpdaterId GetUpdaterId()
        {
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Dynamic Drawing Updater";
        }
        #endregion

        private void GetViewMapping(Document doc)
        {
            mapSheetToViewport.Clear();
            mapViewportToSheet.Clear();
            List<Element> sheets = new List<Element>();

            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc);
            sheetCollector.OfClass(typeof(ViewSheet));

            sheets = sheetCollector.ToList();

            foreach(ViewSheet sheet in sheets)
            {
                ElementId idSheet = sheet.Id;

                ISet<ElementId> viewHash = sheet.GetAllPlacedViews();

                if (viewHash.Count > 0)
                {
                    List<ElementId> viewIds = viewHash.ToList();
                    foreach (ElementId viewId in viewIds)
                    {

                        if (!mapSheetToViewport.ContainsKey(idSheet))
                        {
                            mapSheetToViewport.Add(idSheet, new List<ElementId>());
                        }

                        mapSheetToViewport[idSheet].Add(viewId);

                        if (mapViewportToSheet.ContainsKey(viewId))
                            mapViewportToSheet[viewId] = idSheet;
                        else
                            mapViewportToSheet.Add(viewId, idSheet);
                    }
                }
            }

        }
    }
}
