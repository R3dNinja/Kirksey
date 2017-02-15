using System.Collections.Generic;

using Autodesk.Revit.DB;

namespace KirkseyAppsRibbon
{
    class TitleBlockProperties
    {
        public string PageSize { get; set; }
        public double cellWidth { get; set; }
        public double cellHeight { get; set; }
        public double baseOffset { get; set; }
        public double sideOffset { get; set; }
        public int horizontalCellCount { get; set; }
        public int verticalCellCount { get; set; }
        public Dictionary<string, Outline> CellIDs { get; set; }

        public TitleBlockProperties(string width, BoundingBoxXYZ box)
        {
            double minX = box.Min.X;
            double minY = box.Min.Y;
            this.CellIDs = new Dictionary<string, Outline>();
            this.cellWidth = (6.875 / 12.0);
            this.cellHeight = (7.125 / 12.0);
            this.baseOffset = (.75 / 12.0) + minY;
            switch (width)
            {
                case "3":
                    this.PageSize = "D";
                    this.sideOffset = (3.75 / 12.0) + minX;
                    this.horizontalCellCount = 4;
                    this.verticalCellCount = 3;
                    break;
                case "3.5":
                    this.PageSize = "E1";
                    this.sideOffset = (2.875 / 12.0) + minX;
                    this.horizontalCellCount = 5;
                    this.verticalCellCount = 4;
                    break;
                case "4":
                    this.PageSize = "E";
                    this.sideOffset = (2.0 / 12.0) + minX;
                    this.horizontalCellCount = 6;
                    this.verticalCellCount = 5;
                    break;
                default:
                    this.PageSize = "VOID";
                    this.horizontalCellCount = 0;
                    this.verticalCellCount = 0;
                    break;
            }
            if (this.PageSize != "VOID")
            {
                for (int x = 0; x < this.horizontalCellCount; x++)
                {
                    for (int y = 0; y < this.verticalCellCount; y++)
                    {
                        XYZ minPoint = new XYZ((this.sideOffset + (x * this.cellWidth)), (this.baseOffset + (y * this.cellHeight)), 0);
                        XYZ maxPoint = new XYZ((this.sideOffset + ((x + 1) * this.cellWidth)), (this.baseOffset + ((y + 1) * this.cellHeight)), 0);
                        updateCellBoundary(minPoint, maxPoint, x, y);
                    }
                }
            }

        }

        public void updateCellBoundary(XYZ minPoint, XYZ maxPoint, int x, int y)
        {
            switch (x)
            {
                case 0:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A1", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B1", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C1", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D1", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E1", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                case 1:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A2", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B2", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C2", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D2", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E2", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                case 2:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A3", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B3", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C3", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D3", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E3", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A4", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B4", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C4", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D4", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E4", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                case 4:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A5", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B5", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C5", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D5", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E5", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                case 5:
                    switch (y)
                    {
                        case 0:
                            this.CellIDs.Add("A6", new Outline(minPoint, maxPoint));
                            break;
                        case 1:
                            this.CellIDs.Add("B6", new Outline(minPoint, maxPoint));
                            break;
                        case 2:
                            this.CellIDs.Add("C6", new Outline(minPoint, maxPoint));
                            break;
                        case 3:
                            this.CellIDs.Add("D6", new Outline(minPoint, maxPoint));
                            break;
                        case 4:
                            this.CellIDs.Add("E6", new Outline(minPoint, maxPoint));
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
