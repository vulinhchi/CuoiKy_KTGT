using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CuoiKy_APIMp3.Models;
using System.IO;

namespace CuoiKy_APIMp3.Controllers
{
    public class HomeController : Controller
    {
        private AudioFunction file;
        private audoSteg sh;
        private string mess;
        [HttpGet]
        public ActionResult TAINHAC()
        {
            ViewBag.Message = "site tai nhac";

            return View(GoogleDriveFilesRepository.GetDriveFiles());
        }

        //check watermark
        public ActionResult Check()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Checked(HttpPostedFileBase file)
        {
            string signature = GoogleDriveFilesRepository.checkWatermark(file);
            ViewBag.Message = signature;
            return View();
        }

        [HttpGet]
        public ActionResult GetGoogleDriveFiles()
        {
            return View(GoogleDriveFilesRepository.GetDriveFiles());
        }

        [HttpPost]
        public ActionResult DeleteFile(GoogleDriveFiles file)
        {
            GoogleDriveFilesRepository.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            GoogleDriveFilesRepository.FileUpload(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        public void DownloadFile(string id)
        {
            string FilePath = GoogleDriveFilesRepository.DownloadGoogleFile(id);

            file = new AudioFunction(new FileStream(FilePath, FileMode.Open, FileAccess.Read));
            sh = new audoSteg(file);
            mess = "downloaded at " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy zzzz");

            sh.waterMess(mess);
            file.writeFile(FilePath);

            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            Response.End();
            Response.Flush();

            System.IO.File.Delete(FilePath);
        }
    }
}