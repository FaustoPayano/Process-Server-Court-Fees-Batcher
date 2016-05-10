using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourtFeesBatchComposer.Model {
    public class CourtFeesFile {
        public List<CourtFeesObject> CourtFees = new List<CourtFeesObject>();
        public string CourtDate { get; set; }
        public FileInfo CourtFeeFileInfo { get; set; }
        public string FileName { get; set; }
        private int NumberofRecords { get; set; }
        private DataTable AllFeesInRecords { get; set; }


        public CourtFeesFile(string courtFeeFilePath) {
            CourtFeeFileInfo = new FileInfo(courtFeeFilePath);
            FileName = Path.GetFileNameWithoutExtension(CourtFeeFileInfo.Name);
            //The first element of the array will always be the date.. as long as they follow the same naming convention.
            CourtDate = FileName.Split(null)[0];
            ExtractFileContents();
        }
        
        /// <summary>
        /// Pulls court fee information from this object's file, line by line, adding it to the Courtfees property.
        /// </summary>
        public void ExtractFileContents() {
            var reader = new StreamReader(CourtFeeFileInfo.FullName);
            
            using (reader) {
                var line = String.Empty;
                while ((line = reader.ReadLine()) != null) {
                    //The first line will of these files will always contain this string, indicating the fields. This line prevents them from being added to our results.
                    if (line != "" && !line.Contains("\"Defendant\",\"IndxNumber\",\"AttyFileNum\",\"AmountCharged\",\"InvoiceNum")) {
                        CourtFees.Add(ConvertToCourtFeesObject(line));
                    }
                }
            }
            NumberofRecords = CourtFees.Count;

        }

        /// <summary>
        /// Generates a CourtFeesObject from a single entry within a court fee file;
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        private CourtFeesObject ConvertToCourtFeesObject(string inputLine) {
            string[] courtFeeInfoWithQuotes = inputLine.Split(',');
            List<string> unquotedCourtFeeInfo = new List<string>();
            foreach (var line in courtFeeInfoWithQuotes) {
                 unquotedCourtFeeInfo.Add(line.Replace("\"", ""));
            }
            string[] courtFeeInfo = unquotedCourtFeeInfo.ToArray();
            
            
            string defendant = courtFeeInfo[0];
            string indexNumber = courtFeeInfo[1];
            string matterNumber = courtFeeInfo[2];

            decimal courtFee;
            decimal.TryParse(courtFeeInfo[3], out courtFee);
            int invoiceNum;
            int.TryParse(courtFeeInfo[4], out invoiceNum);
            // DateTime is provided in the following format  - YYYY-mm-DD
            DateTime dateCompleted = DateTime.ParseExact(courtFeeInfo[5], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                         
            return new CourtFeesObject() {Defendant = defendant,
                IndexNumber = indexNumber,
                MatterNumber = matterNumber,
                CourtFee = courtFee,
                InvoiceNumber = invoiceNum,
                DateCompleted = dateCompleted
            };
        }

        public override string ToString() {
            return $"Process Server File {CourtDate} with {NumberofRecords.ToString()} record(s).";
        }
    }
}
