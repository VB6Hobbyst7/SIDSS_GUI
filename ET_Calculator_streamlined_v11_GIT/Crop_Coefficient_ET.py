import parameters_ref_ET
import rasterio
import sys,os
import numpy as np
import pandas as pd
import time
temp = np.seterr(divide='ignore', invalid='ignore')
import matplotlib.pyplot as plt
plt.rcParams['figure.figsize'] = [10, 10] # Command to incerase the plot size.(numbers represent inches?)

tif_path = parameters_ref_ET.KC_MS_file_path
tif_folder = os.path.dirname(tif_path)

try:
      Daily_ET_sum = float(sys.argv[1])
except:
        print("Couldn't read daily ET value, running with defualt value of 5.4527")
        Daily_ET_sum = 5.4527


print (Daily_ET_sum)
f = open(tif_folder+"\\daily_ET_Sum.txt", "a")
text_line = str(tif_path) + "; 24 Hour ET = "+ str(Daily_ET_sum)+";"
f.write(text_line)
f.close()
# Read calculated CSV ref et file as pandas table.
# ref_et = pd.read_csv("results_ref_ET_asce.csv")
# hourly_ref_ET = np.asarray((ref_et['Etr_ETR']))

# Replace nodata values in hourly ref et with zeros
# hourly_ref_ET = np.nan_to_num(hourly_ref_ET)

#clean -ve hourly ref ET values

# hourly_ref_ET = np.where(hourly_ref_ET<0,0,hourly_ref_ET)
# Daily_ET_sum = np.nansum(hourly_ref_ET)


# Load red and NIR bands - note all PlanetScope 4-band images have band order BGRN

with rasterio.open(parameters_ref_ET.KC_MS_file_path) as src:
        if src.count == 4:
                # 4 band raster, most likely Planed image with:-
                # Band order Blue (1); Green (2); Red(3); NIR(4);
                NIR_band_refl = src.read(4)/100
                Red_band_refl = src.read(3)/100
                Green_band_refl = src.read(2)/100
        elif src.count == 3:
                NIR_band_refl = src.read(1)/100
                Red_band_refl = src.read(2)/100
                Green_band_refl = src.read(3)/100
        else:                        
                print("Number of bands should be 3 or 4, this script doesn't support any other combination.")
                exit()

# Set spatial characteristics of the output object to mirror the input
kwargs = src.meta
kwargs.update(
    dtype=rasterio.float32,
    count = 1,
    compress='lzw')

# Remove nan in Red and NIR band
NIR_band_refl = np.nan_to_num(NIR_band_refl)
Red_band_refl = np.nan_to_num(Red_band_refl)


# Calculate canopy cover CC, NDVI and Kcb rasters.
NDVI_array = np.where(NIR_band_refl-Red_band_refl == 0, 0, ((NIR_band_refl-Red_band_refl)/(NIR_band_refl+Red_band_refl)))
NDVI_array = np.nan_to_num(NDVI_array)


# Clean NDVI values <-1 and >+1
NDVI_array = np.where(((NDVI_array>1) | (NDVI_array<-1)), 0, NDVI_array)
CC_array = 1.22*NDVI_array -0.21
Kcb_array = 1.13 *CC_array+0.14

Daily_ET = Kcb_array*Daily_ET_sum

# Removing -ve and too high values 
Daily_ET = np.where(Daily_ET > 100, 0,Daily_ET)
Daily_ET = np.where(Daily_ET < 0, 0, Daily_ET)

# Write band calculations to a new raster file
output_tiff = parameters_ref_ET.KC_MS_file_path.replace(".tif", "_daily_ET.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Daily_ET.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = parameters_ref_ET.KC_MS_file_path.replace(".tif", "_NDVI.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, NDVI_array.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = parameters_ref_ET.KC_MS_file_path.replace(".tif", "_Canopy_Cover.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, CC_array.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = parameters_ref_ET.KC_MS_file_path.replace(".tif", "_Kcb.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Kcb_array.astype(rasterio.float32))
dst.close()

print("Code ran sucessfullly. Waiting for 5 sec. to eixt. You can close this window anytime.")
time.sleep(2)