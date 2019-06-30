#!/usr/bin/env python
# coding: utf-8

# In[66]:


import pandas as pd # This is always assumed but is included here as an introduction.
import numpy as np
import matplotlib.pyplot as plt
import parameters_ref_ET
import sqlite3

def read_sql_parameters():
    connection = sqlite3.connect("SIDSS_database.db")
    cur = connection.cursor()
    col_name='DOY'
    cur.execute(("SELECT {0},Date from MainDataGrid_Table WHERE {0}='160'").format(col_name))
    all_rows = cur.fetchall()
    print(all_rows)
    pd_table = pd.DataFrame(all_rows)
    pd_table
    print(all_rows)
    print("DOY",all_rows[0][0])
    print("Done")


def write_ETo2_SQL():
    connection = sqlite3.connect("ETo_Table.db")
    cur = connection.cursor()
    col_name='DOY'
    cur.execute(("SELECT {0},Date from MainDataGrid_Table WHERE {0}='160'").format(col_name))
    all_rows = cur.fetchall()
    print(all_rows)
    pd_table = pd.DataFrame(all_rows)
    pd_table
    print(all_rows)
    print("DOY",all_rows[0][0])
    print("Done")


Lat = np.deg2rad(parameters_ref_ET.Lat)  # Decimal degrees, convert it to radians.
Lm = np.deg2rad(parameters_ref_ET.Lm)  # Decimal degrees, convert to radians.
Elev = parameters_ref_ET.Elev  # Elevation of GLY04 in ft
Lz = parameters_ref_ET.Lz  # Longitude of the center of the local time zone(Rocky Mountain zone)
df = parameters_ref_ET.df  # data record frequency [sec]
# Define Constants
const_g = parameters_ref_ET.const_g  # acceleration due to gravity[m/s**2]
const_k = parameters_ref_ET.const_k  # Von Karman constant
const_Cpa=parameters_ref_ET.const_Cpa  # Specific heat capacities of air [J/kg.K]
const_Z_u = parameters_ref_ET.const_Z_u  # Wind reference height [m]
const_Z_T = parameters_ref_ET.const_Z_T  # Air Temp reference height [m], can be edited...
const_min_u = parameters_ref_ET.const_min_u  # Lowest allowed wind speed value [m/s]

calib_tif_file = parameters_ref_ET.tif_file_path
csv_weather_data_file = parameters_ref_ET.csv_file_path
# In[ ]:




# Reading data from onlin CoAgMet site and extract it for analysis.
#<img src = "https://coagmet.colostate.edu/images/signature-oneline.svg" />
#!pip install BeautifulSoup4
# In[68]:


table_hourly = pd.read_csv(csv_weather_data_file, header=None,
    names =['StationID', 'DateTime', 'MeanT', 'RH', 'VapPres', 'SolarRad', 'WindSpd', 'WindDir', 'STDVWindDir', 'Precip', 'SoilT5cm', 'SoildT15cm', 'Gust','GustTime', 'GustDir', 'ET_ASCE'])

sheet_2 = table_hourly
# pd.DataFrame(table_hourly)


# In[69]:


dateETR = table_hourly.iloc[:, 1]  # Date
dateETR = pd.to_datetime(dateETR)
#timeETR = table_hourly.iloc[:,2]  # Decimal hours 0 -1
TaETR = np.asarray(table_hourly.iloc[:, 2])  # Air Temp C
RsETR = np.asarray(table_hourly.iloc[:,5])  # Incoming Short-Wave Radiation Rs (KJ/m2/min)
RHETR = np.asarray(table_hourly.iloc[:,3]*100 ) # RH (%)
uETR = np.asarray(table_hourly.iloc[:,6])  # Wind Speed (m/s)

yyyyETR = dateETR.dt.year
MOYETR=dateETR.dt.month
ddETR = dateETR.dt.day
DOYETR = np.asarray(dateETR.dt.dayofyear)
#temp = pd.to_datetime(timeETR, format = "%Y-%m-%d %H:%M:%S")
timeETR = np.asarray(dateETR.dt.hour/24)
decimal_DOY_table_hourly = np.asarray(DOYETR+timeETR)


# In[61]:
Zm=const_Z_u  # reference height [m], for wind speed measurement


# In[62]:


esETR=0.6108 * np.exp(17.27 * TaETR / (TaETR+ 237.3)) #Saturated vapor pressure (kPa)

eaETR=esETR*RHETR/100 #actual vapor pressure (kPa)

T_aKETR = TaETR+ 273.15 # Converting the air temperature from Celsius degree to Kelvin degree


# Calculating Clear-Sky Irradiance to determine cloudiness ratio (Rs/Rso)


tETR = 24 * timeETR + 0.50 # Standard time at Mid Point as a function of time as hourly fraction

DLETR = 0.409 * np.sin(((2 * np.pi) / 365) * DOYETR - 1.39) # Solar declination angle in radians

HSETR = np.arccos(-(np.tan(Lat) * np.tan(DLETR))) # Sunset hour angle in radians

DRETR = 1 + 0.033 * np.cos(((2 * np.pi) / 365) * DOYETR) # Inverse relative distance factor



BETR = (2 * np.pi * (DOYETR - 81)) / 364 # Solar correction constant

ScETR = 0.1645 * np.sin(2 * BETR) - 0.1255 * np.cos(BETR) - 0.025 * np.sin(BETR) # Seasonal correction for solar time

OMEGAETR = (np.pi / 12) * ((tETR + 0.06667 * (Lz - np.rad2deg(Lm)) + ScETR) - 12) # Solar time angle at the midpoint of the period in radians 

t1ETR = 1.0 # Length of the calculation period

OMEGA1ETR = OMEGAETR - ((np.pi * t1ETR) / 24) # Solar time angle at the beginning of the period in radians

