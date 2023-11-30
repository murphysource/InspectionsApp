using System.Collections.Generic;

namespace InspectionsApp.ViewModels
{
    public class TableVM
    {
        public List<TableItemVM> Items { get; set; }
        public List<TableColumnVM> Columns { get; set; }
        public string EntityTag { get; set; }
        public int SelectedId { get; set; }

        public TableVM()
        {
            Items = new List<TableItemVM>();
            Columns = new List<TableColumnVM>();
        }

        public TableVM(string tag) : this()
        {
            EntityTag = tag;
        }

    }

    public class TableItemVM
    {
        public int EntityId { get; set; }
        public bool Active { get; set; }
        public List<string> CellData { get; set; }

        public TableItemVM()
        {
            CellData = new List<string>();
        }

        public TableItemVM(int id, bool active) : this()
        {
            EntityId = id;
            Active = active;
        }
    }

    public class TableColumnVM
    {
        public string Heading { get; set; }
        public string Style { get; set; }

        public TableColumnVM()
        {
        }

        public TableColumnVM(string heading, string style)
        {
            Heading = heading;
            Style = style;
        }
    }
}