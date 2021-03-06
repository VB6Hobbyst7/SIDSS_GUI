import rasterio
import sys,os
import numpy as np

import time
temp = np.seterr(divide='ignore', invalid='ignore')

arg_length = len(sys.argv)
Full_Day_ET = 0
if arg_length == 3:
        try:
                Full_Day_ET = float(sys.argv[2])
                Full_Day_ET = Full_Day_ET * 25.4
                Full_Day_ET = round(Full_Day_ET,3)
                tif_path = sys.argv[1]
        except:
            pass
else:
        try:
                Full_Day_ET = float(sys.argv[1])
                # Convert ETr from inches to mm
                Full_Day_ET = Full_Day_ET * 25.4
                Full_Day_ET = round(Full_Day_ET,3)
        except:
            pass

# tif_path = parameters_ref_ET.KC_MS_file_path
tif_folder = os.path.dirname(tif_path)
tif_file = os.path.split(tif_path)[1]

print("24 hr ETr = {0}; tif file==> {1}".format(Full_Day_ET, tif_file))
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

# Calculate SAVI
L = 0.5
SAVI_array = np.where((NIR_band_refl+Red_band_refl+L)==0,0,(NIR_band_refl-Red_band_refl)/(NIR_band_refl+Red_band_refl+L)*(1+L))

# Cleanup SAVI min/max vaules
SAVI_array = np.where(SAVI_array<-1,0,SAVI_array)
SAVI_array = np.where(SAVI_array>1,0,SAVI_array)

# Johnson & Trout 2012, Satellite NDVI Assisted Monitoring of Vegetable Crop
# Evapotranspiration in California???s San Joaquin Valley, Page 446, Equation 1.
CC_array = 1.26*NDVI_array - 0.18
# Removing any -ve values from crop
CC_array = np.where(CC_array<0.0001,0,CC_array)

# Trout; J. Irrig. Drain Eng. 2018 (144(6); Page 10; Fig. 10a; multi season (corn as crop 2008 to 2013)
# Experiment site at LIRF, Greeley.
Kcb_trout = 1.1 *CC_array+0.17

# Removing values lower than min Kcb values 0.15 (reflectance based).
Kcb_trout = np.where(Kcb_trout<=0.15, 0, Kcb_trout)

Daily_ET_Trout  = Kcb_trout*Full_Day_ET
# Removing any -ve values from daily ET
Daily_ET_Trout  = np.where(Daily_ET_Trout<0, 0, Daily_ET_Trout)

# Removing -ve and too high values
Daily_ET_Trout  = np.where(Daily_ET_Trout  > 100, 0,Daily_ET_Trout)
Daily_ET_Trout  = np.where(Daily_ET_Trout  < 0, 0, Daily_ET_Trout)

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
output_tiff = tif_path.replace(".tif", "_Kcb_trout.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Kcb_trout.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = tif_path.replace(".tif", "_ET_mm_trout.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Daily_ET_Trout.astype(rasterio.float32))
dst.close()

print("Code ran sucessfullly.")
time.sleep(0)