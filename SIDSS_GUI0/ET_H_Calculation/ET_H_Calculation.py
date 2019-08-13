#!/usr/bin/env python
# coding: utf-8

# -----------------------
# # Python script to calculate ET and other parameters using imagery & weather data
# ------------------

# In[88]:

import parameters
import os
import sys
import numpy as np
import datetime
from decimal import  Decimal
import math
import xlrd
import pandas as pd
import matplotlib.pyplot as plt
#get_ipython().run_line_magic('matplotlib', 'inline')
plt.rcParams['figure.figsize'] = [10, 10] # Command to increase the plot size.(numbers represent inches?)
print(f"Path to python execuable is:- {sys.executable}")
print ("This script is running from", sys.argv[0])
temp=np.seterr(all='ignore')


Lat = np.deg2rad(parameters.Lat)    # Decimal degrees, convert it to radians.
Lm = np.deg2rad(parameters.Lm)      # Decimal degrees, convert to radians
Elev = parameters.Elev              # Elevation of GLY04 in ft
Lz = parameters.Lz                  # Longitude of the center of the local time zone(Rocky Mountain zone)
df = parameters.df                  #data record frequency [sec]
# Define Constants
const_g = parameters.const_g        # acceleration due to gravity[m/s**2]
const_k = parameters.const_k        # Von Karman constant
const_Cpa= parameters.const_Cpa     # Specific heat capacities of air [J/kg.K]
const_Z_u = parameters.const_Z_u    # Wind reference height [m]
const_Z_T = parameters.const_Z_T    # Air Temp reference height [m], can be edited...
const_min_u= parameters.const_min_u # Lowest allowed wind speed value [m/s]

excel_file_path_1 =\
 parameters.excel_file_path_1       # Excel file's first sheet converted to csv
excel_file_path_2 = \
    parameters.excel_file_path_2    # Excel file's second sheet converted to csv

# # 1. LE Model calculation

# ----------------
# > Import excel spreadsheet (sheet no. 1 of 2).
# 
# > Assign data from each column to a variable  name based on excel sheet.
# -----------
# 

## In[42]:


sheet_1=pd.read_csv(excel_file_path_1)
sheet_1_df = pd.DataFrame(sheet_1)
sheet_1_val = sheet_1_df.values

# sheet_2=pd.read_csv(excel_file_path_2, sep=',')

col_date = sheet_1_val[:,0]  # Date
col_date = pd.to_datetime(col_date) # Convert to datetime format
col_time = sheet_1_val[:,1]  # Decimal hours 0 -1
col_time = pd.to_datetime(col_time,format = "%H:%M")
col_time = col_time.hour
col_Ta = sheet_1_val[:,2].astype(float)  # Air Temp C
col_Ts = sheet_1_val[:,3].astype(float)  # Surface Temp C
col_Rs = sheet_1_val[:,4].astype(float)  # Radiation SW, incoming
col_NDVI = sheet_1_val[:,5].astype(float)  # NDVI band ratio
col_OSAVI = sheet_1_val[:,6].astype(float)  # OSAVI band values
col_RH = sheet_1_val[:,7].astype(float)  # Relative Humidity
col_wind_spd = sheet_1_val[:,8].astype(float)  # Wind speed
col_theta = sheet_1_val[:,9].astype(float)  # Wind direction.
u = col_wind_spd* (4.87 / np.log(67.8 * 3.3 - 5.42))**(-1)
row = 1


# -------------
# > Convert to DOY & decimal_DOY arrays from column 1 & column 2

# In[43]:


yyyy = col_date.year
MOY=col_date.month
dd = col_date.day
DOY = col_date.dayofyear
col_time_dec = col_time/24 #col_time converted to decimal time hours/24 to get decimal part of the day.
decimal_DOY_sheet_1 = DOY+col_time_dec


# --------------
# > Calculating several parameters
# -------------
# 

# In[44]:


u = np.where(const_min_u>u,const_min_u,u) # Min. wind speed correction. Limit minimum wind speed value to const_min_u.
lambda_v=(2.501-0.00236*col_Ta)*10**6 #latent heat of vaporization (J/Kg)
LAMBDA_V = lambda_v
Pa = 101325*(1-2.25577e-5*Elev*0.3048)**5.25588  # Atmospheric pressure [Pascal]
es=0.6108 * np.exp(17.27 * col_Ta / (col_Ta+ 237.3))  # Saturated vapor pressure (kPa)
ea=es*col_RH/100  #actual vapor pressure (kPa)
Tkv=(col_Ta+273.15)/(1-0.378*ea/Pa/1000)  # virtual temperature (K)
rho_a=3.486*Pa/1000/Tkv  # [Kg/m3], Pa=[Pascal], Ta=[C], (ASCE EWRI 2005)
Zm=const_Z_u  # reference height [m], for wind speed measurement


