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
        public DataTable Csv2dgv { get { return _Csv2dgv(csv_path); } }

        public DataTable _Csv2dgv(string csv_path)
        {
            var csv_data = new StreamReader(csv_path);

            var csv_datatable = new DataTable();
            var curr_row = csv_data.ReadLine().Split(',');
            csv_data.DiscardBufferedData();
            csv_data = new StreamReader(csv_path);

            csv_datatable.Columns.Add("SNo", Type.GetType("System.Int32"));
            csv_datatable.Columns.Add("Date", Type.GetType("System.String"));
            csv_datatable.Columns.Add("StdTime", Type.GetType("System.Double"));
            csv_datatable.Columns.Add("DOY", Type.GetType("System.Int32"));
            csv_datatable.Columns.Add("AirTemp", Type.GetType("System.Double"));
            csv_datatable.Columns.Add("RH", Type.GetType("System.Double"));
            csv_datatable.Columns.Add("Rs", Type.GetType("System.Double"));
            csv_datatable.Columns.Add("wind__spd", Type.GetType("System.Double"));

            int i = 1;
            while (!csv_data.EndOfStream)
            {
                var table_row = csv_datatable.NewRow();
                curr_row = csv_data.ReadLine().Split(',');
                
                // combile date time
                DateTime cur_date;
                //DateTime cur_date = new DateTime((int)Convert.ToDouble(curr_row[0]), (int)Convert.ToDouble(curr_row[1]), (int)Convert.ToDouble(curr_row[2]), (int)Convert.ToDouble(curr_row[3]),0,0);
                table_row["SNo"] = i;
                string temp_string = curr_row[0] + "/" + curr_row[1] + "/" + curr_row[2];
                cur_date = System.DateTime.Parse(curr_row[0] + "-" + curr_row[1] + "-" + curr_row[2]);

                //cur_date = System.DateTime.Parse(curr_row[0] + "," + curr_row[1] + "," + curr_row[2] + "," + curr_row[3] + "," + "0" + "," + "0");
                //table_row["StdTime"] = string.Format("{0:HH}", Convert.ToDateTime( curr_row[1]));
                table_row["DOY"] = cur_date.DayOfYear;
                table_row["StdTime"] = curr_row[3];
                table_row["Date"] = cur_date.Year + "/"+ cur_date.Month + "/" + cur_date.Day;
                table_row["AirTemp"] = curr_row[4];
                table_row["RH"] = curr_row[5];
                table_row["Rs"] = curr_row[6];
                table_row["wind__spd"] = curr_row[7];
                csv_datatable.Rows.Add(table_row);



                //int _year, _month,_date , _hour=0;

                //if (curr_row[0] is "" || curr_row[1] is "" || curr_row[2] is "" || curr_row[3] is "")
                //{
                //    curr_row[0] = "0";
                //    curr_row[1] = "0";
                //    curr_row[2] = "0";
                //    curr_row[3] = "0";
                //}


                //if (curr_row[3] == "24")
                //{
                //    _year = (int)Convert.ToDouble(curr_row[0]);
                //    _month = (int)Convert.ToDouble(curr_row[1]);
                //    _date = (int)Convert.ToDouble(curr_row[2]);
                //    _hour = (int)Convert.ToDouble(curr_row[3]);

                //    cur_date = new DateTime(_year, _month, _date, 0, 0, 0);

                //    cur_date = cur_date.AddDays(1);

                //    table_row["Date"] = string.Format("{0:MM/dd/yyy}", cur_date);

                //    table_row["StdTime"] = string.Format("{0:HH}", Convert.ToDateTime("0:00"));
                //}
                //else
                //{
                //    _year = (int)Convert.ToDouble(curr_row[0]);
                //    _month = (int)Convert.ToDouble(curr_row[1]);
                //    _date = (int)Convert.ToDouble(curr_row[2]);
                //    _hour = (int)Convert.ToDouble(curr_row[3]);

                //    cur_date = new DateTime(_year, _month, _date, _hour, 0, 0);


                //    table_row["Date"] = string.Format("{0:MM/dd/yyyy}", cur_date);

                //    table_row["StdTime"] = string.Format("{0:HH}", cur_date);
                //}


                ////table_row["StdTime"] = string.Format("{0:HH}", Convert.ToDateTime( curr_row[1]));
                //table_row["DOY"] = string.Format("{0}", cur_date.DayOfYear);
                //table_row["AirTemp"] = Math.Round(Convert.ToDouble(curr_row[4]), 3);
                //table_row["RH"] = Math.Round(Convert.ToDouble(curr_row[5]), 3);
                //table_row["Rs"] = Math.Round(Convert.ToDouble(curr_row[6]), 3);
                //table_row["wind__spd"] = Math.Round(Convert.ToDouble(curr_row[7]), 3);
                //csv_datatable.Rows.Add(table_row);
                i++;
            }

            csv_data.Close();
            return csv_datatable;
        }
    }
}
