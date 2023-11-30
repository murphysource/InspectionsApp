using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using InspectionsApp.Models;
using InspectionsApp.ViewModels;
//using InspectionsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace InspectionsApp.Controllers
{
    public class InspectionController : Controller
    {
        private db_a9ffb8_murphysourceEntities db = new db_a9ffb8_murphysourceEntities();
        public ActionResult EditInspection(int id = 0)
        {
            ViewBag.AssemblyLines = db.AssemblyLines.Select(a => new SelectListItem
            {
                Value = a.AssemblyLineId.ToString(),
                Text = a.AssemblyLineName
            });

            ViewBag.Shifts = db.Shifts.Select(a => new SelectListItem
            {
                Value = a.ShiftId.ToString(),
                Text = a.ShiftName
            });

            ViewBag.Areas = db.Areas.Select(a => new SelectListItem
            {
                Value = a.AreaId.ToString(),
                Text = a.AreaName
            });

            ViewBag.AffectedParts = db.AffectedParts.Select(a => new SelectListItem
            {
                Value = a.AffectedPartId.ToString(),
                Text = a.AffectedPartName
            });

            ViewBag.Defects = db.Defects.Select(a => new SelectListItem
            {
                Value = a.DefectId.ToString(),
                Text = a.DefectName
            });

            Inspection i = db.Inspections.Find(id) ?? new Inspection()
            {
                InspectionDate = DateTime.Now
            };

            return View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInspection(Inspection i)
        {
            if (ModelState.IsValid)
            {
                //Submit entry as Windows user, create user if does not exist
                string windowsUsername = System.Web.HttpContext.Current.User.Identity.Name ?? "N/A";

                // Split the domain and username in case the name comes in 'DOMAIN\username' format
                if (windowsUsername.Contains("\\"))
                {
                    windowsUsername = windowsUsername.Split('\\')[1];
                }
                Models.Users user = db.Users.Where(u => u.UserName == windowsUsername).FirstOrDefault();
                if (user == null)
                {
                    // User doesn't exist, add them to the table
                    db.Users.Add(new Models.Users { UserName = windowsUsername });
                    db.SaveChanges();
                }

                if (i.InspectionId == 0) //new entry
                {
                    i.InspectionDate = DateTime.Now;
                    i.UserId = db.Users.Where(u => u.UserName == user.UserName).Select(u => u.UserId).FirstOrDefault();

                    db.Entry(i).State = EntityState.Added;

                    db.SaveChanges();
                }
                else //update entry
                {
                    i.LastModified = DateTime.Now;
                    db.Entry(i).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            return RedirectToAction("EditInspection");
        }

        public ActionResult InspectionTable()
        {
            ViewBag.AssemblyLines = db.AssemblyLines.Select(a => new SelectListItem
            {
                Value = a.AssemblyLineId.ToString(),
                Text = a.AssemblyLineName
            });

            ViewBag.Shifts = db.Shifts.Select(a => new SelectListItem
            {
                Value = a.ShiftId.ToString(),
                Text = a.ShiftName
            });

            ViewBag.Areas = db.Areas.Select(a => new SelectListItem
            {
                Value = a.AreaId.ToString(),
                Text = a.AreaName
            });

            ViewBag.AffectedParts = db.AffectedParts.Select(a => new SelectListItem
            {
                Value = a.AffectedPartId.ToString(),
                Text = a.AffectedPartName
            });

            ViewBag.Defects = db.Defects.Select(a => new SelectListItem
            {
                Value = a.DefectId.ToString(),
                Text = a.DefectName
            });
            return View();
        }

        public ActionResult ViewInspectionTable(string dateStart = null, string dateEnd = null, int shiftId = 0, int areaId = 0, int affectedPartId = 0, int defectId = 0)
        {
            ////DateTime filterDate = DateTime.Now.AddDays(-7);
            //DateTime startDate = DateTime.Parse(dateStart);
            //DateTime endDate = DateTime.Parse(dateEnd);

            //InspectionVM inspection = new InspectionVM();
            //inspection.Items = db.InspectionSearches.Where(d => d.InspectionDate >= startDate
            //                                                    && d.InspectionDate <= endDate).ToList();

            //return PartialView(inspection);
            return PartialView();
        }

        [HttpPost]
        public ActionResult GetInspectionData(string dateStart = null, string dateEnd = null, int shiftId = 0, int areaId = 0, int affectedPartId = 0, int defectId = 0)
        {
            DateTime now = DateTime.Now;

            DateTime startDate = DateTime.Parse(dateStart);
            DateTime endDate = DateTime.Parse(dateEnd);

            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            IQueryable<InspectionSearch> result;
            using (db_a9ffb8_murphysourceEntities db = new db_a9ffb8_murphysourceEntities())
            {
                result = db.InspectionSearches.Where(i => i.InspectionDate > startDate
                                                        && (i.InspectionDate <= endDate)
                                                        && (i.ShiftId == shiftId || shiftId == 0)
                                                        && (i.AreaId == areaId || areaId == 0)
                                                        && (i.AffectedPartId == affectedPartId || affectedPartId == 0)
                                                        && (i.DefectId == defectId || defectId == 0));

                int totalrows = result.ToList().Count;

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    result = result.Where(x => x.InspectionId.ToString().Contains(searchValue.ToLower())
                                               || x.ShiftName.ToLower().Contains(searchValue.ToLower())
                                               || x.AreaName.ToLower().Contains(searchValue.ToLower())
                                               || x.DefectName.ToLower().Contains(searchValue.ToLower())
                                               || x.AffectedPartName.ToLower().Contains(searchValue.ToLower())
                                               || x.SerialNumber.ToLower().Contains(searchValue.ToLower()));
                }
                int totalrowsafterfiltering = result.ToList().Count;
                //Sort
                result = result.OrderBy(sortColumnName + " " + sortDirection);
                //Paging
                result = result.Skip(start).Take(length);

                JsonResult jsonResult = Json(new
                {
                    data = result.ToList(),
                    draw = Request["draw"],
                    recordsTotal = totalrows,
                    recordsFiltered = totalrowsafterfiltering
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        public ActionResult ViewPersonalInspectionTable()
        {
            DateTime filterDate = DateTime.Now.AddDays(-1);
            string windowsUsername = User.Identity.Name;

            // Split the domain and username in case the name comes in 'DOMAIN\username' format
            if (windowsUsername.Contains("\\"))
            {
                windowsUsername = windowsUsername.Split('\\')[1];
            }
            InspectionVM inspection = new InspectionVM();
            inspection.Items = db.InspectionSearches.Where(d => d.UserName == windowsUsername
                                                                && d.InspectionDate >= filterDate).ToList();

            return PartialView(inspection);
        }

        public ActionResult GetFilteredValues(int areaId = 0)
        {
            var filteredAffetedPartValues = db.AffectedParts.Where(i => i.AreaId == areaId || areaId == 0).Select(a => new SelectListItem
            {
                Value = a.AffectedPartId.ToString(),
                Text = a.AffectedPartName
            });
            var selectListForAffectedParts = new SelectList(filteredAffetedPartValues, "Value", "Text");

            // Return the filtered values as JSON
            return Json(new
            {
                affectedPartValues = selectListForAffectedParts,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult DownloadInspectionReport(string dateStart = null, string dateEnd = null, int shiftDrop = 0, int areaDrop = 0, int partDrop = 0, int defectDrop = 0)
        {
            DateTime filterStart = DateTime.Parse(dateStart);
            DateTime filterEnd = DateTime.Parse(dateEnd);

            System.Data.DataTable dt = new System.Data.DataTable("ELIReport");
            dt.Columns.AddRange(new System.Data.DataColumn[11]
            {
                new System.Data.DataColumn("InspectionId"),
                new System.Data.DataColumn("InspectionDate"),
                new System.Data.DataColumn("SerialNumber"),
                new System.Data.DataColumn("ShiftName"),
                new System.Data.DataColumn("AreaName"),
                new System.Data.DataColumn("DefectName"),
                new System.Data.DataColumn("AffectedPartName"),
                new System.Data.DataColumn("Comments"),
                new System.Data.DataColumn("UserName"),
                new System.Data.DataColumn("Quantity"),
                new System.Data.DataColumn("LastModified"),
            });
            IQueryable<InspectionSearch> result = from item
                         in db.InspectionSearches
                                                where item.InspectionDate >= filterStart
                                                      && item.InspectionDate <= filterEnd
                                                      && (item.ShiftId == shiftDrop || shiftDrop == 0)
                                                      && (item.AreaId == areaDrop || areaDrop == 0)
                                                      && (item.AffectedPartId == partDrop || partDrop == 0)
                                                      && (item.DefectId == defectDrop || defectDrop == 0)
                                                select item;
            foreach (InspectionSearch item in result)
            {
                dt.Rows.Add(item.InspectionId, item.InspectionDate, item.SerialNumber,
                    item.ShiftName, item.AreaName, item.DefectName, item.AffectedPartName,
                    item.Comment, item.UserName, item.Quantity, item.LastModified);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                IXLWorksheet ws = wb.Worksheets.Add(dt);
                ws.Columns().AdjustToContents();
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheet.sheet", "ELIReport.xlsx");
                }
            }
        }

        public ActionResult InspectionChart(string dateStart = null, string dateEnd = null, int shiftId = 0,
          int assemblyLineId = 0, int areaId = 0, int affectedPartId = 0, int defectId = 0)
        {
            InspectionVM inspection = new InspectionVM();
            DateTime filterStart = DateTime.Parse(dateStart);
            DateTime filterEnd = DateTime.Parse(dateEnd);

            try
            {
                Dictionary<string, int> defectProduction = new Dictionary<string, int>();
                Dictionary<string, int> areaProduction = new Dictionary<string, int>();
                Dictionary<string, int> partProduction = new Dictionary<string, int>();
                Dictionary<string, int> shiftProduction = new Dictionary<string, int>();
                Dictionary<string, int> lineProduction = new Dictionary<string, int>();
                IQueryable<InspectionSearch> result;
                    result = db.InspectionSearches.Where(d => d.InspectionDate > filterStart
                                                              && d.InspectionDate <= filterEnd);

                result = shiftId == 0 ? result : result.Where(r => r.ShiftId == shiftId);
                result = assemblyLineId == 0 ? result : result.Where(r => r.AssemblyLineId == assemblyLineId);
                result = areaId == 0 ? result : result.Where(r => r.AreaId == areaId);
                result = affectedPartId == 0 ? result : result.Where(r => r.AffectedPartId == affectedPartId);
                result = defectId == 0 ? result : result.Where(r => r.DefectId == defectId);
                foreach (InspectionSearch l in result.ToList())
                {
                        if (!defectProduction.ContainsKey(l.DefectName))
                        {
                            defectProduction.Add(l.DefectName, 0);
                        }
                        defectProduction[l.DefectName] += l.Quantity;

                        if (!areaProduction.ContainsKey(l.AreaName))
                        {
                            areaProduction.Add(l.AreaName, 0);
                        }
                        areaProduction[l.AreaName] += l.Quantity;

                        if (!partProduction.ContainsKey(l.AffectedPartName))
                        {
                            partProduction.Add(l.AffectedPartName, 0);
                        }
                        partProduction[l.AffectedPartName] += l.Quantity;

                        if (!shiftProduction.ContainsKey(l.ShiftName))
                        {
                            shiftProduction.Add(l.ShiftName, 0);
                        }
                        shiftProduction[l.ShiftName] += l.Quantity;

                        if (!lineProduction.ContainsKey(l.AssemblyLineName))
                        {
                            lineProduction.Add(l.AssemblyLineName, 0);
                        }
                        lineProduction[l.AssemblyLineName] += l.Quantity;
                }
                int totalDefects = result.ToList().Sum(x => x.Quantity);
                int totalInspections = result.Select(s => s.SerialNumber).Distinct().Count();

                defectProduction.Remove("No Issue");
                areaProduction.Remove("No Issue");
                partProduction.Remove("No Issue");

                //DEFECT
                ViewBag.DefectProduction = defectProduction;

                ViewBag.TotalDefects = totalDefects;
                ViewBag.TotalInspections = totalInspections;

                //AREA
                ViewBag.AreaProduction = areaProduction;
                //PART
                ViewBag.PartProduction = partProduction;
                //SHIFT
                ViewBag.ShiftProduction = shiftProduction;
                //DEFECT
                ViewBag.LineProduction = lineProduction;

                return PartialView(inspection);
            }
            catch
            {
                return View("Error");
            }
        }
    }
}