# ### SECTION 01 - NET RADIATION MODEL

# > Adjust Ta at the U level

# In[45]:


if const_Z_u-const_Z_T>0.15:
    ws = 621.97*es/(Pa/1000-es)
    w=col_RH/100*ws  # mixing ratio
    Lapse=const_g*(1+lambda_v*w/(287*(col_Ta+273.15)))/(const_Cpa+0.622*lambda_v**2*w/(287*(col_Ta+273.15)**2))  # moist adiabatic lapse rate (Montieth (2013))
    Ta=col_Ta-Lapse*(const_Z_u-const_Z_T)  # adjusted air temperature at u level

ea_mb = 10*ea


# In[46]:


Ta_K = col_Ta+273.15
Ts_K = col_Ts+273.15
Rs_inc = col_Rs*(1000/60)

t = 24* col_time_dec + 0.5

DL = 0.409 * np.sin(2*np.pi/365*DOY-1.39)

HS = np.arccos(-np.tan(Lat)*np.tan(DL))

DR = 1 + 0.033  * np.cos((2 * np.pi / 365) * DOY)

B = (2*np.pi*(DOY-81))/364

Sc = 0.1645 * np.sin(2 * B) - 0.1255 * np.cos(B) - 0.025 * np.sin(B)

OMEGA = (np.pi / 12) * ((t + 0.06667 * (Lz - np.rad2deg(Lm)) + Sc) - 12)  # Solar time angle at the midpoint of the period in radians

t1 = 1  # Length of the calculation period.


# In[47]:


OMEGA1 = OMEGA - (np.pi * t1/24)

OMEGA2 = OMEGA + (np.pi*t1/24 )

OMEGA1 = np.where(OMEGA1 <  (-HS), -HS,OMEGA1)

OMEGA2 = np.where(OMEGA2 <  (-HS), -HS,OMEGA2)

OMEGA2 = np.where(OMEGA2 > HS, HS,OMEGA2)

OMEGA1 = np.where(OMEGA1 > OMEGA2, OMEGA2,OMEGA1)


# In[48]:


G_sc = 4.92  # Solar constant

R_a = (12 / np.pi) * G_sc * DR * ((OMEGA2 - OMEGA1) * np.sin(Lat) * np.sin(DL) + np.cos(Lat) * np.cos(DL) * (np.sin(OMEGA2) - np.sin(OMEGA1)))  # Extraterrestrial radiation (MJ/m**2/h)

R_SO = (0.75 + 2 * (10 ** (-5)) * Elev * 0.3048) * R_a  # Clear-Sky Irradiance (MJ/m**2/h)

R_so = R_SO * (1000000 / 3600)  # Clear-Sky Irradiance in W/m**2

s = np.where(R_so>0,Rs_inc/R_so,0)

s = np.where(s<0.3,0.3,s)

s = np.where(s>1,1,s)

clf = 1-s  # # Cloud Fraction Term


# > Calculating the Emissivity of Air    

# In[49]:


Ea = clf + (1 - clf) * (1.22 + 0.06 * np.sin((MOY + 2) * (np.pi / 6))) * (ea_mb / Ta_K) ** (1 / 7)  # Emissivity of the air (Crawford and Dushon, 1999)

# Ea = 1.24* ((ea_mb/T_aK)**(1/7))  # # Ea stands for Air emissivity (Brutsaert, 1975)

# The number 10 and 273.15 are added as conversion factors
# The 10 term is to convert vapor pressure from kPa to mb
# The 273.15 term is to account the conversion temperature from degree Celsius to degree Kelvin


# > Calculating Fractional Percent Cover

# In[50]:


fc = np.where(col_NDVI<=0.15,0,(1.26*col_NDVI-0.18))


# > Calculating the Surface Emissivity from Fractional Vegetation Cover

# In[51]:


e_v = 0.98 # e_v stands for Emissivity for a fully vegetated area and is equal to 0.98

e_s = 0.93  # e_s stands for Emissivity for a bare soil and is equal to 0.93

