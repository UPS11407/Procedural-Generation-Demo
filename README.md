# Procedural Generation Demo
 
This is a simple demo project for a procedural generation system in Unity.

I was inspired by [this video](https://youtu.be/ob3VwY4JyzE) and decided to try my hand at this project. This is only a demo of what the generation system might look like, not a full game.

Some tips for generation: 
- If you have a high chunk width, keep the world length and width low. This will reduce the amount of time it takes to generate fairly large worlds (I was using values like 32 for chunk width and 4 for world length and width as well as 110 world height).
- Very large generations will take some time so be patient if you do something like 32 chunk width with 10 world width and length, sadly Unity functions are not thread safe so multithreading is not possible to leverage it for generating gameobjects.

Here are a few cool seeds I found (using 32 chunk width with 4 world length and width as well as 110 world height)
- 5032060 | Massive holes in the ground
- 3730911 | A nice example of the generation
