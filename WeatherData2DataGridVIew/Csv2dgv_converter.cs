using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace WeatherData2DataGridVIew
{
    public class Csv2dgv_converter
    {
        private string csv_path;
        public string _csv_path { set { csv_path = value; } }
        public DataTable get_data_table;
        public DataTable Csv2dgv { get  { return _Csv2dgv(csv_path);  } }

        public DataTable _Csv2dgv(string csv_path)
        {
            var csv_data = new StreamReader(csv_path);

            var csv_datatable = new DataTable();
            var curr_row = csv_data.ReadLine().Split(',');
            csv_data.DiscardBufferedData();
            csv_data = new StreamReader(csv_path);

            csv_datatable.Columns.Add("SNo");
            csv_datatable.Columns.Add("Date");
            csv_datatable.Columns.Add("StdTime");
            csv_datatable.Columns.Add("DOY");
            csv_datatable.Columns.Add("AirTemp");
            csv_datatable.Columns.Add("RH");
            csv_datatable.Columns.Add("Rs");
            csv_datatable.Columns.Add("wind__spd");

            int i = 1;
            while (!csv_data.EndOfStream)
            {
                var table_row = csv_datatable.NewRow();
                curr_row = csv_data.ReadLine().Split(',');
                //csv_datatable.Rows.Add();
                table_row["SNo"] = i;
                table_row["Date"] = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(curr_row[0]));
                table_row["StdTime"] = string.Format("{0:HH}", Convert.ToDateTime(curr_row[1]));
                table_row["DOY"] = string.Format("{0}", Convert.ToDateTime(curr_row[0]).DayOfYear);
                table_row["AirTemp"] = Math.Round(  Convert.ToDouble(curr_row[2]),3);
                table_row["RH"] = Math.Round(Convert.ToDouble(curr_row[3]), 3);
                table_row["Rs"] = Math.Round(Convert.ToDouble(curr_row[4]), 3);
                table_row["wind__spd"] = Math.Round(Convert.ToDouble(curr_row[5]), 3);
                csv_datatable.Rows.Add(table_row);
                i++;
            }

            csv_data.Close();
            return csv_datatable;
        }

           

    }
}