Es = fc * e_v + (1 - fc) * e_s  # Surface Emissivity equation (Brunsell and Gillies, 2002) 
# Es stands for Surface Emissivity (dimensioneless)


# > Calculating albedo based on the emipirical relationship developed based on LAI

# In[52]:


LAI = 0.263 * np.exp(3.813 * col_OSAVI)  # Leaf Area Index Calculation (Chavez et al., 2010)

ALBEDO = 0.0179 * LAI**(-3.27) + 0.1929  # Example of albedo calculation
# Replace this equation. Use albedo = 0.512 (RED) + 0.418 (NIR)
albedo = 0.1746 - 0.0037 * np.cos(211.2 * ALBEDO)+0.0048 * np.sin(211.2 * ALBEDO) # Calibrated Albedo Function
##


# > Calculating the Net Radiation Flux (Rn)

# In[53]:


Rs_out = albedo*Rs_inc # Rs_out stands for Outgoing Shortwave Radiation in W/m**2

Rl_in = Es* Ea * (5.67*10**(-8))*(Ta_K**4) # Rl_out stands for the Incoming Longwave Radiation in W/m**2

Rl_out = Es * (5.67*10**(-8))*(Ts_K**4)  # Rl_in stands for the outgoing Longwave Radiation in W/m**2

Rn = Rs_inc - Rs_out + Rl_in - Rl_out # Rn stands for the Estimated Net Radiation in W/m**2


# ### SECTION 02 - SOIL HEAT FLUX MODEL

# > Calculating the Soil Heat Flux (G)

# In[54]:


G = 34.15 * np.log(col_Ts) - 48.31 * np.exp(col_OSAVI) + 0.02 * (Rn**2 * albedo**3)+20.64 * fc**5 # Soil Heat Flux Model

#row = row + 1 # Term to keep the interation process for each cell of the imported spreadsheet


# ### SECTION 03 - SENSIBLE HEAT FLUX MODEL

# In[55]:


Ho= 0.697 *np.exp(0.236*LAI)-3.42*np.exp(-3.177*LAI)  # Model to calculate crop height based on LAI
crop_height_max_index = np.argmax(Ho)
crop_height_max = Ho.max()
hc = np.zeros(Ho.shape)

# hc is derived from Ho, it keeps increasing with Ho, once Ho reaches max value hc also reaces max. value.
# Then hc stays at the max value and doesn't decline like Ho.
for i in range (len(Ho)):
    if i<=crop_height_max_index:
        hc[i]=Ho[i]
    else:
        hc[i]=crop_height_max

    
Wind_Direc = col_theta  # # Wind Speed Direction (degrees decimal)
Wind_Speed = col_wind_spd  # # Wind Speed (m/s)


#     
# > Calculating the roughness length variables of the canopy

# In[56]:


d = 0.67 * hc  # #zero plane displacement [m]
Zom = 0.123 * hc  # #roughness length for momentum [m]
Zoh = Zom * 0.10  # #roughness length for heat transfer [m]
rp = np.ones(col_theta.shape)


# > Calculating the Turbulence-Mixing Row Resistance (rp)

# In[57]:


# if Wind_Direc <= 90
rp = np.where(Wind_Direc<=90,(Wind_Direc/(180-Wind_Direc)*1/Wind_Speed),rp)

#    elseif theta(z) > 90 && theta(z) <= 180
rp = np.where((Wind_Direc>90) & (Wind_Direc<=180),((180-Wind_Direc)/Wind_Direc/Wind_Speed),rp)

#    elseif Wind_Direc > 180 && Wind_Direc <= 270
rp = np.where((Wind_Direc>180) & (Wind_Direc<=270),((Wind_Direc-180)/(360-Wind_Direc)/Wind_Speed),rp) 

#    elseif Wind_Direc > 270 && Wind_Direc <= 360
rp = np.where((Wind_Direc>270) & (Wind_Direc<=360),((360-Wind_Direc)/(Wind_Direc-180)/Wind_Speed),rp) 


# > Calculating Surface Aerodynamic Temperature (To)

# In[58]:


To = np.zeros(col_Ta.shape)

