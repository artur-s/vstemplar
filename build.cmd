@echo off
cls
if not exist packages\FAKE.Core\tools\FAKE.exe (
  .nuget\nuget.exe install FAKE.Core -OutputDirectory packages -ExcludeVersion
)
packages\FAKE.Core\tools\FAKE.exe build.fsx %*
pause
