### Paletter

Simple program to extract colours palette from one image and find distance to palette of another image using [Delta E](https://en.wikipedia.org/wiki/Color_difference) method.

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