OMEGA2ETR = OMEGAETR + ((np.pi * t1ETR) / 24) # Solar time angle at the end of the period in radians

OMEGA1ETR = np.where(OMEGA1ETR<-HSETR,-HSETR,OMEGA1ETR)
OMEGA2ETR = np.where(OMEGA2ETR<-HSETR,-HSETR,OMEGA2ETR)
OMEGA1ETR = np.where(OMEGA1ETR>HSETR,HSETR,OMEGA1ETR)
OMEGA2ETR = np.where(OMEGA2ETR>HSETR,HSETR,OMEGA2ETR)
OMEGA1ETR = np.where(OMEGA1ETR>OMEGA2ETR,OMEGA2ETR,OMEGA1ETR)

G_scETR = 4.92 # Solar Constant (MJ/m**2/h)

R_aETR = (12 / np.pi) * G_scETR * DRETR * ((OMEGA2ETR - OMEGA1ETR) * np.sin(Lat) * np.sin(DLETR) + np.cos(Lat) * np.cos(DLETR) * (np.sin(OMEGA2ETR) - np.sin(OMEGA1ETR)))  # Extraterrestrial radiation (MJ/m**2/h)

R_SOETR = (0.75 + 2 * (10 **(-5)) * Elev * 0.3048) * R_aETR # Clear-Sky Irradiance (MJ/m**2/h)


# In[64]:


# Initial variables for the calculation:

# Cn and Cd are the coefficients for Alfalfa based reference ET calculation

# Calculating Atmospheric Pressure:
P = 101.3 *((293-0.0065*Elev*0.3048)/(293))**5.26 # Atmospheric Pressure in kPa

# Calculating the Psychometric Constant (gamma):
gamma = 0.000665 * P # kPa/C


#Calculating the slope of the saturation vapor pressure curve (delta) in kPa/C:
deltaETR = (2503 * np.exp((17.27 * TaETR)/(TaETR+237.3)))/((TaETR+237.3)**2) 


# Calculation of Net Radiation (Rn_ASCE) in (MJ/m^2/h):
# Calculating Net Short-Wave Radiation (Rns_ASCE) in (MJ/m^2/h):

albedo_ASCEETR = 0.23 # Fixed albedo assumed by Allen et al. (2005)

Rs_ASCEETR = (RsETR / 1000) * 60    # Converting Rs from KJ/m^2/min to MJ/m^2/h
# Divide Rs by 1000 (KJ/MJ) and multiply by 60 (min/h) to get (MJ/h/m^2 = MJ/m^2/h)
# x (KJ/min/m^2) = x (KJ/min/m^2)/(1000)(KJ/MJ)*(60)(min/h)

Rns_ASCEETR = (1-albedo_ASCEETR) * Rs_ASCEETR # NET SHORT-WAVE INCOMING RADIATION (MJ/m^2/h)


# Calculating Net Long-Wave Radiation (Rnl_ASCE) in (MJ/m^2/h):
sigma = 2.042 * 10**(-10) # Stefan-Boltzmann Constant in MJ/K^4/m^2/h

# Calculating the Clear-sky Radiation (Rso_ASCE) in (MJ/m^2/h):
Rso_ASCEETR = R_SOETR

# Cloudiness function (0.05 < fcd < 1.00)
fcdETR = 1.35 * (Rs_ASCEETR / Rso_ASCEETR)-0.35
fcdETR = np.where(fcdETR<0.05,0.05,fcdETR)
fcdETR = np.where(fcdETR>1,1,fcdETR)


# Net Long-Wave Radiation (Rnl_ASCE) in (MJ/m^2/h):
Rnl_ASCEETR = sigma * fcdETR * (0.34-0.14 * (eaETR**0.50)) * (TaETR+273.15)**4


# Net Radiation (Rn_ASCE) in (MJ/m^2/h):
Rn_ASCEETR = Rns_ASCEETR - Rnl_ASCEETR

# Rn_ASCEETR in Watts/m2
Rn_ASCEETR_watts = Rn_ASCEETR/0.0036

# Soil Heat Flux (G_ASCE) in (MJ/m^2/h):
G_ASCEETR = np.where(Rn_ASCEETR>0, 0.04*Rn_ASCEETR,0.2*Rn_ASCEETR)

# Adjusting wind speed height to standardized weather station:
if const_Z_u != 2:

    u2ETR = uETR * (4.87 / (np.log(67.8 * const_Z_u - 5.42)))
else:
    u2ETR = uETR


NUMERATORETR = (0.408 * deltaETR * (Rn_ASCEETR - G_ASCEETR) + gamma * (66 /(TaETR+273))* u2ETR * (esETR-eaETR))

DENOMINATORETR = np.where(Rnl_ASCEETR>0,(deltaETR + gamma *(1+ 0.25 * u2ETR)),(deltaETR + gamma *(1+ 1.70 * u2ETR)))

# Instantaneous Alfalfa Reference ET based on ASCE Standardized Method
# (Allen et al., 2005)

ETr_ETR = NUMERATORETR/DENOMINATORETR

print("all calculations done")


# In[65]:


plt.rcParams['figure.figsize'] = [20, 15] # Command to incerase the plot size.(numbers represent inches?)

formated_results_sheet2 = pd.DataFrame({      'Etr_ETR': ETr_ETR,
                                              'Rn_ASCEETR_watts': Rn_ASCEETR_watts,
                                              'G_ASCEETR': G_ASCEETR,
                                        }, index=decimal_DOY_table_hourly
                                       )
formated_results_sheet2.to_csv('results_ref_ET_asce.csv')
plotted_fig = formated_results_sheet2.plot(subplots=True)
plt.savefig('results_ref_ET_asce.png')
print("Done!!!!")

