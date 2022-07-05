Asciigame code explanation
==========================

Yeah man, object oriented programming is awesome. That's why I feel the need to document and chart what all the classes mean. Pretty neat, right? I've never heard of self documenting code.

Configuration
-------------

Various aspects of the application and the game are configured via JSON files which are queried in code via data classes.

###### AppConfig.json (Data/AppConfig.cs)

Configures global app settings and Canvas drawing settings. Tile size, clear colour, window size and font are configured here.

###### WidgetStyles.json (UI/WidgetStyle.cs)

Allows reusable named styles to be applied to widgets, similar to classes in CSS. Allows styling of colours, border styles, etc.


Top-Level 
---------

###### Entry Point (Program.cs)

Top level OS class. Just creates a Window object.

###### Window (Core/Window.cs)

The global XNA Game object. Interacts with the OS.

Instructs the Canvas object to add glyphs representing the current screen to a SpriteBatch object. Then draws the SpriteBatch to a screen buffer. 

Looks after:
 * UIManager
 * GameManager
 * Canvas


Drawing
-------

###### Canvas (Core/Canvas.cs)

Essentially a screen-sized tilemap. A tile is a CanvasTile object that represents one or more coloured glyphs.

The Canvas is instructed to add glyphs to the screen via the WriteTo methods. The screen buffer is **not** cleared, this means that WriteTo commands need not be sent repeatedly unless the glyphs in question change.

Looks after:
 * GlyphPalette

###### GlyphPalette (Core/GlyphPalette.cs)

An atlas texture containing all previously used glyphs for drawing to a Canvas. Is initialised and used directly the Canvas only.

When the Canvas requests a glyph be drawn to the screen, the GlyphPalette is responsible for ensuring the glyph in question is available and it's location on the atlas.

###### PrimitveDrawing (Core/PrimitiveDrawing.cs)

Adds simple rectangle and line drawing by utilising a passed SpriteBatch. Is currently only used by Canvas.


Low-Level UI System
-------------------

###### UIManager (UI/UIManager.cs)

Holds the root UI Widget. Is told by Window when resizes happen and cascades resizes down the widget tree.

###### Widget (UI/Widget.cs)

Base class for all widgets. Is responsible for determining size and position on screen, taking into account styling. 
All widgets must have a parent widget (apart from the root).

Widgets write characters directly to the Canvas when they are told to draw. The root widget is told to draw by Window and all children are drawn in a cascade fashion from there.

###### WidgetStyle (UI/WidgetStyle.cs)

Holds values describing the style for the widget - padding, borders, colours, etc.


Widget Types
------------

###### Panel (UI/Widgets/WidgetPanel.cs)

A usually empty container widget that can have optional title text.

###### Text (UI/Widgets/WidgetText.cs)

Just text. No other behaviours.


Game States
-----------

###### GameManager (Game/GameManager.cs)

A state machine that ensures the correct UI states are currently shown.

###### ScreenTitle (Game/UI/ScreenTitle.cs)
