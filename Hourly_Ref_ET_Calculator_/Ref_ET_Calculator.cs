using System;
using System.Collections.Generic;

namespace Hourly_Ref_ET_Calculator
{
    public class HourlyRefET
    {
        ////////////////////////////////////////////////////////////
        // Weather data & site info. entered by the user.
        ////////////////////////////////////////////////////////////
        private double Lm_longitude;
        //Longitude of center of selected time zone.
        private double Lz_longitude;
        // Latitude in degrees.
        private double phi_degree;
        private double z_elevation;
        // Set crop type alfalfa/grass.
        private string ref_crop = "";
        //Longitude of the center of the site's time zone.
        //Wind speed from weather station and the height at which wind speed was measured.
        private double Uz_WindSpeed;
        private double Zw_agl_WindRH_measurement;
        // Julian day DOY 0-366
        private double J_doy;
        private double Mjph2Wm2 = 277.7;
        private double Wm2toMjph = 1 / 277.7;
        private double beta_old, beta_avg =0;
        //private double fcd_03 = 0;
        private bool fcd_set_morning = false;
        private bool fcd_set_evening = false;
        //private double fcd_prev = 0;
        private double fcd_evening;
        private double fcd_morning;
        private double fcd_evening_hour;
        private double fcd_morning_hour;
        private double noon_time = 12;


        ////////////////////////////////////////////////////////////
        // Hourly data from CSV file.
        ////////////////////////////////////////////////////////////
        // Relative humidity %ge, Ta in C from weather station csv file.
        private double RH_humidity;
        private double Ta_air_Temperature;
        private double Rs_measured_rad;

        //Standard time
        private double t_std_time;
        private double t_mid_time;
        public double _t_mid_time { set { t_mid_time = value; } }

        // Set all weather and site parameters.
        public double _Lm_longitude
        {
            set { Lm_longitude = value; }
        }

        // Summary:
        //     Sets the site longitude, vaules are in decimal degrees.
        public double _Lz_longitude
        {
            // Sets the site longitude, vaules are in decimal degrees.
            set { Lz_longitude = value; }
        }
        public double _z_elevation
        {
            set { z_elevation = value; }
        }
        public string _ref_crop
        {
            set { ref_crop = value; }
        }

        public double _Uz_WindSpeed
        {
            set { Uz_WindSpeed = value; }
        }
        public double _Zw_agl_WindRH_measurement
        {
            set { Zw_agl_WindRH_measurement = value; }
        }
        public double _J_doy
        {
            set { J_doy = value; }
        }
        public double _t_std_time
        {
            set { t_std_time = value; }
        }
        public double _RH_humidity
        {
            set { RH_humidity = value; }
        }
        public double _Ta_air_Temperature
        {
            set { Ta_air_Temperature = value; }
        }

        public double _Rs_measured_rad
        {
            set { Rs_measured_rad = value*Wm2toMjph; }
        }

        public double _phi_degree
        {
            set { phi_degree = value; }
        }

