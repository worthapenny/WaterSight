import sys
import clr

assembly_path = r"D:\Development\DotNET\WaterSight\WaterSight\Output\WaterSight.Web\bin\x64\Debug"
assembly_name = "WaterSight.Web"
sys.path.append(assembly_path)

loaded = clr.AddReference(assembly_name)

if loaded:
  print("Assembly loaded")
else:
  print(f"Failed to load assembly from: {assembly_path}, with name: {assembly_name}")
  exit()


from WaterSight.Web.Core import Env, WS
env = Env.Prod

registry_path = r"SOFTWARE\WaterSight\BentleyProdOIDCToken" if env == Env.Prod else r"SOFTWARE\WaterSight\BentleyQaOIDCToken"


ws = 
