clear

dotnet nuget delete Blindness 1.0.0 -s ($env:USERPROFILE + "\.nuget\packages") --non-interactive

cd src
dotnet pack -c Release -o output
cd ..

cd smp
cd sample1
dotnet remove package Blindess
dotnet add package -s ..\..\src\output Blindness
dotnet build
cd ..
cd ..

cd src
rm output -r
cd ..