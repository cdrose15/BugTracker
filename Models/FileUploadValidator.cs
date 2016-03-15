using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class FileUploadValidator
    {
        public static bool AttachmentUpload(HttpPostedFileBase file)
        {
            var supportedFiles = new[] { "txt", "pdf", "doc", "docx" };
            var fileExtention = System.IO.Path.GetExtension(file.FileName.Substring(1));
            // Check for actual file object
            if (file == null)
                return false;
            // Check to make sure file extension is a supported file
            if (!supportedFiles.Contains(fileExtention))
                return false;
            // Check to make sure size of file is less than 2MB and more than 1 KB
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}