To = np.where(LAI<=1.5,-18.882 * fc + 0.513 * Ta + 0.643 * col_Ts + 0.843 * rp + 6.044,To)  
To = np.where((LAI>1.5) & (LAI<=2.5),1.783* fc + 0.287 * Ta + 0.773 * col_Ts -1.212 * rp,To)  
To = np.where((LAI>2.5) & (LAI<=3.5),5.419 * fc + 0.388 * Ta + 0.534 * col_Ts + 0.03 * rp,To)  
To = np.where((LAI>3.5),-5.103 * fc + 0.476 * Ta + 0.471 * col_Ts - 0.039 * rp + 8.160,To)  


# #### Monin-Obhukov Stability Theory (MOST), for atmospheric stabiiity corrections
# #### First Iteration, Assume Neutral Atmospheric Conditions:

# In[59]:


U_star=(u*const_k)/(np.log((Zm-d)/Zom))
rah=np.log((Zm-d)/Zom)*np.log((Zm-d)/Zoh)/(u*const_k**2)
H=(rho_a*const_Cpa)*(To-Ta)/rah
L_mo=-(U_star**3*(Ta+273.15)*rho_a*const_Cpa)/(const_g*const_k*H) # MOST length (L, m)
error=999
iter=np.zeros(col_Ta.shape)
state=np.zeros(col_Ta.shape)
x1=np.zeros(col_Ta.shape)
x2=np.zeros(col_Ta.shape)
x3=np.zeros(col_Ta.shape)
x4=np.zeros(col_Ta.shape)
phi_h1=np.zeros(col_Ta.shape)
phi_h2=np.zeros(col_Ta.shape)
phi_m1=np.zeros(col_Ta.shape)
phi_m2=np.zeros(col_Ta.shape)


# #### Stability is determined according to the Monin-Obukhov length (L, m)

# In[60]:


for i in range(len(Ta)):
    j=0
    error=99
    if (L_mo[i] < 0) & (L_mo[i] > -10): # #Unstable Conditions
        state[i]=-1
        while error > 0.05:
            j=j+1
            dumm=rah[i]  # dummy variable
        #Following equations from Yasuda (1988)
            x1[i]=(1-16*(Zm-d[i])/L_mo[i])**0.25
            x2[i]=(1-16*(Zoh[i])/L_mo[i])**0.25
            x3[i]=(1-16*(Zom[i])/L_mo[i])**0.25

            phi_h1[i]=2*np.log((1+x1[i]**2)/2)
            phi_h2[i]=2*np.log((1+x2[i]**2)/2)
            phi_m1[i]=2*np.log((1+x1[i])/2)+np.log((1+x1[i]**2)/2)-2*np.arctan(x1[i])+np.pi/2
            phi_m2[i]=2*np.log((1+x3[i])/2)+np.log((1+x3[i]**2)/2)-2*np.arctan(x3[i])+np.pi/2
        
        #Calculate Parameters (U*,rah,H,L_mo)
            U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i])-phi_m1[i]+phi_m2[i])
            rah[i]=(np.log((Zm-d[i])/Zoh[i])-phi_h1[i]+phi_h2[i])/(U_star[i]*const_k)
            H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
            L_mo[i]=-(U_star[i]**3*(Ta[i]+273.15)*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])
            error = abs(rah[i]-dumm)/rah[i]
    
        
    elif (L_mo[i] > 0) & (L_mo[i] < 10): #Stable Conditions
        state[i]=1
        while error > 0.05:
            j=j+1
            dumm=rah[i] #dummy variable
            phi_h1[i]=6*((Zm-d[i])/L_mo[i])*np.log((1-Zm/L_mo[i]))
            phi_h2[i]=6*(Zom[i]/L_mo[i])*np.log((1-Zm/L_mo[i]))
            phi_m1[i]=phi_h1[i]
            phi_m2[i]=phi_h2[i]


        #Calculate Parameters (U*,rah,H,L_mo)
            U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i])-phi_m1[i]+phi_m2[i])
            rah[i]=(np.log((Zm-d[i])/Zoh[i])-phi_h1[i]+phi_h2[i])/(U_star[i]*const_k)
            H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
            L_mo[i]=-(U_star[i]**3*(Ta[i]+273.15)*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])     
            error = abs(rah[i]-dumm)/rah[i]


    else: #Neutral Conditions
        state[i]=0
        U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i]))
        rah[i]=np.log((Zm-d[i])/Zom[i])*np.log((Zm-d[i])/Zoh[i])/(u[i]*const_k**2)
        H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
        L_mo[i]=-(U_star[i]**3*Ta[i]*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])


# In[61]:


