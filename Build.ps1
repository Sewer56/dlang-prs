Set-Location (Split-Path $MyInvocation.MyCommand.Path)

## Build X86 Release
dub -b release --arch=x86 --force
Rename-Item -Path "Build/dlang-prs.dll" -NewName "dlang-prs32.dll"

## Build X86-64 Release
dub -b release --arch=x86_64 --force
Rename-Item -Path "Build/dlang-prs.dll" -NewName "dlang-prs64.dll"