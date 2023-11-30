$key = gc .\.env

cd src
$csproj = gc .\Blindness.csproj
$versionText = $csproj | % {
    if ($_.Contains("PackageVersion"))
    {
        $_
    }
}

$version = ""
$flag = 0
for ($i = 0; $i -lt $versionText.Length; $i++)
{
    $char = $versionText[$i]

    if ($flag -eq 1)
    {
        if ($char -eq "<")
        {
            break
        }

        $version += $char
    }

    if ($char -eq ">")
    {
        $flag = 1
    }
}

dotnet pack -c Release
$file = ".\bin\Release\Blindness." + $version + ".nupkg"
cp $file Blindness.nupkg

dotnet nuget push Blindness.nupkg --api-key $key --source https://api.nuget.org/v3/index.json
rm .\Blindness.nupkg
cd..