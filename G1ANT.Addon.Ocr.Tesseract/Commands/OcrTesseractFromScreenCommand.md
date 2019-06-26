# ocrtesseract.fromscreen

## Syntax

```G1ANT
ocrtesseract.fromscreen search ⟦text⟧ area ⟦rectangle⟧ relative ⟦bool⟧ language ⟦text⟧ 
```

## Description

This command captures part of the screen and recognizes text from it.

| Argument       | Type                                                         | Required | Default Value                                                | Description                                                  |
| -------------- | ------------------------------------------------------------ | -------- | ------------------------------------------------------------ | ------------------------------------------------------------ |
| `area`         | [rectangle](G1ANT.Robot/G1ANT.Language/G1ANT.Language/Structures/RectangleStructure.md) | yes      |                                                              | Area on the screen to be captured specified in `x0⫽y0⫽x1⫽y1` format, where `x0⫽y0` are the coordinates of the top left and `x1⫽y1` are the coordinates of the right bottom corner of the area |
| `relative`     | [bool](G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no       | true                                                         | Determines whether the `area` argument is specified with absolute coordinates (top left corner of the screen) or refers to the currently opened window (its top left corner) |
| `lang`         | [text](G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no       | eng                                                          | Language to be used for text recognition                     |
| `sensitivity`  | [float](G1ANT.Language/G1ANT.Language/Structures/FloatStructure.md) | no       | 2.0                                                          | Factor of image zoom that allows better recognition of smaller text |
| `result`       | [variable](G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no       | `♥result`                                                    | Name of a variable where the command's result will be stored |
| `if`           | [bool](G1ANT.Language/G1ANT.Language/Structures/BooleanStructure.md) | no       | true                                                         | Executes the command only if a specified condition is true   |
| `timeout`      | [timespan](G1ANT.Language/G1ANT.Language/Structures/TimeSpanStructure.md) | no       | [♥timeoutcommand](G1ANT.Language/G1ANT.Addon.Core/Variables/TimeoutCommandVariable.md) | Specifies time in milliseconds for G1ANT.Robot to wait for the command to be executed |
| `errorcall`    | [procedure](G1ANT.Language/G1ANT.Language/Structures/ProcedureStructure.md) | no       |                                                              | Name of a procedure to call when the command throws an exception or when a given `timeout` expires |
| `errorjump`    | [label](G1ANT.Language/G1ANT.Language/Structures/LabelStructure.md) | no       |                                                              | Name of the label to jump to when the command throws an exception or when a given `timeout` expires |
| `errormessage` | [text](G1ANT.Language/G1ANT.Language/Structures/TextStructure.md) | no       |                                                              | A message that will be shown in case the command throws an exception or when a given `timeout` expires, and no `errorjump` argument is specified |
| `errorresult`  | [variable](G1ANT.Language/G1ANT.Language/Structures/VariableStructure.md) | no       |                                                              | Name of a variable that will store the returned exception. The variable will be of [error](G1ANT.Language/G1ANT.Language/Structures/ErrorStructure.md) structure |

For more information about `if`, `timeout`, `errorcall`, `errorjump`, `errormessage` and `errorresult` arguments, see [Common Arguments](G1ANT.Manual/appendices/common-arguments.md) page.

> **Note:** In order to use the OCR Tesseract Addon, please follow [these steps](G1ANT.Manual/appendices/rdp-controller.md) to install the addon.

## Example

In this example the robot opens G1ANT website and after a 4-second delay necessary to load the page it captures the specified area. The recognized text is then displayed in a dialog box:

```G1ANT
program iexplore arguments ‴g1ant.com/#the-facts‴
delay 4
ocrtesseract.fromscreen 10⫽57⫽1160⫽950 result ♥ocr
dialog ♥ocr
```

