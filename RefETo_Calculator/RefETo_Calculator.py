import parameters_ref_ET
import rasterio
import numpy as np
import pandas as pd
temp = np.seterr(divide='ignore', invalid='ignore')
import matplotlib.pyplot as plt
plt.rcParams['figure.figsize'] = [10, 10] # Command to incerase the plot size.(numbers represent inches?)


# Read calculated CSV ref et file as pandas table.
ref_et = pd.read_csv("results_ref_ET_asce.csv")
hourly_ref_ET = np.asarray((ref_et['Etr_ETR']))

# Replace nodata values in hourly ref et with zeros
hourly_ref_ET = np.nan_to_num(hourly_ref_ET)

#clean -ve hourly ref ET values

hourly_ref_ET = np.where(hourly_ref_ET<0,0,hourly_ref_ET)
Daily_ET_sum = np.nansum(hourly_ref_ET)


# Load red and NIR bands - note all PlanetScope 4-band images have band order BGRN

with rasterio.open(parameters_ref_ET.tif_file_path) as src:
    NIR_band_refl = src.read(1)/100
    Red_band_refl = src.read(2)/100
    Green_band_refl = src.read(3)/100

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
output_tiff = parameters_ref_ET.tif_file_path.replace(".tif", "_daily_ET.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, Daily_ET.astype(rasterio.float32))
dst.close()

# Write band calculations to a new raster file
output_tiff = parameters_ref_ET.tif_file_path.replace(".tif", "_NDVI.tif")
with rasterio.open(output_tiff, 'w', **kwargs) as dst:
        dst.write_band(1, NDVI_array.astype(rasterio.float32))
dst.close()

# raster = rasterio.open(tif_file_path_1)
# rasterio.plot.show(raster.read(),transform=src.transform)