        // Summary:
        //     Caclulates Ref. ETrz for each hour, one hour at a time.
        public Dictionary<string, double> Main_Calculation_Module()
        {
            double Sc = Calc_Sc(J_doy);
            double omega = Calc_omega(Lm_longitude, Lz_longitude, Sc, t_std_time);
            double dr = Calc_dr(J_doy);
            double delta_vapor = Calc_delta_vapor(Ta_air_Temperature);
            double delta_angle = Calc_delta_angle(J_doy);
            double phi = Calc_phi(phi_degree);
            double beta = Calc_beta(phi, delta_angle, omega);
            double omega_s = Calc_omega_s(phi, delta_angle);
            var omega_1n2 = Calc_omega1_omega2(omega, omega_s);
            double omega1 = omega_1n2.Item1;
            double omega2 = omega_1n2.Item2;
            double Ra = Calc_Ra(dr, omega2, omega1, phi, delta_angle);
            double Rso = Calc_Rso(Ra, z_elevation);
            //double fcd = Calc_fcd(Rs_measured_rad, Rso,beta);
            double TKhr = Calc_TKhr(Ta_air_Temperature);
            double es = Calc_es(Ta_air_Temperature);
            double ea = Calc_ea(RH_humidity, es);
            double Rns = Calc_Rns(Rs_measured_rad);
            double P = Calc_P(z_elevation);
            double gamma = Calc_gamma(P);
            double u2 = Calc_u2(Uz_WindSpeed, Zw_agl_WindRH_measurement, ref_crop);
            double Cn = Calc_Cn(ref_crop);
            double Cd = Calc_Cd(beta, ref_crop);
            double sin_phi = Calc_sin_phi(phi,delta_angle,omega);
            double W_precip_water = Calc_W(ea, P);
            double Kb = Calc_Kb(P, sin_phi, W_precip_water);
            double Kd = Calc_Kd(Kb);
            double Rso_adv = Calc_Rso_adv(Kb, Kd, Ra);
            double Rs_Rso_adv = Calc_Rs_Rso_adv(Rso_adv, Rs_measured_rad, beta,t_mid_time);
            double fcd_adv = Calc_fcd_adv(Rs_Rso_adv, beta, t_std_time);
            double Rnl = Calc_Rnl(fcd_adv, ea, TKhr);
            double Rn = Calc_Rn(Rns, Rnl);            
            double ETsz = 0;
            string CropName="";
            double G = Calc_G(Rn, Rs_measured_rad, ref_crop);

            if (ref_crop == "alfalfa")
            {
                CropName = "ETr";
                ETsz = (0.408 * delta_vapor * (Rn - G) + gamma * Cn / (Ta_air_Temperature + 273) * u2 * (es - ea)) / (delta_vapor + gamma * (1 + Cd * u2));
            }
            else
            {
                CropName = "ETo";
                ETsz = (0.408 * delta_vapor * (Rn - G) + gamma * Cn / (Ta_air_Temperature + 273) * u2 * (es - ea)) / (delta_vapor + gamma * (1 + Cd * u2));
            }

            //Create and send variable name and its value.
            Dictionary<string, double> results_output = new Dictionary<string, double>()
            {
                {"Sc",Sc},
                {"omega",omega},
                {"dr",dr},
                {"omega__1",omega1},
                {"omega__2",omega2},
                {"omega__s",omega_s},
                {"Ra",Ra *Mjph2Wm2},
                {"Rso",Rso*Mjph2Wm2},
                //{"fcd",fcd},
                {"TKhr",TKhr},
                {"es",es},
                {"ea",ea},
                {"Rnl",Rnl*Mjph2Wm2},
                {"Rns",Rns*Mjph2Wm2},
                {"Rn",Rn*Mjph2Wm2},
                {"G",G *Mjph2Wm2},
                {"P",P},
                {"gamma",gamma},
                {"u2",u2},
                {"Cn",Cn},
                {"Cd",Cd},
                {"delta__vapor",delta_vapor},
                {"delta__angle",delta_angle},
                {"phi",phi},
                {"beta",beta},
                {string.Format("{0}",CropName),ETsz},
                {"Tmid", t_mid_time},
                {"Rs_Rso_adv",Rs_Rso_adv },
                {"fcd_adv",fcd_adv },
                {"Kd",Kd },
                {"Kb",Kb },
                {"Rso_adv",Rso_adv*Mjph2Wm2 },
                {"W", W_precip_water },
                {"sin_phi", sin_phi },
            };

            return results_output;

        }

        private double Calc_sin_phi( double phi, double delta, double omega)
        {
            double sin_phi = Math.Sin(phi) * Math.Sin(delta) + Math.Cos(phi) * Math.Cos(delta) * Math.Cos(omega);
            return sin_phi;
        }
        private double Calc_Sc(double J_doy)
        //Seasonal correction for solar time.
        {
            double b = 2 * Math.PI * (J_doy - 81) / 364;

            double Sc = 0.1645 * Math.Sin(2 * b) - 0.1255 * Math.Cos(b) - 0.025 * Math.Sin(b);
            return Sc;
        }

        private double Calc_W(double ea, double P)
        {
            double W = 0.14 * ea * P + 2.1;
            return W;
        }

        private double Calc_Kb(double P, double sin_phi, double W_precip_water)
        {
            double Kt = 1;
            double Kb = 0.98 * Math.Exp(-0.00146 * P / Kt/sin_phi - 0.075 * Math.Pow((W_precip_water / sin_phi), 0.4));
            return Kb;
        }

        private double Calc_Kd(double Kb)
        {
            double Kd = 0.35 - 0.36 * Kb;
            return Kd;
        }

        private double Calc_Rso_adv(double Kb, double Kd, double Ra)
        {
            double Rso_adv = (Kb + Kd) * Ra;
            if (Rso_adv>Ra)
            {
                Rso_adv = 0.7 * Ra;
            }
            return Rso_adv;
        }

