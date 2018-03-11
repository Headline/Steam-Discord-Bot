import sys
import time
import os
import psutil
import urllib.request
import glob
import zipfile
import shutil
import errno

def get_clean_path(array):
    somestr = ""
    for x in range(0, len(array) - 1):
        somestr += array[x] + "\\"
    return somestr

def wait_for_creator():
    while psutil.pid_exists(int(sys.argv[1])):
        time.sleep(.25)




# wait for creating process to end
wait_for_creator()

# grab clean path to executing folder
path = get_clean_path(os.path.realpath(__file__).split("\\"))

# create directories if needed
zip_path = path + "\\update"
if not os.path.exists(zip_path):
    os.makedirs(zip_path)

# download file to dir
print("Download file to: " + zip_path)
urllib.request.urlretrieve(sys.argv[2], zip_path + "\\update.zip")
print("Done! Created: update.zip")

# delete outdated fiels
print("Cleaning outdated files...")
for cleanup in glob.glob(path + "*.*"):
    if (cleanup.endswith(".dll") or cleanup.endswith(".exe")):
        print("Cleaning: " + cleanup);
        os.remove(cleanup)

# extract zip to update folder
print("Extracting zip...")
with zipfile.ZipFile(zip_path + "\\update.zip", "r") as zip_ref:
    zip_ref.extractall(zip_path)

# delete zip file & blank Settings file
print("Deleting zip file...")
os.remove(zip_path + "\\update.zip")
os.remove(zip_path + "\\settings.json")

# copy files from update to root
print("Replacing outdated files...")
for src_dir, dirs, files in os.walk(zip_path):
    dst_dir = src_dir.replace(zip_path, path, 1)
    if not os.path.exists(dst_dir):
        os.makedirs(dst_dir)
    for file_ in files:
        src_file = os.path.join(src_dir, file_)
        dst_file = os.path.join(dst_dir, file_)
        if os.path.exists(dst_file):
            os.remove(dst_file)
        shutil.copy(src_file, dst_dir)

# delete update folder
print("Removing update folder...")
shutil.rmtree(zip_path)

print("Running bot...")
os.system(path + "\\Steam-Discord-Bot.exe")