i=1
j=0
error=99
for i in range(len(Ta)):
    j=0
    error=99
    if (L_mo[i] < 0) & (L_mo[i] > -10):   # Unstable Conditions
        state[i]=-1
        while error > 0.0001:
            j=j+1
            dumm=rah[i]   # dummy variable
            x1[i]=(1-16*(Zm-d[i])/L_mo[i])**0.25
            x2[i]=(1-16*(Zoh[i])/L_mo[i])**0.25
            x3[i]=(1-16*(Zom[i])/L_mo[i])**0.25
            phi_h1[i]=2*np.log((1+x1[i]**2)/2)
            phi_h2[i]=2*np.log((1+x2[i]**2)/2)
            phi_m1[i]=2*np.log((1+x1[i])/2)+np.log((1+x1[i]**2)/2)-2*np.arctan(x1[i])+np.pi/2
            phi_m2[i]=2*np.log((1+x3[i])/2)+np.log((1+x3[i]**2)/2)-2*np.arctan(x3[i])+np.pi/2

            # Calculate Parameters (U*,rah,H,L_mo)
            U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i])-phi_m1[i]+phi_m2[i])
            rah[i]=(np.log((Zm-d[i])/Zoh[i])-phi_h1[i]+phi_h2[i])/(U_star[i]*const_k)
            H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
            L_mo[i]=-(U_star[i]**3*(Ta[i]+273.15)*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])
            error = abs(rah[i]-dumm)/rah[i]

        
    elif (L_mo[i] > 0) & (L_mo[i] < 10):   # Stable Conditions
        state[i]=1
        while error > 0.0001:
            j=j+1
            dumm=rah[i]   # dummy variable
            phi_h1[i]=6*((Zm-d[i])/L_mo[i])*np.log((1-Zm/L_mo[i]))
            phi_h2[i]=6*(Zom[i]/L_mo[i])*np.log((1-Zm/L_mo[i]))
            phi_m1[i]=phi_h1[i]
            phi_m2[i]=phi_h2[i]

              # Calculate Prameters (U*,rah,H,L_mo)
            U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i])-phi_m1[i]+phi_m2[i])
            rah[i]=(np.log((Zm-d[i])/Zoh[i])-phi_h1[i]+phi_h2[i])/(U_star[i]*const_k)
            H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
            L_mo[i]=-(U_star[i]**3*(Ta[i]+273.15)*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])
            error = abs(rah[i]-dumm)/rah[i]

    else:   # Neutral Conditions
        state[i]=0
        U_star[i]=(u[i]*const_k)/(np.log((Zm-d[i])/Zom[i]))
        rah[i]=np.log((Zm-d[i])/Zom[i])*np.log((Zm-d[i])/Zoh[i])/(u[i]*const_k**2)
        H[i]=(rho_a[i]*const_Cpa)*(To[i]-Ta[i])/rah[i]
        L_mo[i]=-(U_star[i]**3*Ta[i]*rho_a[i]*const_Cpa)/(const_g*const_k*H[i])


# In[62]:


# Filter real part of the variables.
H=np.real(H)
rah=np.real(rah)
U_star=np.real(U_star)
phi_h1=np.real(phi_h1)
phi_h2=np.real(phi_h2)
phi_m1=np.real(phi_m1)
phi_m2=np.real(phi_m2)
x1=np.real(x1)
x2=np.real(x2)
x3=np.real(x3)
x4=np.real(x4)
L_mo=np.real(L_mo)


# ### SECTION 04 - INSTANTANEOUS LATENT HEAT FLUX

# In[63]:


#ET from Energy Balance
LE=Rn-G-H
LE = np.where(LE<0,np.nan,LE)


# ### SECTION 05 - CONVERTING INSTANTANEOUS LATENT HEAT FLUX TO INSTANTANEOUS ET WATER DEPTH

# In[64]:


ET_inst=df*LE/LAMBDA_V/1


# ### SECTION 06 - OUTLIERS FILTERING FOR ALL MODELS (Rn, G, H, LE)
# 

# In[65]:


#Filter Output from outliers:
#---------------------------
interp='lower'
p_min = 1/100 # Minimum cutoff percentile
p_max = 99/100 # Max cutoff percentile.
#Note matlab's percentile funcition is essentially same as numpy's quantile function.
# There is numpy percentile function too, but the results doesn't match well with matlab's percentile function.


# In[66]:


