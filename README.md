# Main Plugin Here
https://github.com/ScriptedEvents/ScriptedEvents

# Scripted Events Extension
This extension allows to have multiple hints on one screen by adding action called AdvancedHint.

### Working Action

| Action Name   | Mode[0] | PLAYERS[1] | ID[2] | Align[3] | xPos[4]  | yPos[5]  | removeAfter[6] | FontSize[7] | Text[8] |
|---------------|---------|------------|-------|----------|----------|----------|----------------|-------------|---------|
| ADVANCEDHINT  | SET     | PLAYERS    | ID    | 1/2/3    | Position | Position | Seconds        | Size        | Text    |
| ADVANCEDHINT  | REMOVE  | PLAYERS    | ID    | X        | X        | X        | X              | X           |         |

### Example

| Action Name   | Mode[0] | PLAYERS[1] | ID[2]    | Align[3] | xPos[4]  | yPos[5]  | removeAfter[6] | FontSize[7] | Text[8]     |
|---------------|---------|------------|----------|----------|----------|----------|----------------|-------------|-------------|
| ADVANCEDHINT  | SET     | {PLAYERS}  | ThisIsId | 2        | 100,32f  | 42       | 5              | 20          | Hello World |
| ADVANCEDHINT  | REMOVE  | {PLAYERS}  | ThisIsId | X        | X        | X        | X              | X           | X           |

### Explanation
We Set a hint for all players on server with Id ThisIsId aligned to center in position 100 yPos 42 which removed after 5 seconds and say's hello world.

That is too much text let me explain more.

* SET `Mode` needs all the properties: `Mode`, `Players`, `Id`, `Align`, `xPos`, `yPos`, `removeAfter`, `Text`.
* Remove `Mode` needs **only** properties: `Mode`, `Players`, `Id` | Nothing else is used.

* Id - Can be just text it doesn't have to be number, but do not use spaces.
* Align - There is **3 types of align**, `Left`, `Center` and `Right` which is set by numbers `1` being `Left`, `2` being `Center` and `3` being `Right`. If you are trying to get something over to `Left` **make sure it's aligned to `Left`** so you don't have boundary (not save with right align).
* xPos - little harder basically when using desimals make sure to add F at the end.
* yPos - refer to xPos.
* removeAfter - the time the hint will remove after **KNOWN BUG, DOESN'T WORK**
* fontSize - the size of the text basically <size> tag it's number.
* text - the text you want to add use _ for spaces.

  # Instalation
  Add the `.dll` of this into plugins and refer to [HSM](https://github.com/MeowServer/HintServiceMeow)
