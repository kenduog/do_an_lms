using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class FileValidator
    {
        private static readonly List<string> AllowedImageExtensions = new List<string>
        {
            ".jpg", ".jpeg", ".png"
        };
        private static readonly List<string> AllowedAudioExtensions = new List<string>
        {
           ".mp3", ".wav", ".ogg"
        };

        private static readonly List<string> AllowedVideoExtensions = new List<string>
        {
           ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv", ".mpg", ".mpge", ".webm"
        };

        private static readonly List<string> AllowedOfficeExtensions = new List<string>
        {
           ".doc", ".docx", ".pdf", ".txt", ".csv", ".xls", ".xlsx", ".pptx", ".ppt"
        };

        public static bool IsVideoFile(IFormFile file)
        {
            if (file == null)
                return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return AllowedVideoExtensions.Contains(fileExtension);
        }
        public static bool IsImageFile(IFormFile file)
        {
            if (file == null)
                return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return AllowedImageExtensions.Contains(fileExtension);
        }
        public static bool IsOfficeFile(IFormFile file)
        {
            if (file == null)
                return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return AllowedOfficeExtensions.Contains(fileExtension);
        }
        public static bool IsAudioFile(IFormFile file)
        {
            if (file == null)
                return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return AllowedAudioExtensions.Contains(fileExtension);
        }
        public static bool IsDocumentFile(IFormFile file)
        {
            if (file == null)
                return false;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            return IsAudioFile(file) || IsImageFile(file) || IsOfficeFile(file) || IsVideoFile(file);
        }
        public static int GetCurriculumType(IFormFile file)
        {
            if (file == null)
                return 0;
            return IsAudioFile(file) ? 1 : IsImageFile(file) ? 3 : IsOfficeFile(file) ? 2 : 0;
        }
        public static List<string> GetAllowedExtension()
        {
            return AllowedOfficeExtensions.Concat(AllowedImageExtensions).Concat(AllowedAudioExtensions).Concat(AllowedVideoExtensions).ToList();
        }
    }
}
