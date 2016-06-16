### Paletter

Simple program to extract colours palette from one image and find distance to palette of another image using [Delta E](https://en.wikipedia.org/wiki/Color_difference) method.

#### Delta E - description
To compare two color mathematically or programatically we should describe difference between two colors in measurable way ex. numbers. One could simply calculate vector between two values describing color in any system: RGB, CMYK or HSL. But that may not ilustrate differences in universal way as color perception is subjective and depends on colours combination ex. in CMYK model yellow color is perceived as lighter than blue of the same intensity thus it lays further from black.

In this solution CIELab space is used as it is more uniformal than others, includes all perceivable colors - what means that its gamut exceeds thos of CMYK and RGB. Important thing is that CIELab space is uniformal: two different point in CIELab space (objective colors) may be seen as one color by human (subjective color).

Lab stands for: Lightness, a - balance between green and red, b - balance between yellow and blue. It is possible to construct derivative values: lightness, chromaticity and hue.

CIELab is device-independent thus it is used in convertion between RBG and CMYK models.

##### How does it work?
1. Load images
2. Generate historgram for each image
3. Get palette (most representative colors) for each image counting DeltaE distance between colors and removing too similar; best, experimentally obtained, threshold distance is **9**
4. Count similarity of palletes of images by:
 - counting distance between most similar colors of pallettes
 - inserting these distances into the vector
 - counting vector length
5. Order images by distance to next, not previously used image

#### TODO
- better palette extraction (see photo of green fields with trees and sunset in the background)

Sample results:

![https://i.imgur.com/nI1qZO6.jpg](https://i.imgur.com/nI1qZO6.jpg)
