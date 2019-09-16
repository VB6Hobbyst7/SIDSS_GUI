# -----------------------
# # Python script to calculate ET and other parameters using imagery & weather data
# ------------------

import parameters_ref_ET as parameters
import pickle
from time import time
import sys
import os
import numpy as np
import math
# import xlrd
import pandas as pd
import matplotlib.pyplot as plt
import rasterio
#get_ipython().run_line_magic('matplotlib', 'inline')
plt.rcParams['figure.figsize'] = [10, 10] # Command to increase the plot size.(numbers represent inches?)
print(f"Path to python execuable is:- {sys.executable}")
print ("This script is running from", sys.argv[0])
temp=np.seterr(all='ignore')

# ground_data = {"Date":"8/31/2017 12:00",	"Ta":25.97,	"Ts":26.90,"Rs":48.57,"NDVI":0.885,"OSAVI":0.747,"RH":48.2,"Wnd_spd":1.267,"Wnd_dir":125}



calib_MS_file_name=r"F:\RSET_0\2018_RSET\LIRF_2018\Quick_Mosaic\TIFF\Rect_to_Basemap\ET_Prep\20180831_LIRF_MS_mosaic_rect_calib_cut1.tif"
# calib_MS_file_name=parameters.EB_MS_file_path
with rasterio.open(calib_MS_file_name) as src:
    NIR = src.read(1)/100
    Red = src.read(2)/100

FLIR_calib=r"F:\RSET_0\2018_RSET\LIRF_2018\Quick_Mosaic\TIFF\Rect_to_Basemap\ET_Prep\20180831_LIRF_DOY243_FLIR_mosaic_calib_rect_cut_resz.tif"
# FLIR_calib = parameters.EB_Thermal_file_path
with rasterio.open(FLIR_calib) as src:
    # Surface temperature Ts is obtained from the image instead of point data from on-site measurements.
    col_Ts = src.read(1)

kwargs = src.meta
kwargs.update(
    dtype=rasterio.float32,
    count = 1,
    compress='lzw')


def save_raster(f_name,array_name):
    with rasterio.open(f_name, 'w', **kwargs) as dst:
        dst.write_band(1, array_name.astype(rasterio.float32))
    dst.close()

def read_raster(f_name):
    with rasterio.open(f_name) as src:
        array_name = src.read(1)
        return array_name

# def calc_NDVI():
#     NIR, RED  = read_raster_as_array()
NDVI = np.where((NIR+Red)==0,0,(NIR-Red)/(NIR+Red))

Y=0.16 
OSAVI = (1+Y)*(NIR-Red)/(NIR+Red+Y)
save_raster("OSAVI.tif",OSAVI)

# Site specific information
Lat = np.deg2rad(parameters.Lat)    # Decimal degrees, convert it to radians.
Lm = np.deg2rad(parameters.Lm)      # Decimal degrees, convert to radians
Elev = (1424)*100/2.54/12           # Elevation of GLY04 in ft
Lz = parameters.Lz                  # Longitude of the center of the local time zone(Rocky Mountain zone)
interval_sec = 3600                 # data record frequency 3600 sec for 1 hour.

# Define Constants
const_g = parameters.const_g        # acceleration due to gravity[m/s**2]
const_k = 0.41        # Von Karman constant
const_Cpa= 1005     # Specific heat capacities of air [J/kg.K]
const_Z_u = 3.3    # Wind reference height [m]
const_Z_T = 3.3   # Air Temp reference height [m], can be edited...
Z_uET_r = 2 # WEATHER STATION Wind reference height [m]
Z_TET_r = 1.5  # WEATHER STATION Air Temp reference height [m]
const_min_u= 0.5 # Lowest allowed wind speed value [m/s]


# # 1. LE Model calculation

#######################################################################
# On-Site parameters obtained from the datalogger tower within the research field.
#######################################################################
col_date = pd.to_datetime(parameters.EB_YYYYMMDDHH_txt)  # Date
col_time = col_date.hour
col_Ta = 28.44 # Air Temp C from on site sensor aug 13 2018
#col_Ts = ground_data["Ts"]  # Surface Temp C  "Add thermal image array"
col_Rs = parameters.EB_Rs_txt  # Radiation SW, incoming KJ/m2/min
col_NDVI = NDVI  # NDVI band ratio
col_OSAVI = OSAVI  # OSAVI band values
col_RH = parameters.EB_RH_txt  # Relative Humidity
col_wind_spd = parameters.EB_Wind_Spd_txt # Wind speed
col_theta = parameters.EB_wind_dir_txt  # Wind direction.
u = col_wind_spd


