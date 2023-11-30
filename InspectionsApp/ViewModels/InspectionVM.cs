using InspectionsApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace InspectionsApp.ViewModels
{
    public class InspectionVM
    {
        public List<InspectionSearch> Items { get; set; }

        public InspectionVM()
        {
            Items = new List<InspectionSearch>();
        }
    }
}