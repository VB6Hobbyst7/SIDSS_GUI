using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyBalanceET
{
    class SiteInfo
    {

        double accl_g = 9.81;
        double karman_k = 0.41;
        double data_freq = 3600;
        double Cpa = 1005;
        public double SiteElevation { get; set; }
        public double Z_u { get; set; }

        public double Z_t { get; set; }
        public double min_u { get; set; }




    }
}
