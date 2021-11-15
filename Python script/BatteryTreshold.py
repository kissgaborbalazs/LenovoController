# -*- coding: utf-8 -*-

import sys
import psutil
import subprocess

treshold = 80
path="C:\_Pack_\LenovoController\LenovoController.exe"

if(len(sys.argv) > 1):
    try:
        treshold = int(sys.argv[1])        
        if (treshold < 61) | (treshold >99):
            treshold = 80    
    except:
        pass

battery = psutil.sensors_battery()
percent = battery[0]

if percent < treshold:
    subprocess.run(path +" BatteryMode Normal")
else:   
    subprocess.run(path +" BatteryMode Conservation")