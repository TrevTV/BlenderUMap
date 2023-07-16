# BlenderUMap
A port of [Amrsatrio/BlenderUmap](https://github.com/Amrsatrio/BlenderUmap) to C# and CUE4Parse.

# Usage
1. Download the latest release from [here](https://github.com/TrevTV/BlenderUMap/releases).
2. Extract the zip to where you want the files exported.
3. Run the exe to generate the config file.
4. Open `config.json` in any text editor and fill the data as needed.
5. Run the exe again and wait as it finds and exports the map's actors.
   - You may receive warnings like `Did not read "MaterialInstanceConstant" correctly, X bytes remaining`, this should not impact the export and can be ignored.

# Importing into Blender
1. Create a new project in Blender and delete all the starter objects.
2. Open the Scripting tab and create a new file.
3. Copy and paste the Python script from [here](https://github.com/TrevTV/BlenderUMap/blob/main/ImporterSource/blender_import.py) into the text editor.
    - At this point, if you do not have the `import_psk_psa` plugin installed, please download it from [here](https://github.com/Befzz/blender3d_import_psk_psa) and install it.
4. In the script, change `data_dir` to be where the BlenderUMap exe is located.
5. Press `Alt + P` to run the script and wait as it imports the meshes and materials.
    - If you would like to view the progress of the import, before starting open the System Console by going to `Window > Toggle System Console`

# Importing into Unity
1. Follow the `Importing into Blender` section.
2. Save the blend file, making sure that you are in the Imported Map scene, and drag that into Unity's project window.
3. Click on the file and in the Inspector, in the Materials section, press `Extract Materials` and select a folder to extract them to (preferably an empty one)
4. Place the file into a scene.
5. Download the Editor script from [here](https://github.com/TrevTV/BlenderUMap/blob/main/ImporterSource/UMapTextureApplicator.cs) and place it into any folder in the project named `Editor`
6. Go to `Tools > UMap Texture Applicator` and fill out the fields
    - `BlenderUMap Path` is the same as the Blender script's `data_dir`
    - `Texture Import Path` is where all the textures are placed in the project
    - `Root Object` is the base model object in the scene
7. Press `Begin Import` and wait as all the textures are imported and applied to the objects.
    - You may get warnings like `Failed to find object X` or `Failed to find material X`, this will cause those objects/materials to not get their textures applied and it may need to be done manually.
8. Save the scene and you may get a pop-up that mentions normal map issues, simply press "Fix All" and it'll all work.

# Credits
 - [Amrsatrio](https://github.com/Amrsatrio/) for the original BlenderUmap
 - [iAmAsval](https://github.com/iAmAsval) and contributers for FModel
 - [FabianFG](https://github.com/FabianFG) and contributers for CUE4Parse
