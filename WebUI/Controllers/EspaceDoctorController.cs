using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class EspaceDoctorController : Controller
    {
        // GET: EspaceDoctor
        public ActionResult Index()
        {
            return this.View();
        }

        // GET: EspaceDoctor/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EspaceDoctor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EspaceDoctor/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EspaceDoctor/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EspaceDoctor/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EspaceDoctor/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EspaceDoctor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
