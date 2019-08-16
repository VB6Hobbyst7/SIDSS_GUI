# import parameters_ref_ET
import rasterio
import sys,os
import numpy as np
# import pandas as pd
import time
temp = np.seterr(divide='ignore', invalid='ignore')
import matplotlib.pyplot as plt
# plt.rcParams['figure.figsize'] = [10, 10] # Command to incerase the plot size.(numbers represent inches?)
arg_length = len(sys.argv)
Full_Day_ET = 0		
if arg_length == 3:
        try:
                Full_Day_ET = float(sys.argv[2])
                tif_path = sys.argv[1]
        except:
                pass
else:
        try:
                Full_Day_ET = float(sys.argv[1])
        except:
                pass
				
# tif_path = parameters_ref_ET.KC_MS_file_path
tif_folder = os.path.dirname(tif_path)

		

print(Full_Day_ET)
f = open(tif_folder+"\\Full_Day_ET.txt", "a")
text_line = str(tif_path) + "; 24 Hour ET = "+ str(Full_Day_ET)+";\n"
f.write(text_line)
f.close()

# Load red and NIR bands - note all PlanetScope 4-band images have band order BGRN

with rasterio.open(tif_path) as src:
        if src.count == 4:
                # 4 band raster, most likely Planed image with:-
                # Band order Blue (1); Green (2); Red(3); NIR(4);
                NIR_band_refl = src.read(4)
                Red_band_refl = src.read(3)
                Green_band_refl = src.read(2)
        elif src.count == 3:
                NIR_band_refl = src.read(1)
                Red_band_refl = src.read(2)
                Green_band_refl = src.read(3)
        else:                        
                print("Number of bands should be 3 or 4, this script doesn't support any other combination.")
                time.sleep(20)
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
NDVI_array = np.where(NIR_band_refl+Red_band_refl == 0, 0, ((NIR_band_refl-Red_band_refl)/(NIR_band_refl+Red_band_refl)))
NDVI_array = np.nan_to_num(NDVI_array)

# Clean NDVI values <-1 and >+1
NDVI_array = np.where(((NDVI_array>1) | (NDVI_array<-1)), 0, NDVI_array)

# Creating Kcr reflectance based crop coefficient (Walter Baush, 1993, page 214, eq. 2)
# Soil Background Effects on Reflectance-Based Crop Coefficients for Corn
Kcr_array1 = 1.092*NDVI_array-0.053
Kcr_array1 = np.where(Kcr_array1<0,0,Kcr_array1)

# Creating Kcr reflectance based crop coefficient (Walter Baush, 1993, page 214, eq. 3)
# Soil Background Effects on Reflectance-Based Crop Coefficients for Corn
Kcr_array2 = 1.181*NDVI_array-0.026
Kcr_array2 = np.where(Kcr_array2<0,0,Kcr_array2)

# Johnson & Trout 2012, Satellite NDVI Assisted Monitoring of Vegetable Crop
# Evapotranspiration in California’s San Joaquin Valley, Page 446, Equation 1.
CC_array = 1.26*NDVI_array - 0.18
# Removing any -ve values from crop 
CC_array = np.where(CC_array<0,0,CC_array)



Kcb_array = 1.13 *CC_array+0.14
# Removing -ve values from Kcb (reflectance based).
Kcb_array = np.where(Kcb_array<0, 0, Kcb_array)

Daily_ET = Kcb_array*Full_Day_ET
# Removing any -ve values from daily ET
Daily_ET = np.where(Daily_ET<0, 0, Daily_ET)

Daily_ET_Kcr = Kcr_array2*Full_Day_ET
# Removing any -ve values from daily ET
Daily_ET_Kcr = np.where(Daily_ET_Kcr<0, 0, Daily_ET_Kcr)

# Removing -ve and too high values 
Daily_ET = np.where(Daily_ET > 100, 0,Daily_ET)
Daily_ET = np.where(Daily_ET < 0, 0, Daily_ET)

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_daily_ET.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Daily_ET.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_NDVI.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, NDVI_array.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_Canopy_Cover.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, CC_array.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_Kcb.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Kcb_array.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_Kcr1.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Kcr_array1.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_Kcr2.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Kcr_array2.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_ET_Kcr2.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Daily_ET_Kcr.astype(rasterio.float32))
dst.close()

print("Code ran sucessfullly. Waiting for 1 sec. to eixt. You can close this window anytime.")
time.sleep(1)