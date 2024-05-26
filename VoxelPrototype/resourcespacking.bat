set "DESTINATION=%1\core"
xcopy "%2" "%DESTINATION%" /E /H /C /Y /I
del "%1\core.resources"
powershell Compress-Archive -Path "%DESTINATION%\*" -DestinationPath "%1\core.zip"
ren "%1\core.zip" "core.resources"
rmdir /s /q "%DESTINATION%"
