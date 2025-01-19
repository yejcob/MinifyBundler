# WF.MinifyBundler
Just another MSBuild Task bundler to bundle your static web asset files, like javascript files or css files

it will also do a very simple minify for the bundled files.

## compilerSettings.json
Be sure to add a 'compilerSettings.json' file to your project

```
[
    {
        "outputPath": "wwwroot/minifyBundled.min.js",
        "sourceFolders": ["Scripts"],
        "fileType": "js"
    }
]
```