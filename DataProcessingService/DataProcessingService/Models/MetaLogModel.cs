using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessingService.Models
{
    public class MetaLogModel
    {
        public int parsed_files { get; set; } = 0;
        public int parced_lines { get; set; } = 0;
        public int found_errors { get; set; } = 0;

        public List<string> invalid_files { get; set; } = new List<string>();
    }
}
