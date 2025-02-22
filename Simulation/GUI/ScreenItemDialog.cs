﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Simulation.Graphics;

namespace Simulation.GUI
{
    public class ScreenItemDialog : ScreenItem
    {
        protected Vector4 TitlePadding = new Vector4(1, 5, 1, 5);
        public ScreenItemDialog(Game game, ApplicationSkin skin, string title, bool modal) :
            base(game, skin, 0, 0, -1, -1)
        {
            Title = title;
            Layer = 4;
            Modal = modal;
            AutoSize = true;
            Positioning = ScreenItemDialogPositioning.AutoCenter;
            Vector2 titleSize = skin.Font.MeasureString(Title);
            titleBarHeight = skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight");
            Width = (int)(TitlePadding.Y + TitlePadding.W + Math.Ceiling(titleSize.X)) + 2;
            Height = (int)(TitlePadding.X + TitlePadding.Z + titleBarHeight) + 3;
            items.ItemCountChanged += new ScreenItemDialogItems.ItemCountChangedHandler(OnNumberItemsChanged);
        }
        public ScreenItemDialog(Game game, ApplicationSkin skin, float x, float y, string title, bool modal)
            : base(game, skin, x, y, -1, -1)
        {
            Title = title;
            Layer = 4;
            Modal = modal;
            AutoSize = true;
            Vector2 titleSize = skin.Font.MeasureString(Title);
            titleBarHeight = skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight");
            Width = (int)(TitlePadding.Y + TitlePadding.W + Math.Ceiling(titleSize.X)) + 2;
            Height = (int)(TitlePadding.X + TitlePadding.Z + titleBarHeight) + 3;
            items.ItemCountChanged += new ScreenItemDialogItems.ItemCountChangedHandler(OnNumberItemsChanged);
            Positioning = ScreenItemDialogPositioning.Absolute;
        }
        public ScreenItemDialog(Game game, ApplicationSkin skin, int x, int y, int width, int height, string title, bool modal)
            : base(game, skin, x, y, width, height)
        {
            Title = title;
            Layer = 4;
            Modal = modal;
            items.ItemCountChanged += new ScreenItemDialogItems.ItemCountChangedHandler(OnNumberItemsChanged);
            Positioning = ScreenItemDialogPositioning.Absolute;
        }
        public ScreenItemDialog(Game game, ApplicationSkin skin, float x, float y, float width, float height, string title,
            bool modal)
            : base(game, skin, x, y, width, height)
        {
            Title = title;
            Layer = 4;
            Modal = modal;
            items.ItemCountChanged += new ScreenItemDialogItems.ItemCountChangedHandler(OnNumberItemsChanged);
            Positioning = ScreenItemDialogPositioning.Absolute;
        }
        protected int titleBarHeight;
        protected ScreenItemDialogItems items = new ScreenItemDialogItems();
        public bool AutoSize { get; set; }
        private bool closeButton = true;
        public bool CloseButton
        {
            get { return closeButton; }
            set
            {
                closeButton = value;
                OnNumberItemsChanged();
            }
        }
        public string Title { get; set; }
        public ScreenItemDialogPositioning Positioning { get; set; }
        public ScreenItemDialogItems Items
        {
            get { return items; }
            set
            {
                int oldItemCount = items.Count;
                items = value;
                if (value.Count != oldItemCount)
                    OnNumberItemsChanged();
            }
        }
        public bool Modal { get; set; }
        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                OnMove(base.Position, value);
                base.Position = value;
            }
        }
        protected void OnMove(Vector2 oldPosition, Vector2 newPosition)
        {
            Vector2 positionDelta = newPosition - oldPosition;
            foreach (ScreenItem item in Items)
                item.Position += positionDelta;
        }
        protected void OnNumberItemsChanged()
        {
            if (!AutoSize)
                return;
            Vector2 greatestLocationReached = new Vector2(0, 0);
            foreach (ScreenItem screenItem in items)
            {
                if (screenItem.Parent != this)
                {
                    screenItem.Parent = this;
                    screenItem.Y += (int)(TitlePadding.X + TitlePadding.Z + titleBarHeight + this.Y);
                    screenItem.X += this.X;
                }
                if (screenItem.Y + screenItem.Height > greatestLocationReached.Y)
                    greatestLocationReached.Y = screenItem.Y + screenItem.Height;
                if (screenItem.X + screenItem.Width > greatestLocationReached.X)
                    greatestLocationReached.X = screenItem.X + screenItem.Width;
            }
            int minWidthOfTitle = (int)(TitlePadding.Y + TitlePadding.W + Math.Ceiling(skin.Font.MeasureString(Title).X)) + 2
                + (closeButton ? titleBarHeight : 0);
            Width = (minWidthOfTitle > greatestLocationReached.X - (int)this.X + 5 ? minWidthOfTitle :
                greatestLocationReached.X - (int)this.X + 5);
            Height = greatestLocationReached.Y - (int)this.Y + 5;
            if (Positioning == ScreenItemDialogPositioning.AutoCenter)
                Position = new Vector2((int)((GlobalSettings.ScreenWidth - Width) / 2),
                    (int)((GlobalSettings.ScreenHeight - Height) / 2));
        }
        private Nullable<Vector2> leftMouseDownLocation = null;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && leftMouseDownLocation == null)
                leftMouseDownLocation = new Vector2(mouseState.X, mouseState.Y);
            else if (mouseState.LeftButton == ButtonState.Released && leftMouseDownLocation != null)
                leftMouseDownLocation = null;

            if (closeButton)
            {
                int closeButtonWidth = (int)(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleCloseButton").Width *
                    ((float)skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight") /
                    (float)skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleCloseButton").Height));
                Rectangle closeButtonRectangle = new Rectangle((int)(X + Width -
                    skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd").Width -
                    closeButtonWidth),
                    (int)Y, closeButtonWidth,
                    skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight"));
                bool hoveringCloseButton = (mouseState.X >= closeButtonRectangle.X &&
                    mouseState.X < closeButtonRectangle.Right - 1 && mouseState.Y >= closeButtonRectangle.Y &&
                    mouseState.Y < closeButtonRectangle.Bottom - 1);
                if (leftMouseDownLocation != null && leftMouseDownLocation.Value.X >= closeButtonRectangle.X &&
                    leftMouseDownLocation.Value.X < closeButtonRectangle.Right - 1 && leftMouseDownLocation.Value.Y >=
                    closeButtonRectangle.Y && leftMouseDownLocation.Value.Y < closeButtonRectangle.Bottom - 1)
                    this.Close();
            }

            foreach (ScreenItem item in Items)
                item.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Modal)
                spriteBatch.FillRectangle(0, 0, GlobalSettings.ScreenWidth, GlobalSettings.ScreenHeight,
                    new Color(Color.WhiteSmoke, 175));
            float titleUsableWidth = Width - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleLeftEnd").Width -
                skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd").Width;
            spriteBatch.Draw(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleLeftEnd"),
                new Rectangle((int)X, (int)Y, skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleLeftEnd").Width,
                    skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")), Color.White);
            spriteBatch.TileTexture(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleBarBackground"),
                new Vector2(X + skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleLeftEnd").Width,
                    Y), new Vector2(titleUsableWidth, skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")),
                    Color.White);
            spriteBatch.Draw(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd"),
                new Rectangle((int)(X + Width -
                    skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd").Width),
                    (int)Y, (int)skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd").Width,
                    skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")), Color.White);
            if (closeButton)
            {
                MouseState mouseState = Mouse.GetState();
                int closeButtonWidth = (int)(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleCloseButton").Width *
                    ((float)skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight") /
                    (float)skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleCloseButton").Height));
                Rectangle closeButtonRectangle = new Rectangle((int)(X + Width -
                    skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleRightEnd").Width -
                    closeButtonWidth),
                    (int)Y, closeButtonWidth,
                    skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight"));
                bool hoveringCloseButton = (mouseState.X >= closeButtonRectangle.X &&
                    mouseState.X < closeButtonRectangle.Right - 1 && mouseState.Y >= closeButtonRectangle.Y &&
                    mouseState.Y < closeButtonRectangle.Bottom - 1);
                spriteBatch.Draw(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("TitleCloseButton"),
                    closeButtonRectangle, Color.White);
                titleUsableWidth -= closeButtonRectangle.Width;
            }
            string drawTitle = Title;
            bool titleShortened = false;
            while (drawTitle.Length > 0 && (int)Skin.Fonts[13].MeasureString(drawTitle +
                (titleShortened ? "..." : "")).X >= titleUsableWidth)
            {
                if (!titleShortened)
                    titleShortened = true;
                drawTitle = drawTitle.Substring(0, drawTitle.Length - 1);
            }
            Vector2 titleSize = skin.Fonts[13].MeasureString(drawTitle + (titleShortened ? "..." : ""));
            spriteBatch.DrawString(Skin.Fonts[13], drawTitle + (titleShortened ? "..." : ""),
                new Vector2(X + TitlePadding.W + (int)((titleUsableWidth - titleSize.X) / 2),
                    Y + TitlePadding.X + (int)((skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight") -
                    titleSize.Y) / 2)), Color.Black);

            spriteBatch.Draw(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomLeft"),
                new Vector2(X, Y + Height - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomLeft").Height),
                Color.White);
            spriteBatch.Draw(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomRight"),
                new Vector2(X + Width - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomRight").Width,
                    Y + Height - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomRight").Height),
                    Color.White);
            spriteBatch.TileTexture(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Bottom"),
                new Vector2(X + skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomLeft").Width,
                    Y + Height - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Bottom").Height),
                    new Vector2(Width - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomLeft").Width -
                        skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomRight").Width,
                        skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Bottom").Height), Color.White);

            spriteBatch.TileTexture(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Background"),
                new Vector2(X + skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Left").Width,
                    Y + skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")),
                    new Vector2(Width - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Left").Width -
                    skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Right").Width,
                    Height - skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight") -
                    skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Bottom").Height), Color.White);
            int tiledHeight = (int)Height - skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight") -
                        skin.ScreenItemSkins["Dialog"].Get<Texture2D>("BottomLeft").Height;
            spriteBatch.TileTexture(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Left"),
                new Vector2(X, Y + skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")),
                    new Vector2(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Left").Width,
                        tiledHeight), Color.White);
            spriteBatch.TileTexture(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Right"),
                new Vector2(X + Width - skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Right").Width,
                    Y + skin.ScreenItemSkins["Dialog"].Get<Int32>("TitleBarHeight")),
                    new Vector2(skin.ScreenItemSkins["Dialog"].Get<Texture2D>("Right").Width, tiledHeight),
                    Color.White);


            foreach (ScreenItem item in Items)
                item.Draw(gameTime, spriteBatch);
        }
        public void Close()
        {
            if (Closed != null)
                Closed.Invoke();
        }
        public delegate void CloseDialogHandler();
        public event CloseDialogHandler Closed;
        public override bool ProcessUpdateMouse()
        {
            bool baseProcessUpdateMouse = base.ProcessUpdateMouse();
            if (Modal ? true : baseProcessUpdateMouse)
            {
                foreach (ScreenItem item in Items)
                    if (item.ProcessUpdateMouse())
                        break;
            }
            return (Modal ? true : baseProcessUpdateMouse);
        }
    }
    public class ScreenItemDialogItems : List<ScreenItem>
    {
        public new void Add(ScreenItem screenItem)
        {
            base.Add(screenItem);
            if (ItemAdded != null)
                ItemAdded.Invoke(screenItem);
            if (ItemCountChanged != null)
                ItemCountChanged.Invoke();
        }
        public delegate void ItemAddedHandler(ScreenItem item);
        public event ItemAddedHandler ItemAdded;
        public delegate void ItemCountChangedHandler();
        public event ItemCountChangedHandler ItemCountChanged;
    }
    public enum ScreenItemDialogPositioning
    {
        Absolute,
        AutoCenter
    }
}