#######################################################################
# Weather Station parameters, to be read from the parameters file. 
# And finally add textboxes/fields for user input in SIDSS.
#######################################################################
dateET_r = col_date #"07/25/2018" # textET_r(2:length(textET_r),1); % Weather Station Date in mm/dd/yyyy format
timeET_r = 12.5  # dataET_r(1:length(dataET_r),1); % Weather Station time as fraction of hour
TimeStringET_r = ""  # datestr(timeET_r); % Weather Station Time as a name
TaET_r = 25.6 # dataET_r(1:length(dataET_r),2); % Weather Station Air Temperature (C)
RsET_r = 568.5  # dataET_r(1:length(dataET_r),3); % Weather Station Incoming short-wave radiation (KJ/m^2/min)
RHET_r = 84.2  # dataET_r(1:length(dataET_r),4); % Weather Station Relative Humidity (%)
uET_r = 1.6 # dataET_r(1:length(dataET_r),5); % Weather Station Wind Speed @ 2 m measurement height (m/s)



# No need to convert wind speed at 
# u = col_wind_spd* (4.87 / np.log(67.8 * 3.3 - 5.42))**(-1)

# row = 1


# -------------
# > Convert to DOY & decimal_DOY arrays from column 1 & column 2

yyyy = col_date.year
MOY=col_date.month
dd = col_date.day
DOY = col_date.dayofyear
col_time_dec = col_time/24 #col_time converted to decimal time hours/24 to get decimal part of the day.
decimal_DOY_sheet_1 = DOY+col_time_dec


# --------------
# > Calculating several parameters
# -------------

# u = np.where(const_min_u>u,const_min_u,u) # Min. wind speed correction. Limit minimum wind speed value to const_min_u.
if const_min_u>u:
    u=const_min_u

# lambda_v=(2.501-0.2361*col_Ta*10**-3)*10**6 #latent heat of vaporization (MJ/Kg)
lambda_v=(2.501-0.00236 * col_Ta)*10**6 #latent heat of vaporization (J/Kg), On-Site
lambda_vET_r = lambda_v # WEATHER STATION latent heat of vaporization (J./Kg)
LAMBDA_V = lambda_v

Pa = 101325*(1-2.25577e-5*Elev*0.3048)**5.25588  # Atmospheric pressure [Pascal]
es=0.6108 * np.exp(17.27 * col_Ta / (col_Ta+ 237.3))  # Saturated vapor pressure (kPa)
esET_r = 0.6108 * np.exp(17.27 * TaET_r/ (TaET_r+ 237.3)) # WEATHER STATION Saturated vapor pressure (kPa)

ea=es*col_RH/100  #actual vapor pressure (kPa)
eaET_r = esET_r * RHET_r/100  # WEATHER STATION actual vapor pressure (kPa)

Tkv=(col_Ta+273.15)/(1-0.378*ea/Pa/1000)  # virtual temperature (K)
rho_a=3.486*Pa/1000/Tkv  # [Kg/m3], Pa=[Pascal], Ta=[C], (ASCE EWRI 2005)
Zm=const_Z_u  # reference height [m], for wind speed measurement


# ### SECTION 01 - NET RADIATION MODEL

#  Adjust Ta at the U level

if const_Z_u-const_Z_T>0.15:
    ws = 621.97*es/(Pa/1000-es)
    w=col_RH/100*ws  # mixing ratio    
    # moist adiabatic lapse rate (Montieth (2013))
    Lapse=const_g*(1+lambda_v*w/(287*(col_Ta+273.15)))/(const_Cpa+0.622*lambda_v**2*w/(287*(col_Ta+273.15)**2))
    Ta=col_Ta-Lapse*(const_Z_u-const_Z_T)  # adjusted air temperature at u level
else:
    Ta = col_Ta

ea_mb = 10*ea


Ta_K = col_Ta+273.15
Ts_K = np.asarray(col_Ts)+273.15
Rs_inc = col_Rs*(1000/60) # MJ/m2/hour

t = 24* col_time_dec + 0.5

DL = 0.409 * np.sin(2*np.pi/365*DOY-1.39)

HS = np.arccos(-np.tan(Lat)*np.tan(DL))

DR = 1 + 0.033  * np.cos((2 * np.pi / 365) * DOY)

B = (2*np.pi*(DOY-81))/364

Sc = 0.1645 * np.sin(2 * B) - 0.1255 * np.cos(B) - 0.025 * np.sin(B)

