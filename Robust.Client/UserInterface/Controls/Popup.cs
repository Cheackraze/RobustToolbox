﻿using System;
using Robust.Shared.Maths;

namespace Robust.Client.UserInterface.Controls
{
    [Virtual]
    public class Popup : Control
    {
        public Popup()
        {
            Visible = false;
        }

        public event Action? OnPopupHide;

        private Vector2 _desiredSize;

        public bool CloseOnClick { get; set; } = true;

        public bool CloseOnEscape { get; set; } = true;

        public void Open(UIBox2? box = null, Vector2? altPos = null)
        {
            if (Visible)
            {
                UserInterfaceManagerInternal.RemoveModal(this);
            }

            if (box != null &&
                (_desiredSize != box.Value.Size ||
                 PopupContainer.GetPopupOrigin(this) != box.Value.TopLeft ||
                 PopupContainer.GetAltOrigin(this) != altPos))
            {
                PopupContainer.SetPopupOrigin(this, box.Value.TopLeft);
                PopupContainer.SetAltOrigin(this, altPos);

                _desiredSize = box.Value.Size;
                InvalidateMeasure();
            }

            Visible = true;
            UserInterfaceManagerInternal.PushModal(this);
        }

        public void Close()
        {
            if (!Visible) return;
            UserInterfaceManagerInternal.RemoveModal(this);
        }


        protected internal override void ModalRemoved()
        {
            base.ModalRemoved();

            Visible = false;
            OnPopupHide?.Invoke();
        }

        protected override Vector2 MeasureOverride(Vector2 availableSize)
        {
            return Vector2.ComponentMax(
                _desiredSize,
                base.MeasureOverride(Vector2.ComponentMax(availableSize, _desiredSize)));
        }
    }
}
