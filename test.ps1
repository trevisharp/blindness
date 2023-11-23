cd src
dotnet pack -o output
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