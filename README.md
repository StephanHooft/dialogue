# dialogue
This repository is a Unity package for a dialogue framework based on [ink](https://github.com/inkle/ink).

You will need to add `"com.inkle.ink-unity-integration": "https://github.com/inkle/ink-unity-integration.git#upm"` to your project's package manifest for this package to work!

The package offers various MonoBehaviours that wrap around one or more ink stories, allowing their input to be handled by a custom processor class which is itself oblivious to the specifics of ink. This separation of concerns makes for clean implementations.
Included are:
- A `DialogueManager` MonoBehaviour, which wraps around an ink story
- A `DialogueAsset` abstract ScriptableObject, with a default `SingleDialogueAsset` implementation, which provides `DialogueManager` with an ink JSON file
- A `DialogueProcessor` abstract MonoBehaviour, which must be implemented before a `DialogueManager` can function at runtime
- Some custom structs (`DialogueLine`, `DialogueTag`, and `DialogueChoice`), used to fuel the `DialogueProcessor` with all required info, decoupled from its ink source
- A `DialogueTrigger` MonoBehaviour, which can be used to kickstart a `DialogueManager` from a particular story knot
- An optional `DialogueVariables` ScriptableObject, to allow a `DialogueManager` to track, save, and load global ink variables
- Some basic custom inspectors with signposting for proper in-editor configuration