# Clean H values
percentiles_H = np.nanquantile(H,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values


# In[67]:


# Remove very low or very high values(1 & 99 percentile).
H = np.where((H<=percentiles_H[0]) | (H>=percentiles_H[1]), np.nan,H)  
H = np.where((H>Rn) | (H<0),np.nan,H)    


# In[68]:


# Clean rah values
percentiles_rah = np.nanquantile(rah,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values
rah = np.where((rah<=percentiles_rah[0]) | (rah>=percentiles_rah[1]), np.nan,rah)


# In[69]:


# Clean G values
percentiles_G = np.nanquantile(G,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values
G = np.where((G<=percentiles_G[0]) | (G>=percentiles_G[1]), np.nan,G)
G = np.where(G<0,np.nan,G)


# In[70]:


# Clean Rn values
percentiles_Rn = np.nanquantile(Rn,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values

Rn = np.where((Rn<=percentiles_Rn[0]) | (Rn>=percentiles_Rn[1]), np.nan,Rn)
Rn = np.where(Rn<0,np.nan,Rn)


# In[71]:


# Clean LE values
percentiles_LE = np.nanquantile(LE,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values

# In[72]:


LE = np.where((LE<=percentiles_LE[0]) | (LE>=percentiles_LE[1]), np.nan,LE)


# In[73]:


LE = np.where((Rn<H),np.nan,LE)  # Verify statement, should there be LE in the conditional statement?


# In[74]:


# Clean ET_inst values
percentiles_ET = np.nanquantile(ET_inst,(p_min,p_max), interpolation=interp)  # Find 1 & 99th percentile values

ET_inst = np.where((ET_inst<=percentiles_ET[0]) | (ET_inst>=percentiles_ET[1]), np.nan,ET_inst)
ET_inst = np.where(ET_inst<0,np.nan,ET_inst)


# ### SECTION 07
# > CALCULATING INSTANTANEOUS ALFALFA REFERENCE ET FROM ASCE STANDARDIZED METHOD 
# > (ALLEN ET AL., 2005)

# In[75]:


# Initial variables for the calculation:

# Cn and Cd are the coefficients for Alfalfa based reference ET calculation

Cn = 66
Cd = 0.25

# Calculating Atmospheric Pressure:

P = 101.3 *((293-0.0065*Elev*0.3048)/(293))**5.26 # Atmospheric Pressure in kPa

# Calculating the Psychometric Constant (gamma):

gamma = 0.000665 * P # kPa/C


# Calculating the slope of the saturation vapor pressure curve (delta) in kPa/C:

delta = (2503 * np.exp((17.27 * Ta)/(Ta+237.3)))/((Ta+237.3)**2) 

# Calculation of Net Radiation (Rn_ASCE) in (MJ/m^2/h):

# Calculating Net Short-Wave Radiation (Rns_ASCE) in (MJ/m^2/h):

albedo_ASCE = 0.23 # Fixed albedo assumed by Allen et al. (2005)

Rs_ASCE = (col_Rs / 1000) * 60 # Converting Rs from KJ/m^2/min to MJ/m^2/h
# Divide Rs by 1000 (KJ/MJ) and multiply by 60 (min/h) to get (MJ/h/m^2 = MJ/m^2/h)
# x (KJ/min/m^2) = x (KJ/min/m^2)/(1000)(KJ/MJ)*(60)(min/h)

Rns_ASCE = (1-albedo_ASCE) * Rs_ASCE # NET SHORT-WAVE INCOMING RADIATION (MJ/m^2/h)

# Calculating Net Long-Wave Radiation (Rnl_ASCE) in (MJ/m^2/h):

sigma = 2.042 * 10**(-10) # Stefan-Boltzmann Constant in MJ/K^4/m^2/h

# Calculating the Clear-sky Radiation (Rso_ASCE) in (MJ/m^2/h):

Rso_ASCE = R_SO

# Cloudiness function (0.05 < fcd < 1.00)

fcd = 1.35 * (Rs_ASCE / Rso_ASCE)-0.35

fcd = np.where(fcd<0.05,0.05,fcd)
fcd = np.where(fcd>1,1,fcd)


# Net Long-Wave Radiation (Rnl_ASCE) in (MJ/m^2/h):

Rnl_ASCE = sigma * fcd * (0.34-0.14 * (ea**0.50)) * (Ta+273.15)**4

# Net Radiation (Rn_ASCE) in (MJ/m^2/h):

Rn_ASCE = Rns_ASCE - Rnl_ASCE

# Soil Heat Flux (G_ASCE) in (MJ/m^2/h):

G_ASCE = 0.04 * Rn_ASCE

# Adjusting wind speed height to standardized weather station:

if const_Z_u != 2:

    u2 = u * (4.87 / (np.log(67.8 * const_Z_u - 5.42)))

else:

    u2 = u


# ### SECTION 08 - INSTANTANEOUS EVAPORATIVE FRACTION

# In[76]:


# Instantaneous Alfalfa Reference ET based on ASCE Standardized Method
# (Allen et al., 2005)

NUMERATOR = (0.408 * delta * (Rn_ASCE - G_ASCE) + gamma * (Cn /(Ta+273))* u2 * (es-ea))

DENOMINATOR = (delta + gamma *(1+ Cd * u2))

ETr = NUMERATOR/DENOMINATOR

ETr = np.where(ETr<0,np.nan,ETr)


# _____________
# # 2. Calculating daily Reference ET.
# -----------------

# ### SECTION 09 - DAILY ALFALFA REFERENCE ET BASED ON INTEGRATION OF HOURLY TIME STEP

# In[77]:

sheet_2=pd.read_csv(excel_file_path_2)
sheet_2_df = pd.DataFrame(sheet_2)
sheet_2_val = sheet_2_df.values

# sheet_2=pd.read_csv(excel_file_path_2, sep=',')

# sheet_2=pd.read_csv(excel_file_path_2)

# Assign data from each column to a variable  name based on excel sheet.
dateETR = sheet_2_val[:,0]  # Date
dateETR = pd.to_datetime(dateETR)
timeETR = sheet_2_val[:,1]  # Decimal hours 0 -1
TaETR = sheet_2_val[:,2].astype(float)  # Air Temp C
RsETR = sheet_2_val[:,3].astype(float)  # Incoming Short-Wave Radiation Rs (KJ/m2/min)
RHETR = sheet_2_val[:,4].astype(float)  # RH (%)
uETR = sheet_2_val[:,5].astype(float)  # Wind Speed (m/s)

yyyyETR = dateETR.year
MOYETR=dateETR.month
ddETR = dateETR.day
DOYETR = dateETR.dayofyear
temp = pd.to_datetime(timeETR, format = "%H:%M")
timeETR = temp.hour/24
decimal_DOY_sheet_2 = DOYETR+timeETR


# In[78]:


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

R_aETR = (12 / np.pi) * G_scETR * DRETR * ((OMEGA2ETR - OMEGA1ETR) * np.sin(Lat) * np.sin(DLETR)          + np.cos(Lat) * np.cos(DLETR) * (np.sin(OMEGA2ETR) - np.sin(OMEGA1ETR))) # Extraterrestrial radiation (MJ/m**2/h)

R_SOETR = (0.75 + 2 * (10 **(-5)) * Elev * 0.3048) * R_aETR # Clear-Sky Irradiance (MJ/m**2/h)


# In[79]:


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


# ----------
# # Review & Export resutls
# ------------

# In[81]:


plt.rcParams['figure.figsize'] = [20, 15] # Command to incerase the plot size.(numbers represent inches?)
formated_results_sheet1 = pd.DataFrame({'H':H,
                  'rah':rah,
                  'G':G,
                  'Rn':Rn,
                  'LE':LE,
                  'ET_inst':ET_inst,
                                            
},
index = decimal_DOY_sheet_1
)
formated_results_sheet1.to_csv('results_LE_Calculation_sheet1.csv')
plotted_fig = formated_results_sheet1.plot(subplots=True)
plt.savefig('results_LE_Calculation_sheet1.png')


# In[87]:


plt.rcParams['figure.figsize'] = [20, 15] # Command to incerase the plot size.(numbers represent inches?)

formated_results_sheet2 = pd.DataFrame({'deltaETR':deltaETR,
                                              'Etr_ETR':ETr_ETR,
                                              'Rn_ASCEETR_watts':Rn_ASCEETR_watts,
                                              'G_ASCEETR':G_ASCEETR,
                                            },
                                            index = decimal_DOY_sheet_2                                         
                                            )
formated_results_sheet2.to_csv('results_ref_ET_sheet2.csv')
plotted_fig = formated_results_sheet2.plot(subplots=True)
plt.savefig('results_ref_ET_sheet2.png')

