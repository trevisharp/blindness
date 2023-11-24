clear

cd gen
dotnet build -c Release
cd ..

cp .\gen\bin\Release\net7.0\CodeGenerator.exe .\src\buildTransitive\tools\CodeGenerator.exe
cp .\gen\bin\Release\net7.0\CodeGenerator.dll .\src\buildTransitive\tools\CodeGenerator.dll
cp .\gen\bin\Release\net7.0\CodeGenerator.runtimeconfig.json .\src\buildTransitive\tools\CodeGenerator.runtimeconfig.json


cd src
dotnet pack -c Release -o output
cd ..

cd smp
cd sample0
dotnet add package -s ..\..\src\output Blindness
dotnet run
cd ..
cd ..

cd src
rm output
cd ..