using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIFinalRooms
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts = new Transaction(doc, "insert rooms");
            ts.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in levels)
            {
                rooms = doc.Create.NewRooms2(level);
            }
            FilteredElementCollector roomfilter = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms);
            IList<ElementId> roomsID = roomfilter
                .ToElementIds() as IList<ElementId>;
            foreach (ElementId id in roomsID)
            {
                Element e = doc.GetElement(id);
                Room r = e as Room;
                string lName = r.Level.Name.Substring(6);
                r.Name = $"{lName}_{r.Number}";
                doc.Create.NewRoomTag(new LinkElementId(id),
                                        new UV(0, 0),
                                        null);
            }
            ts.Commit();

            return Result.Succeeded;
        }
    }
}
