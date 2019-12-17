# ocrtesseract.find

## Syntax

```G1ANT
ocrtesseract.find search ⟦text⟧ area ⟦rectangle⟧ relative ⟦bool⟧ language ⟦text⟧
```

## Description

This command finds a specified text on the active screen and returns its position in a [rectangle](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/RectangleStructure.md) format.

| Argument | Type | Required | Default Value | Description |
| -------- | ---- | -------- | ------------- | ----------- |
|`search`| [text](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | yes |  | Text to be found on the screen (the fewer words, the better results) |
| `area`         | [rectangle](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/RectangleStructure.md) | no | (equal to the current screen area) | Area on the screen to find text in, specified in `x0⫽y0⫽x1⫽y1` format, where `x0⫽y0` are the coordinates of the top left and `x1⫽y1` are the coordinates of the right bottom corner of the area |
|`relative`| [bool](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no | true | Determines whether the `area` argument is specified with absolute coordinates (top left corner of the screen) or refers to the currently opened window (its top left corner) |
|`lang`| [text](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no | eng | Language to be used for text recognition |
|`sensitivity`| [float](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/FloatStructure.md) | no | 2.0 | Factor of image zoom that allows better recognition of smaller text |
| `result`       | [variable](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no       | `♥result`                                                   | Name of a variable where the command's result will be stored |
| `if`           | [bool](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no       | true                                                        | Executes the command only if a specified condition is true   |
| `timeout`      | [timespan](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TimeSpanStructure.md) | no       | [♥timeoutcommand](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Addon.Core/Variables/TimeoutCommandVariable.md) | Specifies time in milliseconds for G1ANT.Robot to wait for the command to be executed |
| `errorcall`    | [procedure](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/ProcedureStructure.md) | no       |                                                             | Name of a procedure to call when the command throws an exception or when a given `timeout` expires |
| `errorjump`    | [label](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/LabelStructure.md) | no       |                                                             | Name of the label to jump to when the command throws an exception or when a given `timeout` expires |
| `errormessage` | [text](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no       |                                                             | A message that will be shown in case the command throws an exception or when a given `timeout` expires, and no `errorjump` argument is specified |
| `errorresult`  | [variable](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no       |                                                             | Name of a variable that will store the returned exception. The variable will be of [error](https://manual.g1ant.com/link/G1ANT.Language/G1ANT.Language/Structures/ErrorStructure.md) structure  |

For more information about `if`, `timeout`, `errorcall`, `errorjump`, `errormessage` and `errorresult` arguments, see [Common Arguments](https://manual.g1ant.com/link/G1ANT.Manual/appendices/common-arguments.md) page.

> **Note:** In order to use the OCR Tesseract Addon, please follow [these steps](https://manual.g1ant.com/link/G1ANT.Manual/appendices/ocr-tesseract-addon.md) to install the addon.

## Example

In this example the robot opens Notepad, types “*TEST*” and after a 1-second delay searches for a word “test” on the screen using OCR, then displays its coordinates in a dialog box:

```G1ANT
program notepad
keyboard ⋘ENTER⋙‴   TEST    ‴
delay 1
ocrtesseract.find search test
dialog ♥result
```

