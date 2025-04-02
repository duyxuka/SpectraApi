using System;

namespace Spectra.Services
{
    internal class FileRepo
    {
        public object FileName { get; set; }
        public object FileExtension { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}