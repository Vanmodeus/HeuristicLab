REM --Start script--
DEL /F/Q/S export > NUL
RMDIR export
MKDIR export
XCOPY *.dll export
XCOPY *.pdb export
XCOPY *.exe export

cd export

ILMerge.exe HeuristicLab.*.dll /out:HeuristicLab.Merged.dll /wildcards /allowDup
RENAME HeuristicLab.Merged.dll a.b
RENAME HeuristicLab.Merged.pdb c.d

DEL HeuristicLab.*.dll
DEL HeuristicLab.*.pdb

RENAME a.b HeuristicLab.Merged.dll 
RENAME c.d HeuristicLab.Merged.pdb 