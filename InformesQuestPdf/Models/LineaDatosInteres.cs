using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformesQuestPdf.Models
{
    public class LineaDatosInteres
    {
        
        public long IdVale { get; set; }
        public string IdCoop { get; set; }
        public string IdEmpa { get; set; }
        public long Nlin { get; set; }

        public string Producto { get; set; }
        public string Termografo { get; set; }
        public string Temp { get; set; }
    
    }
}
