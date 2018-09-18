using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HyggeMail.ImportExport
{
    public class ImportExcel
    {
        public DataSet ImportExcelSheet(HttpPostedFileBase uploadfile)
        {
            if (uploadfile != null && uploadfile.ContentLength > 0)
            {
                //ExcelDataReader works on binary excel file
                Stream stream = uploadfile.InputStream;
                //We need to written the Interface.
                IExcelDataReader reader = null;
                if (uploadfile.FileName.EndsWith(".xls"))
                {
                    //reads the excel file with .xls extension
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (uploadfile.FileName.EndsWith(".xlsx"))
                {
                    //reads excel file with .xlsx extension
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    //Shows error if uploaded file is not Excel file
                    //("File", "This file format is not supported");
                    return null;
                }
                //treats the first row of excel file as Coluymn Names
                reader.IsFirstRowAsColumnNames = true;
                //Adding reader data to DataSet()
                DataSet result = reader.AsDataSet();
                reader.Close();
                //Sending result data to View


                return result;
            }

            return null;
        }
    }
}
