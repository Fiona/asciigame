using System;
using System.Collections.Generic;
using asciigame.Core;
using asciigame.Data;
using Microsoft.Xna.Framework;

namespace asciigame.UI
{

    public enum WidgetLayout
    {
        None=0,
        MaxWidth=1,
        MaxHeight=2,
        MaxWidthHeight=3,
    }

    public struct WidgetBounds
    {
        public WidgetBounds((int x, int y) pos, (int width, int height) size)
        {
            topLeft = (pos.x, pos.y);
            topRight = (pos.x + size.width - 1, pos.y);
            bottomLeft = (pos.x, pos.y + size.height - 1);
            bottomRight = (pos.x + size.width - 1, pos.y + size.height - 1);
        }

        public WidgetBounds(WidgetBounds cloneBounds)
        {
            topLeft = cloneBounds.topLeft;
            topRight = cloneBounds.topRight;
            bottomLeft = cloneBounds.bottomLeft;
            bottomRight = cloneBounds.bottomRight;
        }

        public (int x, int y) topLeft;
        public (int x, int y) topRight;
        public (int x, int y) bottomLeft;
        public (int x, int y) bottomRight;

        public int x => topLeft.x;
        public int y => topLeft.y;
        public int width => topRight.x - topLeft.x + 1;
        public int height => bottomLeft.y - topLeft.y + 1;

        public void Shrink(int amount)
        {
            Shrink(amount, amount, amount, amount);
        }

        public void Shrink(int topAmount, int rightAmount, int bottomAmount, int leftAmount)
        {
            // top
            topLeft.y += topAmount;
            topRight.y += topAmount;
            // right
            topRight.x -= rightAmount;
            bottomRight.x -= rightAmount;
            // bottom
            bottomLeft.y -= bottomAmount;
            bottomRight.y -= bottomAmount;
            // left
            topLeft.x += leftAmount;
            bottomLeft.x += leftAmount;
        }

        public override string ToString()
        {
            return
                $"WidgetBounds[ TL: {topLeft.x},{topLeft.y} TR: {topRight.x},{topRight.y} BL: {bottomLeft.x},{bottomLeft.y} BR: {bottomRight.x},{bottomRight.y} W: {width} H: {height}]";
        }
    }

    public class Widget
    {
        public bool showDebugText = false;

        protected WidgetStyle style;
        protected WidgetBounds bounds;
        protected WidgetBounds contentBounds;
        protected Widget parent;
        protected List<Widget> children;
        protected WidgetLayout layout = WidgetLayout.None;
        private (int x, int y) setPosition;
        private (int width, int height) setSize;
        private bool redraw = true;

        protected Widget(Widget parent)
        {
            this.parent = parent;
            children = new List<Widget>();
            style = WidgetStyle.Get("none");
            redraw = true;
            UpdatePosSize((0, 0), (0,0));
            parent?.AddChild(this);
        }

        public virtual void UpdatePosSize((int x, int y) newPosition, (int width, int height) newSize)
        {
            setPosition = newPosition;
            setSize = (newSize.width, newSize.height);
            UpdateBounds();
            UpdateChildPosSize();
            ForceRedraw();
        }

        public void ForceRedraw()
        {
            redraw = true;
            foreach(var child in children)
                child.ForceRedraw();
        }

