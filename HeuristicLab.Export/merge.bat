REM --Start script--
DEL /F/Q/S export > NUL
RMDIR export
MKDIR export
XCOPY *.dll export
XCOPY *.pdb export
XCOPY *.exe export

cd export
DEL System.*.dll
DEL Microsoft.*.dll
DEL System.*.pdb
DEL Microsoft.*.pdb

cd ..
ILMerge.exe export/HeuristicLab.*.dll /out:export/HeuristicLab.Merged.dll /wildcards /allowDup
cd export

RENAME HeuristicLab.Merged.dll a.b
RENAME HeuristicLab.Merged.pdb c.d

DEL HeuristicLab.*.dll
DEL HeuristicLab.*.pdb
DEL ILMerge.exe

RENAME a.b HeuristicLab.Merged.dll 
RENAME c.d HeuristicLab.Merged.pdb 
cd ..