# Solitaire

A Solitaire clone made in Unity.

A majority of the game is based off of the ["Let's Make Soltaire in Unity" series by Megalomobile](https://www.youtube.com/watch?v=1Cmb181-quI&list=PLuq_iMEtykn8XiZb6453f61U7HC7u8KiC). I then made a few adjustments based on what I know (and am still learning!) about OOP and Unity.

As for the card art, I chose a pixel art style because I was inspired by [Inscryption](https://store.steampowered.com/app/1092790/Inscryption/) at the time. This is the first time I've actually done this type of style as well.

# Notes

## Changes done compared to the original tutorial

- **Encapsulation**: Several variables were set as `public`, which I've switched to `private` as there was no need for the Unity Inspector to see certain variables. Public methods were added if those private variables needed to be retrieved.

    - For variables that I wanted to keep public to be shared between scripts but still did not want it to be shown in the Inspector, I added  
    `{ get; private set; }`

- **Avoided `Update()`** on all scripts except for `PlayerInput.cs`. The game is interactive only through mouse clicks, so calling any other method through `Update()` was unnecessary.

- **Utilized `Awake()`**, where I instantiated the script's private variables and any `GetComponent` on the `GameObject` they are attached to.

    - Anything that involved other scripts and/or GameObjects was done in `Start()`.

## Further Development

- Allow player to choose between drawing **one or three cards** for the Talon pile.

    - Current implementation uses `Toggle` and is saved in `PlayerPrefs`. Upon exiting the Options Menu, the game is restarted (scene is NOT reloaded) with the updated preference.

    - Alternatively, ask option at the start of each new game. (May get tedious over time?)

- Allow the player to **build sequences with double-clicks and/or drag-and-drop**.

    - Double-clicks: Will probably need to use the `EventSystem`.

    - Drag-and-drop: Compare `Collider` on mouse up?

- Track **score, time, number of moves**, etc. 

# License

All artwork / assets used in this project is licensed under the [Creative Commons Attribution Share Alike 4.0 license](https://creativecommons.org/licenses/by-sa/4.0/).

The project's source code is licensed under the [GNU General Public License](https://www.gnu.org/licenses/gpl-3.0.en.html).