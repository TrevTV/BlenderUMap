# BlenderUMap [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/S6S244CYE)
A port of [Amrsatrio/BlenderUmap](https://github.com/Amrsatrio/BlenderUmap) to C# and CUE4Parse.

# Usage
1. Download the latest release from [here](https://github.com/TrevTV/BlenderUMap/releases).
2. Extract the zip to where you want the files exported.
3. Run the exe to generate the config file.
4. Open `config.json` in any text editor and fill the data as needed.
5. Run the exe again and wait as it finds and exports the map's actors.
   - You may receive warnings like `Did not read "MaterialInstanceConstant" correctly, X bytes remaining`, this should not impact the export and can be ignored.
6. Once complete, create a new project in Blender and delete all the starter objects.
7. Open the Scripting tab and create a new file.
8. Copy and paste the Python script from [here](https://github.com/TrevTV/BlenderUMap/blob/main/BlenderSource/umap.py) into the text editor.
    - At this point, if you do not have the `import_psk_psa` plugin installed, please download it from [here](https://github.com/Befzz/blender3d_import_psk_psa) and install it.
9. In the script, change `data_dir` to be where the BlenderUMap exe is located.
10. Press `Alt + P` to run the script and wait as it imports the meshes and materials.
    - If you would like to view the progress of the import, before starting open the System Console by going to `Window > Toggle System Console`

# Credits
 - [Amrsatrio](https://github.com/Amrsatrio/) for the original BlenderUmap
 - [iAmAsval](https://github.com/iAmAsval) and contributers for FModel
 - [FabianFG](https://github.com/FabianFG) and contributers for CUE4Parse