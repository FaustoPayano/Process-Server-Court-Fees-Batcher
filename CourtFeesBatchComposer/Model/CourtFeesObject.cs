using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourtFeesBatchComposer.Model {
    public class CourtFeesObject {
        public string Defendant { get; set; }
        public string IndexNumber { get; set; }
        public string MatterNumber { get; set; }
        public decimal CourtFee { get; set; }
        public int InvoiceNumber { get; set; }
    }
}
