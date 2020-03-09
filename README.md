# Overview
This is a collection of projects written in C-Sharp and developed on the game engine Unity.

# Requirements
Latest version of Unity and Microsoft Visual Studio.

# Table of contents

## Cameras
Contain different types of cameras that can be found in video games (only first-person for now).

## Graph
Create a 3D graph that can be dragged around with the mouse inside the scene like a chain by using inverse kinematics (only works for open chains for now).

## Random sphere packing
Create spheres inside a bounding box at random without overlaps. The spheres can have their sizes randomized and have a minimum distance separating each of them. Three search methods are used to test whether a newly inserted sphere collides with the existing ones (without using the game engine collision detection) :
* Linear search : Iterate normally through a list of spheres until a collision occurs.
* Random search : Iterate randomly through a list of spheres until a collision occurs.
* Octree search : Iterate through a list of spheres occupying the space, where the new sphere landed, until a collision occurs.

## Others
Contain some accessory classes.

# Work in progress...
