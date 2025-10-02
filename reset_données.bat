@echo off
echo Reinitialisation et alimentation de la base de donnees...
echo.

curl -X POST http://localhost:5049/api/Seed/ResetAndSeed
echo.
echo.
echo Statistiques :
curl http://localhost:5049/api/Seed/GetStats

echo.
echo Termine !
pause