#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace KirkseyAppsRibbon
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        Document m_document;
        UIDocument m_documentUI;
        //applications's private data
        private static ViewportUpdater m_viewportUpdater = null;
        private AddInId m_thisAppId;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Result.Succeeded;
        }

        public Result Execute(UIApplication uiapp)
        {
            try
            {
                m_documentUI = uiapp.ActiveUIDocument;
                m_document = m_documentUI.Document;
                m_thisAppId = uiapp.Application.ActiveAddInId;

                // creating and registering the updater for the document.
                if (m_viewportUpdater == null)
                {
                    using (Transaction t = new Transaction(m_document, "Register Drawing Cell Updater"))
                    {
                        t.Start();

                        m_viewportUpdater = new ViewportUpdater(m_thisAppId);
                        m_viewportUpdater.Register(m_document);

                        t.Commit();
                    }
                }

                //Find all viewports to watch if they move
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Viewports);

                UpdaterRegistry.RemoveAllTriggers(m_viewportUpdater.GetUpdaterId());
                m_viewportUpdater.AddTriggerForUpdater(m_document, filter);


                m_document.DocumentClosing += UnregisterViewportUpdaterOnClose;
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        private void UnregisterViewportUpdaterOnClose(object source, DocumentClosingEventArgs args)
        {
            //idsToWatch.Clear();
            //m_oldSectionId = ElementId.InvalidElementId;

            if (m_viewportUpdater != null)
            {
                UpdaterRegistry.UnregisterUpdater(m_viewportUpdater.GetUpdaterId());
                m_viewportUpdater = null;
            }
        }
    }
}
