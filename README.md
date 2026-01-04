# dialogue
This repository is a Unity package for a dialogue framework based on [ink](https://github.com/inkle/ink).

You will need to add `"com.inkle.ink-unity-integration": "https://github.com/inkle/ink-unity-integration.git#upm"` to your project's package manifest for this package to work.

The package offers various ScriptableObjects that wrap around one or more ink stories.
Included are:
- A `DialogueManager` ScriptableObject which can play back Ink stories.
- Custom data types (`DialogueLine`, `DialogueChoice`, `DialogueTag`, and `DialogueCue`) to structure the processing of an ongoing dialogue.
- An optional `VariablesTracker` ScriptableObject to track, save, and load global ink variables.
- Custom inspectors with signposting for in-editor configuration, debugging, and testing of `DialogueManager` and `VariablesTracker`.