        private double Calc_fcd_adv(double Rs_Rso_adv, double beta, double t_std_time)
        {
            
            double fcd_adv = 1.35 * Rs_Rso_adv - 0.35;

            if (fcd_set_morning ==false || fcd_set_evening == false)
            {

                if (fcd_set_morning == false)
                //Set fcd beta>0.3 value equal to the last fcd value just before sunset
                {
                    if (beta > 0.3 && t_std_time < noon_time)
                    {   // Determining fcd at the sunset time when beta is just above 0.3 radians.
                        fcd_morning = fcd_adv;
                        fcd_set_morning = true;
                        fcd_morning_hour = t_std_time;
                    }
                }


                if (fcd_set_evening == false)
                    //Set fcd beta>0.3 value equal to the last fcd value just before sunset
                {
                    if (beta >= 0.3 && t_std_time > noon_time)
                    {   // Determining fcd at the sunset time when beta is just above 0.3 radians.
                        fcd_evening = fcd_adv;
                        fcd_evening_hour = t_std_time;
                    }

                    else if (beta<0.3 && t_std_time>noon_time)
                    {
                        fcd_set_evening = true;
                    }
                }
            }


            if (fcd_set_morning == true || fcd_set_evening == true)
            {
                if ( t_std_time > fcd_evening_hour)
                { fcd_adv = fcd_evening; }
                else if (t_std_time < fcd_morning_hour)
                { fcd_adv = fcd_evening; }
            }

            return fcd_adv;
        }




        private double Calc_Rs_Rso_adv(double Rso_adv, double Rs_measured_rad, double beta, double t_mid_time)
        {
            double Rs_Rso_adv = Rs_measured_rad / Rso_adv;
            if (Rs_Rso_adv < 0.3 || double.IsNaN(Rs_Rso_adv))
            {
                Rs_Rso_adv = 0.3;
            }
            else if (Rs_Rso_adv > 1)
            {
                Rs_Rso_adv = 1;
            }
            return Rs_Rso_adv;
        }

        private double Calc_beta(double phi, double delta, double omega)
        // beta, sun angle, angle of line of sight to Sun Vs flat ground. Checked REF-ET calculations, it is always +ve and -ve values are set to zero.
        {
            //double temp = Math.Asin(1);
            double beta = Math.Asin(Math.Sin(phi) * Math.Sin(delta) + Math.Cos(phi) * Math.Cos(delta) * Math.Cos(omega));
            if (beta < 0)
                beta = 0;

            beta_avg = (beta + beta_old) / 2;
            beta_old = beta;
            return beta_avg;
        }

        private double Calc_omega(double Lm_longitude, double Lz_longitude, double Sc, double t_std_time)
        {
            //convert time to mid hour time.
            if (t_std_time == 0)
            {
                t_mid_time = 23.5;
            }
            else
            { t_mid_time = t_std_time - 0.5; }

            double omega = Math.PI / 12 * ((t_mid_time) + 0.06667 * (Lz_longitude - Lm_longitude) + Sc - 12);

            return omega;
        }

        private double Calc_delta_angle(double J_doy)
        {
            double delta_angle = 0.409 * Math.Sin((2 * Math.PI * J_doy / 365) - 1.39);
            return delta_angle;
        }

        private double Calc_delta_vapor(double Ta_air_Temperature)
        {
            double delta_vapor = (2503 * Math.Exp(17.27 * Ta_air_Temperature / (Ta_air_Temperature + 237.3))) / Math.Pow((Ta_air_Temperature + 237.3), 2);
            return delta_vapor;
        }

        private double Calc_phi(double phi_degree)
        {

            return Math.PI / 180 * phi_degree;
        }

        private double Calc_omega_s(double phi, double delta)
        {
            double omega_s = Math.Acos(-Math.Tan(phi) * Math.Tan(delta));

            return omega_s;
        }

        private double Calc_dr(double J_doy)
        {
            double dr = 1 + 0.033 * Math.Cos(2 * Math.PI * J_doy / 365);
            return dr;
        }

        private Tuple<double, double> Calc_omega1_omega2(double omega, double omega_s)
        {
            double omega1 = omega - Math.PI / 24;
            double omega2 = omega + Math.PI / 24;

            if (omega1 < -omega_s)
            {
                omega1 = -omega_s;
            }
            if (omega2 < -omega_s)
            {
                omega2 = -omega_s;
            }

            if (omega1 > omega_s)
            {
                omega1 = omega_s;
            }

            if (omega2 > omega_s)
            {
                omega2 = omega_s;
            }
            if (omega1 > omega2)
            {
                omega1 = omega2;
            }

            return new Tuple<double, double>(omega1, omega2);
        }