        public virtual void Draw(Canvas canvas)
        {
            if(redraw)
            {
                // Top border
                if(style.border.top != 0)
                    canvas.WriteAt(
                        new string(style.border.top, bounds.width),
                        bounds.x, bounds.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground
                    );
                // Bottom border
                if(style.border.bottom != 0)
                    canvas.WriteAt(
                        new string(style.border.bottom, bounds.width),
                        bounds.x, bounds.bottomLeft.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground
                    );
                // Left border
                if(style.border.left != 0)
                    for(int leftBorderPos = bounds.topLeft.y; leftBorderPos <= bounds.bottomLeft.y; leftBorderPos++)
                        canvas.WriteAt(style.border.left.ToString(), bounds.x, leftBorderPos, clearTiles: true,
                            colour: style.borderColour, clearColour: style.borderColourBackground);
                // Right border
                if(style.border.right != 0)
                    for(int rightBorderPos = bounds.topRight.y; rightBorderPos <= bounds.bottomRight.y; rightBorderPos++)
                        canvas.WriteAt(style.border.right.ToString(), bounds.topRight.x, rightBorderPos,
                            clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground);

                // Corners
                if(style.corners.topLeft != 0)
                    canvas.WriteAt(style.corners.topLeft.ToString(), bounds.topLeft.x, bounds.topLeft.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground);
                if(style.corners.topRight != 0)
                    canvas.WriteAt(style.corners.topRight.ToString(), bounds.topRight.x, bounds.topRight.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground);
                if(style.corners.bottomleft != 0)
                    canvas.WriteAt(style.corners.bottomleft.ToString(), bounds.bottomLeft.x, bounds.bottomLeft.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground);
                if(style.corners.bottomRight != 0)
                    canvas.WriteAt(style.corners.bottomRight.ToString(), bounds.bottomRight.x, bounds.bottomRight.y,
                        clearTiles: true, colour: style.borderColour, clearColour: style.borderColourBackground);

                // Background colour
                if(style.backgroundColour.A > 0)
                    for(int contentYPos = contentBounds.topLeft.y; contentYPos <= contentBounds.bottomLeft.y; contentYPos++)
                        canvas.WriteAt(
                            new string(' ', contentBounds.width),
                            contentBounds.x, contentYPos,
                            clearTiles: true, colour:Color.TransparentBlack, clearColour: style.backgroundColour
                        );

                // Any custom drawing
                DrawAdditional(canvas);

                redraw = false;

            }

            foreach(var child in children)
                child.Draw(canvas);
        }

        protected virtual void DrawAdditional(Canvas canvas)
        {
        }

        public void AddChild(Widget childWidget)
        {
            if(!children.Contains(childWidget))
                children.Add(childWidget);
            UpdatePosSize(setPosition, setSize);
        }

        public void RemoveChild(Widget childWidget)
        {
            if(children.Contains(childWidget))
                children.Remove(childWidget);
            UpdatePosSize(setPosition, setSize);
        }

        public void RemoveAllChildren()
        {
            children.Clear();
            UpdatePosSize(setPosition, setSize);
        }

        public void SetStyle(string styleName)
        {
            style = WidgetStyle.Get(styleName);
            UpdatePosSize(setPosition, setSize);
        }

        public void SetLayout(WidgetLayout newLayout)
        {
            layout = newLayout;
            UpdatePosSize(setPosition, setSize);
        }

        private void UpdateChildPosSize()
        {
            foreach(var child in children)
                child.UpdatePosSize((contentBounds.x, contentBounds.y), (contentBounds.width, contentBounds.height));
        }

        private void UpdateBounds()
        {
            var newSize = setSize;
            switch(layout)
            {
                case WidgetLayout.MaxHeight:
                    newSize = (newSize.width, parent.contentBounds.height);
                    break;
                case WidgetLayout.MaxWidth:
                    newSize = (parent.contentBounds.width, newSize.height);
                    break;
                case WidgetLayout.MaxWidthHeight:
                    newSize = (parent.contentBounds.width, parent.contentBounds.height);
                    break;
            }

            bounds = new WidgetBounds(setPosition, newSize);
            bounds.Shrink(style.margin.top, style.margin.right, style.margin.bottom, style.margin.left);
            contentBounds = new WidgetBounds(bounds);
            contentBounds.Shrink(
                style.border.top != 0 ? 1 : 0,
                style.border.right != 0 ? 1 : 0,
                style.border.bottom != 0 ? 1 : 0,
                style.border.left != 0 ? 1 : 0
            );
            contentBounds.Shrink(style.padding.top, style.padding.right, style.padding.bottom, style.padding.left);
        }
    }
}