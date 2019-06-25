Camera Path
===========

Description
This package simplifies the process of animating a camera along a path. It gives you tools to preview what you're doing as you go along so you don't have to play your scene out to test it. It also gives you controls at runtime to play, pause and stop the animation. Within the IDE you can control how the path itself behaves, editing things like pausing or delaying the animation at specific points and easing in or out of points.

Instructions

Quick Start
Import the package into your scene
There will be a new entry under Gameobject called Create New Camera Path
A new game object called New Camera Path has been created
Select it and it's inspector will give you the option to Add Point
Click on Add point twice and you have now created a new path of two points.

Camera Path is made from three things.
The parent "new camera path" contains the Camera Path Bezier Script and the Camera Path Bezier Animation script.
The first script deals with the mathematical creation of the path while the second controls the animation itself.
The child points use the Camera Path Bezier Control point which contain all the properties for that part of the path and the subsequent curve.

Notes
You can use the slider below the Animation Preview to preview the animation between these points at the specified time.
Adding new points will now extrapolate the current path positions to create points and extend the path.

Icon
To get the icon working, drag the CameraPath2/Gizmos folder to the root Asset folder.

Modifying the Points and the Path
Within the New Camera Path are the control points
These points define the curve.
The Curve will firstly go through each point
The nature of that curve is also affected by the control points within these. This is located at the same position as the main point and it has a discoloured move handle that you can use to change it's position.
Each point has the setting in the editor called animation mode to decide how it's curve will animate and what kind of ease is placed on it.

Site
http://camerapath.jasperstocker.com/

Bugs
Ah! Found a bug, send it my way and I'll do my very best to nail it down. 
If you send a bug, please try to include as much information as possible! 
Full error messages, screenshots. How you managed to do it and how to recreate it.
Thanks!
email@jasperstocker.com