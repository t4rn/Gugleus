using Gugleus.Core.Domain;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.WebUI.Models.Logs
{
    public class FileLogVM
    {
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        [Display(Name = "File Size")]
        public string FileSize { get; set; }
        [Display(Name = "Modification Date")]
        public string ModificationDate { get; set; }

        [Display(Name = "Content")]
        public string FileContent { get; set; }

        public EnvType? Env { get; internal set; }
    }
}
