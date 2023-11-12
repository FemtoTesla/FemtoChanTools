@echo on

set MMGEN="GenerateTools\MasterMemory.Generator\win-x64\MasterMemory.Generator.exe"  
set MPC="GenerateTools\mpc\win\mpc.exe"  
set NAME_SPACE=Femto.MasterData
set SCRIPT_PATH="..\Assets\Scripts"
  
del /S /Q %SCRIPT_PATH%\Generated
  
%MMGEN% -i %SCRIPT_PATH%\Tables -o %SCRIPT_PATH%\Generated -n %NAME_SPACE%  
%MPC% -i ..\ -o %SCRIPT_PATH%\Generated\MessagePackGenerated\