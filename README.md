# Unity Save System

A small Unity package for easily saving and loading custom data in a JSON format.

## Usage

Install the package according to the [installation instructions](#installation). Once installed, you can use the methods available.

```cs
using Lajawi;       // Either add this using to your script,
                    // or use Lajawi.<method> to call the methods

public class Data   // Example class meant for serializing
{
    public int id;

    // All variables that need to be saved,
    // need to be publicly accessible
    public string Name { get; private set; }

    public Data(string name)
    {
        Name = name;
    }
}

public class GameManager : MonoBehaviour
{
    readonly string PATH = "player\\data";

    void Start()
    {
        // Because of Name's setter accessibility, you have
        // to use the constructor to set it
        Data data = new Data("lajawi") { id = 1 };

        // Your data will be saved in the persistent datapath
        // that Unity provides, at your path
        // In the case of WebGL builds, data will be stored
        // in the PlayerPrefs instead, with the PATH as key
        SaveSystem.Save(PATH, data);

        // Retrieve previously saved data with the same path
        // SaveSystem.Load returns a boolean, to easily check
        // whether reading the data was successful
        if (SaveSystem.Load(PATH, out Data readData))
        {
            Debug.Log($"ID: {readData.id}, name: {readData.name}");
        }

        // Saving other types, like primitives, lists,
        // dictionaries, arrays... is possible too
        SaveSystem.Save("string", "Hello World!");
        SaveSystem.Save("List", new List<int>() { 0, 1, 1, 2, 3, 5 });
        SaveSystem.Save("Dictionary", new Dictionary<int, string>() {
            { 0, "Zero" },
            { 2, "Two" },
            { 25, "Twenty-five" },
        });

        // And of course, custom classes with all variables
        // of any type above will work too
    }
}
```

## Installation

### Option 1: Package Manager (recommended)

Open the Package Manager window, click on `Add Package from Git URL ...`, then enter the following:

```
https://github.com/lajawi/unity-save-system.git
```

### Option 2: Manually Editing `package.json`

Add the following line to your project's `Packages/manifest.json`:

```json
"com.github.lajawi.savesystem": "https://github.com/lajawi/unity-save-system.git"
```
