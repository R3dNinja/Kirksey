#region Namespaces
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using RibbonItem = Autodesk.Revit.UI.RibbonItem;
using UIFramework;
#endregion

namespace Entry
// <summary>
// Revit Ribbon Implementation
// </summary>
// <remarks></remarks>
{
    public class AppMain : IExternalApplication
    {
        private const string CTabName = "Kirksey";
        private string _path;
        private UIControlledApplication _uiApp;
        private bool _toolsExistsSubsc = false;
        private bool _toolsExistsFree = false;
        private string mikesComputer = "C04400830";
        private string login = WindowsIdentity.GetCurrent().Name;
        private string Currentuser;
        private static ControlledApplication m_CtrlApp;
        private Domain CurrentDomain = Domain.GetComputerDomain();
        private string KirkseyDomain = "kirksey.com";

        #region "Public Memebers - Revit IExternalApplication Implementation"
        // <summary>
        // Startup
        // </summary>
        // <param name="a"></param>
        // <returns>
        // Result
        // </returns>
        // <remarks></remarks>
        public Result OnStartup(UIControlledApplication a)
        {
            try
            {
                //Path
                _path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //UI App
                _uiApp = a;
                //Controlled App
                m_CtrlApp = a.ControlledApplication;
                //Set an event handler for opening documents.
                m_CtrlApp.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(CtrlApp_DocumentOpened);
                //get current user
                Currentuser = GetUser(login);

                //General Buttons
                if (CurrentDomain.Name.ToLower() == KirkseyDomain.ToLower())
                {
                    LoadItems(Currentuser);
                }
                
            }
            catch
            {
            }
            return Result.Succeeded;
        }

        // <summary>
        // Shutdown
        // </summary>
        // <param name="a"></param>
        // <returns>
        // Result
        // </returns>
        // <remarks></remarks>
        public Result OnShutdown(UIControlledApplication a)
        {
            m_CtrlApp.DocumentOpened -= CtrlApp_DocumentOpened;
            return Result.Succeeded;
        }
        #endregion

        #region Event handler
        void CtrlApp_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            var command = new KirkseyAppsRibbon.Command();
            command.Execute(new UIApplication(sender as Application));
        }
        #endregion

        #region "Private Members - Ribbon Functions"
        // <summary>
        // Get a Pushbutton Object
        // </summary>
        // <param name="cmdName"></param>
        // <param name="cmdText"></param>
        // <param name="filePath"></param>
        // <param name="className"></param>
        // <param name="img16"></param>
        // <param name="img32"></param>
        // <param name="tooltipText"></param>
        // <param name="cmdAvail"></param>
        // <returns></returns>
        // <remarks></remarks>
        private PushButtonData GetPushButtonData(string cmdName, string cmdText, string filePath, string className, string img16, string img32, string tooltipText, string cmdAvail)
        {
            PushButtonData m_pb = new PushButtonData(cmdName, cmdText, filePath, className);
            m_pb.Image = LoadPngImgSource(img16);
            m_pb.LargeImage = LoadPngImgSource(img32);
            m_pb.ToolTip = tooltipText;
            return m_pb;
        }

        // <summary>
        // Adds a set of PushbuttonData to the ribbon on a specified panel as stacked and/or buttons
        // </summary>
        // <param name = "tabname" ></ param >
        // <param name = "panelName" ></ param >
        // <param name = "buttons" ></ param >
        // <remarks></remarks>
        private void AddAsStackedAndButtons(string tabname, string panelName, Dictionary<string, List<PushButtonData>> buttons)//, buttons As Dictionary(Of String, List(Of PushButtonData)) 
        {

            Autodesk.Revit.UI.RibbonPanel m_panelSubsc = GetRibbonPanelByTabName(tabname, panelName);
            int m_iCnt = 0;
            foreach (var kvp in buttons)
            {
                if (kvp.Value.Count > 0)
                {
                    if (m_iCnt > 0)
                    {
                        m_panelSubsc.AddSeparator();
                        m_iCnt += 1;
                    }

                    switch (kvp.Value.Count)
                    {
                        case 12:
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            m_panelSubsc.AddStackedItems(kvp.Value[3], kvp.Value[4], kvp.Value[5]);
                            m_panelSubsc.AddStackedItems(kvp.Value[6], kvp.Value[7], kvp.Value[8]);
                            m_panelSubsc.AddStackedItems(kvp.Value[9], kvp.Value[10], kvp.Value[11]);
                            break;

                        case 11:
                            m_panelSubsc.AddStackedItems(kvp.Value[9], kvp.Value[10]);
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            m_panelSubsc.AddStackedItems(kvp.Value[3], kvp.Value[4], kvp.Value[5]);
                            m_panelSubsc.AddStackedItems(kvp.Value[6], kvp.Value[7], kvp.Value[8]);
                            break;

                        case 10:
                            m_panelSubsc.AddItem(kvp.Value[9]);
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            m_panelSubsc.AddStackedItems(kvp.Value[3], kvp.Value[4], kvp.Value[5]);
                            m_panelSubsc.AddStackedItems(kvp.Value[6], kvp.Value[7], kvp.Value[8]);
                            break;

                        case 9:
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            m_panelSubsc.AddStackedItems(kvp.Value[3], kvp.Value[4], kvp.Value[5]);
                            m_panelSubsc.AddStackedItems(kvp.Value[6], kvp.Value[7], kvp.Value[8]);
                            break;

                        case 8:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            m_panelSubsc.AddItem(kvp.Value[1]);
                            m_panelSubsc.AddStackedItems(kvp.Value[2], kvp.Value[3], kvp.Value[4]);
                            m_panelSubsc.AddStackedItems(kvp.Value[5], kvp.Value[6], kvp.Value[7]);
                            break;

                        case 7:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            m_panelSubsc.AddStackedItems(kvp.Value[1], kvp.Value[2], kvp.Value[3]);
                            m_panelSubsc.AddStackedItems(kvp.Value[4], kvp.Value[5], kvp.Value[6]);
                            break;

                        case 6:
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            m_panelSubsc.AddStackedItems(kvp.Value[3], kvp.Value[4], kvp.Value[5]);
                            break;

                        case 5:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            m_panelSubsc.AddItem(kvp.Value[1]);
                            m_panelSubsc.AddStackedItems(kvp.Value[2], kvp.Value[3], kvp.Value[4]);
                            break;

                        case 4:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            m_panelSubsc.AddStackedItems(kvp.Value[1], kvp.Value[2], kvp.Value[3]);
                            break;

                        case 3:
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1], kvp.Value[2]);
                            break;

                        case 2:
                            m_panelSubsc.AddStackedItems(kvp.Value[0], kvp.Value[1]);
                            break;

                        case 1:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            break;

                        default:
                            m_panelSubsc.AddItem(kvp.Value[0]);
                            break;
                    }
                }
            }
        }

        // <summary>
        // Get the Ribbon Panel by Tab and Panel Name
        // </summary>
        // <param name="tabName">Tab Name</param>
        // <param name="panelName">Panel Name</param>
        // <returns></returns>
        // <remarks></remarks>
        private Autodesk.Revit.UI.RibbonPanel GetRibbonPanelByTabName(string tabName, string panelName)
        {
            Autodesk.Revit.UI.RibbonPanel m_panel = null;
            bool exists = false;
            try
            {
                // Does it exist already?
                foreach (Autodesk.Revit.UI.RibbonPanel x in _uiApp.GetRibbonPanels(tabName))
                {
                    if (x.Title.ToLower() == panelName.ToLower())
                    {
                        exists = true;
                    }
                }
            }
            catch
            {
            }
            if (exists == false)
            {
                try
                {
                    // Add the Panel
                    m_panel = _uiApp.CreateRibbonPanel(tabName, panelName);
                }
                catch
                {
                    m_panel = null;
                }
            }
            return m_panel;
        }

        // <summary>
        // Load an Image Source from File
        // </summary>
        // <param name="sourceName"></param>
        // <returns></returns>
        // <remarks></remarks>
        private ImageSource LoadPngImgSource(string sourceName)
        {
            Assembly m_assembly = Assembly.GetExecutingAssembly();
            Stream m_icon = m_assembly.GetManifestResourceStream(sourceName);
            PngBitmapDecoder m_decoder = new PngBitmapDecoder(m_icon, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            ImageSource m_source = m_decoder.Frames[0];
            return m_source;
        }

        // <summary>
        // Add a button to a Ribbon Tab
        // </summary>
        // <param name="rpanel">The ribbon panel</param>
        // <param name="buttonName">The Name of the Button</param>
        // <param name="buttonText">Command Text</param>
        // <param name="imagePath16">Small Image</param>
        // <param name="imagePath32">Large Image</param>
        // <param name="dllPath">Path to the DLL file</param>
        // <param name="dllClass">Full qualified class descriptor</param>
        // <param name="tooltip">Tooltip to add to the button</param>
        // <param name="pbAvail">Pushbutton availability class, blank if none</param>
        // <remarks></remarks>
        private void AddButton(Autodesk.Revit.UI.RibbonPanel rpanel, string buttonName, string buttonText, string dllPath, string dllClass, string imagePath16, string imagePath32, string toolTip, string pbAvail)
        {
            PushButtonData m_pbData = GetPushButtonData(buttonName, buttonText, dllPath, dllClass, imagePath16, imagePath32, toolTip, pbAvail);
            rpanel.AddItem(m_pbData);
        }
        #endregion

        #region "Private Members - Buttons (General)"
        // <summary>
        // Load the Controls
        // </summary>
        // <remarks></remarks>
        private void LoadItems(string Currentuser)
        {
            // First Create the Tab
            _uiApp.CreateRibbonTab(CTabName);

            LoadKeynotes();

            LoadSheetData();

            DrawingIssusance();

            LoadAutomationTools();

            LoadDWGTool();

            string computerName = Environment.MachineName.ToString();

            LoadBackPocket(computerName, Currentuser);

            //LoadSheetDataAlternate();
        }

        private void LoadBackPocket(string computerName, string Currentuser)
        {
            // Template Creator
            List<string> vaildUser = new List<string>();
            vaildUser.Add("michaelr");
            vaildUser.Add("clays");
            vaildUser.Add("russellw");
            foreach (String user in vaildUser)
            {
                if (user == Currentuser.ToLower())
                {
                    string m_publishTemplatePath = Path.Combine(_path, "PublishTemplate1.dll");
                    if (File.Exists(m_publishTemplatePath))
                    {
                        Autodesk.Revit.UI.RibbonPanel m_panelKeynote = GetRibbonPanelByTabName(CTabName, "Template Tool");
                        AddButton(m_panelKeynote, "PublishTemplate", "Publish\nTemplate", m_publishTemplatePath, "PublishTemplate.Command", "KirkseyAppsRibbon.Icons.PublishTemplate16.png", "KirkseyAppsRibbon.Icons.PublishTemplate32.png", "Back Pocket BIM Tools", "");
                    }
                }
            }

            // Keynote Edit - Large Button
            if (computerName == mikesComputer)
            {
                string m_backPocketPath = Path.Combine(_path, "ReplaceAssemblyCode.dll");
                if (File.Exists(m_backPocketPath))
                {
                    Autodesk.Revit.UI.RibbonPanel m_panelKeynote = GetRibbonPanelByTabName(CTabName, "Back Pocket");
                    AddButton(m_panelKeynote, "BackPocket", "Back\nPocket", m_backPocketPath, "ReplaceAssemblyCode.Command", "KirkseyAppsRibbon.Icons.BackPocket16.png", "KirkseyAppsRibbon.Icons.BackPocket32.png", "Back Pocket BIM Tools", "");
                }
            }
        }

        private void LoadSheetDataAlternate()
        {
            List<PushButtonData> l_kpbd = new List<PushButtonData>();
            List<PushButtonData> l_spbd = new List<PushButtonData>();
            Dictionary<string, List<PushButtonData>> m_kirkseyStuff = new Dictionary<string, List<PushButtonData>>();
            PushButtonData m_pbd;

            //Groups
            //m_kirkseyStuff.Add("keynotes", new List<PushButtonData>());
            //m_kirkseyStuff.Add("sheet", new List<PushButtonData>());

            //Keynotes
            string m_keynotePath = Path.Combine(_path, "KeynoteData.dll");
            m_pbd = GetPushButtonData("KeynoteData",
                                    "Manage \nKeynotes",
                                    m_keynotePath,
                                    "KeynoteData.Command",
                                    "KirkseyAppsRibbon.Icons.Keynote16.png",
                                    "KirkseyAppsRibbon.Icons.Keynote32.png",
                                    "Edit and Reload Keynote definitions",
                                    "");
            //m_kirkseyStuff("keynotes").Add(m_pbd);
            l_kpbd.Add(m_pbd);
            m_kirkseyStuff.Add("keynotes", l_kpbd);

            string m_codePath = Path.Combine(_path, "ManageCodeInformation.dll");
            string m_masterSchedulePath = Path.Combine(_path, "ManageMasterSchedule.dll");
            string m_sheetSpecsPath = Path.Combine(_path, "ManageSheetSpecs.dll");
            string m_sheetIndexPath = Path.Combine(_path, "SheetIndex.dll");

            m_pbd = GetPushButtonData("SheetIndex",
                                      "Import \nConsultants Sheets",
                                      m_sheetIndexPath,
                                      "SheetIndex.Command",
                                      "KirkseyAppsRibbon.Icons.SheetIndex16.png",
                                      "KirkseyAppsRibbon.Icons.SheetIndex32.png",
                                      "Load Consultants Sheet Data to Sheet Index",
                                      "");
            l_spbd.Add(m_pbd);

            m_pbd = GetPushButtonData("ManageCodeInformation",
                                      "Import \nCode Analysis",
                                      m_codePath,
                                      "ManageCodeInformation.Command",
                                      "KirkseyAppsRibbon.Icons.CodeInformation16.png",
                                      "KirkseyAppsRibbon.Icons.CodeInformation32.png",
                                      "Load and Place Code Analysis Information",
                                      "");
            l_spbd.Add(m_pbd);

            m_pbd = GetPushButtonData("ManageMasterSchedule",
                                      "Import \nMaster Schedule",
                                      m_masterSchedulePath,
                                      "ManageMasterSchedule.Command",
                                      "KirkseyAppsRibbon.Icons.MasterScheduleUpdate16.png",
                                      "KirkseyAppsRibbon.Icons.MasterScheduleUpdate32.png",
                                      "Load and Place Master Schedule Data",
                                      "");
            l_spbd.Add(m_pbd);

            m_pbd = GetPushButtonData("ManageSheetSpecs",
                                      "Import \nSheet Specs",
                                      m_sheetSpecsPath,
                                      "ManageSheetSpecs.Command",
                                      "KirkseyAppsRibbon.Icons.SheetSpecUpdate16.png",
                                      "KirkseyAppsRibbon.Icons.SheetSpecUpdate32.png",
                                      "Load and Place Sheet Specifications",
                                      "");
            l_spbd.Add(m_pbd);

            m_kirkseyStuff.Add("sheet", l_spbd);

            AddAsStackedAndButtons(CTabName, "Manage External Data", m_kirkseyStuff);

        }

        private void LoadSheetData()
        {
            Autodesk.Revit.UI.RibbonPanel m_panelSheetData = GetRibbonPanelByTabName(CTabName, "External Data Management");

            string m_codePath = Path.Combine(_path, "ManageCodeInformation.dll");
            if (File.Exists(m_codePath))
            {
                m_panelSheetData.AddItem(GetPushButtonData("ManageCodeInformation",
                                                "Import \nCode Analysis",
                                                m_codePath,
                                                "ManageCodeInformation.Command",
                                                "KirkseyAppsRibbon.Icons.CodeInformation16.png",
                                                "KirkseyAppsRibbon.Icons.CodeInformation32.png",
                                                "Load and Place Code Analysis Information",
                                                ""));
            }

            string m_masterSchedulePath = Path.Combine(_path, "ManageMasterSchedule.dll");
            string m_ExcelImportPath = Path.Combine(_path, "ExcelImport.dll");
            if (File.Exists(m_masterSchedulePath))
            {
                if (File.Exists(m_ExcelImportPath))
                {
                    //Both Exist
                    AddSplitButton(m_panelSheetData, m_masterSchedulePath, m_ExcelImportPath);
                }
                else
                {
                    //Only Master Schedule Exists
                    m_panelSheetData.AddItem(GetPushButtonData("ManageMasterSchedule",
                                                    "Import \nMaster Schedule",
                                                    m_masterSchedulePath,
                                                    "ManageMasterSchedule.Command",
                                                    "KirkseyAppsRibbon.Icons.MasterScheduleUpdate16.png",
                                                    "KirkseyAppsRibbon.Icons.MasterScheduleUpdate32.png",
                                                    "Load and Place Master Schedule Data",
                                                    ""));
                }
            }
            else
            {
                if (File.Exists(m_ExcelImportPath))
                {
                    //Only ExcelImport Exists
                    m_panelSheetData.AddItem(GetPushButtonData("ManageMasterSchedule",
                                                    "Import \nMaster Schedule",
                                                    m_ExcelImportPath,
                                                    "ExcelImport.Command",
                                                    "KirkseyAppsRibbon.Icons.MasterScheduleUpdate16.png",
                                                    "KirkseyAppsRibbon.Icons.MasterScheduleUpdate32.png",
                                                    "Load and Place Master Schedule Data",
                                                    ""));
                }
                else
                {
                    //None exists
                }
            }

            string m_sheetSpecsPath = Path.Combine(_path, "ManageSheetSpecs.dll");
            if (File.Exists(m_sheetSpecsPath))
            {
                m_panelSheetData.AddItem(GetPushButtonData("ManageSheetSpecs",
                                                "Import \nSheet Specs",
                                                m_sheetSpecsPath,
                                                "ManageSheetSpecs.Command",
                                                "KirkseyAppsRibbon.Icons.SheetSpecUpdate16.png",
                                                "KirkseyAppsRibbon.Icons.SheetSpecUpdate32.png",
                                                "Load and Place Sheet Specifications",
                                                ""));
            }


        }

        private void DrawingIssusance()
        {
            Autodesk.Revit.UI.RibbonPanel m_panelIssueData = GetRibbonPanelByTabName(CTabName, "Drawing Issuance");

            string m_sheetIndexPath = Path.Combine(_path, "SheetIndex.dll");
            if (File.Exists(m_sheetIndexPath))
            {
                m_panelIssueData.AddItem(GetPushButtonData("SheetIndex",
                                                "Import \nConsultants Sheets",
                                                m_sheetIndexPath,
                                                "SheetIndex.Command",
                                                "KirkseyAppsRibbon.Icons.SheetIndex16.png",
                                                "KirkseyAppsRibbon.Icons.SheetIndex32.png",
                                                "Load Consultants Sheet Data to Sheet Index",
                                                ""));
            }

            string m_revisionNarrativePath = Path.Combine(_path, "RevisionNarrative.dll");
            if (File.Exists(m_revisionNarrativePath))
            {
                m_panelIssueData.AddItem(GetPushButtonData("RevisionNarrative",
                                                "Generate \nRevision Narrative",
                                                m_revisionNarrativePath,
                                                "RevisionNarrative.Command",
                                                "KirkseyAppsRibbon.Icons.RevisionNarrative16.png",
                                                "KirkseyAppsRibbon.Icons.RevisionNarrative32.png",
                                                "Export Revision Data to Word Document for Revision Narrative",
                                                ""));
            }
        }

        private void LoadKeynotes()
        {
            // Keynote Edit - Large Button
            string m_keynotePath = Path.Combine(_path, "KeynoteData.dll");
            if (File.Exists(m_keynotePath))
            {
                Autodesk.Revit.UI.RibbonPanel m_panelKeynote = GetRibbonPanelByTabName(CTabName, "Keynote Managment");
                AddButton(m_panelKeynote, "KeynoteData", "Manage \nKeynotes", m_keynotePath, "KeynoteData.Command", "KirkseyAppsRibbon.Icons.Keynote16.png", "KirkseyAppsRibbon.Icons.Keynote32.png", "Edit and Reload Keynote definitions", "");
            }
        }

        private void LoadDWGTool()
        {
            // Keynote Edit - Large Button
            Autodesk.Revit.UI.RibbonPanel m_panelUtilities = null;
            string m_DWGPath = Path.Combine(_path, "ConvertDWGtoLines.dll");
            if (File.Exists(m_DWGPath))
            {
                if (m_panelUtilities == null)
                {
                    m_panelUtilities = GetRibbonPanelByTabName(CTabName, "Drawing Utilities");
                }
                AddButton(m_panelUtilities, "EZDWG", "EZ DWG \nConverter", m_DWGPath, "ConvertDWGtoLines.Command", "KirkseyAppsRibbon.Icons.EZDWGConverter16.png", "KirkseyAppsRibbon.Icons.EZDWGConverter32.png", "Convert a selected DWG to detail lines.", "");
            }
            m_DWGPath = Path.Combine(_path, "InteriorHelper.dll");
            if (File.Exists(m_DWGPath))
            {
                if (m_panelUtilities == null)
                {
                    m_panelUtilities = GetRibbonPanelByTabName(CTabName, "Drawing Utilities");
                }
                
                AddButton(m_panelUtilities, "Interior Converter", "Arch to Interior \nConverter", m_DWGPath, "InteriorHelper.Command", "KirkseyAppsRibbon.Icons.AtoI16.png", "KirkseyAppsRibbon.Icons.AtoI32.png", "Convert Architectural Views and Sheets to Interior Views and Sheets.", "");
            }
            m_DWGPath = Path.Combine(_path, "SVG Decode.dll");
            if (File.Exists(m_DWGPath))
            {
                if (m_panelUtilities == null)
                {
                    m_panelUtilities = GetRibbonPanelByTabName(CTabName, "Drawing Utilities");
                }

                AddButton(m_panelUtilities, "PDF Importer", "PDF \nImporter", m_DWGPath, "SVG_Decode.Command", "KirkseyAppsRibbon.Icons.PDFImport16.png", "KirkseyAppsRibbon.Icons.PDFImport32.png", "Vector and Raster import of PDF files.", "");
            }
        }

        private void LoadAutomationTools()
        {
            //List<string> vaildUser = new List<string>();
            //vaildUser.Add("michaelr");
            //vaildUser.Add("clays");
            //vaildUser.Add("russellw");
            //vaildUser.Add("garthw");
            //vaildUser.Add("craigp");
            //vaildUser.Add("amandam");
            //foreach (String user in vaildUser)
            //{
                //if (user == Currentuser.ToLower())
                //{
                    string m_detailPath = Path.Combine(_path, "DetailBuilder.dll");
                    if (File.Exists(m_detailPath))
                    {

                        Autodesk.Revit.UI.RibbonPanel m_panelUtilities = GetRibbonPanelByTabName(CTabName, "Automation Tools");

                        PushButtonData m_pbData = GetPushButtonData("Detail Builder", "Detail \nBuilder", m_detailPath, "DetailBuilder.Command", "KirkseyAppsRibbon.Icons.DetailBuilder16.png", "KirkseyAppsRibbon.Icons.DetailBuilder32.png", "Formerly Project Seagul. *M*A*G*I*C*", "");

                        var button1 = m_panelUtilities.AddItem(m_pbData);

                        var tempPath = Path.Combine(Path.GetTempPath(), "Magic.swf");

                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("KirkseyAppsRibbon.Magic.swf"))
                        {
                            var buffer = new byte[stream.Length];

                            stream.Read(buffer, 0, buffer.Length);

                            using (FileStream fs = new FileStream(
                              tempPath, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(buffer, 0, buffer.Length);
                            }
                        }

                        RibbonToolTip toolTip1 = new RibbonToolTip()
                        {
                            Title = "Detail Builder",
                            Content = "Import standard details based on model componets",
                            ExpandedContent = "Formerly Project Seagul. *M*A*G*I*C*",
                            ExpandedVideo = new Uri(tempPath),
                            HelpSource = new Uri("https://www.youtube.com/watch?v=U9t-slLl30E"),
                            IsHelpEnabled = true,
                            IsProgressive = true
                        };

                        SetRibbonItemToolTip(button1, toolTip1);
                    }
                //}
            //}
        }

        private string GetUser(string login)
        {
            string domainUser = Regex.Replace(login, ".*\\\\(.*)", "$1", RegexOptions.None);
            return domainUser;
        }

        void SetRibbonItemToolTip(RibbonItem item, RibbonToolTip toolTip)
        {
            IUIRevitItemConverter itemConverter =
                new InternalMethodUIRevitItemConverter();

            var ribbonItem = itemConverter.GetRibbonItem(item);
            if (ribbonItem == null)
                return;
            ribbonItem.ToolTip = toolTip;
        }

        private void AddSplitButton(Autodesk.Revit.UI.RibbonPanel panel, string m_masterSchedulePath, string m_ExcelImportPath)
        {
            PushButtonData buttonOne = new PushButtonData("ButtonOne", "Image Based Import", m_masterSchedulePath, "ManageMasterSchedule.Command");
            buttonOne.LargeImage = LoadPngImgSource("KirkseyAppsRibbon.Icons.MasterScheduleUpdateImage32.png");

            PushButtonData buttonTwo = new PushButtonData("ButtonTwon", "Text/Schedule Based Import", m_ExcelImportPath, "ExcelImport.Command");
            buttonTwo.LargeImage = LoadPngImgSource("KirkseyAppsRibbon.Icons.MasterScheduleUpdateText32.png");

            SplitButtonData sb1 = new SplitButtonData("splitButton1", "Split");
            SplitButton sb = panel.AddItem(sb1) as SplitButton;
            sb.AddPushButton(buttonOne);
            sb.AddPushButton(buttonTwo);
        }
        #endregion
    }
}