        private double Calc_Ra(double dr, double omega2, double omega1, double phi, double delta)
        {
            //Gsc is a solar constant MJ m-2 h-1.
            double Gsc = 4.92;
            double Ra = 12 / Math.PI * Gsc * dr * ((omega2 - omega1) * Math.Sin(phi) * Math.Sin(delta) + Math.Cos(phi) * Math.Cos(delta) * (Math.Sin(omega2) - Math.Sin(omega1)));
            return Ra;
        }

        private double Calc_Rso(double Ra, double z_elevation)
        {
            double Rso = (0.75 + 2 * Math.Pow(10, -5) * z_elevation) * Ra;
            return Rso;
        }


        private double Calc_TKhr(double Ta_air_Temperature)
        {
            double TKhr = Ta_air_Temperature + 273.16;

            return TKhr;
        }

        private double Calc_es(double Ta_air_Temperature)
        {
            double es = 0.6108 * Math.Exp(17.27 * Ta_air_Temperature / (Ta_air_Temperature + 237.3));
            return es;
        }

        private double Calc_ea(double RH_humidity, double es)
        {
            double ea = RH_humidity / 100 * es;

            return ea;
        }

        private double Calc_Rnl(double fcd, double ea, double TKhr)
        {
            double sigma = 2.042 * Math.Pow(10, -10);

            double Rnl = sigma * fcd * (0.34 - 0.14 * Math.Pow(ea, 0.5)) * Math.Pow(TKhr, 4);
            return Rnl;

        }

        private double Calc_Rns(double Rs)
        {
            double albedo = 0.23;
            double Rns = (1 - albedo) * Rs;
            return Rns;
        }

        private double Calc_Rn(double Rns, double Rnl)
        {
            double Rn = Rns - Rnl;
            return Rn;
        }

        private double Calc_G(double Rn, double Rs_measured_rad, string ref_crop)
        {
            double G = -99999;
            if (ref_crop == "alfalfa")
            {
                if (Rs_measured_rad <= 0)
                {
                    G = 0.2 * Rn;
                }
                else
                {
                    G = 0.04 * Rn;
                }
            }
            if (ref_crop == "grass")
            {
                if (Rs_measured_rad <= 0)
                {
                    G = 0.5 * Rn;
                }
                else
                {
                    G = 0.1 * Rn;
                }
            }

            return G;
        }

        private double Calc_P(double z)
        {

            double P = 101.3 * Math.Pow((293 - 0.0065 * z) / 293, 5.26);
            return P;
        }

        private double Calc_gamma(double P)
        {
            double gamma = P * 0.000665;
            return gamma;
        }

        private double Calc_u2(double uz, double Zw, string ref_crop)
        {
            double u2= 0;
            double h = 0;
            double d = 0;
            double Zom = 0;

            if (ref_crop == "alfalfa")
            {   h = 0.5;  }
            else if (ref_crop == "grass")
            {   h = 0.12; }

            d = h * 0.67;
            Zom = 0.123 * h;
            u2 = uz * Math.Log((2 - d) / Zom) / Math.Log((Zw - d) / Zom);
            return u2;
        }


        private double Calc_Cn(string ref_crop)
        {
            double Cn = 0;
            if (ref_crop == "alfalfa")
            { //Day time & night time Cn are same.
                Cn = 66;
            }
            else if (ref_crop == "grass")
            {// Cn for grass is same either it is night or day.
                Cn = 37;
              }
            return Cn;
        }

        private double Calc_Cd(double Rs_measured_rad, string ref_crop)
        {
            double Cd = 0;

            if (ref_crop == "alfalfa")
            {

                if (Rs_measured_rad >0)
                {// Daytime Cd for alfalfa.
                    Cd = 0.25;
                }
                else
                {// Nighttime Cd for alfalfa
                    Cd = 1.7;
                }
            }
            else if (ref_crop == "grass")
            {
                if (Rs_measured_rad > 0)
                {// Daytime Cd for grass.
                    Cd = 0.34;
                }
                else
                {// Nighttime Cd for grass
                    Cd = 0.96;
                }
            }

            return Cd;
        }

    }
}
