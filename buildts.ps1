# Build the project
dotnet build

# Copy the DLL to the Crab Game plugins folder
Copy-Item -Path "C:\Users\slash\Code\UpdateSequences\bin\Debug\netstandard2.1\UpdateSequences.dll" `
          -Destination "C:\Program Files (x86)\Steam\steamapps\common\Crab Game\BepInEx\plugins" -Force