# Solar time angle at the midpoint of the period in radians
OMEGA = (np.pi / 12) * ((t + 0.06667 * (Lz - np.rad2deg(Lm)) + Sc) - 12)

t1 = 1  # Length of the calculation period.


OMEGA1 = OMEGA - (np.pi * t1/24)

OMEGA2 = OMEGA + (np.pi*t1/24 )

# OMEGA1 = np.where(OMEGA1 <  (-HS), -HS,OMEGA1)
if OMEGA1<-HS:
    OMEGA1=-HS

# OMEGA2 = np.where(OMEGA2 <  (-HS), -HS,OMEGA2)
if OMEGA2<-HS:
    OMEGA2=-HS

# OMEGA2 = np.where(OMEGA2 > HS, HS,OMEGA2)
if OMEGA2>HS:
    OMEGA2=HS

# OMEGA1 = np.where(OMEGA1 > OMEGA2, OMEGA2,OMEGA1)
if OMEGA1>OMEGA2:
    OMEGA1=OMEGA2


G_sc = 4.92  # Solar constant

# Extraterrestrial radiation (MJ/m**2/h)
R_a = (12 / np.pi) * G_sc * DR * \
    ((OMEGA2 - OMEGA1) * np.sin(Lat) * np.sin(DL)
     + np.cos(Lat) * np.cos(DL) * (np.sin(OMEGA2)
     - np.sin(OMEGA1)))

R_SO = (0.75 + 2 * (10 ** (-5)) * Elev * 0.3048) * R_a  # Clear-Sky Irradiance (MJ/m**2/h)

R_so = R_SO * (1000000 / 3600)  # Clear-Sky Irradiance in W/m**2

if R_so>0:
    s=Rs_inc/R_so
else:
    s=0


# s = np.where(s<0.3,0.3,s)
if s<0:
    s=0.3


# s = np.where(s>1,1,s)
if s>1:
    s=1

clf = 1-s  # # Cloud Fraction Term


# > Calculating the Emissivity of Air    

# Emissivity of the air (Crawford and Dushon, 1999)
Ea = clf + (1 - clf) * (1.22 + 0.06 * np.sin((MOY + 2) * (np.pi / 6))) * (ea_mb / Ta_K) ** (1 / 7)  

# > Calculating Fractional Percent Cover

fc = np.where(col_NDVI<=0.15,0,(1.26*col_NDVI-0.18))

# > Calculating the Surface Emissivity from Fractional Vegetation Cover

e_v = 0.98 # e_v stands for Emissivity for a fully vegetated area and is equal to 0.98

e_s = 0.93  # e_s stands for Emissivity for a bare soil and is equal to 0.93

Es = fc * e_v + (1 - fc) * e_s  # Surface Emissivity equation (Brunsell and Gillies, 2002) 
# Es stands for Surface Emissivity (dimensionless)

# > Calculating albedo based on the empirical relationship developed based on LAI

LAI = 0.263 * np.exp(3.813 * col_OSAVI)  # Leaf Area Index Calculation (Chavez et al., 2010)

ALBEDO = 0.0179 * LAI**(-3.27) + 0.1929  # Example of albedo calculation
# Replace this equation. Use albedo = 0.512 (RED) + 0.418 (NIR)

# Calibrated Albedo Function
albedo = 0.1746 - 0.0037 * np.cos(211.2 * ALBEDO)+0.0048 * np.sin(211.2 * ALBEDO)

# > Calculating the Net Radiation Flux (Rn)
Rs_out = albedo*Rs_inc # Rs_out stands for Outgoing Shortwave Radiation in W/m**2

Rl_in = Es* Ea * (5.67*10**(-8))*(Ta_K**4) # Rl_out stands for the Incoming Long-wave Radiation in W/m**2

Rl_out = Es * (5.67*10**(-8))*(Ts_K**4)  # Rl_in stands for the outgoing Long-wave Radiation in W/m**2

Rn = Rs_inc - Rs_out + Rl_in - Rl_out # Rn stands for the Estimated Net Radiation in W/m**2


# ### SECTION 02 - SOIL HEAT FLUX MODEL

# > Calculating the Soil Heat Flux (G)

G = 34.15 * np.log(col_Ts) - 48.31 * np.exp(col_OSAVI) + 0.02 * (Rn**2 * albedo**3)+20.64 * fc**5 # Soil Heat Flux Model

#row = row + 1 # Term to keep the interation process for each cell of the imported spreadsheet


