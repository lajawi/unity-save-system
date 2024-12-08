# Unity Save System

A small Unity package for easily saving and loading custom data in a JSON format.

## Usage

Install the package according to the [installation instructions](#installation). Once installed, you can use the methods available.

```cs
using Lajawi;       // Either add this using to your script,
                    // or use Lajawi.<method>

[Serializable]
public class Data   // Class meant for serializing
{
    public int id;
    public string name;

    // Classes meant for saving can only include parameterless
    // constructors
    //public Data(int id, string name) { }          // INVALID
    //public Data(int id = 0, string name = "") { } // INVALID
    //public Data() { }                             // ALLOWED
}

public class GameManager : MonoBehaviour
{
    readonly string PATH = "player\\data";

    void Start()
    {
        Data data = new Data{ id = 1, name = "lajawi" };

        // Your data will be saved in the persistent datapath
        // that Unity provides
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
    }
}
```

```cs
// Saving lists and arrays is currently not possible
// Use a wrapping object instead
[Serializable]
public class Data
{
    public List<string> names = new List<string>();
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
