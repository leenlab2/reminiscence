# Interaction Detection Documentation
---
## Scripts
### Player Scripts
 - `InteractableDetector` - Detects if the player is looking at an interactable object and delegates functionality accordingly.
     - Defines `OnCursorHitChange` event to keep track of what the player is looking at
 - `PickupInteractable` - Handles the communication between the player and the object. Specifically manages what object is held (`HeldObj`)

 ### Object Scripts
 - `PickupInteractable` - Contains the functions for modifying the object (transforming position, modifying rigidbody, modifying size) as well as managing its placement guide

 ---

 ## Prefab Setup
 These files work with the `Interactable Prop` prefab and its variants. The prefab is set up as follows:
 ```
 |- Interactable Prop Root
 |--- Model
	|--- Mode Asset
 |--- Placement Guide
	|--- Placement Guide Asset
```

Requirements for the code to function:
 - `PickupInteractable` must be attached to the `Model` object
 - All objects in the heirarchy, except the assets themselves, must have an initial local scale of (1, 1, 1)
 - The position of the `Model` and `Placement Guide` transforms should be on the bottom of the object (aka its attach point to the floor), to prevent the object clipping upon placement.
 - `Placement Guide` *and all of its children* must be on the "Ignore Raycast" layer