# ### SECTION 03 - SENSIBLE HEAT FLUX MODEL

# Model to calculate crop height based on LAI
Ho= 0.697 *np.exp(0.236*LAI)-3.42*np.exp(-3.177*LAI)

crop_height_max_index = np.argmax(Ho)
crop_height_max = Ho.max()
hc = Ho  # hc is crop height obtained using vegetation index. Chavez et. al used hc_corn = (1.86*OSAVI-0.2)*(1+4.82E-7*EXP(17.69*OSAVI))
hc_2 = (1.86*col_OSAVI-0.2)*(1+4.82E-7*np.exp(17.69*col_OSAVI))

    
Wind_Direc = col_theta  # # Wind Speed Direction (degrees decimal)
Wind_Speed = col_wind_spd  # # Wind Speed (m/s)

#     
# > Calculating the roughness length variables of the canopy
X = 0.2* LAI
# d = 0.67 * hc  # #zero plane displacement height [m]
# Below equation giving -Ve numbers for zero plane displacement height.
# d=hc*(np.log(1+(X**(1/6)))+0.03*np.log(1+(X**6))) # zero plane displacement (Choudh 369
# Refered Chowdhary et.al paper 1988 and found following equation.
d=1.1*hc*(np.log(1+(np.power(X,0.25)))) # zero plane displacement (Choudh 369
# REPLACE THE Zom EQUATION FOR THE FOLLOWING ONE (lines 337 to 345)
Zom = np.where(X<0.2,(0.01 + 0.28*hc*(X**0.5)),(0.3 * hc * (1-d/hc)))

# Zom = 0.123 * hc  # #roughness length for momentum [m]

Zoh = Zom * 0.10  # #roughness length for heat transfer [m]
# rp = np.ones(col_theta.shape)
rp = col_theta

# Calculating the Turbulence-Mixing Row Resistance (rp) (PROPOSED MODEL  FROM Edson's THESIS)  

if Wind_Direc<=90:
    rp=Wind_Direc/(180-Wind_Direc)/Wind_Speed


#    elseif theta(z) > 90 && theta(z) <= 180
# rp = np.where((Wind_Direc>90) & (Wind_Direc<=180),((180-Wind_Direc)/Wind_Direc/Wind_Speed),rp)
elif Wind_Direc>90 and Wind_Direc<=180:
    rp=(180-Wind_Direc)/Wind_Direc/Wind_Speed


#    elseif Wind_Direc > 180 && Wind_Direc <= 270
# rp = np.where((Wind_Direc>180) & (Wind_Direc<=270),((Wind_Direc-180)/(360-Wind_Direc)/Wind_Speed),rp) 
elif Wind_Direc>180 and Wind_Direc<=270:
    rp=(Wind_Direc-180)/(360-Wind_Direc)/Wind_Speed


#    elseif Wind_Direc > 270 && Wind_Direc <= 360
# rp = np.where((Wind_Direc>270) & (Wind_Direc<=360),((360-Wind_Direc)/(Wind_Direc-180)/Wind_Speed),rp) 
elif Wind_Direc>270 and Wind_Direc<=360:
    rp=(360-Wind_Direc)/(Wind_Direc-180)/Wind_Speed

To = np.empty(LAI.shape)
To.fill(0.0)
# > Calculating Surface Aerodynamic Temperature (To); New model based.

To = np.where(LAI<=1.5,(-8.742 * fc + 0.571 * Ta + 0.529 * col_Ts + 0.806 * rp + 3.295),To) 
To = np.where((LAI>1.5) & (LAI<=2.5),(-9.168* fc + 0.485 * Ta + 0.575 * col_Ts - 0.160 * rp + 6.491),To)  
To = np.where((LAI>2.5) & (LAI<=3.5),(4.708 * fc + 0.350 * Ta + 0.580 * col_Ts + 0.086 * rp),To)  
To = np.where((LAI>3.5),(-1.912 * fc + 0.443 * Ta + 0.509 * col_Ts + 0.115 * rp + 5.014),To) 


# #### Monin-Obhukov Stability Theory (MOST), for atmospheric stability corrections
# #### First Iteration, Assume Neutral Atmospheric Conditions:

U_star=(u*const_k)/(np.log((Zm-d)/Zom))
rah=np.log((Zm-d)/Zom)*np.log((Zm-d)/Zoh)/(u*const_k**2)
H=(rho_a*const_Cpa)*(To-Ta)/rah
L_mo=-(U_star**3*(Ta+273.15)*rho_a*const_Cpa)/(const_g*const_k*H) # MOST length (L, m)

