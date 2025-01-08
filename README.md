# Pixel Art Converter
A program that converts an image to pixel art using C# and WPF

Quantization and dither implemented using the [KGYSoft.Drawing](https://github.com/koszeggy/KGySoft.Drawing) package

## Features

![pizza_example](https://github.com/user-attachments/assets/8ad847eb-f49d-477e-bcf1-9ebba15c828d)

Open File: selects image to convert

Pixel size: selects number pixels in a colour block

Palette: chooses colour palette

Dither: chooses pattern to transition colours

Convert: converts original image into a pixelated image using the pixel size, palette and dither

Save image: saves the pixelated image file, supporting .png or .jpg

## Examples

### Example 1 | Pixel Size: 8 | Quantization: RGB 888 | Dither: None
![RGB888-None-Pixel8](https://github.com/user-attachments/assets/b66d2b58-8e7c-497a-a5d7-6b33f75ccefb)


### Example 2 | Pixel Size: 8 | Quantization: RGB 332 | Dither: Bayer 8x8
![RGB332-Bayer8x8-Pixel8](https://github.com/user-attachments/assets/c8017044-b6b5-46df-8607-cc0c8cc9b963)



### Example 3 | Pixel Size: 8 | Quantization: Grayscale | Dither: Dotted Halftone
![Grayscale-DottedHalftone-Pixel8](https://github.com/user-attachments/assets/042a7793-59c7-49bb-b737-d964f961210a)



### Example 4 | Pixel Size: 12 | Quantization: RGB 332 | Dither: Floyd-Steinberg
![RGB332-FloydSteinberg-Pixel12](https://github.com/user-attachments/assets/bb0d4807-35af-4576-9137-070e2488355f)
