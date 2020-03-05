# Overview
This is a collection of projects written in C-Sharp and developed on the game engine Unity.

# Requirements
Latest version of Unity and Microsoft Visual Studio.

# Table of contents

## Cameras
Contain different type of camera that can be found in video games (only first-person for now).

## Graph
Create a 3D graph that can be dragged around inside the scene like a chain by using inverse kinematics (only works for open chains for now).

## Random sphere packing
Create spheres inside a bounding box randomly without overlaps. Each sphere can have different size, and a minimum distance can be defined for every spheres. Three methods are used to test whether a newly inserted sphere collides with the existing ones :
* Naive method : Iterate normally through a list of spheres until a collision occurs.
* Random method : Iterate randomly through a list of spheres until a collision occurs.
* Octree method : Iterate through a list of spheres occupying the space where the new sphere landed until a collision occurs.

# Work in progress...