# #### Stability is determined according to the Monin-Obukhov length (L, m)

img_height, img_width =NDVI.shape

#############################################
# Save all the rasters to tiff files for C++ program input.
# Note TIFF file names should note change.
save_raster("L_mo_in.tif",L_mo)
save_raster("U_star_in.tif",U_star)
save_raster("rah_in.tif",rah)
save_raster("H_in.tif",H)
save_raster("Zom_in.tif",Zom)
save_raster("d_in.tif",d)
save_raster("Zoh_in.tif",Zoh)
save_raster("To_in.tif",To)
############################################


execution_command = '"'+r"C:\Users\HomeUser\source\repos\Array_Processor_Gdaled\x64\Release\Array_Processor_Gdaled.exe"+'"'+" \
    {0} {1} {2} {3} {4} {5} {6} {7} {8}".format(u,const_k,const_Cpa,const_g,rho_a,Ta,Zm,img_width,img_height)
print (execution_command)
os.system(execution_command)

##########################################################
#  Read the output of C++ module back into the arrays
##########################################################
H=read_raster("H_out.tif")
L_mo=read_raster("L_mo_out.tif")
rah=read_raster("rah_out.tif")
U_star=read_raster("U_star_out.tif")

# ### SECTION 04 - INSTANTANEOUS LATENT HEAT FLUX

#ET from Energy Balance
LE=Rn-G-H
# LE = np.where(LE<0,np.nan,LE)


# ### SECTION 05 - CONVERTING INSTANTANEOUS LATENT HEAT FLUX TO INSTANTANEOUS ET WATER DEPTH

ET_inst=interval_sec*LE/lambda_v/1

# ### SECTION 06 - OUTLIERS FILTERING FOR ALL MODELS (Rn, G, H, LE)
# 

#Filter Output from outliers:
#---------------------------
interp = 'lower'
p_min = 1/100 # Minimum cutoff percentile
p_max = 99/100 # Max cutoff percentile.
#Note matlab's percentile funcition is essentially same as numpy's quantile function.
# There is numpy percentile function too, but the results doesn't match well with matlab's percentile function.


# Clean H values
percentiles_H = np.nanquantile(H,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values


# Remove very low or very high values(1 & 99 percentile).
H = np.where((H<=percentiles_H[0]) | (H>=percentiles_H[1]), np.nan,H)  
H = np.where((H>Rn) | (H<0),np.nan,H)    


# Clean rah values
percentiles_rah = np.nanquantile(rah,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values
rah = np.where((rah<=percentiles_rah[0]) | (rah>=percentiles_rah[1]), np.nan,rah)


# Clean G values
percentiles_G = np.nanquantile(G,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values
G = np.where((G<=percentiles_G[0]) | (G>=percentiles_G[1]), np.nan,G)
G = np.where(G<0,np.nan,G)



# Clean Rn values
percentiles_Rn = np.nanquantile(Rn,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values

Rn = np.where((Rn<=percentiles_Rn[0]) | (Rn>=percentiles_Rn[1]), np.nan,Rn)
Rn = np.where(Rn<0,np.nan,Rn)


# Clean LE values
percentiles_LE = np.nanquantile(LE,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values

LE = np.where((LE<=percentiles_LE[0]) | (LE>=percentiles_LE[1]), np.nan,LE)

LE = np.where((Rn<H),np.nan,LE)  # Verify statement, should there be LE in the conditional statement?

# Clean ET_inst values
percentiles_ET = np.nanquantile(ET_inst,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values
# ET_inst = percentiles_ET

ET_inst = np.where((ET_inst<=percentiles_ET[0]) | (ET_inst>=percentiles_ET[1]), np.nan,ET_inst)
ET_inst = np.where((ET_inst > 0.001) | (ET_inst < 999),ET_inst,0)
ETr = 0.27 # Alfalfa reference ET from SIDSS for 24 hour in inches.

# Energy Balance ETa 24 hour in inches
ETa = ET_inst*ETr

save_raster("LE.tif",LE)
save_raster("Rn.tif",Rn)
save_raster("G.tif",G)
save_raster("H.tif",H)
save_raster("ET_inst.tif",ET_inst)
save_raster("L_mo.tif",L_mo)
save_raster("rah.tif",rah)
save_raster("U_star.tif",U_star)
save_raster("OSAVI.tif",OSAVI)
save_raster("LAI.tif",LAI)
save_raster("NDVI.tif",NDVI)
save_raster("ETa_24hr_inches.tif",ETa)

